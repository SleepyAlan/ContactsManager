using AngleSharp.Html.Dom;
using ContactsManager.Tests.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ContactsManager.Tests;

public class PersonsControllerIntegrationTest
{
    private readonly HttpClient _client;
    public PersonsControllerIntegrationTest()
    {
        // Arrange
        var webApplicationFactory = new CustomWebApplicationFactory();
        _client = webApplicationFactory.CreateClient();
    }

    #region Index

    [Fact]
    public async Task Index_HttpGetRequest_ReturnsViewResult()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Index");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();
        Assert.NotNull(document.GetElementById("persons-filter"));
        Assert.NotNull(document.GetElementById("persons-table"));
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_HttpGetRequest_ReturnsViewResult()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Create");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();
        Assert.NotNull(document.GetElementById("PersonName"));
        Assert.NotNull(document.GetElementById("Email"));
        Assert.NotNull(document.GetElementById("DateOfBirth"));
        Assert.NotNull(document.GetElementById("Gender"));
        Assert.NotNull(document.GetElementById("CountryId"));
        Assert.NotNull(document.GetElementById("ReceiveNewsLetters"));
        Assert.NotNull(document.GetElementById("Address"));
    }

    [Fact]
    public async Task Create_HttpPostRequestWithValidData_ReturnsRedirectToActionResult()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "PersonName", "John Doe" },
            { "Email", "johndoe@example.com" }
        };

        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Persons/Create", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("/Persons/Index", response.RequestMessage?.RequestUri?.AbsolutePath.ToString());
    }

    [Fact]
    public async Task Create_HttpPostRequestWithInvalidData_ReturnsViewResultWithModelErrors()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "PersonName", "" }, // Invalid: Name is required
            { "Email", "invalid-email" } // Invalid: Not a valid email format
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Persons/Create", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("/Persons/Create", response.RequestMessage?.RequestUri?.AbsolutePath.ToString());
        IHtmlDocument document = await response.GetDocumentAsync();

        // Assert that the validation messages are present
        var validationSpanName = document
            .QuerySelector("span[data-valmsg-for='PersonName']");
        var validationSpanEmail = document
            .QuerySelector("span[data-valmsg-for='Email']");
        Assert.Equal("true", validationSpanName?.GetAttribute("data-valmsg-replace"));
        Assert.Equal("true", validationSpanEmail?.GetAttribute("data-valmsg-replace"));
    }
    #endregion

    #region Update

    [Fact]
    public async Task Update_HttpGetRequestWithValidPersonId_ReturnsViewResult()
    {
        // Arrange - Create a person first
        var createFormData = new Dictionary<string, string>
        {
            { "PersonName", "Jane Smith" },
            { "Email", "jane@example.com" }
        };
        var createContent = new FormUrlEncodedContent(createFormData);

        await _client.PostAsync("/Persons/Create", createContent);

        // Get the index page to extract the PersonId from the first person's Details link
        var indexResponse = await _client.GetAsync("/Persons/Index");
        var indexDocument = await indexResponse.GetDocumentAsync();
        var personLinks = indexDocument.QuerySelectorAll("a[data-row-link]");
        Assert.NotEmpty(personLinks);
        var firstPersonLink = personLinks.FirstOrDefault();
        var href = firstPersonLink?.GetAttribute("href");
        // Extract GUID from URL like /Persons/Details/550e8400-e29b-41d4-a716-446655440000
        var personIdMatch = Regex.Match(href ?? "", @"Details/([a-f0-9\-]+)");
        Assert.True(personIdMatch.Success);
        var personId = personIdMatch.Groups[1].Value;


        // Act
        var response = await _client.GetAsync($"/Persons/Update/{personId}");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();
        Assert.NotNull(document.GetElementById("PersonName"));
        Assert.NotNull(document.GetElementById("Email"));
        Assert.NotNull(document.GetElementById("DateOfBirth"));
        Assert.NotNull(document.GetElementById("Gender"));
        Assert.NotNull(document.GetElementById("CountryId"));
        Assert.NotNull(document.GetElementById("ReceiveNewsLetters"));
        Assert.NotNull(document.GetElementById("Address"));
    }

    [Fact]
    public async Task Update_HttpGetRequestWithInvalidPersonId_ReturnsNotFound()
    {
        // Arrange
        var invalidPersonId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/Persons/Update/{invalidPersonId}");

        // Assert
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.NotFound,
            $"Expected NotFound, but got {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Update_HttpPostRequestWithValidData_ReturnsRedirectToIndex()
    {
        // Arrange - Create a person first
        var createFormData = new Dictionary<string, string>
        {
            { "PersonName", "Bob Johnson" },
            { "Email", "bob@example.com" }
        };
        var createContent = new FormUrlEncodedContent(createFormData);
        await _client.PostAsync("/Persons/Create", createContent);

        // Get the person ID
        var indexResponse = await _client.GetAsync("/Persons/Index");
        var indexDocument = await indexResponse.GetDocumentAsync();
        var personId = ExtractPersonIdFromIndexDocument(indexDocument);

        // Update with valid data
        var updateFormData = new Dictionary<string, string>
        {
            { "PersonId", personId },
            { "PersonName", "Bob Johnson Updated" },
            { "Email", "bob.updated@example.com" }
        };
        var updateContent = new FormUrlEncodedContent(updateFormData);

        // Act
        var response = await _client.PostAsync($"/Persons/Update/{personId}", updateContent);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("/Persons/Index", response.RequestMessage?.RequestUri?.AbsolutePath.ToString());
    }

    [Fact]
    public async Task Update_HttpPostRequestWithInvalidData_ReturnsViewResultWithModelErrors()
    {
        // Arrange - Create a person first
        var createFormData = new Dictionary<string, string>
        {
            { "PersonName", "Alice Brown" },
            { "Email", "alice@example.com" }
        };
        var createContent = new FormUrlEncodedContent(createFormData);
        await _client.PostAsync("/Persons/Create", createContent);

        // Get the person ID
        var indexResponse = await _client.GetAsync("/Persons/Index");
        var indexDocument = await indexResponse.GetDocumentAsync();
        var personId = ExtractPersonIdFromIndexDocument(indexDocument);

        // Update with invalid data
        var updateFormData = new Dictionary<string, string>
        {
            { "PersonId", personId },
            { "PersonName", "" }, // Invalid: Name is required
            { "Email", "invalid-email" } // Invalid: Not a valid email format
        };
        var updateContent = new FormUrlEncodedContent(updateFormData);

        // Act
        var response = await _client.PostAsync($"/Persons/Update/{personId}", updateContent);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal($"/Persons/Update/{personId}", response.RequestMessage?.RequestUri?.AbsolutePath.ToString());
        IHtmlDocument document = await response.GetDocumentAsync();

        // Assert that the validation messages are present
        var validationSpanName = document.QuerySelector("span[data-valmsg-for='PersonName']");
        var validationSpanEmail = document.QuerySelector("span[data-valmsg-for='Email']");
        Assert.Equal("true", validationSpanName?.GetAttribute("data-valmsg-replace"));
        Assert.Equal("true", validationSpanEmail?.GetAttribute("data-valmsg-replace"));
    }

    #endregion

    #region Details

    [Fact]
    public async Task Details_HttpGetRequestWithValidPersonId_ReturnsViewResult()
    {
        // Arrange - Create a person first
        var createFormData = new Dictionary<string, string>
        {
            { "PersonName", "Charlie Davis" },
            { "Email", "charlie@example.com" }
        };
        var createContent = new FormUrlEncodedContent(createFormData);
        await _client.PostAsync("/Persons/Create", createContent);

        // Get the person ID
        var indexResponse = await _client.GetAsync("/Persons/Index");
        var indexDocument = await indexResponse.GetDocumentAsync();
        var personId = ExtractPersonIdFromIndexDocument(indexDocument);

        // Act
        var response = await _client.GetAsync($"/Persons/Details/{personId}");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();
        var headings = document.QuerySelectorAll("h3");
        Assert.NotEmpty(headings);
        Assert.Contains(headings, h => h.TextContent.Contains("Charlie Davis"));
    }

    [Fact]
    public async Task Details_HttpGetRequestWithInvalidPersonId_ReturnsNotFound()
    {
        // Arrange
        var invalidPersonId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/Persons/Details/{invalidPersonId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_HttpPostRequestWithValidPersonId_RedirectsToIndexAndRemovesContact()
    {
        // Arrange - Create a person first
        var createFormData = new Dictionary<string, string>
        {
            { "PersonName", "David Evans" },
            { "Email", "david@example.com" }
        };
        var createContent = new FormUrlEncodedContent(createFormData);
        await _client.PostAsync("/Persons/Create", createContent);

        // Get the person ID
        var indexResponse = await _client.GetAsync("/Persons/Index");
        var indexDocument = await indexResponse.GetDocumentAsync();
        var personId = ExtractPersonIdFromIndexDocument(indexDocument);

        // Get the Details page to extract the XSRF token
        var detailsResponse = await _client.GetAsync($"/Persons/Details/{personId}");
        var detailsDocument = await detailsResponse.GetDocumentAsync();
        var xsrfToken = detailsDocument.QuerySelector("input[name='__RequestVerificationToken']")?.GetAttribute("value");
        Assert.NotNull(xsrfToken);

        // Act - Delete the person with XSRF token
        var deleteFormData = new Dictionary<string, string>
        {
            { "__RequestVerificationToken", xsrfToken }
        };
        var deleteContent = new FormUrlEncodedContent(deleteFormData);
        var deleteResponse = await _client.PostAsync($"/Persons/Delete/{personId}", deleteContent);

        // Assert - Verify redirect to Index
        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal("/Persons/Index", deleteResponse.RequestMessage?.RequestUri?.AbsolutePath.ToString());

        // Assert - Verify the contact is no longer accessible via Details endpoint
        var checkDetailsResponse = await _client.GetAsync($"/Persons/Details/{personId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, checkDetailsResponse.StatusCode);
    }

    #endregion

    #region Table

    [Fact]
    public async Task Table_HttpGetRequest_ReturnsPartialViewWithTableContent()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Table");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify the table structure is present
        var table = document.QuerySelector("table");
        Assert.NotNull(table);

        var tbody = document.QuerySelector("tbody");
        Assert.NotNull(tbody);

        var thead = document.QuerySelector("thead");
        Assert.NotNull(thead);
    }

    #endregion

    #region Export (PDF, CSV, Excel)

    [Fact]
    public async Task PersonsPDF_HttpGetRequest_ReturnsFileWithCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/Persons/PersonsPDF");

        // Assert
        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.NotNull(contentType);
        Assert.Contains("application/pdf", contentType);
    }

    [Fact]
    public async Task PersonsCSV_HttpGetRequest_ReturnsFileWithCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/Persons/PersonsCSV");

        // Assert
        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.NotNull(contentType);
        Assert.Contains("text/csv", contentType);
    }

    [Fact]
    public async Task PersonsExcel_HttpGetRequest_ReturnsFileWithCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/Persons/PersonsExcel");

        // Assert
        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.NotNull(contentType);
        Assert.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", contentType);
    }

    #endregion

    #region Helper Methods

    private string ExtractPersonIdFromIndexDocument(IHtmlDocument indexDocument)
    {
        var personLinks = indexDocument.QuerySelectorAll("a[data-row-link]");
        var firstPersonLink = personLinks.FirstOrDefault();
        var href = firstPersonLink?.GetAttribute("href");
        var personIdMatch = Regex.Match(href ?? "", @"Details/([a-f0-9\-]+)");
        Assert.True(personIdMatch.Success);
        return personIdMatch.Groups[1].Value;
    }

    #endregion
}
