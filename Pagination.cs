// Adopted from https://github.com/LinkedInLearning/php-techniques-pagination-2884225/blob/main/php_pagination/private/classes/pagination.class.php
// Credit to Kevin Skoglund https://github.com/kevinskoglund
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class Pagination
{
    private readonly int currentPage;
    private readonly int itemsPerPage;
    private readonly int totalItemsCount;
    private readonly int? previousPage;
    private readonly int? nextPage;
    private readonly int totalPages;

    public int Offset { get; }
    public int ItemPerPages { get => itemsPerPage; }

    public Pagination(int totalItemsCount, int currentPage, int itemsPerPage = 20)
    {
        this.totalItemsCount = totalItemsCount;
        this.itemsPerPage = itemsPerPage;
        totalPages = (int)Math.Ceiling((decimal)(this.totalItemsCount / this.itemsPerPage));

        this.currentPage = currentPage < 1 || currentPage > totalPages ? 1 : currentPage;
        Offset = this.itemsPerPage * (this.currentPage - 1);

        var prev = this.currentPage - 1;
        previousPage = prev > 0 ? prev : null;

        var next = this.currentPage + 1;
        nextPage = next <= totalPages ? next : null;
    }

    public IHtmlContent GetPageLinks(string baseUrl, object? htmlAttributes = null)
    {
        var tag = new TagBuilder("div");
        tag.AddCssClass("ellipsis-pagination");
        var attributes = GetHtmlAttributeDictionaryOrNull(htmlAttributes);
        if (attributes != null)
        {
            tag.MergeAttributes(attributes);
        }

        if (totalPages > 1)
        {
            tag.InnerHtml.AppendHtml(GetPreviousLink(baseUrl));
            foreach (var link in GetNumberLinks(baseUrl))
            {
                tag.InnerHtml.AppendHtml(link);
            }
            tag.InnerHtml.AppendHtml(GetNextLink(baseUrl));
        }

        return tag;
    }

    private IHtmlContent GetPreviousLink(string url)
    {
        if (!previousPage.HasValue)
        {
            return new HtmlString(string.Empty);
        }

        var tag = new TagBuilder("a");
        tag.AddCssClass("previous");
        tag.Attributes.Add("href", $"{url}?page={previousPage}");
        tag.InnerHtml.AppendHtml("< Previous");
        return tag;
    }

    private IHtmlContent GetNextLink(string url)
    {
        if (!nextPage.HasValue)
        {
            return new HtmlString(string.Empty);
        }

        var tag = new TagBuilder("a");
        tag.AddCssClass("next");
        tag.Attributes.Add("href", $"{url}?page={nextPage}");
        tag.InnerHtml.AppendHtml("Next >");
        return tag;
    }

    private IEnumerable<IHtmlContent> GetNumberLinks(string url, int window = 2)
    {
        var gap = false;
        for (var i = 1; i <= totalPages; i++)
        {
            if (window > 0 && i > 1 + window && i < totalPages - window && Math.Abs(i - currentPage) > window)
            {
                if (!gap)
                {
                    var tag = new TagBuilder("span");
                    tag.AddCssClass("ellipsis");
                    tag.InnerHtml.AppendHtml("...");
                    yield return tag;
                    gap = true;
                }
                continue;
            }

            gap = false;
            if (currentPage == i)
            {
                var tag = new TagBuilder("span");
                tag.AddCssClass("current");
                tag.InnerHtml.Append(i.ToString());
                yield return tag;
            }
            else
            {
                var tag = new TagBuilder("a");
                tag.AddCssClass("number");
                tag.Attributes.Add("href", $"{url}?page={i}");
                tag.InnerHtml.Append(i.ToString());
                yield return tag;
            }
        }
    }

    // Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
    private static IDictionary<string, object?>? GetHtmlAttributeDictionaryOrNull(object? htmlAttributes)
    {
        return htmlAttributes == null
            ? null
            : htmlAttributes as IDictionary<string, object?> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
    }
}
