using Entities;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Extensions;
using Services;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Services.Tests;

public class PersonServiceTests
{
    private readonly IPersonService _sut;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ICountryService _countryService;

    public PersonServiceTests(
        ITestOutputHelper testOutputHelper,
        ApplicationDbContext db)
    {// TODO: Fix all tests. migrated to database.
        _testOutputHelper = testOutputHelper;
        _countryService = new CountryService(db);
        _sut = new PersonService(db);
    }

    #region AddPerson() Tests
    [Fact]
    public async Task AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public async Task AddPerson_PersonNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = null
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public async Task AddPerson_PersonNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public async Task AddPerson_PersonNameIsWhitespace_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = " "
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public async Task AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true,
        };
        // Act
        PersonResponse actualPersonResponse = await _sut.AddPerson(personAddRequest);

        // Assert
        Assert.True(actualPersonResponse.PersonId != Guid.Empty);

    }

    [Fact]
    public async Task AddPerson_ValidPersonAddRequest_VerifyItPersistsViaGetAllPersons()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.UtcNow,
            ReceiveNewsLetters = true,
        };

        // Act
        PersonResponse expectedPersonResponse = await _sut.AddPerson(personAddRequest);
        List<PersonResponse> actualPersonResponses = await _sut.GetAllPersons();


        // Assert
        Assert.Contains(expectedPersonResponse, actualPersonResponses);
    }

    #endregion

    #region GetAllPersons() Tests
    [Fact]
    public async Task GetAllPersons_IfNoPersons_ReturnsEmptyList()
    {
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetAllPersons();

        // Assert
        Assert.Empty(actualPersonResponses);
    }

    [Fact]
    public async Task GetAllPersons_IfFewPersons_ReturnsListOfPersons()
    {
        // Arrange
        List<PersonResponse> expectedPersonResponses = await AddSamplePersons();
        PrintPersonResponses(expectedPersonResponses, "Expected:");

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetAllPersons();
        PrintPersonResponses(actualPersonResponses, "Actual:");

        // Assert
        foreach (var expectedPersonResponse in expectedPersonResponses)
        {
            Assert.Contains(expectedPersonResponse, actualPersonResponses);

        }
    }
    #endregion

    #region GetPersonById() Tests
    [Fact]
    public async Task GetPersonById_NullPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = null;

        // Act
        PersonResponse? response = await _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetPersonById_EmptyPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = Guid.Empty;

        // Act
        PersonResponse? response = await _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetPersonById_InvalidPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = Guid.NewGuid();

        // Act
        PersonResponse? response = await _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetPersonById_ValidPersonId_ReturnsPersonResponse()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true,
        };
        PersonResponse expectedPersonResponse = await _sut.AddPerson(personAddRequest);

        // Act
        PersonResponse? actualPersonResponse =
            await _sut.GetPersonById(expectedPersonResponse.PersonId);

        // Assert
        Assert.NotNull(actualPersonResponse);
        Assert.Equal(expectedPersonResponse, actualPersonResponse);
    }
    #endregion

    #region GetFilteredPersons() Tests
    [Fact]
    public async Task GetFilteredPersons_IfNoPersons_ReturnsEmptyList()
    {
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(null, SearchOptions.ByName);

        // Assert
        Assert.Empty(actualPersonResponses);
    }

    [Theory]
    [InlineData(null, SearchOptions.ByName)]
    [InlineData("", SearchOptions.ByName)]
    [InlineData(null, SearchOptions.ByEmail)]
    [InlineData("", SearchOptions.ByEmail)]
    [InlineData(null, SearchOptions.ByAddress)]
    [InlineData("", SearchOptions.ByAddress)]
    [InlineData(null, SearchOptions.ByGender)]
    [InlineData("", SearchOptions.ByGender)]
    [InlineData(null, SearchOptions.ByDateOfBirth)]
    [InlineData("", SearchOptions.ByDateOfBirth)]
    [InlineData(null, SearchOptions.ByCountry)]
    [InlineData("", SearchOptions.ByCountry)]
    [InlineData(null, SearchOptions.ByReceiveNewsLetters)]
    [InlineData("", SearchOptions.ByReceiveNewsLetters)]
    [InlineData(null, SearchOptions.GreaterThanAge)]
    [InlineData("", SearchOptions.GreaterThanAge)]
    [InlineData(null, SearchOptions.LessThanAge)]
    [InlineData("", SearchOptions.LessThanAge)]
    [InlineData(null, SearchOptions.EqualToAge)]
    [InlineData("", SearchOptions.EqualToAge)]
    public async Task GetFilteredPersons_NullOrEmptySearchStringOrNoneOption_ReturnsAllPersons(string? search, SearchOptions option)
    {
        // Arrange
        List<PersonResponse> allPersonResponses = await AddSamplePersons();

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, option);

        // Assert
        allPersonResponses.ForEach(expectedPerson =>
            Assert.Contains(expectedPerson, actualPersonResponses));


    }

    [Theory]
    [InlineData("tes", "test3", "test4")]
    [InlineData("ALAN", "Alan S")]
    [InlineData("zzz")]
    public async Task GetFilteredPersons_ByName_ReturnsMatchedCaseInsensitiveSubstring(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByName);

        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("canada", "Adam W", "test4")]
    [InlineData("USA", "Alan S")]
    [InlineData("zzz")]
    public async Task GetFilteredPersons_ByCountry_ReturnsMatchedCaseInsensitiveSubstring(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByCountry);

        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("example.com", "Adam W", "test3", "test4")]
    [InlineData("gmail", "Alan S")]
    [InlineData("zzz")]
    public async Task GetFilteredPersons_ByEmail_ReturnsMatchedCaseInsensitiveSubstring(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByEmail);

        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("sam", "test3", "test4")]
    [InlineData("ROAD", "Alan S")]
    [InlineData("StreeT", "Adam W")]
    [InlineData("zzz")]
    public async Task GetFilteredPersons_ByAddress_ReturnsMatchedCaseInsensitiveSubstring(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();

        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByAddress);

        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("2007", "test3", "Alan S")]
    [InlineData("-01-", "test3", "Alan S", "test4")]
    [InlineData("2007-01-01", "test3", "Alan S")]
    [InlineData("2000-01-02")]
    [InlineData("1900")]
    public async Task GetFilteredPersons_ByDateOfBirth_MatchesSubstringOfIsoDate(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByDateOfBirth);
        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("15")]
    [InlineData("20")]
    [InlineData("25")]
    [InlineData("30")]
    [InlineData("40")]
    [InlineData("50")]
    [InlineData("100")]
    [InlineData("500")]
    public async Task GetFilteredPersons_GreaterThanAge_ReturnsPersonsWhoseAgeIsGreaterThan(
        string search)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.GreaterThanAge);
        // Assert
        Assert.All(actualPersonResponses, p => Assert.True(p.Age > int.Parse(search)));
    }

    [Theory]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("15")]
    [InlineData("20")]
    [InlineData("25")]
    [InlineData("30")]
    [InlineData("40")]
    [InlineData("50")]
    [InlineData("100")]
    [InlineData("500")]
    public async Task GetFilteredPersons_LessThanAge_ReturnsPersonsWhoseAgeIsLessThan(
        string search)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.LessThanAge);
        // Assert
        Assert.All(actualPersonResponses, p => Assert.True(p.Age < int.Parse(search)));
    }

    [Fact]
    public async Task GetFilteredPersons_EqualToAge_ReturnsPersonsWhoseAgeMatchesExactly()
    {
        // Arrange
        List<PersonResponse> expectedPersonResponses = await AddSamplePersons();
        // This and one other person have this age
        var age1 = expectedPersonResponses.First(p => p.PersonName == "Alan S").Age;
        // Only one with this age
        var age2 = expectedPersonResponses.First(p => p.PersonName == "Adam W").Age;

        // Act
        List<PersonResponse> actualPersonResponsesAge1 = await _sut.GetFilteredPersons(age1.ToString(), SearchOptions.EqualToAge);
        List<PersonResponse> actualPersonResponsesAge2 = await _sut.GetFilteredPersons(age2.ToString(), SearchOptions.EqualToAge);

        // Assert
        Assert.All(actualPersonResponsesAge1, p => Assert.Equal(age1, p.Age));
        Assert.All(actualPersonResponsesAge2, p => Assert.Equal(age2, p.Age));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("26x")]
    [InlineData(" ")]
    [InlineData("12 34")]
    public async Task GetFilteredPersons_AgeOptions_WithNonNumericSearch_ThrowsArgumentException(string search)
    {
        var options = new[]
        {
            SearchOptions.GreaterThanAge,
            SearchOptions.LessThanAge,
            SearchOptions.EqualToAge
        };
        foreach (var option in options)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.GetFilteredPersons(search, option));
            Assert.Contains("valid number", ex.Message);
        }
    }

    [Theory]
    [InlineData("male", "test3", "Alan S")]
    [InlineData("MALE", "test3", "Alan S")]
    [InlineData("female", "Adam W")]
    [InlineData("Other", "test4")]
    public async Task GetFilteredPersons_ByGender_WhenSearchIsEnumName_MatchesExactly(
        string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByGender);
        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("ale", "test3", "Alan S", "Adam W")]
    [InlineData("emal", "Adam W")]
    [InlineData("zzz")]
    public async Task GetFilteredPersons_ByGender_WhenSearchIsNotEnumName_FallsBackToSubstring(string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByGender);
        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("true", "Alan S", "test3")]
    [InlineData("TRUE", "Alan S", "test3")]
    [InlineData("false", "Adam W", "test4")]
    [InlineData("FaLSe", "Adam W", "test4")]
    public async Task GetFilteredPersons_ByReceiveNewsLetters_FiltersOnBoolean(string search, params string[] expected)
    {
        // Arrange
        AddSamplePersons();
        // Act
        List<PersonResponse> actualPersonResponses = await _sut.GetFilteredPersons(search, SearchOptions.ByReceiveNewsLetters);
        // Assert
        Assert.Equal(
            expected.OrderBy(e => e),
            actualPersonResponses.Select(p => p.PersonName).OrderBy(n => n));
    }

    [Theory]
    [InlineData("yes")]
    [InlineData("1")]
    [InlineData("abc")]
    public async Task GetFilteredPersons_ByReceiveNewsLetters_AnythingOtherThanTrueIsTreatedAsFalse(string search)
    {
        // Arrange
        AddSamplePersons();

        // Act
        var responses = await _sut.GetFilteredPersons(search, SearchOptions.ByReceiveNewsLetters);

        // Assert
        Assert.All(responses, p => Assert.False(p.ReceiveNewsLetters));
    }

    // ---------- Unsupported option ----------

    [Theory]
    [InlineData((SearchOptions)999)]
    [InlineData((SearchOptions)30)]
    [InlineData((SearchOptions)(-1))]
    [InlineData((SearchOptions)(-10))]
    public async Task GetFilteredPersons_UnknownEnumValue_ThrowsArgumentOutOfRangeException(SearchOptions searchOption)
    {
        // Arrange
        AddSamplePersons();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _sut.GetFilteredPersons("anything", searchOption);
        });
    }
    #endregion

    #region GetSortedPersons() Tests
    [Fact]
    public void GetSortedPersons_PersonsToSortIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<PersonResponse> personsToSort = null!;
        FilterOptions option = FilterOptions.ByName;
        ListSortDirection order = ListSortDirection.Ascending;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _sut.GetSortedPersons(personsToSort, option, order));
    }

    [Fact]
    public void GetSortedPersons_PersonsToSortIsEmpty_ReturnsEmptyList()
    {
        // Arrange
        IEnumerable<PersonResponse> personsToSort = new List<PersonResponse>();
        FilterOptions option = FilterOptions.ByName;
        ListSortDirection order = ListSortDirection.Ascending;

        // Act
        List<PersonResponse> result = _sut.GetSortedPersons(personsToSort, option, order);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSortedPersons_SingleElement_ReturnsSameElement()
    {
        // Arrange
        List<PersonResponse> personsToSort = (await AddSamplePersons()).Take(1).ToList();
        FilterOptions option = FilterOptions.ByName;
        ListSortDirection order = ListSortDirection.Ascending;

        // Act
        List<PersonResponse> result = _sut.GetSortedPersons(personsToSort, option, order);

        // Assert
        Assert.Single(result);
        Assert.Equal(personsToSort.First(), result.First());
    }

    [Fact]
    public async Task GetSortedPersons_TwoElements_ReturnsSortedList()
    {
        // Arrange
        List<PersonResponse> personsToSort = (await AddSamplePersons()).Take(2).ToList();
        FilterOptions option = FilterOptions.ByName;
        ListSortDirection order = ListSortDirection.Ascending;
        // Act
        List<PersonResponse> sortedResult = _sut.GetSortedPersons(personsToSort, option, order);
        // Assert
        Assert.Equal(2, sortedResult.Count);
        Assert.True(string.Compare(
            sortedResult[0].PersonName,
            sortedResult[1].PersonName) <= 0);
    }

    // Fields that are or can be sorted as string: Name, Email, Gender, CountryName, Address, ReceiveNewsLetters (as string)
    // Sorts in both directions
    public static IEnumerable<object[]> SortableFieldsData()
    {
        var selectors = new (FilterOptions option, Func<PersonResponse, string?> keySelector)[]
        {
            (FilterOptions.ByName, p => p?.PersonName),
            (FilterOptions.ByEmail, p => p?.Email),
            (FilterOptions.ByGender, p => p?.Gender),
            (FilterOptions.ByCountry, p => p?.CountryName),
            (FilterOptions.ByAddress, p => p?.Address),
            (FilterOptions.ByReceiveNewsLetters, p => p?.ReceiveNewsLetters.ToString())
        };

        var sortingOrders = new[] { ListSortDirection.Ascending, ListSortDirection.Descending };

        foreach (var (option, keySelector) in selectors)
        {
            foreach (var order in sortingOrders)
            {
                yield return new object[] { option, keySelector, order };
            }
        }
    }

    [Theory]
    [MemberData(nameof(SortableFieldsData))]
    public async Task GetSortedPersons_SortByStringFields_ReturnsSortedList(
        FilterOptions option,
        Func<PersonResponse, string?> keySelector,
        ListSortDirection order)
    {
        // Arrange
        List<PersonResponse> input = await AddSamplePersons(addMore: true);
        var snapShotOfInput = input.ToList();

        // Act
        List<PersonResponse> sortedResult = _sut.GetSortedPersons(input, option, order);

        // Assert
        Assert.Equal(snapShotOfInput.Count, sortedResult.Count);
        Assert.True(snapShotOfInput.ToHashSet().SetEquals(sortedResult),
            "All original elements should be present in the sorted result.");
        Assert.Equal(snapShotOfInput, input);

        for (int i = 0; i < sortedResult.Count - 1; i++)
        {
            if (order == ListSortDirection.Ascending)
            {
                Assert.True(string.Compare(
                    keySelector(sortedResult[i]),
                    keySelector(sortedResult[i + 1])) <= 0);
            }
            else
            {
                Assert.True(string.Compare(
                    keySelector(sortedResult[i]),
                    keySelector(sortedResult[i + 1])) >= 0);
            }
        }

        // Displaying the sorted result for debugging purposes
        PrintPersonResponses(sortedResult, $"Sorted by {option} in {order} order:");
    }

    [Theory]
    [InlineData(ListSortDirection.Ascending)]
    [InlineData(ListSortDirection.Descending)]
    public async Task GetSortedPersons_SortByAge_ReturnsSortedList(
        ListSortDirection order)
    {
        // Arrange
        List<PersonResponse> input = await AddSamplePersons(addMore: true);
        var snapShotOfInput = input.ToList();
        FilterOptions option = FilterOptions.ByAge;

        // Act
        List<PersonResponse> sortedResult = _sut.GetSortedPersons(input, option, order);

        // Assert
        Assert.NotSame(input, sortedResult);
        Assert.Equal(snapShotOfInput, input);
        Assert.Equal(snapShotOfInput.Count, sortedResult.Count);
        Assert.True(snapShotOfInput.ToHashSet().SetEquals(sortedResult),
            "All original elements should be present in the sorted result.");
        var ages = sortedResult.Select(p => p.Age).ToList();
        var expectedAges = order == ListSortDirection.Ascending
            ? ages.OrderBy(a => a).ToList()
            : ages.OrderByDescending(a => a).ToList();
        Assert.Equal(expectedAges, ages);

        // Displaying the sorted result for debugging purposes
        PrintPersonResponses(sortedResult, $"Sorted by Age in {order} order:");
    }

    // Sort by DateOfBirth, which is nullable DateTime
    [Theory]
    [InlineData(ListSortDirection.Ascending)]
    [InlineData(ListSortDirection.Descending)]
    public async Task GetSortedPersons_SortByDateOfBirth_ReturnsSortedList(
    ListSortDirection order)
    {
        // Arrange
        List<PersonResponse> input = await AddSamplePersons(addMore: true);
        var snapShotOfInput = input.ToList();
        FilterOptions option = FilterOptions.ByDateOfBirth;

        // Act
        List<PersonResponse> sortedResult = _sut.GetSortedPersons(input, option, order);

        // Assert
        Assert.NotSame(input, sortedResult);
        Assert.Equal(snapShotOfInput, input);
        Assert.Equal(snapShotOfInput.Count, sortedResult.Count);
        Assert.True(snapShotOfInput.ToHashSet().SetEquals(sortedResult),
            "All original elements should be present in the sorted result.");
        var datesOfBirth = sortedResult.Select(p => p.DateOfBirth).ToList();
        var expectedDatesOfBirth = order == ListSortDirection.Ascending
            ? datesOfBirth.OrderBy(d => d).ToList()
            : datesOfBirth.OrderByDescending(d => d).ToList();
        Assert.Equal(expectedDatesOfBirth, datesOfBirth);

        // Displaying the sorted result for debugging purposes
        PrintPersonResponses(sortedResult, $"Sorted by DateOfBirth in {order} order:");
    }

    [Theory]
    [InlineData((FilterOptions)999)]
    [InlineData((FilterOptions)30)]
    [InlineData((FilterOptions)(-1))]
    [InlineData((FilterOptions)(-10))]
    public async Task GetSortedPersons_UnknownEnumValue_ThrowsArgumentOutOfRangeException(FilterOptions filterOption)
    {
        // Arrange
        List<PersonResponse> input = await AddSamplePersons(addMore: true);
        ListSortDirection order = ListSortDirection.Ascending;
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.GetSortedPersons(input, filterOption, order));
    }

    #endregion

    #region UpdatePerson Tests
    [Fact]
    public async Task UpdatePerson_NullPersonUpdateRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.UpdatePerson(personUpdateRequest));
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonId_ThrowsArgumentException()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = Guid.NewGuid()
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.UpdatePerson(personUpdateRequest));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdatePerson_InvalidPersonName_ThrowsArgumentException(
        string? personName)
    {
        // Arrange
        var personResponse = (await AddSamplePersons()).First();

        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.PersonName = personName;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.UpdatePerson(personUpdateRequest));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email.format")]
    public async Task UpdatePerson_InvalidEmail_ThrowsArgumentException(
        string? email)
    {
        // Arrange
        var personResponse = (await AddSamplePersons()).First();

        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.Email = email;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.UpdatePerson(personUpdateRequest));
    }

    [Fact]
    public async Task UpdatePerson_ValidUpdate_ReturnsUpdatedPersonResponse()
    {
        // Arrange
        var personResponseBefore = (await AddSamplePersons()).First();
        var personUpdateRequest = personResponseBefore.ToPersonUpdateRequest();
        personUpdateRequest.PersonName = "Updated Name";
        personUpdateRequest.Email = "updated@example.com";
        personUpdateRequest.Gender = GenderOptions.Male;
        personUpdateRequest.ReceiveNewsLetters = true;
        personUpdateRequest.Address = "Updated Address";
        Guid personId = personResponseBefore.PersonId;

        // Act
        PersonResponse personResponseExpectedAfter = await _sut.UpdatePerson(personUpdateRequest);
        PersonResponse? personResponseActualAfter = await _sut.GetPersonById(personId);

        // Assert
        Assert.NotEqual(personResponseBefore, personResponseExpectedAfter);
        Assert.Equal(personResponseExpectedAfter, personResponseActualAfter);
    }
    #endregion

    #region DeletePerson Tests
    [Fact]
    public async Task DeletePerson_NullPersonId_ThrowsArgumentNullException()
    {
        // Arrange
        await AddSamplePersons();
        Guid? invalidPersonId = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.DeletePerson(invalidPersonId));
    }

    [Fact]
    public async Task DeletePerson_InvalidPersonId_ReturnsFalse()
    {
        // Arrange
        await AddSamplePersons();
        Guid invalidPersonId = Guid.NewGuid();

        // Act
        bool result = await _sut.DeletePerson(invalidPersonId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeletePerson_ValidPersonId_ReturnsTrueAndValuePersists()
    {
        // Arrange
        var personToDelete = (await AddSamplePersons()).First();
        Guid personId = personToDelete.PersonId;

        // Act
        var personResponseBefore = await _sut.GetPersonById(personId);
        bool result = await _sut.DeletePerson(personId);
        var personResponseAfter = await _sut.GetPersonById(personId);

        // Assert
        Assert.True(result);
        Assert.NotNull(personResponseBefore);
        Assert.Null(personResponseAfter);
        Assert.True(!personResponseBefore?.Equals(personResponseAfter));
    }
    #endregion

    #region Helper Methods
    private void PrintPersonResponses(List<PersonResponse> personResponses, string title)
    {
        _testOutputHelper.WriteLine(title);
        foreach (var personResponse in personResponses)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }
    }

    private async Task<List<PersonResponse>> AddSamplePersons(bool addMore = false)
    {
        CountryResponse countryUSA =  await _countryService.AddCountry(new CountryAddRequest() { CountryName = "USA" });
        CountryResponse countryCanada = await _countryService.AddCountry(new CountryAddRequest() { CountryName = "Canada" });

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Alan S",
            Email = "alan@gmail.com",
            Address = "4 oiler road",
            CountryId = countryUSA.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = new DateTime(2007, 1, 1),
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Adam W",
            Email = "adam@example.com",
            Address = "3 main street",
            CountryId = countryCanada.CountryId,
            Gender = GenderOptions.Female,
            DateOfBirth = new DateTime(2011, 5, 1),
            ReceiveNewsLetters = false,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "test3",
            Email = "test3@example.com",
            Address = "sample address 3",
            CountryId = null,
            Gender = GenderOptions.Male,
            DateOfBirth = new DateTime(2007, 1, 1),
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest4 = new PersonAddRequest()
        {
            PersonName = "test4",
            Email = "test4@example.com",
            Address = "sample address 4",
            CountryId = countryCanada.CountryId,
            Gender = GenderOptions.Other,
            DateOfBirth = new DateTime(2000, 1, 1),
            ReceiveNewsLetters = false,
        };

        List<PersonResponse> expectedPersonResponses = new List<PersonResponse>
        {
            await _sut.AddPerson(personAddRequest1),
            await _sut.AddPerson(personAddRequest2),
            await _sut.AddPerson(personAddRequest3),
            await _sut.AddPerson(personAddRequest4)
        };

        if (!addMore) return expectedPersonResponses;

        // Needed for GetSortedPersons() tests
        PersonAddRequest personAddRequest5 = new PersonAddRequest()
        {
            PersonName = "barker",
            Email = "barker@example.com",
            Address = "sample address 3",
            CountryId = countryCanada.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest6 = new PersonAddRequest()
        {
            PersonName = "Parker Null",
            Email = "null@parker.com",
            Address = null,
            CountryId = null,
            Gender = null,
            DateOfBirth = null,
            ReceiveNewsLetters = false,
        };

        expectedPersonResponses.Add(await _sut.AddPerson(personAddRequest5));
        expectedPersonResponses.Add(await _sut.AddPerson(personAddRequest6));

        return expectedPersonResponses;
    }
    #endregion
}
