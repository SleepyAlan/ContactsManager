using OfficeOpenXml;
using OfficeOpenXml.Style;
using ServiceContracts.DTOs;
using System.Drawing;

namespace Services.Helpers;


public static class ExcelStylingHelper
{
    public static void ApplyPersonsStyling(ExcelWorksheet worksheet)
    {
        if (worksheet.Dimension is null)
            return;

        int firstRow = worksheet.Dimension.Start.Row;
        int lastRow = worksheet.Dimension.End.Row;
        int firstCol = worksheet.Dimension.Start.Column;
        int lastCol = worksheet.Dimension.End.Column;

        var brandColor = ColorTranslator.FromHtml("#1B7A5A");
        var fontColor = ColorTranslator.FromHtml("#14201B");
        var backgroundColor = ColorTranslator.FromHtml("#F2F6F4");
        var borderColor = ColorTranslator.FromHtml("#D8E2DD");

        // Header styling.
        using (var header = worksheet.Cells[firstRow, firstCol, firstRow, lastCol])
        {
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(brandColor);
            header.Style.Font.Color.SetColor(Color.White);
            header.Style.Font.Bold = true;
            header.Style.Font.Size = 11;
            header.Style.HorizontalAlignment =
                ExcelHorizontalAlignment.Center;
            header.Style.VerticalAlignment =
                ExcelVerticalAlignment.Center;
            header.Style.WrapText = true;
        }

        worksheet.Row(firstRow).Height = 25;

        // Alternating row colors and borders.
        if (lastRow > firstRow)
        {
            using var dataRows = worksheet
                .Cells[firstRow + 1, firstCol, lastRow, lastCol];

            dataRows.Style.Fill.PatternType = ExcelFillStyle.Solid;
            dataRows.Style.Fill.BackgroundColor.SetColor(backgroundColor);
            dataRows.Style.Font.Color.SetColor(fontColor);
            dataRows.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            dataRows.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRows.Style.Border.Bottom.Color.SetColor(borderColor);
        }

        // Fit and cap column widths.
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        for (int col = firstCol; col <= lastCol; col++)
        {
            worksheet.Column(col).Width = Math.Clamp(
                worksheet.Column(col).Width, 15, 25);
        }

        // Freeze header and enable filtering.
        worksheet.View.FreezePanes(firstRow + 1, firstCol);

        worksheet.Cells[firstRow, firstCol, lastRow, lastCol]
            .AutoFilter = true;
    }
}
