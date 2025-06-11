using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;
using Microsoft.Extensions.Logging;
using TourPlanner.UI.Views;

namespace TourPlanner.UI.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService; 
        private readonly TourImportExportService _importExportService;
        private readonly ReportGenerationService _reportGenerationService;
        private readonly ILogger<TourManagementViewModel> _logger;

        private CreateTourViewModel _createTourViewModel;
        public TourListViewModel TourListViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }

        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;

        public ICommand CreateTourCommand => new RelayCommand(
            execute => CreateTour()
        );

        public ICommand EditTourCommand => new RelayCommand(
            execute => UpdateTour()
        );

        public ICommand DeleteTourCommand => new RelayCommand(
            execute => DeleteTour()
        );
        public ICommand ImportToursCommand { get; }
        public ICommand ExportToursCommand { get; }
        public ICommand GenerateTourReportCommand => new RelayCommand(_ => GenerateTourReport());
        public ICommand GenerateSummaryReportCommand => new RelayCommand(_ => GenerateSummaryReport());

        public TourManagementViewModel(
            CreateTourViewModel createTourViewModel,
            TourListViewModel tourListViewModel,
            SearchBarViewModel searchBarViewModel,
            EventAggregator eventAggregator,
            TourService tourService,
            TourLogService tourLogService,
            TourImportExportService tourImportExportService,
            ReportGenerationService reportGenerationService,
            ILogger<TourManagementViewModel> logger
        )
        {
            _createTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            SearchBarViewModel = searchBarViewModel;
            _eventAggregator = eventAggregator;
            _tourService = tourService;
            _tourLogService = tourLogService;
            _importExportService = tourImportExportService;
            _reportGenerationService = reportGenerationService;
            _logger = logger;

            _createTourViewModel.TourCreated += OnTourCreated;
            _createTourViewModel.TourUpdated += OnTourUpdated;
            SearchBarViewModel.SearchParamsChanged += OnPerformSearch;
            _createTourViewModel.Cancelled += OnCancel;
            _eventAggregator.Subscribe<Tour>(OnTourSelected);

            ImportToursCommand = new RelayCommand(execute => ImportTours());
            ExportToursCommand = new RelayCommand(execute => ExportTours());

            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
        }


        public void CreateTour()
        {
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void UpdateTour()
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to edit.");
                return;
            }

            _createTourViewModel.LoadTour(_selectedTour);
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void DeleteTour()
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you would like to delete this tour log?",
                $"Delete Tour {_selectedTour.Name}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes) return;

            _tourService.DeleteTour(_selectedTour);
            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            Log.Information("Tour deleted => {@_selectedTour}", _selectedTour);
            _selectedTour = null;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;

            _tourService.CreateTour(tour);
            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            _eventAggregator.Publish("ShowHome");
        }

        public void OnTourUpdated(object sender, Tour tour)
        {
            if (tour == null) return;

            _tourService.UpdateTour(tour);
            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());

            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
            _eventAggregator.Publish("ShowHome");
        }

        public void OnPerformSearch(object sender, string searchText)
        {
            if(string.IsNullOrEmpty(searchText)) { 
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            }
            else
            {
                TourListViewModel.ReloadTours(_tourService.SearchTours(searchText).ToList());
            }
        }

        //  dont use magic strings, save as constant
        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish("ShowHome");
        }

        private async void ImportTours()
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                    Title = "Import Tour Data"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    List<Tour> importedTours = new List<Tour>();
                    if (openFileDialog.FileName.EndsWith(".json"))
                        importedTours = await _importExportService.ImportToursFromJsonAsync(openFileDialog.FileName);
                    if (openFileDialog.FileName.EndsWith(".xlsx"))
                        importedTours = _importExportService.ImportToursFromExcel(openFileDialog.FileName);

                    if (importedTours.Count == 0)
                    {
                        MessageBox.Show("No tours found in the selected file.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    foreach (var tour in importedTours)
                    {
                        _tourService.CreateTour(tour);
                    }

                    TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
                    MessageBox.Show("Tours imported successfully.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import tours.\nDetails: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExportTours()
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                    Title = "Export Tour Data",
                    FileName = "TourExport"
                };
                if (saveFileDialog.ShowDialog() != true) return;
                var tours = _tourService.GetAllTours();
                if (saveFileDialog.FileName.EndsWith(".json"))
                    await _importExportService.ExportToursToJsonAsync(tours, saveFileDialog.FileName);
                if (saveFileDialog.FileName.EndsWith(".xlsx"))
                    _importExportService.ExportToursToExcel(tours, saveFileDialog.FileName);

                MessageBox.Show("Tours exported successfully.", "Export Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export tours.\nDetails: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateTourReport()
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour first.", "No Tour Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                _logger.LogWarning("Attempted to generate tour report without selecting a tour");
                return;
            }

            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Save Tour Report",
                    FileName = $"TourReport_{_selectedTour.Name}_{DateTime.Now:yyyyMMdd}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _reportGenerationService.GenerateTourReport(_selectedTour, saveFileDialog.FileName);
                    _logger.LogInformation("Tour report generated successfully: {FilePath}", saveFileDialog.FileName);
                    MessageBox.Show("Tour report generated successfully.", "Export Result", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate tour report");
                MessageBox.Show($"Failed to generate tour report.\nDetails: {ex.Message}", "Report Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void GenerateSummaryReport()
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Save Summary Report",
                    FileName = $"TourSummary_{DateTime.Now:yyyyMMdd}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var tours = _tourService.GetAllTours();
                    _reportGenerationService.GenerateSummaryReport(tours, saveFileDialog.FileName);
                    _logger.LogInformation("Summary report generated successfully: {FilePath}", saveFileDialog.FileName);
                    MessageBox.Show("Summary report generated successfully.", "Export Result", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate summary report");
                MessageBox.Show($"Failed to generate summary report.\nDetails: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
