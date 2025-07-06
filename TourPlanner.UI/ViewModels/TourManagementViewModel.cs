using iText.StyledXmlParser.Jsoup.Parser;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BL.API;
using TourPlanner.BL.Services;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService; 
        private readonly ImportExportService _importExportService;
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
        public ICommand ReloadCommand => new RelayCommand(
            execute => ReloadList()
        );
        public ICommand DeleteAllToursCommand => new RelayCommand(
            execute => DeleteAllTours()
        );
        public ICommand ImportToursCommand => new RelayCommand(
            execute => ImportTours()
        );
        public ICommand ExportAllToursCommand => new RelayCommand(
            execute => ExportAllTours()
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
            ImportExportService tourImportExportService,
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
            _createTourViewModel.TourUpdated += OnTourEdited;
            SearchBarViewModel.SearchParamsChanged += OnPerformSearch;
            _createTourViewModel.Cancelled += OnCancel;

            _eventAggregator.Subscribe<TourEvent>(e =>
            {
                EventHandler(e);
            });

            //  Reload the tours on initialization
            ReloadList();
        }

        private void EventHandler(TourEvent tourEvent)
        {
            switch (tourEvent.Type)
            {
                case TourEvent.EventType.SelectTour:
                    OnTourSelected(tourEvent.Tour);
                    break;
                case TourEvent.EventType.EditTour:
                    OnEditTour();
                    break;
                case TourEvent.EventType.DeleteTour:
                    OnDeleteTour();
                    break;
                case TourEvent.EventType.ExportTour:
                    OnExportTour();
                    break;
            }
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
        }

        public void ReloadList()
        {
            Result result = _tourService.GetAllTours();

            if (result.Code != Result.ResultCode.Success)
            {
                ShowErrorMessage(result, "Reload Error");
                return;
            }
            if (result.Data is not List<Tour> tours)
            {
                ShowErrorMessage(new Result(Result.ResultCode.UnknownError), "Reload Error");
                return;
            }
            TourListViewModel.ReloadTours(tours);
            _eventAggregator.Publish(new TourEvent(TourEvent.EventType.Reload));
            _selectedTour = null;
        }

        public void CreateTour()
        {
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.CreateTour));
        }

        public void OnEditTour()
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
                _eventAggregator.Publish(new TourEvent(TourEvent.EventType.TourDeleted));
                ReloadList();
                _selectedTour = null;
            }
            else
                ShowErrorMessage(result, "Delete Error");
        }

        public async void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;

            Result result = await _openRouteService.GetTourInformationAsync(tour);
            if (result.Code != Result.ResultCode.Success)
            {
                ShowErrorMessage(result, "Create Error");
                return;
            }
            tour = (Tour)result.Data;

            result = _tourService.CreateTour(tour);

            if (result.Code == Result.ResultCode.Success)
                ReloadList();
            else
                ShowErrorMessage(result, "Create Error");

            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        public async void OnTourEdited(object sender, Tour tour)
        {
            if (tour == null) return;

            Result result = await _openRouteService.GetTourInformationAsync(tour);
            if(result.Code != Result.ResultCode.Success)
            {
                ShowErrorMessage(result, "Edit Error");
                return;
            }
            tour = (Tour)result.Data;

            result = _tourService.UpdateTour(tour);

            if (result.Code == Result.ResultCode.Success)
            {
                ReloadList();
                _eventAggregator.Publish(new TourEvent(TourEvent.EventType.TourEdited));
            }
            else
                ShowErrorMessage(result, "Edit Error");
            
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
                _eventAggregator.Publish(new TourEvent(TourEvent.EventType.AllToursDeleted));
                ReloadList();
                _selectedTour = null;
            }
            else
                ShowErrorMessage(result, "Delete Error");
        }

        public void OnPerformSearch(object sender, string searchText)
        {
            //  If the search bar is empty, show all tours again
            if (string.IsNullOrEmpty(searchText))
                ReloadList();
            else
            {
                Result result = _tourService.SearchToursAndLogs(searchText, _tourLogService.SearchTourLogs(searchText));
                if (result.Code != Result.ResultCode.Success)
                {
                    ShowErrorMessage(result, "Search Error");
                    return;
                }
                if (result.Data is not List<Tour> tours)
                {
                    ShowErrorMessage(result, "Search Error");
                    return;
                }
                TourListViewModel.ReloadTours(tours);
            }
        }

        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        private async void ImportTours()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                Title = "Import Tour(s)"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Result result = new(Result.ResultCode.UnknownError);
                List<Tour> importedTours = [];

                if (openFileDialog.FileName.EndsWith(".json"))
                    result = await _importExportService.ImportToursFromJsonAsync(openFileDialog.FileName);
                else if (openFileDialog.FileName.EndsWith(".xlsx"))
                    result = _importExportService.ImportTourFromExcel(openFileDialog.FileName);

                if (result.Code == Result.ResultCode.Success)
                {
                    importedTours = (List<Tour>)result.Data;
                }
                else
                {
                    ShowErrorMessage(result, "Import Error");
                    return;
                }
                    
                if (importedTours.Count <= 0)
                {
                    MessageBox.Show("No tours found in the selected file.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (Tour tour in importedTours)
                {
                    result = _tourService.CreateTour(tour);

                    if (result.Code != Result.ResultCode.Success)
                    {
                        ShowErrorMessage(result, "Import Error");
                        ReloadList();
                        return;
                    }
                }

                MessageBox.Show("Tours imported successfully.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Information);
                ReloadList();
            }
        }

        private async void ExportAllTours()
        {
            if (TourListViewModel.Tours == null || TourListViewModel.Tours.Count <= 0)
            {
                MessageBox.Show("There are no tours to export!", "Export Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                Title = "Export All Tours",
                FileName = "AllTours"
            };
            if (saveFileDialog.ShowDialog() != true) return;

            Result result = _tourService.GetAllTours();
            if (result.Code != Result.ResultCode.Success)
            {
                ShowErrorMessage(result, "Export Error");
                return;
            }
            if (result.Data is not List<Tour> tours)
            {
                ShowErrorMessage(new Result(Result.ResultCode.UnknownError), "Export Error");
                return;
            }

            if (saveFileDialog.FileName.EndsWith(".json"))
                result = await _importExportService.ExportToursToJsonAsync(tours, saveFileDialog.FileName);
            if (saveFileDialog.FileName.EndsWith(".xlsx"))
                result = _importExportService.ExportToursToExcel(tours, saveFileDialog.FileName);

            if (result.Code == Result.ResultCode.Success)
                MessageBox.Show("Tours exported successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"Failed to export tours.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void OnExportTour()
        {
            if (_selectedTour == null) 
            {
                MessageBox.Show("Please select a tour to export.", "Export Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                Title = "Export Tour Data",
                FileName = $"{_selectedTour.Name}"
            };
            if (saveFileDialog.ShowDialog() != true) return;

            Result result = new Result(Result.ResultCode.UnknownError);

            if (saveFileDialog.FileName.EndsWith(".json"))
                result = await _importExportService.ExportToursToJsonAsync(new List<Tour> { _selectedTour }, saveFileDialog.FileName);
            else if (saveFileDialog.FileName.EndsWith(".xlsx"))
                result = _importExportService.ExportToursToExcel(new List<Tour> { _selectedTour }, saveFileDialog.FileName);
            
            if (result.Code == Result.ResultCode.Success) 
                MessageBox.Show("Tour exported successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"Failed to export tour.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            
        }

        private async void GenerateTourReport()
        {
            try
            {
                if (_selectedTour == null)
                {
                    MessageBox.Show("Please select a tour first.", "No Tour Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Save Tour Report",
                    FileName = $"TourReport_{_selectedTour.Name}_{DateTime.Now:yyyyMMdd}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    Result result = await _reportGenerationService.GenerateTourReport(_selectedTour, saveFileDialog.FileName);
                    if (result.Code == Result.ResultCode.Success)
                        MessageBox.Show("Tour report generated successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show($"Failed to generate tour report: {result.Code}", "Report Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate tour report.\nDetails: {ex.Message}", "Report Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateSummaryReport()
        {
            if (TourListViewModel.Tours == null || TourListViewModel.Tours.Count <= 0)
            {
                MessageBox.Show("There are no tours to create a report for!", "Report Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save Summary Report",
                FileName = $"TourSummary_{DateTime.Now:yyyyMMdd}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Result result = _tourService.GetAllTours();
                if (result.Code != Result.ResultCode.Success)
                {
                    ShowErrorMessage(result, "Summary Error");
                    return;
                }
                if (result.Data is not List<Tour> tours)
                {
                    ShowErrorMessage(result, "Summary Error");
                    return;
                }

                _reportGenerationService.GenerateSummaryReport(tours, saveFileDialog.FileName);
                MessageBox.Show("Summary report generated successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ShowErrorMessage(Result result, string errorType)
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
                case Result.ResultCode.FileAccessError:
                    message = "Unable to access the file. It might be open in another program, or you lack the necessary permissions.";
                    break;
                case Result.ResultCode.PdfGenerationError:
                    message = "There was a problem generating the PDF file.";
                    break;
                case Result.ResultCode.ParseError:
                    message = "Failed to process the route data. Please check the input data.";
                    break;
                case Result.ResultCode.ApiError:
                    message = "A network error occurred while contacting the route service.";
                    break;
                case Result.ResultCode.UnknownError:
                    message = "An unknown error occurred.";
                    break;
            }

            MessageBox.Show(message, errorType, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
