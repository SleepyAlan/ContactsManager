using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities;

public class ContactsManagerDbContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    public ContactsManagerDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        // Seeding database with initial data from json files.
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        // Seed countries data from json on model creation.
        string countriesJson = File.ReadAllText("SeedData/Countries.json");
        var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson, options);
        modelBuilder.Entity<Country>().HasData(countries);

        // Seed persons data from json on model creation.
        string personsJson = File.ReadAllText("SeedData/Persons.json");
        var persons = JsonSerializer.Deserialize<List<Person>>(personsJson, options);
        modelBuilder.Entity<Person>().HasData(persons);
    }
}
