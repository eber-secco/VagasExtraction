
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VagasExtraction.Selenium;
using VagasExtraction.Services;

namespace VagasExtraction.Config;

public static class InjectionConfiguration
{
    public static void AddServicesInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton<ExtractionService>();
        services.AddScoped<DriverHelper>();
    }
}
