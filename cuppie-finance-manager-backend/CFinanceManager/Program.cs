using System.Text;
using Application.Interfaces.Clients;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services.User;
using Application.UseCases;
using Infrastructure.Clients;
using Infrastructure.DB;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IAuthClient, AuthClient>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT settings
    var jwtSettings = builder.Configuration.GetSection("JWT");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
    var tokenName = jwtSettings["ACCESS_TOKEN_COOKIE_NAME"];

    if (string.IsNullOrEmpty(tokenName))
    {
        throw new InvalidOperationException("JWT ACCESS_TOKEN_COOKIE_NAME is not configured");
    }

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };

            // Берём токен из HttpOnly cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue(tokenName, out var token) && !string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                        var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                        var logger = loggerFactory?.CreateLogger("JwtBearer");
                        logger?.LogDebug("JWT token found in cookie: {CookieName}", tokenName);
                    }
                    else
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                        var logger = loggerFactory?.CreateLogger("JwtBearer");
                        logger?.LogWarning("JWT token not found in cookie: {CookieName}. Available cookies: {Cookies}", 
                            tokenName, string.Join(", ", context.Request.Cookies.Keys));
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory?.CreateLogger("JwtBearer");
                    logger?.LogError(context.Exception, "JWT Authentication failed");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory?.CreateLogger("JwtBearer");
                    logger?.LogWarning("JWT Challenge triggered. Error: {Error}, ErrorDescription: {ErrorDescription}", 
                        context.Error, context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        });

builder.Services.AddAuthorization();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();