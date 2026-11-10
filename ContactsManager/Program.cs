using OfficeOpenXml;
using Rotativa.AspNetCore;
using Serilog;
using ContactsManager.StartupExtensions;
using ContactsManager.Middleware;

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

    builder.Services.ConfigureServices(
        builder.Configuration, builder.Environment);

    var app = builder.Build();

    if (builder.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandlingMiddleware();
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