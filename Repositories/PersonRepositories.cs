using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace Repositories;

public class PersonRepositories : IPersonsRepository
{
    private readonly ApplicationDbContext _db;

    public PersonRepositories(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
        var personAdded = _db.Persons.Add(person);
        await _db.SaveChangesAsync();
        return personAdded.Entity;
    }

    public async Task<bool> DeletePerson(Guid personId)
    {
        var person = await _db.Persons
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        if (person is null) return false;

        _db.Persons.Remove(person);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _db.Persons
            .Include(p => p.Country)
            .ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(
        Expression<Func<Person, bool>> predicate)
    {
        return await _db.Persons
            .Include(p => p.Country)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Person?> GetPerson(Guid personId)
    {
        return await _db.Persons
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personId);
    }

    public async Task<List<Person>> GetPersonsByCountry(Guid countryId)
    {
        return await _db.Persons
            .Include(p => p.Country)
            .Where(p => p.CountryId == countryId)
            .ToListAsync();
    }

    public async Task<List<Person>> GetPersonsByCountry(string countryName)
    {
        return await _db.Persons
            .Include(p => p.Country)
            .Where(p =>
                p.Country != null &&
                p.Country.CountryName == countryName)
            .ToListAsync();
    }

    public async Task<List<Person>> GetPersonsByCountry(Country country)
    {
        return await _db.Persons
            .Include(p => p.Country)
            .Where(p => p.CountryId == country.CountryId)
            .ToListAsync();
    }

    public async Task<bool> PersonExists(Guid personId)
    {
        var person = await _db.Persons
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        return person is not null ? true : false;
    }

    public async Task<Person?> UpdatePerson(Person person)
    {
        var personToUpdate = await _db.Persons
            .FirstOrDefaultAsync(p => p.PersonId == person.PersonId);

        if (personToUpdate is null) return null;

        personToUpdate.PersonName = person.PersonName;
        personToUpdate.Email = person.Email;
        personToUpdate.DateOfBirth = person.DateOfBirth;
        personToUpdate.Gender = person.Gender;
        personToUpdate.CountryId = person.CountryId;
        personToUpdate.Address = person.Address;
        personToUpdate.ReceiveNewsLetters = person.ReceiveNewsLetters;

        await _db.SaveChangesAsync();
        return personToUpdate;
    }
}
