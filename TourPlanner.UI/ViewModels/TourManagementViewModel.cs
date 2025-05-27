using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;
        private readonly TourImportExportService _importExportService = new TourImportExportService();
        public ICommand ImportToursCommand { get; }
        public ICommand ExportToursCommand { get; }
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


        public TourManagementViewModel(
            CreateTourViewModel createTourViewModel,
            TourListViewModel tourListViewModel,
            SearchBarViewModel searchBarViewModel,
            EventAggregator eventAggregator,
            TourService tourService
        ) {
            _createTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            SearchBarViewModel = searchBarViewModel;
            _eventAggregator = eventAggregator;
            _tourService = tourService;

            ImportToursCommand = new RelayCommand(execute => ImportTours());
            ExportToursCommand = new RelayCommand(execute => ExportTours());

            _createTourViewModel.TourCreated += OnTourCreated;
            _createTourViewModel.TourUpdated += OnTourUpdated;
            _createTourViewModel.Cancelled += OnCancel;
            _eventAggregator.Subscribe<Tour>(OnTourSelected);

            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;
        
            _tourService.CreateTour(tour);
            // Clear search query to show all tours after create
            TourListViewModel.SearchQuery = "";
            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            _eventAggregator.Publish("ShowHome");
        }

        public void OnTourUpdated(object sender, Tour tour)
        {
            if (tour == null) return;
        
            _tourService.UpdateTour(tour);
            TourListViewModel.SearchQuery = "";
            TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
            
            _selectedTour = tour;
            TourListViewModel.SelectedTour = tour;
            _eventAggregator.Publish("ShowHome");
        }

        //  dont use magic strings, save as constant
        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish("ShowHome");
        }

        public void CreateTour()
        {
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void UpdateTour()
        {
            if(_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to edit.");
                return;
            }

            _createTourViewModel.LoadTour(_selectedTour);
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void DeleteTour()
        {
            if(_selectedTour == null)
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

        private async void ImportTours()
        {
            try {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog {
                    Filter = "JSON files (*.json)|*.json|Excel files (*.xlsx)|*.xlsx",
                    Title = "Import Tour Data"
                };
                
                if (openFileDialog.ShowDialog() == true) {
                    List<Tour> importedTours = new List<Tour>();
                    if (openFileDialog.FileName.EndsWith(".json"))
                        importedTours = await _importExportService.ImportToursFromJsonAsync(openFileDialog.FileName);
                    if (openFileDialog.FileName.EndsWith(".xlsx"))
                        importedTours = _importExportService.ImportToursFromExcel(openFileDialog.FileName);

                    if (importedTours.Count == 0) {
                        MessageBox.Show("No tours found in the selected file.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    foreach (var tour in importedTours) {
                        _tourService.CreateTour(tour); 
                    }
                    
                    TourListViewModel.ReloadTours(_tourService.GetAllTours().ToList());
                    MessageBox.Show("Tours imported successfully.", "Import Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Failed to import tours.\nDetails: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExportTours()
        {
            try {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog {
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
            catch (Exception ex) {
                MessageBox.Show($"Failed to export tours.\nDetails: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
