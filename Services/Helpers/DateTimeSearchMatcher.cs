using System.Globalization;

namespace Services.Helpers;

internal static class DateTimeSearch
{
    private static readonly CultureInfo Culture =
        CultureInfo.InvariantCulture;

    private static readonly string[] DateFormats =
    {
        "yyyy-M-d",
        "yyyy-MM-dd",
        "yyyy-M",
        "yyyy-MM",
        "M/d/yyyy",
        "MM/dd/yyyy",
        "M-d-yyyy",
        "MM-dd-yyyy",
        "MMM d, yyyy",
        "MMMM d, yyyy",
        "MMM d",
        "MMMM d",
        "d MMM yyyy",
        "d MMMM yyyy"
    };

    public static bool Matcher(DateTime? date, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return true;
        if (date is null) return false;

        search = search.Trim();

        // 1. Year: e.g. "2026", "1990"
        if (search.Length == 4 &&
            int.TryParse(search, out int year) &&
            year >= 1 && year <= 9999)
        {
            return date.Value.Year == year;
        }

        // 2. Month name: e.g. "Jan", "January"
        for (int month = 1; month <= 12; month++)
        {
            string fullName = Culture.DateTimeFormat
                .GetMonthName(month);

            string abbreviatedName = Culture.DateTimeFormat
                .GetAbbreviatedMonthName(month);

            if (fullName.StartsWith(
                    search, StringComparison.OrdinalIgnoreCase) ||
                abbreviatedName.StartsWith(
                    search, StringComparison.OrdinalIgnoreCase))
            {
                return date.Value.Month == month;
            }
        }

        // 3. Exact or partial supported date formats
        foreach (string format in DateFormats)
        {
            if (DateTime.TryParseExact(
                    search,
                    format,
                    Culture,
                    DateTimeStyles.None,
                    out DateTime parsed))
            {
                // Month/day without a year matches any year.
                if (format is "MMM d" or "MMMM d")
                {
                    return date.Value.Month == parsed.Month &&
                           date.Value.Day == parsed.Day;
                }

                // Year-month matches any day in that month.
                if (format is "yyyy-M" or "yyyy-MM")
                {
                    return date.Value.Year == parsed.Year &&
                           date.Value.Month == parsed.Month;
                }

                return date.Value.Date == parsed.Date;
            }
        }

        return false;
    }
}


