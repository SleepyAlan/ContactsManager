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
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    #endregion
}
