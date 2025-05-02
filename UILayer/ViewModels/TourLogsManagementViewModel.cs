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

        public RelayCommand CreateTourLogCommand => new RelayCommand(
            execute => CreateTourLog()
        );
        public RelayCommand DeleteTourLogCommand => new RelayCommand(
            execute => DeleteTourLog(), 
            canExecute => _selectedTourLog != null
        );
        public RelayCommand EditTourLogCommand => new RelayCommand(
            execute => EditTourLog(), 
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
            _createTourLogViewModel.TourLogCreated += OnTourLogCreated;
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            UpdateTourLogs();
        }

        public void OnTourLogSelected(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;
            _selectedTourLog = tourLog;
        }

        public void OnTourLogCreated(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;
            TourLogListViewModel.OnTourLogCreated(tourLog);
            _tourLogService.CreateTourLog(_selectedTour, tourLog);
            _eventAggregator.Publish("ShowHomeView");
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

        private void CreateTourLog() 
        {
            if (_selectedTour == null) return;
            _eventAggregator.Publish("ShowCreateTourLog");
        }

        private void DeleteTourLog() 
        {
            if (_selectedTourLog == null || _selectedTour == null) return;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you would like to delete this tour log?",
                "DelteTour Log ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes) return;

            TourLogListViewModel.OnTourLogDeleted(_selectedTourLog);
            _tourLogService.DeleteTourLog(_selectedTourLog, _selectedTour);
            _selectedTourLog = null;
        }

        //  TODO: Fix this 
        private void EditTourLog() {
            if (_selectedTourLog == null || _selectedTour == null) return;
            _eventAggregator.Publish("ShowCreateTourLog");

            _createTourLogViewModel.LoadTourLog(_selectedTourLog);
            _eventAggregator.Publish("ShowCreateTourLog");

            /*Window editTourLogWindow = new Window
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

            editTourLogWindow.ShowDialog();*/
        }
    }
}
