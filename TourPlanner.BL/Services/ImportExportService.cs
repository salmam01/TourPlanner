using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TourPlanner.BL.Utils;
using TourPlanner.Models.Entities;

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
                using (FileStream fs = File.Create(filePath))
                {
                    await JsonSerializer.SerializeAsync(fs, tours, _options);
                }
                _logger.LogInformation("Tour(s) export to JSON successful: {FilePath}", filePath);
                return new Result(Result.ResultCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!Error exporting tours to JSON: {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        public Result ExportToursToExcel(IEnumerable<Tour> tours, string filePath)
        {
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    // Tours sheet
                    IXLWorksheet toursSheet = workbook.Worksheets.Add("Tours");
                    toursSheet.Cell(1, 1).Value = "Id";
                    toursSheet.Cell(1, 2).Value = "Name";
                    toursSheet.Cell(1, 3).Value = "Date";
                    toursSheet.Cell(1, 4).Value = "Description";
                    toursSheet.Cell(1, 5).Value = "From";
                    toursSheet.Cell(1, 6).Value = "To";
                    toursSheet.Cell(1, 7).Value = "Transport Type";
                    toursSheet.Cell(1, 8).Value = "Distance";
                    toursSheet.Cell(1, 9).Value = "Estimated Time";
                    toursSheet.Cell(1, 10).Value = "Logs Count";
                    int tourRow = 2;

                    List<Tour> tourList = tours.ToList();
                    foreach (Tour tour in tourList)
                    {
                        toursSheet.Cell(tourRow, 1).Value = tour.Id.ToString();
                        toursSheet.Cell(tourRow, 2).Value = tour.Name ?? "";
                        var dateCell = toursSheet.Cell(tourRow, 3);
                        dateCell.Value = tour.Date;
                        dateCell.Style.NumberFormat.Format = "DD.MM.YYYY";
                        dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        toursSheet.Cell(tourRow, 4).Value = tour.Description ?? "";
                        toursSheet.Cell(tourRow, 5).Value = tour.From ?? "";
                        toursSheet.Cell(tourRow, 6).Value = tour.To ?? "";
                        toursSheet.Cell(tourRow, 7).Value = tour.TransportType ?? "";
                        toursSheet.Cell(tourRow, 8).Value = tour.Distance;
                        toursSheet.Cell(tourRow, 9).Value = tour.EstimatedTime.ToString();
                        toursSheet.Cell(tourRow, 10).Value = tour.TourLogs?.Count ?? 0;
                        tourRow++;
                    }

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
                    attributesSheet.Cell(1, 1).Value = "Tour Id";
                    attributesSheet.Cell(1, 2).Value = "Popularity";
                    attributesSheet.Cell(1, 3).Value = "Child Friendly";

                    int attributesRow = 2;
                    foreach (Tour tour in tourList)
                    {
                        if (tour.TourAttributes == null)
                            continue;

                        attributesSheet.Cell(attributesRow, 1).Value = tour.TourAttributes.Id.ToString();
                        attributesSheet.Cell(attributesRow, 2).Value = tour.TourAttributes.Popularity.ToString();
                        attributesSheet.Cell(attributesRow, 3).Value = tour.TourAttributes.ChildFriendliness ? "Yes" : "No";
                    }

                    //  Set column widths for Attributes Sheet
                    attributesSheet.Column(1).Width = 36;
                    attributesSheet.Column(2).Width = 12;
                    attributesSheet.Column(3).Width = 20;

                    //  TourLogs Sheet
                    IXLWorksheet logsSheet = workbook.Worksheets.Add("Logs");
                    logsSheet.Cell(1, 1).Value = "Id";
                    logsSheet.Cell(1, 2).Value = "Tour Id";
                    logsSheet.Cell(1, 3).Value = "Date";
                    logsSheet.Cell(1, 4).Value = "Difficulty";
                    logsSheet.Cell(1, 5).Value = "Rating";
                    logsSheet.Cell(1, 6).Value = "Comment";
                    logsSheet.Cell(1, 7).Value = "Total Distance";
                    logsSheet.Cell(1, 8).Value = "Total Time";
                    int logRow = 2;

                    int totalLogs = 0;
                    foreach (Tour tour in tourList)
                    {
                        if (tour.TourLogs == null)
                            continue;

                        foreach (TourLog log in tour.TourLogs)
                        {
                            logsSheet.Cell(logRow, 1).Value = log.Id.ToString();
                            logsSheet.Cell(logRow, 2).Value = tour.Id.ToString();
                            var dateCell = logsSheet.Cell(logRow, 3);
                            dateCell.Value = log.Date;
                            dateCell.Style.NumberFormat.Format = "DD.MM.YYYY";
                            dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            logsSheet.Cell(logRow, 4).Value = log.Difficulty;
                            logsSheet.Cell(logRow, 5).Value = log.Rating;
                            logsSheet.Cell(logRow, 6).Value = log.Comment ?? "";
                            logsSheet.Cell(logRow, 7).Value = log.TotalDistance;
                            logsSheet.Cell(logRow, 8).Value = log.TotalTime.ToString();
                            logRow++;
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
                    _logger.LogInformation("Tour export to Excel successful: {FilePath}. Tours: {TourCount}, Logs: {LogCount}", filePath, tourList.Count, totalLogs);
                    return new Result(Result.ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting tours to Excel: {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        //  Return result instead
        public async Task<List<Tour>> ImportToursFromJsonAsync(string filePath)
        {
            try {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    List<Tour>? tours = await JsonSerializer.DeserializeAsync<List<Tour>>(fs, _options);

                    if (tours != null && tours.Count > 0)
                    {
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
                    }

                    _logger.LogInformation("Imported {Count} tours from JSON: {FilePath}", tours?.Count ?? 0, filePath);
                    return tours ?? new List<Tour>();
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error importing tours from JSON: {FilePath}", filePath);
                throw;
            }
        }

        public List<Tour> ImportTourFromExcel(string filePath)
        {
            try {
                List<Tour> tours = new List<Tour>();
                Dictionary<Guid, Tour> tourMap = new Dictionary<Guid, Tour>();

                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    // Read Tours sheet
                    IXLWorksheet? toursSheet = workbook.Worksheet("Tours");
                    // skip header
                    foreach (var row in toursSheet.RowsUsed().Skip(1))
                    {
                        Guid tourId = Guid.NewGuid();
                        Guid.TryParse(row.Cell(1).GetString(), out tourId);
                        Tour tour = new Tour
                        {
                            Id = tourId,
                            Name = row.Cell(2).GetString(),
                            Date = DateTime.TryParse(row.Cell(3).GetString(), out var date) ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : DateTime.UtcNow,
                            Description = row.Cell(4).GetString(),
                            From = row.Cell(5).GetString(),
                            To = row.Cell(6).GetString(),
                            TransportType = row.Cell(7).GetString(),
                            Distance = double.TryParse(row.Cell(8).GetString(), out var dist) ? dist : 0,
                            EstimatedTime = TimeSpan.TryParse(row.Cell(9).GetString(), out var time) ? time : TimeSpan.Zero,
                            TourLogs = new List<TourLog>()
                        };

                        // Initialize TourAttributes
                        tour.TourAttributes = new TourAttributes
                        {
                            Id = tour.Id,
                            Popularity = 0,
                            ChildFriendliness = false,
                            SearchAlgorithmRanking = 0
                        };

                        tours.Add(tour);
                        tourMap[tour.Id] = tour;
                    }

                    // Read TourLogs sheet
                    int totalLogs = 0;
                    if (workbook.Worksheets.Contains("TourLogs"))
                    {
                        IXLWorksheet? logsSheet = workbook.Worksheet("TourLogs");
                        // skip header
                        foreach (var row in logsSheet.RowsUsed().Skip(1))
                        {
                            Guid logId = Guid.NewGuid();
                            Guid.TryParse(row.Cell(1).GetString(), out logId);
                            Guid parentTourId = Guid.NewGuid();
                            Guid.TryParse(row.Cell(2).GetString(), out parentTourId);

                            var log = new TourLog
                            {
                                Id = logId,
                                TourId = parentTourId,
                                Date = DateTime.TryParse(row.Cell(3).GetString(), out var date) ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : DateTime.UtcNow,
                                Difficulty = int.TryParse(row.Cell(4).GetString(), out var diff) ? diff : 1,
                                Rating = double.TryParse(row.Cell(5).GetString(), out var rating) ? rating : 1.0,
                                Comment = row.Cell(6).GetString(),
                                TotalDistance = double.TryParse(row.Cell(7).GetString(), out var dist) ? dist : 0,
                                TotalTime = TimeSpan.TryParse(row.Cell(8).GetString(), out var ttime) ? ttime : TimeSpan.Zero
                            };

                            if (tourMap.TryGetValue(parentTourId, out var parentTour))
                            {
                                parentTour.TourLogs.Add(log);
                                totalLogs++;
                            }
                            else
                            {
                                _logger.LogWarning("TourLog {LogId} references non-existent Tour {TourId}", logId, parentTourId);
                            }
                        }
                    }

                    // Recalculate TourAttributes for all tours
                    foreach (var tour in tours)
                    {
                        if (tour.TourLogs.Any())
                        {
                            tour.TourAttributes.Popularity = tour.TourLogs.Count;
                            tour.TourAttributes.ChildFriendliness = tour.TourLogs.All(log => log.Difficulty <= 2);
                            tour.TourAttributes.SearchAlgorithmRanking = tour.TourAttributes.Popularity / 100.0;
                            if (tour.TourAttributes.ChildFriendliness)
                                tour.TourAttributes.SearchAlgorithmRanking *= 1.5;
                            tour.TourAttributes.SearchAlgorithmRanking = Math.Min(tour.TourAttributes.SearchAlgorithmRanking, 1.0);
                        }
                    }

                    _logger.LogInformation("Imported {TourCount} tours and {LogCount} logs from Excel: {FilePath}", tours.Count, totalLogs, filePath);
                }
                return tours;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error importing tours from Excel: {FilePath}", filePath);
                throw;
            }
        }
    }
}