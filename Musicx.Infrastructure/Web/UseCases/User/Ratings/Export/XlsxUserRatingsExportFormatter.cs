using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Application.Web.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.User.Ratings.Export;

public sealed class XlsxUserRatingsExportFormatter : IUserRatingsExportFormatter
{
    public UserRatingsExportFormat Format => UserRatingsExportFormat.Xlsx;

    public UserRatingsExportFile Export(IEnumerable<OutUserAlbumAttribute> ratings, string userName)
    {
        using var ms = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Ratings"
            });

            sheetData.Append(CreateRow("Album", "Artist", "Rating", "CollectionType", "DiscoveryDate", "Review"));
            foreach (var rating in ratings)
            {
                sheetData.Append(CreateRow(
                    rating.Album.Name,
                    rating.Album.Artist?.Name ?? string.Empty,
                    rating.Rating?.ToString() ?? string.Empty,
                    rating.CollectionType?.ToString() ?? string.Empty,
                    rating.DiscoveryDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                    rating.Review ?? string.Empty
                ));
            }

            workbookPart.Workbook.Save();
        }

        return new UserRatingsExportFile(
            ms.ToArray(),
            $"{Sanitize(userName)}-ratings.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    private static Row CreateRow(params string[] values)
    {
        var row = new Row();
        foreach (var value in values)
        {
            row.Append(new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value)
            });
        }

        return row;
    }

    private static string Sanitize(string value)
        => string.Join('-', value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}