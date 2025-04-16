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

namespace TourPlanner.UILayer.ViewModels
{
    public class TourLogsManagementViewModel : BaseViewModel
    {
        private readonly TourLogService _tourLogService;
        private Tour _selectedTour;
        private ObservableCollection<TourLog> _tourLogs;
        private TourLog _selectedTourLog;


        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour == value) return;
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
                UpdateTourLogs();
            }
        }

        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            private set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
            }
        }

        public RelayCommand CreateTourLogCommand => new RelayCommand(execute => OnCreateTourLog(), canExecute => SelectedTour != null);
        public RelayCommand DeleteTourLogCommand => new RelayCommand(execute => OnDeleteTourLog(), canExecute => SelectedTourLog != null);
        public RelayCommand EditTourLogCommand => new RelayCommand(execute => OnEditTourLog(), canExecute => SelectedTourLog != null);

        public TourLogsManagementViewModel()
        {
            _tourLogService = new TourLogService();
            TourLogs = new ObservableCollection<TourLog>();
        }

        private void UpdateTourLogs()
        {
            if (SelectedTour != null)
            {
                List<TourLog> logs = _tourLogService.GetTourLogs(SelectedTour);
                TourLogs = new ObservableCollection<TourLog>(logs);
            }
            else
            {
                TourLogs = new ObservableCollection<TourLog>();
            }
        }

        private void OnCreateTourLog() {
            if (SelectedTour == null) return;
            Window createTourLogWindow = new Window
            {
                Title = "Create Tour Log",
                Content = new CreateTourLog(),
                Width = 400,
                Height = 600,
                WindowStyle = WindowStyle.ToolWindow,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = Application.Current.MainWindow,
                Background = new SolidColorBrush(Color.FromRgb(30, 70, 32))
            };

            CreateTourLogViewModel viewModel = (CreateTourLogViewModel)((CreateTourLog)createTourLogWindow.Content).DataContext;
            viewModel.TourLogCreated += (sender, tourLog) =>
            {
                try
                {
                    TourLog newTourLog = _tourLogService.CreateTourLog(
                        tourLog.Date,
                        tourLog.Comment,
                        tourLog.Difficulty,
                        tourLog.Rating,
                        tourLog.TotalDistance,
                        tourLog.TotalTime,
                        SelectedTour
                    );
                        
                    UpdateTourLogs();
                    createTourLogWindow.Close();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validierungsfehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            viewModel.Cancelled += (sender, args) =>
            {
                createTourLogWindow.Close();
            };

            createTourLogWindow.ShowDialog();
        }

        private void OnDeleteTourLog() {
            if (SelectedTourLog == null || SelectedTour == null) return;
            MessageBoxResult result = MessageBox.Show(
                "Möchten Sie diesen Tour Log wirklich löschen?",
                "DelteTour Log ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes) return;
            _tourLogService.DeleteTourLog(SelectedTourLog, SelectedTour);
            UpdateTourLogs();
            SelectedTourLog = null;
        }

        private void OnEditTourLog() {
            if (SelectedTourLog == null || SelectedTour == null) return;
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
            viewModel.LoadTourLog(SelectedTourLog);

            viewModel.TourLogCreated += (sender, tourLog) =>
            {
                try
                { 
                    _tourLogService.UpdateTourLog(
                        SelectedTourLog,
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
