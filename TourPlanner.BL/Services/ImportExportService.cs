using ClosedXML.Excel;
using ClosedXML.Excel.Exceptions;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;

/// <summary>
///     Service to handle import/export of tours and logs in JSON and XLSX (Excel)
/// </summary>

namespace TourPlanner.BL.Services
{
    public class ImportExportService
    {
        private readonly ILogger<ImportExportService> _logger;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public ImportExportService(ILogger<ImportExportService> logger)
        {
            _logger = logger;
        }

        public async Task<Result> ExportToursToJsonAsync(IEnumerable<Tour> tours, string filePath)
        {
            try
            {
                if (tours == null || tours.Count() == 0)
                {
                    _logger.LogWarning("Trying to export empty tour list to JSON.");
                    return new Result(Result.ResultCode.NullError);
                }

                using (FileStream fs = File.Create(filePath))
                {
                    await JsonSerializer.SerializeAsync(fs, tours, _options);
                }

                _logger.LogInformation("Exported {TourCount} Tour(s) to JSON successfully: {FilePath}.", tours.Count(), filePath);
                return new Result(Result.ResultCode.Success);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Export to JSON failed due to missing write permissions for file {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Export to JSON failed due to IO error for file {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing tours to JSON for {FilePath}.", filePath);
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown Error occurred while writing to {FilePath}.", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        public Result ExportToursToExcel(IEnumerable<Tour> tours, string filePath)
        {
            try
            {
                if (tours == null || tours.Count() == 0)
                {
                    _logger.LogWarning("Trying to export empty tour list to Excel file.");
                    return new Result(Result.ResultCode.NullError);
                }

                List<Tour> tourList = tours.ToList();
                int titleRow = 1;
                int dataRow = 2;

                using (XLWorkbook workbook = new XLWorkbook())
                {
                    // Tours sheet
                    IXLWorksheet toursSheet = workbook.Worksheets.Add("Tours");
                    toursSheet.Cell(titleRow, 1).Value = "ID";
                    toursSheet.Cell(titleRow, 2).Value = "Name";
                    toursSheet.Cell(titleRow, 3).Value = "Date";
                    toursSheet.Cell(titleRow, 4).Value = "Description";
                    toursSheet.Cell(titleRow, 5).Value = "From";
                    toursSheet.Cell(titleRow, 6).Value = "To";
                    toursSheet.Cell(titleRow, 7).Value = "Transport Type";
                    toursSheet.Cell(titleRow, 8).Value = "Distance";
                    toursSheet.Cell(titleRow, 9).Value = "Estimated Time";
                    toursSheet.Cell(titleRow, 10).Value = "Logs Count";

                    foreach (Tour tour in tourList)
                    {
                        toursSheet.Cell(dataRow, 1).Value = tour.Id.ToString();
                        toursSheet.Cell(dataRow, 2).Value = tour.Name ?? "";
                        var dateCell = toursSheet.Cell(dataRow, 3);
                        dateCell.Value = tour.Date;
                        dateCell.Style.NumberFormat.Format = "DD.MM.YYYY";
                        dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        toursSheet.Cell(dataRow, 4).Value = tour.Description ?? "";
                        toursSheet.Cell(dataRow, 5).Value = tour.From ?? "";
                        toursSheet.Cell(dataRow, 6).Value = tour.To ?? "";
                        toursSheet.Cell(dataRow, 7).Value = tour.TransportType ?? "";
                        toursSheet.Cell(dataRow, 8).Value = tour.Distance;
                        toursSheet.Cell(dataRow, 9).Value = tour.EstimatedTime.ToString();
                        toursSheet.Cell(dataRow, 10).Value = tour.TourLogs?.Count ?? 0;
                        dataRow++;
                    }
                    dataRow = 2;

                    // Set column widths
                    toursSheet.Column(1).Width = 36; // Id (Id columns are 36 characters wide (for GUIDs))
                    toursSheet.Column(2).Width = 20; // Name
                    toursSheet.Column(3).Width = 15; // Date
                    toursSheet.Column(4).Width = 50; // Description
                    toursSheet.Column(5).Width = 40; // From
                    toursSheet.Column(6).Width = 40; // To
                    toursSheet.Column(7).Width = 15; // TransportType
                    toursSheet.Column(8).Width = 12; // Distance
                    toursSheet.Column(9).Width = 20; // EstimatedTime
                    toursSheet.Column(10).Width = 12; // TourLogsCount

                    //  Attributes Sheet
                    IXLWorksheet attributesSheet = workbook.Worksheets.Add("Attributes");
                    attributesSheet.Cell(titleRow, 1).Value = "Tour ID";
                    attributesSheet.Cell(titleRow, 2).Value = "Popularity";
                    attributesSheet.Cell(titleRow, 3).Value = "Child Friendly";
                    attributesSheet.Cell(titleRow, 4).Value = "Search Ranking";

                    foreach (Tour tour in tourList)
                    {
                        if (tour.TourAttributes == null)
                            continue;

                        attributesSheet.Cell(dataRow, 1).Value = tour.TourAttributes.Id.ToString();
                        attributesSheet.Cell(dataRow, 2).Value = tour.TourAttributes.Popularity.ToString();
                        attributesSheet.Cell(dataRow, 3).Value = tour.TourAttributes.ChildFriendliness ? "Yes" : "No";
                        attributesSheet.Cell(dataRow, 4).Value = tour.TourAttributes.SearchAlgorithmRanking.ToString();
                        dataRow++;
                    }
                    dataRow = 2;

                    //  Set column widths for Attributes Sheet
                    attributesSheet.Column(1).Width = 36;
                    attributesSheet.Column(2).Width = 12;
                    attributesSheet.Column(3).Width = 20;
                    attributesSheet.Column(4).Width = 12;

                    //  TourLogs Sheet
                    IXLWorksheet logsSheet = workbook.Worksheets.Add("Logs");
                    logsSheet.Cell(titleRow, 1).Value = "ID";
                    logsSheet.Cell(titleRow, 2).Value = "Tour ID";
                    logsSheet.Cell(titleRow, 3).Value = "Date";
                    logsSheet.Cell(titleRow, 4).Value = "Difficulty";
                    logsSheet.Cell(titleRow, 5).Value = "Rating";
                    logsSheet.Cell(titleRow, 6).Value = "Comment";
                    logsSheet.Cell(titleRow, 7).Value = "Total Distance";
                    logsSheet.Cell(titleRow, 8).Value = "Total Time";

                    int totalLogs = 0;
                    foreach (Tour tour in tourList)
                    {
                        if (tour.TourLogs == null)
                            continue;

                        foreach (TourLog log in tour.TourLogs)
                        {
                            logsSheet.Cell(dataRow, 1).Value = log.Id.ToString();
                            logsSheet.Cell(dataRow, 2).Value = tour.Id.ToString();
                            var dateCell = logsSheet.Cell(dataRow, 3);
                            dateCell.Value = log.Date;
                            dateCell.Style.NumberFormat.Format = "DD.MM.YYYY";
                            dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            logsSheet.Cell(dataRow, 4).Value = log.Difficulty;
                            logsSheet.Cell(dataRow, 5).Value = log.Rating;
                            logsSheet.Cell(dataRow, 6).Value = log.Comment ?? "";
                            logsSheet.Cell(dataRow, 7).Value = log.TotalDistance;
                            logsSheet.Cell(dataRow, 8).Value = log.TotalTime.ToString();
                            dataRow++;
                            totalLogs++;
                        }
                    }

                    // Set column widths for TourLogs sheet
                    logsSheet.Column(1).Width = 36; // Id
                    logsSheet.Column(2).Width = 36; // TourId
                    logsSheet.Column(3).Width = 15; // Date
                    logsSheet.Column(4).Width = 12; // Difficulty
                    logsSheet.Column(5).Width = 12; // Rating
                    logsSheet.Column(6).Width = 50; // Comment
                    logsSheet.Column(7).Width = 12; // TotalDistance
                    logsSheet.Column(8).Width = 15; // TotalTime

                    workbook.SaveAs(filePath);
                    _logger.LogInformation("Tour export to Excel successful: {FilePath}. Tours: {TourCount}, Logs: {LogCount}.", filePath, tourList.Count, totalLogs);
                    return new Result(Result.ResultCode.Success);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Export to Excel failed due to missing write permissions for file {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Export to Excel failed due to IO error for file {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (ClosedXMLException ex)
            {
                _logger.LogError(ex, "Error parsing Tour(s) to Excel {FilePath}.", filePath);
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting tours to Excel: {FilePath}.", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        //  Return result instead
        public async Task<Result> ImportToursFromJsonAsync(string filePath)
        {
            try {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    List<Tour>? tours = await JsonSerializer.DeserializeAsync<List<Tour>>(fs, _options);

                    if (tours == null || tours.Count <= 0)
                    {
                        return new Result(Result.ResultCode.NullError);
                    }

                    foreach (Tour tour in tours)
                    {
                        tour.Id = Guid.NewGuid();

                        if (tour.TourAttributes != null)
                        {
                            tour.TourAttributes.Id = tour.Id;
                        }

                        if (tour.TourLogs != null && tour.TourLogs.Count > 0)
                        {
                            foreach (TourLog log in tour.TourLogs)
                            {
                                log.Id = Guid.NewGuid();
                                log.TourId = tour.Id;
                                log.Tour = tour;
                            }
                        }
                    }

                    _logger.LogInformation("Imported {Count} tours from JSON: {FilePath}.", tours?.Count ?? 0, filePath);
                    return new Result(Result.ResultCode.Success, tours);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Import from JSON file failed due to missing read permissions for {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Import from JSON failed due to IO error for file {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing tours from JSON for {FilePath}.", filePath);
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error importing tours from JSON for {FilePath}.", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        public Result ImportTourFromExcel(string filePath)
        {
            try {
                List<Tour> tours = new List<Tour>();

                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    // Read Tours sheet
                    if (!workbook.Worksheets.Contains("Tours") || !workbook.Worksheets.Contains("Attributes") || !workbook.Worksheets.Contains("Logs")) 
                    {
                        _logger.LogWarning("Excel file is missing required sheet.");
                        return new Result(Result.ResultCode.UnknownError);
                    }

                    IXLWorksheet toursSheet = workbook.Worksheet("Tours");
                    if (toursSheet.RowCount() <= 0)
                    {
                        _logger.LogWarning("No tour data found in 'Tours' sheet.");
                        return new Result(Result.ResultCode.NullError);
                    }

                    foreach (IXLRow row in toursSheet.RowsUsed().Skip(1))
                    {
                        Guid oldTourId = Guid.TryParse(row.Cell(1).GetString(), out Guid tempTourId) 
                            ? tempTourId 
                            : Guid.NewGuid();

                        Tour tour = new Tour
                        {
                            Id = Guid.NewGuid(),
                            Name = row.Cell(2).GetString(),
                            Date = DateTime.TryParse(row.Cell(3).GetString(), out DateTime date) 
                                ? DateTime.SpecifyKind(date, DateTimeKind.Utc) 
                                : DateTime.UtcNow,
                            Description = row.Cell(4).GetString(),
                            From = row.Cell(5).GetString(),
                            To = row.Cell(6).GetString(),
                            TransportType = row.Cell(7).GetString(),
                            Distance = row.Cell(8).GetValue<double>(),
                            EstimatedTime = TimeSpan.TryParse(row.Cell(9).GetString(), out TimeSpan time) ? time : TimeSpan.Zero,
                            TourLogs = new List<TourLog>()
                        };

                        IXLWorksheet attributesSheet = workbook.Worksheet("Attributes");
                        if (attributesSheet.RowCount() <= 0)
                        {
                            _logger.LogWarning("No Tour Attributes data found in 'Attributes' sheet.");
                            return new Result(Result.ResultCode.NullError);
                        }
                        foreach (IXLRow attributesRow in attributesSheet.RowsUsed().Skip(1))
                        {
                            Guid attributesTourId = Guid.TryParse(attributesRow.Cell(1).GetString(), out Guid tempAttributesId) 
                                ? tempAttributesId 
                                : Guid.NewGuid();
                            if (attributesTourId == oldTourId)
                            {
                                tour.TourAttributes = new TourAttributes
                                {
                                    Id = tour.Id,
                                    Popularity = attributesRow.Cell(2).GetValue<int>(),
                                    ChildFriendliness = attributesRow.Cell(3).GetString().Contains("Yes") ? true : false,
                                    SearchAlgorithmRanking = attributesRow.Cell(4).GetValue<double>(),
                                };
                                break;
                            }
                        }

                        // Read TourLogs sheet
                        int totalTourLogs = row.Cell(10).GetValue<int>();
                        IXLWorksheet logsSheet = workbook.Worksheet("Logs");
                        if (totalTourLogs > 0)
                        {
                            // skip header
                            foreach (IXLRow logRow in logsSheet.RowsUsed().Skip(1))
                            {
                                Guid logsTourId = Guid.TryParse(logRow.Cell(2).GetString(), out Guid tempLogsId)
                                    ? tempLogsId
                                    : Guid.NewGuid();

                                if (logsTourId == oldTourId)
                                {
                                    TourLog log = new TourLog
                                    {
                                        Id = Guid.NewGuid(),
                                        TourId = tour.Id,
                                        Date = DateTime.TryParse(logRow.Cell(3).GetString(), out DateTime logDate)
                                            ? DateTime.SpecifyKind(logDate, DateTimeKind.Utc)
                                            : DateTime.UtcNow,
                                        Difficulty = logRow.Cell(4).GetValue<int>(),
                                        Rating = logRow.Cell(5).GetValue<double>(),
                                        Comment = logRow.Cell(6).GetString(),
                                        TotalDistance = logRow.Cell(7).GetValue<double>(),
                                        TotalTime = logRow.Cell(8).GetValue<TimeSpan>(),
                                        Tour = tour
                                    };

                                    tour.TourLogs.Add(log);
                                }
                            }
                        }
                        tours.Add(tour);
                    }
                }
                _logger.LogInformation("Imported {TourCount} tours from Excel file {FilePath}.", tours.Count, filePath);
                return new Result(Result.ResultCode.Success, tours);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Import from Excel file failed due to missing read permissions for {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Import from Excel file failed due to IO error for {FilePath}.", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (ClosedXMLException ex)
            {
                _logger.LogError(ex, "Error parsing Tour(s) from Excel file {FilePath}.", filePath);
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error importing tours from Excel file {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }
    }
}