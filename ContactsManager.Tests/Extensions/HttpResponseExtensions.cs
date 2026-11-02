using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ContactsManager.Tests.Extensions;

public static class HttpResponseExtensions
{
    public static async Task<IHtmlDocument> GetDocumentAsync(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var htmlContent = await response.Content.ReadAsStringAsync();

        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(htmlContent);

        return document;
    }
}
