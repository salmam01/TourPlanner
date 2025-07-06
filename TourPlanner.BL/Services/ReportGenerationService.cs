using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using TourPlanner.Models.Entities;
using System.IO;
using System.Threading.Tasks;
using TourPlanner.UI.API;
using TourPlanner.UI.Utils.DTO;
using Microsoft.Playwright;
using iText.IO.Image;
using TourPlanner.Models.Utils.Helpers;

namespace TourPlanner.UI.Services
{
    public class ReportGenerationService
    {
        private readonly ILogger<ReportGenerationService> _logger;
        private readonly TourAttributesService _tourAttributesService;
        private readonly OpenRouteService _openRouteService;

        public ReportGenerationService(
            ILogger<ReportGenerationService> logger,
            TourAttributesService tourAttributesService,
            OpenRouteService openRouteService)
        {
            _logger = logger;
            _tourAttributesService = tourAttributesService;
            _openRouteService = openRouteService;
        }

        public async Task<Result> GenerateTourReport(Tour tour, string filePath)
        {
            try
            {
                if (tour == null)
                {
                    _logger.LogWarning("Trying to generate Tour Report with NULL tour.");
                    return new Result(Result.ResultCode.NullError);
                }
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("Trying to generate Tour Report with invalid file path.");
                    return new Result(Result.ResultCode.FileAccessError);
                }

                using PdfWriter writer = new PdfWriter(filePath);
                using PdfDocument pdf = new PdfDocument(writer);
                using Document document = new Document(pdf);

                // Add title
                Paragraph? title = new Paragraph($"Tour Report: {tour.Name}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD));
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Add tour details
                Table? tourDetails = new Table(2)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                AddTableRow(tourDetails, "Name", tour.Name);
                AddTableRow(tourDetails, "Description", tour.Description);
                AddTableRow(tourDetails, "From", tour.From);
                AddTableRow(tourDetails, "To", tour.To);
                AddTableRow(tourDetails, "Transport Type", tour.TransportType);
                AddTableRow(tourDetails, "Distance", (tour.Distance / 1000).ToString("F2") + " km");
                AddTableRow(tourDetails, "Estimated Time", tour.EstimatedTime.ToString(@"hh\:mm\:ss"));
                AddTableRow(tourDetails, "Date", tour.Date.ToString("dd.MM.yyyy"));

                document.Add(tourDetails);
                document.Add(new Paragraph("\n"));

                // Add map image
                try
                {
                    var mapImagePath = await GenerateMapImage(tour);
                    _logger.LogInformation($"Map image path: {mapImagePath}");
                    if (File.Exists(mapImagePath))
                    {
                        Paragraph mapTitle = new Paragraph("Route Map")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(16)
                            .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD));
                        document.Add(mapTitle);
                        document.Add(new Paragraph("\n"));

                        ImageData imageData = ImageDataFactory.Create(mapImagePath);
                        Image image = new Image(imageData)
                            .SetWidth(UnitValue.CreatePercentValue(80))
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        document.Add(image);
                        document.Add(new Paragraph("\n"));

                        // Clean up temporary image file
                        File.Delete(mapImagePath);
                    }
                    else
                    {
                        _logger.LogWarning($"Map image file does not exist: {mapImagePath}");
                        document.Add(new Paragraph($"Error: Image : {mapImagePath}"));
                    }
                }
                catch (Exception ex)
                {
                    document.Add(new Paragraph($"Error: Image-Paste: {ex.Message}"));
                    _logger.LogWarning(ex, "Failed to add map image to report for tour {TourId}", tour.Id);
                }

                // Add tour logs
                if (tour.TourLogs != null && tour.TourLogs.Count != 0)
                {
                    Paragraph? logsTitle = new Paragraph("Tour Logs")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16)
                        .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD));
                    document.Add(logsTitle);
                    document.Add(new Paragraph("\n"));

                    Table? logsTable = new Table(6)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    // Add headers
                    AddTableHeader(logsTable, "Date", "Difficulty", "Rating", "Comment", "Total Distance", "Total Time");

                    foreach (TourLog log in tour.TourLogs)
                    {
                        AddTableRow(logsTable,
                            log.Date.ToString("dd.MM.yyyy"),
                            log.Difficulty.ToString(),
                            log.Rating.ToString(),
                            log.Comment ?? "",
                            (log.TotalDistance / 1000).ToString("F2") + " km",
                            log.TotalTime.ToString(@"hh\:mm\:ss"));
                    }
                    
                    document.Add(logsTable);
                }

                _logger.LogInformation("Tour report generated successfully: {FilePath}", filePath);
                return new Result(Result.ResultCode.Success);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError(e, "Access denied writing report to {FilePath}", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException e)
            {
                _logger.LogError(e, "Input / Output error writing report to {FilePath}", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (PdfException e)
            {
                _logger.LogError(e, "PDF generation error for {FilePath}", filePath);
                return new Result(Result.ResultCode.PdfGenerationError);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error generating tour report for {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        public Result GenerateSummaryReport(List<Tour> tours, string filePath)
        {
            try
            {
                if (tours == null || tours.Count == 0)
                {
                    _logger.LogWarning("Trying to generate summary report with no tours.");
                    return new Result(Result.ResultCode.NullError);
                }
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("Trying to generate summary report with invalid file path.");
                    return new Result(Result.ResultCode.FileAccessError);
                }

                using PdfWriter writer = new PdfWriter(filePath);
                using PdfDocument pdf = new PdfDocument(writer);
                using Document document = new Document(pdf);

                // Add title
                Paragraph? title = new Paragraph("Tour Summary Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD));

                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Create summary table
                Table? summaryTable = new Table(7)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // headers
                AddTableHeader(summaryTable, "Tour Name", "Total Logs", "Avg. Distance", "Avg. Time", "Avg. Rating", "Popularity", "Child-Friendliness");

                foreach (Tour tour in tours)
                {
                    int tourLogsCount = 0;
                    double avgDistance = tour.Distance / 1000;
                    TimeSpan avgTime = TimeSpan.FromMinutes(tour.EstimatedTime.TotalMinutes);
                    double avgRating = 0;
                    int popularity = tour.TourAttributes.Popularity;
                    bool childFriendliness = tour.TourAttributes.ChildFriendliness;

                    if (tour.TourLogs != null && tour.TourLogs.Count != 0)
                    {
                        tourLogsCount = tour.TourLogs.Count;
                        avgDistance = tour.TourLogs.Average(l => l.TotalDistance / 1000);
                        avgTime = TimeSpan.FromMinutes(tour.TourLogs.Average(l => l.TotalTime.TotalMinutes));
                        avgRating = tour.TourLogs.Average(l => l.Rating);
                    }

                    AddTableRow(summaryTable,
                                tour.Name,
                                tourLogsCount.ToString(),
                                avgDistance.ToString("F2") + " km",
                                avgTime.ToString(@"hh\:mm") + " h",
                                avgRating.ToString("F1"),
                                popularity.ToString(),
                                childFriendliness ? "Yes" : "No");
                }

                document.Add(summaryTable);
                _logger.LogInformation("Summary report generated successfully: {FilePath}", filePath);
                return new Result(Result.ResultCode.Success);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError(e, "Access denied writing report to {FilePath}", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (IOException e)
            {
                _logger.LogError(e, "Input / Output error writing report to {FilePath}", filePath);
                return new Result(Result.ResultCode.FileAccessError);
            }
            catch (PdfException e)
            {
                _logger.LogError(e, "PDF generation error for {FilePath}", filePath);
                return new Result(Result.ResultCode.PdfGenerationError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary report for {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        private void AddTableHeader(Table table, params string[] headers)
        {
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header)).SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD)));
            }
        }

        private void AddTableRow(Table table, params string[] values)
        {
            foreach (var value in values)
            {
                table.AddCell(new Cell().Add(new Paragraph(value)));
            }
        }

        private async Task<string> GenerateMapImage(Tour tour)
        {
            try
            {
                // 1. fetch MapGeometry 
                Result result = await _openRouteService.GetMapGeometry(tour);
                if (result.Code != Result.ResultCode.Success ||
                    result.Data is not MapGeometry mapGeometry ||
                    mapGeometry == null || 
                    mapGeometry.WayPoints == null || mapGeometry.WayPoints.Count == 0 ||
                    mapGeometry.Bbox == null || 
                    mapGeometry.Distance <= 0 || mapGeometry.Duration == TimeSpan.Zero)
                {
                    throw new InvalidOperationException("Failed to retrieve map geometry for the tour");
                }

                // 2. create directions.js  (like LeafletHelper)
                string leafletDir = FindLeafletDirectory();
                string directionsPath = Path.Combine(leafletDir, "directions.js");
                string mapHtmlPath = Path.Combine(leafletDir, "map.html");
                var directionsObject = new {
                    type = "Feature",
                    geometry = new {
                        type = "LineString",
                        coordinates = mapGeometry.WayPoints.Select(wp => new[] { wp.Longitude, wp.Latitude }).ToArray()
                    },
                    bbox = new[] { mapGeometry.Bbox.MinLongitude, mapGeometry.Bbox.MinLatitude, mapGeometry.Bbox.MaxLongitude, mapGeometry.Bbox.MaxLatitude },
                    fromAddress = tour.From,
                    toAddress = tour.To
                };
                string directionsJson = $"var directions = {System.Text.Json.JsonSerializer.Serialize(directionsObject)};";
                File.WriteAllText(directionsPath, directionsJson);

                // 3. HTML
                string tempPngPath = Path.GetTempFileName() + ".png";

                // 4. Playwright Screenshot
                using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync(new Microsoft.Playwright.BrowserNewPageOptions { ViewportSize = new Microsoft.Playwright.ViewportSize { Width = 800, Height = 600 } });
                await page.GotoAsync($"file:///{mapHtmlPath.Replace("\\", "/")}");
                await page.WaitForTimeoutAsync(2000); // Image loading: wait 2 seconds; 
                await page.ScreenshotAsync(new Microsoft.Playwright.PageScreenshotOptions { Path = tempPngPath });
                await browser.CloseAsync();

                return tempPngPath;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string FindLeafletDirectory()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                var leaflet = Path.Combine(dir.FullName, "TourPlanner.UI", "Leaflet");
                if (Directory.Exists(leaflet) && File.Exists(Path.Combine(leaflet, "map.html")))
                    return leaflet;
                leaflet = Path.Combine(dir.FullName, "Leaflet");
                if (Directory.Exists(leaflet) && File.Exists(Path.Combine(leaflet, "map.html")))
                    return leaflet;
                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException("Leaflet directory with map.html not found.");
        }

    }
} 