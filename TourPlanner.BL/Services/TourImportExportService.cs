using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TourPlanner.Models.Entities;

/// <summary>
///     Service to handle import/export of tours and logs in JSON and XLSX (Excel)
/// </summary>

namespace TourPlanner.BL.Services
{
    public class TourImportExportService
    {
        private readonly ILogger<TourImportExportService> _logger;

        public async Task ExportToursToJsonAsync(IEnumerable<Tour> tours, string filePath)
        {
            JsonSerializerOptions options;
            options = new JsonSerializerOptions { WriteIndented = true };
            using (FileStream fs = File.Create(filePath))
            {
                await JsonSerializer.SerializeAsync(fs, tours, options);
            }
        }

        public async Task<List<Tour>> ImportToursFromJsonAsync(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                List<Tour>? tours = await JsonSerializer.DeserializeAsync<List<Tour>>(fs);
                return tours ?? new List<Tour>();
            }
        }

        public void ExportToursToExcel(IEnumerable<Tour> tours, string filePath)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                // Tours sheet
                IXLWorksheet toursSheet = workbook.Worksheets.Add("Tours");
                toursSheet.Cell(1, 1).Value = "Id";
                toursSheet.Cell(1, 2).Value = "Name";
                toursSheet.Cell(1, 3).Value = "Description";
                toursSheet.Cell(1, 4).Value = "From";
                toursSheet.Cell(1, 5).Value = "To";
                toursSheet.Cell(1, 6).Value = "TransportType";
                toursSheet.Cell(1, 7).Value = "Distance";
                toursSheet.Cell(1, 8).Value = "EstimatedTime";
                toursSheet.Cell(1, 9).Value = "TourLogsCount";
                int tourRow = 2;

                var tourList = tours.ToList();
                foreach (Tour tour in tourList)
                {
                    toursSheet.Cell(tourRow, 1).Value = tour.Id.ToString();
                    toursSheet.Cell(tourRow, 2).Value = tour.Name ?? "";
                    toursSheet.Cell(tourRow, 3).Value = tour.Description ?? "";
                    toursSheet.Cell(tourRow, 4).Value = tour.From ?? "";
                    toursSheet.Cell(tourRow, 5).Value = tour.To ?? "";
                    toursSheet.Cell(tourRow, 6).Value = tour.TransportType ?? "";
                    toursSheet.Cell(tourRow, 9).Value = tour.TourLogs?.Count ?? 0;
                    tourRow++;
                }

                // TourLogs sheet
                IXLWorksheet logsSheet = workbook.Worksheets.Add("TourLogs");
                logsSheet.Cell(1, 1).Value = "Id";
                logsSheet.Cell(1, 2).Value = "TourId";
                logsSheet.Cell(1, 3).Value = "Date";
                logsSheet.Cell(1, 4).Value = "Difficulty";
                logsSheet.Cell(1, 5).Value = "Rating";
                logsSheet.Cell(1, 6).Value = "Comment";
                logsSheet.Cell(1, 7).Value = "TotalDistance";
                logsSheet.Cell(1, 8).Value = "TotalTime";
                int logRow = 2;

                foreach (Tour tour in tourList)
                {
                    if (tour.TourLogs == null)
                        continue;

                    foreach (TourLog log in tour.TourLogs)
                    {
                        logsSheet.Cell(logRow, 1).Value = log.Id.ToString();
                        logsSheet.Cell(logRow, 2).Value = tour.Id.ToString();
                        logsSheet.Cell(logRow, 3).Value = log.Date;
                        logsSheet.Cell(logRow, 4).Value = log.Difficulty;
                        logsSheet.Cell(logRow, 5).Value = log.Rating;
                        logsSheet.Cell(logRow, 6).Value = log.Comment ?? "";
                        logsSheet.Cell(logRow, 7).Value = log.TotalDistance;
                        logsSheet.Cell(logRow, 8).Value = log.TotalTime.ToString();
                        logRow++;
                    }
                }
                workbook.SaveAs(filePath);
            }
        }

        public List<Tour> ImportToursFromExcel(string filePath)
        {
            List<Tour> tours = new List<Tour>();
            Dictionary<Guid, Tour> tourMap = new Dictionary<Guid, Tour>();
            using (XLWorkbook workbook = new XLWorkbook(filePath))
            {
                // Read Tours sheet
                IXLWorksheet? toursSheet = workbook.Worksheet("Tours");
                foreach (var row in toursSheet.RowsUsed().Skip(1)) // skip header
                {
                    Guid tourId = Guid.NewGuid();
                    Guid.TryParse(row.Cell(1).GetString(), out tourId);
                    var tour = new Tour
                    {
                        Id = tourId,
                        Name = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        From = row.Cell(4).GetString(),
                        To = row.Cell(5).GetString(),
                        TransportType = row.Cell(6).GetString(),
                        TourLogs = new List<TourLog>()
                    };
                    tours.Add(tour);
                    tourMap[tour.Id] = tour;
                }

                // Read TourLogs sheet 
                if (workbook.Worksheets.Contains("TourLogs"))
                {
                    IXLWorksheet? logsSheet = workbook.Worksheet("TourLogs");
                    foreach (var row in logsSheet.RowsUsed().Skip(1)) // skip header
                    {
                        Guid logId = Guid.NewGuid();
                        Guid.TryParse(row.Cell(1).GetString(), out logId);
                        Guid parentTourId = Guid.NewGuid();
                        Guid.TryParse(row.Cell(2).GetString(), out parentTourId);

                        var log = new TourLog
                        {
                            Id = logId,
                            TourId = parentTourId,
                            Date = DateTime.TryParse(row.Cell(3).GetString(), out var date) ? date : DateTime.UtcNow,
                            Difficulty = int.TryParse(row.Cell(4).GetString(), out var diff) ? diff : 1,
                            Rating = double.TryParse(row.Cell(5).GetString(), out var rating) ? rating : 1.0,
                            Comment = row.Cell(6).GetString(),
                            TotalDistance = double.TryParse(row.Cell(7).GetString(), out var dist) ? dist : 0,
                            TotalTime = TimeSpan.TryParse(row.Cell(8).GetString(), out var ttime) ? ttime : TimeSpan.Zero
                        };

                        if (tourMap.TryGetValue(parentTourId, out var parentTour))
                        {
                            parentTour.TourLogs.Add(log);
                        }
                    }
                }
            }
            return tours;
        }
    }
}