using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using ServiceContracts;
using Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.EnvironmentName != "Testing")
{
    // Crashes unit tests if Serilog is configured in unit tests,
    // so configure it only when not running unit tests
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
}


try
{
    Log.Information("Starting web application...");

    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    var mvcBuilder = builder.Services.AddControllersWithViews();

    if (builder.Environment.IsDevelopment())
    {
        mvcBuilder.AddRazorRuntimeCompilation();
    }

    builder.Services
        .AddScoped<ICountryService, CountryService>()
        .AddScoped<IPersonService, PersonService>()
        .AddScoped<ICountriesRepository, CountriesRepository>()
        .AddScoped<IPersonsRepository, PersonsRepository>()
        .AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"));
        });

    var app = builder.Build();

    if (builder.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();
    app.UseStaticFiles();
    app.UseRouting();
    app.MapControllers();

    app.UseRotativa();
    ExcelPackage.License.SetNonCommercialPersonal("Alan S");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}