using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;

namespace ContactsManager.StartupExtensions;

public static class ConfigureServiceExtension
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        services.AddSerilog((services, config) => config
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services));

        var mvcBuilder = services.AddControllersWithViews();

        if (hostEnvironment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        services
            .AddScoped<ICountryService, CountryService>()
            .AddScoped<IPersonService, PersonService>()
            .AddScoped<ICountriesRepository, CountriesRepository>()
            .AddScoped<IPersonsRepository, PersonsRepository>()
            .AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

        return services;
    }
}
