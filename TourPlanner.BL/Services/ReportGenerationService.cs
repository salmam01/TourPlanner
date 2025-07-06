using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TourPlanner.Models.Entities;
using System.IO;
using System.Threading.Tasks;
using TourPlanner.UI.API;
using TourPlanner.UI.Utils.DTO;
using Microsoft.Playwright;
using iText.IO.Image;
using TourPlanner.BL.API;
using TourPlanner.BL.Utils.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;
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

                // Generate the PDF using QuestPDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(container => ComposeContent(container, tour));
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                document.GeneratePdf(filePath);
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
            catch (Exception e)
            {
                _logger.LogError(e, "Error generating tour report for {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("TourPlanner").Bold().FontSize(16);
                    column.Item().Text("Tour Report").FontSize(12).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(IContainer container, Tour tour)
        {
            container.PaddingVertical(20).Column(column =>
            {
                // Title
                column.Item().AlignCenter().Text($"Tour Report: {tour.Name}")
                    .FontSize(20)
                    .Bold();

                column.Item().Height(20);

                // Tour Details Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                    });

                    table.Cell().Background(Colors.Grey.Medium).Text("Name").Bold();
                    table.Cell().Text(tour.Name);
                    table.Cell().Background(Colors.Grey.Medium).Text("Description").Bold();
                    table.Cell().Text(tour.Description);
                    table.Cell().Background(Colors.Grey.Medium).Text("From").Bold();
                    table.Cell().Text(tour.From);
                    table.Cell().Background(Colors.Grey.Medium).Text("To").Bold();
                    table.Cell().Text(tour.To);
                    table.Cell().Background(Colors.Grey.Medium).Text("Transport Type").Bold();
                    table.Cell().Text(tour.TransportType);
                    table.Cell().Background(Colors.Grey.Medium).Text("Distance").Bold();
                    table.Cell().Text($"{tour.Distance:F2} km");
                    table.Cell().Background(Colors.Grey.Medium).Text("Estimated Time").Bold();
                    table.Cell().Text(tour.EstimatedTime.ToString(@"hh\:mm\:ss"));
                    table.Cell().Background(Colors.Grey.Medium).Text("Date").Bold();
                    table.Cell().Text(tour.Date.ToString("dd.MM.yyyy"));
                });

                column.Item().Height(20);

                // Map Section
                column.Item().AlignCenter().Text("Route Map")
                    .FontSize(16)
                    .Bold();

                column.Item().Height(10);

                // Add map as embedded HTML/CSS
                try
                {
                    var mapGeometry = GetMapGeometryAsync(tour).Result;
                    if (mapGeometry != null)
                    {
                        column.Item().Background(Colors.Grey.Medium).Padding(10).Column(col =>
                        {
                            col.Item().Text("Route Information:").Bold();
                            col.Item().Text($"From: {tour.From}");
                            col.Item().Text($"To: {tour.To}");
                            col.Item().Text($"Distance: {tour.Distance:F2} km");
                            col.Item().Text($"Estimated Time: {tour.EstimatedTime:hh\\:mm\\:ss}");
                            
                            if (mapGeometry.WayPoints != null && mapGeometry.WayPoints.Count > 0)
                            {
                                col.Item().Height(10);
                                col.Item().Text("Route Coordinates:").Bold();
                                col.Item().Text($"Start: {mapGeometry.WayPoints.First().Latitude:F4}, {mapGeometry.WayPoints.First().Longitude:F4}");
                                col.Item().Text($"End: {mapGeometry.WayPoints.Last().Latitude:F4}, {mapGeometry.WayPoints.Last().Longitude:F4}");
                                col.Item().Text($"Waypoints: {mapGeometry.WayPoints.Count}");
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    column.Item().Background(Colors.Red.Medium).Padding(10).Text($"Error loading map data: {ex.Message}");
                    _logger.LogWarning(ex, "Failed to add map data to report for tour {TourId}", tour.Id);
                }

                column.Item().Height(20);

                // Tour Logs
                if (tour.TourLogs != null && tour.TourLogs.Count > 0)
                {
                    column.Item().AlignCenter().Text("Tour Logs")
                        .FontSize(16)
                        .Bold();

                    column.Item().Height(10);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Date
                            columns.RelativeColumn(1); // Difficulty
                            columns.RelativeColumn(1); // Rating
                            columns.RelativeColumn(2); // Comment
                            columns.RelativeColumn(1); // Total Distance
                            columns.RelativeColumn(1); // Total Time
                        });

                        // Headers
                        table.Header(header =>
                        {
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Difficulty").Bold();
                            header.Cell().Text("Rating").Bold();
                            header.Cell().Text("Comment").Bold();
                            header.Cell().Text("Distance").Bold();
                            header.Cell().Text("Time").Bold();
                        });

                        foreach (var log in tour.TourLogs)
                        {
                            table.Cell().Text(log.Date.ToString("dd.MM.yyyy"));
                            table.Cell().Text(log.Difficulty.ToString());
                            table.Cell().Text(log.Rating.ToString());
                            table.Cell().Text(log.Comment ?? "");
                            table.Cell().Text($"{log.TotalDistance:F2} km");
                            table.Cell().Text(log.TotalTime.ToString(@"hh\:mm\:ss"));
                        }
                    });
                }
            });
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

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(ComposeSummaryHeader);
                        page.Content().Element(container => ComposeSummaryContent(container, tours));
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                document.GeneratePdf(filePath);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary report for {FilePath}", filePath);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        private void ComposeSummaryHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("TourPlanner").Bold().FontSize(16);
                    column.Item().Text("Summary Report").FontSize(12).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeSummaryContent(IContainer container, List<Tour> tours)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Item().AlignCenter().Text("Tour Summary Report")
                    .FontSize(20)
                    .Bold();

                column.Item().Height(20);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Tour Name
                        columns.RelativeColumn(1); // Total Logs
                        columns.RelativeColumn(1); // Avg. Distance
                        columns.RelativeColumn(1); // Avg. Time
                        columns.RelativeColumn(1); // Avg. Rating
                        columns.RelativeColumn(1); // Popularity
                        columns.RelativeColumn(1); // Child-Friendliness
                    });

                    // Headers
                    table.Header(header =>
                    {
                        header.Cell().Text("Tour Name").Bold();
                        header.Cell().Text("Total Logs").Bold();
                        header.Cell().Text("Avg. Distance").Bold();
                        header.Cell().Text("Avg. Time").Bold();
                        header.Cell().Text("Avg. Rating").Bold();
                        header.Cell().Text("Popularity").Bold();
                        header.Cell().Text("Child-Friendly").Bold();
                    });

                    foreach (var tour in tours)
                    {
                        int tourLogsCount = 0;
                        double avgDistance = tour.Distance;
                        double avgTime = tour.EstimatedTime.TotalMinutes;
                        double avgRating = 0;
                        int popularity = tour.TourAttributes?.Popularity ?? 0;
                        bool childFriendliness = tour.TourAttributes?.ChildFriendliness ?? false;

                        if (tour.TourLogs != null && tour.TourLogs.Count > 0)
                        {
                            tourLogsCount = tour.TourLogs.Count;
                            avgDistance = tour.TourLogs.Average(l => l.TotalDistance);
                            avgTime = tour.TourLogs.Average(l => l.TotalTime.TotalMinutes);
                            avgRating = tour.TourLogs.Average(l => l.Rating);
                        }

                        table.Cell().Text(tour.Name);
                        table.Cell().Text(tourLogsCount.ToString());
                        table.Cell().Text($"{avgDistance:F2} km");
                        table.Cell().Text($"{avgTime:F2} min");
                        table.Cell().Text($"{avgRating:F1}");
                        table.Cell().Text(popularity.ToString());
                        table.Cell().Text(childFriendliness ? "Yes" : "No");
                    }
                });
            });
        }

        private async Task<MapGeometry?> GetMapGeometryAsync(Tour tour)
        {
            try
            {
                var result = await _openRouteService.GetMapGeometry(tour);
                if (result.Code == Result.ResultCode.Success && result.Data is MapGeometry mapGeometry)
                {
                    return mapGeometry;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve map geometry for tour {TourId}", tour.Id);
                return null;
            }
        }
    }
} 