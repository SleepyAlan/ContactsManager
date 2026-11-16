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

    // Configure the HTTP request pipeline middleware.
    // Exception handler
    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing")
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseExceptionHandlingMiddleware();
    }
    // Https redirection
    app.UseHsts();
    app.UseHttpsRedirection();

    // Static files
    app.UseStaticFiles();

    // Logging
    app.UseSerilogRequestLogging();

    // Routing / CORS / Authentication / Authorization
    app.UseRouting();
    //app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // Custom middleware
    // Map controllers
    app.MapControllers();

    app.UseRotativa(); // Rotativa setup. Not apart of the middleware pipeline.
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