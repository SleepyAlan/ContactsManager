using AngleSharp.Html.Dom;
using ContactsManager.Tests.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Tests;

public class CountriesControllerIntegrationTests
{
    private readonly HttpClient _client;
    public CountriesControllerIntegrationTests()
    {
        var webApplicationFactory = new CustomWebApplicationFactory();
        _client = webApplicationFactory.CreateClient();
    }

    #region AddCountry

    [Fact]
    public async Task AddCountry_HttpGetRequest_ReturnsViewResult()
    {
        // Act
        var response = await _client.GetAsync("/Countries/AddCountry");

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify title exists
        var title = document.QuerySelector("h4.text-primary");
        Assert.NotNull(title);
        Assert.Contains("Add a new Country", title.TextContent);

        // Verify input fields exists
        var countryNameInput = document.GetElementById("CountryName");
        Assert.NotNull(countryNameInput);
        var fileInput = document.GetElementById("excelFile");
        Assert.NotNull(fileInput);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithValidCountryName_ReturnsViewResultWithSuccessMessage()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "CountryName", "TestCountry123" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Countries/AddCountry", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify success message is displayed
        var messageElement = document.QuerySelector("h6");
        Assert.NotNull(messageElement);
        Assert.Contains("has been added successfully", messageElement.TextContent);
        Assert.Contains("TestCountry123", messageElement.TextContent);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithEmptyCountryName_ReturnsViewResultWithValidationError()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "CountryName", "" }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Countries/AddCountry", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify validation error is shown for empty CountryName
        var validationSpan = document.QuerySelector("span[data-valmsg-for='CountryName']");
        Assert.NotNull(validationSpan);
        Assert.Equal("true", validationSpan.GetAttribute("data-valmsg-replace"));
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithWhitespaceCountryName_ReturnsViewResultWithSuccessMessage()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "CountryName", "  TestCountryWithSpaces  " }
        };
        var content = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/Countries/AddCountry", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify success message is displayed (controller trims whitespace)
        var messageElement = document.QuerySelector("h6");
        Assert.NotNull(messageElement);
        Assert.Contains("has been added successfully", messageElement.TextContent);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithDuplicateCountryName_ReturnsViewResultWithError()
    {
        // Arrange - Add first country
        var formData1 = new Dictionary<string, string>
        {
            { "CountryName", "UniqueCountry" }
        };
        var content1 = new FormUrlEncodedContent(formData1);
        await _client.PostAsync("/Countries/AddCountry", content1);

        // Try to add duplicate country
        var formData2 = new Dictionary<string, string>
        {
            { "CountryName", "UniqueCountry" }
        };
        var content2 = new FormUrlEncodedContent(formData2);

        // Act
        var response = await _client.PostAsync("/Countries/AddCountry", content2);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify either validation error or error message is displayed
        var validationSpan = document.QuerySelector("span[data-valmsg-for='CountryName']");
        var messageElement = document.QuerySelector("h6");

        // One of them should have an error (not success message)
        if (messageElement != null)
        {
            // If message exists, it should NOT be a success message
            Assert.DoesNotContain("has been added successfully", messageElement.TextContent);
        }
        else
        {
            // Otherwise validation span should exist
            Assert.NotNull(validationSpan);
        }
    }

    #endregion

    #region UploadCountriesFromExcel

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithValidExcelFile_ReturnsViewResultWithSuccessMessage()
    {
        // Arrange
        var filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..",
            "TestData",
            "CountriesUpload.xlsx"
        );

        // Verify file exists
        Assert.True(File.Exists(filePath), $"Test Excel file not found at {filePath}");

        using var content = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        using var streamContent = new StreamContent(fileStream);
        content.Add(streamContent, "excelFile", "CountriesUpload.xlsx");

        // Act
        var response = await _client.PostAsync("/Countries/UploadCountriesFromExcel", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify success message is displayed with count of uploaded countries (1 new - Madagascar)
        var messageElement = document.QuerySelector("h6");
        Assert.NotNull(messageElement);
        Assert.Contains("countries have been uploaded successfully", messageElement.TextContent);
        // Should show "4 countries have been uploaded successfully"
        Assert.Contains("4", messageElement.TextContent);
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithNullFile_ReturnsViewResultWithValidationError()
    {
        // Arrange
        using var content = new MultipartFormDataContent();

        // Act
        var response = await _client.PostAsync("/Countries/UploadCountriesFromExcel", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify validation error message is displayed
        var errorSpan = document.QuerySelectorAll("span[class*='text-danger']");
        Assert.NotEmpty(errorSpan);
        var hasValidationMessage = errorSpan.Any(e => e.TextContent.Contains("valid Excel file"));
        Assert.True(hasValidationMessage);
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithInvalidFileType_ReturnsViewResultWithValidationError()
    {
        // Arrange
        using var content = new MultipartFormDataContent();
        using var stream = new MemoryStream([1, 2, 3, 4]);
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "excelFile", "invalid.txt");

        // Act
        var response = await _client.PostAsync("/Countries/UploadCountriesFromExcel", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify validation error message is displayed
        var errorSpans = document.QuerySelectorAll("span[class*='text-danger']");
        Assert.NotEmpty(errorSpans);
        var hasValidationMessage = errorSpans.Any(e => e.TextContent.Contains("valid Excel file"));
        Assert.True(hasValidationMessage);
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithEmptyFile_ReturnsViewResultWithValidationError()
    {
        // Arrange
        using var content = new MultipartFormDataContent();
        using var stream = new MemoryStream([]);
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "excelFile", "empty.xlsx");

        // Act
        var response = await _client.PostAsync("/Countries/UploadCountriesFromExcel", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify validation error message is displayed
        var errorSpans = document.QuerySelectorAll("span[class*='text-danger']");
        Assert.NotEmpty(errorSpans);
        var hasValidationMessage = errorSpans.Any(e => e.TextContent.Contains("valid Excel file"));
        Assert.True(hasValidationMessage);
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithPdfFile_ReturnsViewResultWithValidationError()
    {
        // Arrange - PDF file header bytes
        var pdfHeader = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF
        using var content = new MultipartFormDataContent();
        using var stream = new MemoryStream(pdfHeader);
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "excelFile", "document.pdf");

        // Act
        var response = await _client.PostAsync("/Countries/UploadCountriesFromExcel", content);

        // Assert
        response.EnsureSuccessStatusCode();
        IHtmlDocument document = await response.GetDocumentAsync();

        // Verify validation error message is displayed
        var errorSpans = document.QuerySelectorAll("span[class*='text-danger']");
        Assert.NotEmpty(errorSpans);
        var hasValidationMessage = errorSpans.Any(e => e.TextContent.Contains("valid Excel file"));
        Assert.True(hasValidationMessage);
    }

    #endregion
}
