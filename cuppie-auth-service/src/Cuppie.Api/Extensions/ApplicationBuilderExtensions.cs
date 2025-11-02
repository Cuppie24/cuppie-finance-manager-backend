using Microsoft.AspNetCore.HttpOverrides;

namespace Cuppie.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UsePresentation(this WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }
        app.MapControllers();

        return app;
    }
}