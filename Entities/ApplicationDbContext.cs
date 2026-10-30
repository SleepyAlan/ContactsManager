using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities;

public class ApplicationDbContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Country)
            .WithMany()
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seeding database with initial data from json files.
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Seed countries data from json on model creation.
        string countriesJson = File.ReadAllText("SeedData/Countries.json");
        var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson, options);

        // Seed persons data from json on model creation.
        string personsJson = File.ReadAllText("SeedData/Persons.json");
        var persons = JsonSerializer.Deserialize<List<Person>>(personsJson, options);

        if (countries is null || persons is null) return;

        modelBuilder.Entity<Country>().HasData(countries);
        modelBuilder.Entity<Person>().HasData(persons);
    }

    [Obsolete("This method is for demonstration purposes only. Use LINQ queries instead.")]
    public async Task<List<Person>> sp_GetAllPersons()
    {
        return await Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToListAsync();
    }
}
