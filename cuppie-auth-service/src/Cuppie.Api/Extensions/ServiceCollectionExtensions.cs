using Cuppie.Application.Interfaces.DAO;
using Cuppie.Application.Interfaces.Services;
using Cuppie.Infrastructure.DAO;
using Cuppie.Infrastructure.Data;
using Cuppie.Infrastructure.Services;
using Cuppie.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;

namespace Cuppie.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddOpenApi();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICryptoService, CryptoService>();
        services.AddScoped<IUserDao, UserDao>();
        services.AddScoped<IRefreshTokenDao, RefreshTokenDao>();
        services.Configure<JwtOptions>(config.GetSection("JWT"));

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config["ConnectionStrings:DefaultConnection"]
                               ?? config.GetConnectionString("cuppieContext");
    
        if (connectionString == null)
            throw new InvalidOperationException("Connection string not found.");
    
        services.AddDbContext<CuppieDbContext>(options =>
            options.UseNpgsql(connectionString));
    
        return services;
    }
}