using System;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using System.Windows.Input;

namespace TourPlanner.UILayer.ViewModels
{
    public class CreateTourLogViewModel : BaseViewModel {
        private DateTime _date;
        private string _comment;
        private int _difficulty, _hours, _minutes;
        private double _rating, _totalDistance;
        private bool _isEditing;
        private Guid _editingId;
        public string SubmitButtonText => _isEditing ? "Save Tour Log" : "Create Tour Log";

        public bool CanCreate => ValidateInput();

        public ICommand CreateCommand => new RelayCommand(
            execute => CreateTourLog(),
            canExecute => CanCreate
        );

        public ICommand CancelCommand => new RelayCommand(
            execute => Cancel()
        );

        public event EventHandler<TourLog> TourLogCreated;
        public event EventHandler<TourLog> TourLogUpdated;
        public event EventHandler Cancelled;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public int Difficulty
        {
            get => _difficulty;
            set
            {
                if (value < 1 || value > 5) return;
                _difficulty = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public double Rating
        {
            get => _rating;
            set
            {
                if (!(value >= 1) || !(value <= 5)) return;
                _rating = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public double TotalDistance
        {
            get => _totalDistance;
            set
            {
                if (!(value >= 0)) return;
                _totalDistance = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public int Hours
        {
            get => _hours;
            set
            {
                if (value < 0) return;
                _hours = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                if (value < 0 || value >= 60) return;
                _minutes = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }
        public CreateTourLogViewModel() {
            ResetForm();
        }
        

        public void ResetForm() {
            Date = DateTime.UtcNow;
            Comment = "";
            Difficulty = 1;
            Rating = 1;
            TotalDistance = 0;
            Hours = 0;
            Minutes = 0;
            _isEditing = false;
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void CreateTourLog() {

            DateTime utcDate = DateTime.SpecifyKind(_date, DateTimeKind.Utc);

            TourLog tourLog = new TourLog
            {
                Id = _isEditing ? _editingId : Guid.NewGuid(),
                Date = utcDate,
                Comment = Comment,
                Difficulty = Difficulty,
                Rating = Rating,
                TotalDistance = TotalDistance,
                TotalTime = new TimeSpan(Hours, Minutes, 0)
            };

            if (_isEditing) {
                TourLogUpdated?.Invoke(this, tourLog);
            }
            else {
                TourLogCreated?.Invoke(this, tourLog);
            }
            ResetForm();
        }
        
        public void LoadTourLog(TourLog tourLog) 
        {
            _isEditing = true;
            _editingId = tourLog.Id;
            _date = tourLog.Date;
            _comment = tourLog.Comment;
            _difficulty = tourLog.Difficulty;
            _rating = tourLog.Rating;
            _totalDistance = tourLog.TotalDistance;
            _hours = tourLog.TotalTime.Hours;
            _minutes = tourLog.TotalTime.Minutes;
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void Cancel() {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidateInput()
        {
            (bool IsValid, string Message)[] errors = new (bool IsValid, string Message)[]
            {
                (!string.IsNullOrWhiteSpace(Comment), "Comment is empty"),
                (IsInRange(Difficulty, 1, 5), "Difficulty must be between 1 and 5"),
                (IsInRange(Rating, 1.0, 5.0), "Rating must be between 1.0 and 5.0"),
                (TotalDistance >= 0, "TotalDistance cannot be negative"),
                (Hours >= 0, "Hours cannot be negative"),
                (IsInRange(Minutes, 0, 59), "Minutes must be between 0 and 59")
            };
            
            foreach (var (isValid, message) in errors.Where(e => !e.IsValid))
                Debug.WriteLine($"- {message}");
            return errors.All(e => e.IsValid);
        }
        
        private static bool IsInRange<T>(T value, T min, T max) where T : IComparable<T> {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
     }
 }
