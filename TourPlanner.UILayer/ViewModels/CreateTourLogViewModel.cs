using System;
using System.Windows;
using System.Diagnostics;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    /// <summary>
    /// ViewModel for creating and editing tour logs
    /// </summary>
    public class CreateTourLogViewModel : BaseViewModel
    {
        private readonly TourLogService _tourLogService;
        private bool _isEditing;
        private Guid _editingId;

        // Tour log properties
        private DateTime _date;
        private string _comment;
        private int _difficulty;
        private double _rating;
        private double _totalDistance;
        private int _hours;
        private int _minutes;

        // Events
        public event EventHandler<TourLog> TourLogCreated;
        public event EventHandler Cancelled;

        // Commands
        public RelayCommand CreateCommand => new RelayCommand(execute => CreateTourLog(), canExecute => CanCreate);
        public RelayCommand CancelCommand => new RelayCommand(execute => Cancel());

        // Properties
        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    Debug.WriteLine($"Comment updated: {value}");
                    OnPropertyChanged(nameof(Comment));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public int Difficulty
        {
            get => _difficulty;
            set
            {
                if (_difficulty != value)
                {
                    _difficulty = value;
                    Debug.WriteLine($"Difficulty updated: {value}");
                    OnPropertyChanged(nameof(Difficulty));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public double Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    Debug.WriteLine($"Rating updated: {value}");
                    OnPropertyChanged(nameof(Rating));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public double TotalDistance
        {
            get => _totalDistance;
            set
            {
                if (_totalDistance != value)
                {
                    _totalDistance = value;
                    Debug.WriteLine($"TotalDistance updated: {value}");
                    OnPropertyChanged(nameof(TotalDistance));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public int Hours
        {
            get => _hours;
            set
            {
                if (_hours != value)
                {
                    _hours = value;
                    Debug.WriteLine($"Hours updated: {value}");
                    OnPropertyChanged(nameof(Hours));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                if (_minutes != value)
                {
                    _minutes = value;
                    Debug.WriteLine($"Minutes updated: {value}");
                    OnPropertyChanged(nameof(Minutes));
                    OnPropertyChanged(nameof(CanCreate));
                }
            }
        }

        public string ButtonText => _isEditing ? "Speichern" : "Erstellen";
        public bool CanCreate => ValidateInput();

        public CreateTourLogViewModel()
        {
            _tourLogService = new TourLogService();
            InitializeDefaultValues();
            Debug.WriteLine("CreateTourLogViewModel initialized");
        }

        private void InitializeDefaultValues()
        {
            Date = DateTime.Now;
            Comment = string.Empty;
            Difficulty = 1;
            Rating = 1;
            TotalDistance = 0;
            Hours = 0;
            Minutes = 0;
            _isEditing = false;
        }

        private bool ValidateInput()
        {
            try
            {
                var totalTime = new TimeSpan(Hours, Minutes, 0);
                _tourLogService.ValidateTourLogData(Date, Comment, Difficulty, Rating, TotalDistance, totalTime);
                return true;
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Validation failed: {ex.Message}");
                return false;
            }
        }

        private void CreateTourLog()
        {
            Debug.WriteLine("Creating new tour log");
            var totalTime = new TimeSpan(Hours, Minutes, 0);

            var tourLog = new TourLog
            {
                Id = _isEditing ? _editingId : Guid.NewGuid(),
                Date = Date,
                Comment = Comment,
                Difficulty = Difficulty,
                Rating = Rating,
                TotalDistance = TotalDistance,
                TotalTime = totalTime
            };

            Debug.WriteLine($"Tour log created successfully: {tourLog}");
            TourLogCreated?.Invoke(this, tourLog);
        }

        private void Cancel()
        {
            Debug.WriteLine("Operation cancelled by user");
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        public void LoadTourLog(TourLog tourLog)
        {
            if (tourLog == null)
                throw new ArgumentNullException(nameof(tourLog));

            Debug.WriteLine($"Loading tour log for editing: {tourLog}");
            _isEditing = true;
            _editingId = tourLog.Id;
            
            Date = tourLog.Date;
            Comment = tourLog.Comment;
            Difficulty = tourLog.Difficulty;
            Rating = tourLog.Rating;
            TotalDistance = tourLog.TotalDistance;
            Hours = tourLog.TotalTime.Hours;
            Minutes = tourLog.TotalTime.Minutes;
            
            OnPropertyChanged(nameof(ButtonText));
            Debug.WriteLine("Tour log loaded successfully for editing");
        }
    }
} 