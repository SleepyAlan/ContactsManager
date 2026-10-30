using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

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

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.UseRotativa();
ExcelPackage.License.SetNonCommercialPersonal("Alan S");

app.Run();
