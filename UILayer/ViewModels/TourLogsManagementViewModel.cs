using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Views;
using System.Windows.Media;
using System.Linq;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourLogsManagementViewModel : BaseViewModel
    {
        private readonly TourLogService _tourLogService;

        public TourLogListViewModel TourLogListViewModel { get; }
        private CreateTourLogViewModel _createTourLogViewModel;

        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;
        private TourLog _selectedTourLog;

        public EventHandler CreateTourLog;

        public RelayCommand CreateTourLogCommand => new RelayCommand(
            execute => OnCreateTourLog(), 
            canExecute => _selectedTour != null
            );
        public RelayCommand DeleteTourLogCommand => new RelayCommand(
            execute => OnDeleteTourLog(), 
            canExecute => _selectedTourLog != null
            );
        public RelayCommand EditTourLogCommand => new RelayCommand(
            execute => OnEditTourLog(), 
            canExecute => _selectedTourLog != null
            );

        public TourLogsManagementViewModel(
            CreateTourLogViewModel createTourLogViewModel,
            TourLogListViewModel tourLogListViewModel,
            EventAggregator eventAggregator
        ) {
            _tourLogService = new TourLogService();
            TourLogListViewModel = tourLogListViewModel;
            _createTourLogViewModel = createTourLogViewModel;
            _eventAggregator = eventAggregator;

            _eventAggregator.Subscribe<Tour>(OnTourSelected);
            TourLogListViewModel.TourLogSelected += OnTourLogSelected;
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            Console.WriteLine("Tour selected");
            UpdateTourLogs();
        }

        public void OnTourLogSelected(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;
            _selectedTourLog = tourLog;
        }

        private void UpdateTourLogs()
        {
            if (_selectedTour != null)
            {
                List<TourLog> tourLogs = _tourLogService.GetTourLogs(_selectedTour);
                TourLogListViewModel.TourLogs = new ObservableCollection<TourLog>(tourLogs);
            }
            else
            {
                return;
            }
        }

        private void OnCreateTourLog() {
            if (_selectedTour == null) return;

            Console.WriteLine("Create Tour log triggered");
            _eventAggregator.Publish("ShowCreateTourLog");
        }

        private void OnDeleteTourLog() {
            if (_selectedTourLog == null || _selectedTour == null) return;
            MessageBoxResult result = MessageBox.Show(
                "Möchten Sie diesen Tour Log wirklich löschen?",
                "DelteTour Log ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes) return;

            TourLogListViewModel.OnTourLogDeleted(_selectedTourLog);
            _tourLogService.DeleteTourLog(_selectedTourLog, _selectedTour);
            _selectedTourLog = null;
            
            /*
            UpdateTourLogs();
            _selectedTourLog = null;*/
        }

        private void OnEditTourLog() {
            if (_selectedTourLog == null || _selectedTour == null) return;
            Window editTourLogWindow = new Window
            {
                Title = "Edit Tour Log ",
                Content = new CreateTourLog(),
                Width = 400,
                Height = 600,
                WindowStyle = WindowStyle.ToolWindow,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = Application.Current.MainWindow,
                Background = new SolidColorBrush(Color.FromRgb(30, 70, 32))
            };

            CreateTourLogViewModel viewModel = (CreateTourLogViewModel)((CreateTourLog)editTourLogWindow.Content).DataContext;
            viewModel.LoadTourLog(_selectedTourLog);

            viewModel.TourLogCreated += (sender, tourLog) =>
            {
                try
                { 
                    _tourLogService.UpdateTourLog(
                        _selectedTourLog,
                        tourLog.Date,
                        tourLog.Comment,
                        tourLog.Difficulty,
                        tourLog.Rating,
                        tourLog.TotalDistance,
                        tourLog.TotalTime
                    );
                        
                    UpdateTourLogs();
                    editTourLogWindow.Close();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            viewModel.Cancelled += (sender, args) =>
            {
                editTourLogWindow.Close();
            };

            editTourLogWindow.ShowDialog();
        }
    }
}
