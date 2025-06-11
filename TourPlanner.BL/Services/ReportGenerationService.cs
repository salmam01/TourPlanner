using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using TourPlanner.Models.Entities;

namespace TourPlanner.BL.Services
{
    public class ReportGenerationService
    {
        private readonly ILogger<ReportGenerationService> _logger;
        private readonly TourAttributesService _tourAttributesService;

        public ReportGenerationService(
            ILogger<ReportGenerationService> logger,
            TourAttributesService tourAttributesService)
        {
            _logger = logger;
            _tourAttributesService = tourAttributesService;
        }

        public void GenerateTourReport(Tour tour, string filePath)
        {
            try
            {
                _logger.LogInformation("Generating tour report for tour {TourId} to {FilePath}", tour.Id, filePath);
                
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
                AddTableRow(tourDetails, "Distance", tour.Distance.ToString("F2") + " km");
                AddTableRow(tourDetails, "Estimated Time", tour.EstimatedTime.ToString());
                AddTableRow(tourDetails, "Date", tour.Date.ToString("dd.MM.yyyy"));

                document.Add(tourDetails);
                document.Add(new Paragraph("\n"));

                // Add tour logs
                if (tour.TourLogs != null && tour.TourLogs.Any())
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
                            log.TotalDistance.ToString("F2") + " km",
                            log.TotalTime.ToString());
                    }
                    
                    document.Add(logsTable);
                }

                _logger.LogInformation("Tour report generated successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tour report: {FilePath}", filePath);
                throw;
            }
        }

        public void GenerateSummaryReport(IEnumerable<Tour> tours, string filePath)
        {
            try
            {
                _logger.LogInformation("Generating summary report to {FilePath}", filePath);
                
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

                foreach (var tour in tours)
                {
                    if (tour.TourLogs == null || !tour.TourLogs.Any())
                        continue;

                    double avgDistance = tour.TourLogs.Average(l => l.TotalDistance);
                    double avgTime = tour.TourLogs.Average(l => l.TotalTime.TotalMinutes);
                    double avgRating = tour.TourLogs.Average(l => l.Rating);
                    int popularity = _tourAttributesService.ComputePopularity(tour.TourLogs);
                    bool childFriendliness = _tourAttributesService.ComputeChildFriendliness(tour.TourLogs);

                    AddTableRow(summaryTable,
                        tour.Name,
                        tour.TourLogs.Count.ToString(),
                        avgDistance.ToString("F2") + " km",
                        avgTime.ToString("F2") + " min",
                        avgRating.ToString("F1"),
                        popularity.ToString(),
                        childFriendliness ? "Yes" : "No");
                }

                document.Add(summaryTable);
                _logger.LogInformation("Summary report generated successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary report: {FilePath}", filePath);
                throw;
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
    }
} 