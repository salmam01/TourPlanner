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
using TourPlanner.BL.Utils;
using TourPlanner.BL.API;

namespace TourPlanner.UI.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService; 
        private readonly TourImportExportService _importExportService;
        private readonly ReportGenerationService _reportGenerationService;
        private readonly OpenRouteService _openRouteService;

        private readonly CreateTourViewModel _createTourViewModel;
        public TourListViewModel TourListViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }

        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;

        public ICommand CreateTourCommand => new RelayCommand(
            execute => CreateTour()
        );
        public ICommand DeleteAllToursCommand => new RelayCommand(
            execute => DeleteAllTours()
        );
        public ICommand ImportToursCommand => new RelayCommand(
            execute => ImportTours()
        );
        public ICommand GenerateTourReportCommand => new RelayCommand(
            _ => GenerateTourReport()
        );
        public ICommand GenerateSummaryReportCommand => new RelayCommand(
            _ => GenerateSummaryReport()
        );

        public TourManagementViewModel(
            CreateTourViewModel createTourViewModel,
            TourListViewModel tourListViewModel,
            SearchBarViewModel searchBarViewModel,
            EventAggregator eventAggregator,
            TourService tourService,
            TourLogService tourLogService,
            TourImportExportService tourImportExportService,
            ReportGenerationService reportGenerationService,
            OpenRouteService openRouteService
        ) {
            //  Dependencies
            _createTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            SearchBarViewModel = searchBarViewModel;
            _eventAggregator = eventAggregator;
            _tourService = tourService;
            _tourLogService = tourLogService;
            _importExportService = tourImportExportService;
            _reportGenerationService = reportGenerationService;
            _openRouteService = openRouteService;

            //  Events
            _createTourViewModel.TourCreated += OnTourCreated;
            _createTourViewModel.TourUpdated += OnTourUpdated;
            SearchBarViewModel.SearchParamsChanged += OnPerformSearch;
            _createTourViewModel.Cancelled += OnCancel;

            _eventAggregator.Subscribe<TourEvent>(e =>
            {
                switch (e.Type)
                {
                    case TourEvent.EventType.Select:
                        OnTourSelected(e.Tour);
                        break;
                    case TourEvent.EventType.Edit:
                        OnUpdateTour();
                        break;
                    case TourEvent.EventType.Delete:
                        OnDeleteTour();
                        break;
                    case TourEvent.EventType.Export:
                        OnExportTours();
                        break;
                }
            });

            //  Reload the tours on initialization
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
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.CreateTour));
        }

        public void OnUpdateTour()
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to edit.");
                return;
            }

            _createTourViewModel.LoadTour(_selectedTour);
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.CreateTour));
        }

        public void OnDeleteTour()
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to delete.");
                return;
            }

            MessageBoxResult warning = MessageBox.Show(
                "Are you sure you would like to delete this Tour?",
                $"Delete Tour {_selectedTour.Name}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (warning != MessageBoxResult.Yes) return;

            Result result = _tourService.DeleteTour(_selectedTour);

            if (result.Code == Result.ResultCode.Success)
            {
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
                _selectedTour = null;
            }
            else
                ShowErrorMessage(result);
        }

        public async void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;

            tour = await _openRouteService.GetTourInformationAsync(tour);
            Result result = _tourService.CreateTour(tour);

            if (result.Code == Result.ResultCode.Success)
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            else
                ShowErrorMessage(result);

            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        public async void OnTourUpdated(object sender, Tour tour)
        {
            if (tour == null) return;

            tour = await _openRouteService.GetTourInformationAsync(tour);
            Result result = _tourService.UpdateTour(tour);

            if (result.Code == Result.ResultCode.Success)
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            else
                ShowErrorMessage(result);

            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        public void DeleteAllTours()
        {
            MessageBoxResult warning = MessageBox.Show(
                "Are you sure you would like to delete ALL tours?",
                $"Delete all Tours",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (warning != MessageBoxResult.Yes) return;

            Result result = _tourService.DeleteAllTours();

            if (result.Code == Result.ResultCode.Success)
            {
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
                _selectedTour = null;
            }
            else
                ShowErrorMessage(result);
        }

        public void OnPerformSearch(object sender, string searchText)
        {
            //  If the search bar is empty, show all tours again
            if (string.IsNullOrEmpty(searchText)) 
                TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            else
                TourListViewModel.ReloadTours(
                    _tourService.SearchToursAndLogs(searchText, _tourLogService.SearchTourLogs(searchText))
                );
        }

        //  dont use magic strings, save as constants
        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
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

        private async void OnExportTours()
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
                //_logger.LogWarning("Attempted to generate tour report without selecting a tour");
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
                    //_logger.LogInformation("Tour report generated successfully: {FilePath}", saveFileDialog.FileName);
                    MessageBox.Show("Tour report generated successfully.", "Export Result", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to generate tour report");
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
                    //_logger.LogInformation("Summary report generated successfully: {FilePath}", saveFileDialog.FileName);
                    MessageBox.Show("Summary report generated successfully.", "Export Result", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to generate summary report");
                MessageBox.Show($"Failed to generate summary report.\nDetails: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ShowErrorMessage(Result result)
        {
            string message = string.Empty;

            switch(result.Code)
            {
                case Result.ResultCode.NullError:
                    message = "Please provide valid Tour details.";
                    break;
                case Result.ResultCode.DatabaseError:
                    message = "Database error occurred. Please try again later.";
                    break;
                case Result.ResultCode.UnknownError:
                    message = "An unknown error occurred.";
                    break;
            }

            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
