using System;
using System.Windows;
using System.Diagnostics;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels
{
    public class CreateTourLogViewModel : BaseViewModel
    {
        private DateTime _date;
        private string _comment;
        private int _difficulty, _hours, _minutes;
        private double _rating, _totalDistance;
        private bool _isEditing;
        private Guid _editingId;

        public event EventHandler<TourLog> TourLogCreated;
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
                OnPropertyChanged(nameof(Comment));
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
                OnPropertyChanged(nameof(Difficulty));
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
                OnPropertyChanged(nameof(Rating));
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
                OnPropertyChanged(nameof(TotalDistance));
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
                OnPropertyChanged(nameof(Hours));
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
                OnPropertyChanged(nameof(Minutes));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string ButtonText => _isEditing ? "Save" : "Create";

        public bool CanCreate => ValidateInput();

        public RelayCommand CreateCommand => new RelayCommand(execute => CreateTourLog(), canExecute => CanCreate);
        public RelayCommand CancelCommand => new RelayCommand(execute => Cancel());

        public CreateTourLogViewModel()
        {
            ResetForm();
        }

        public void ResetForm()
        {
            Date = DateTime.Now;
            Comment = "";
            Difficulty = 1;
            Rating = 1;
            TotalDistance = 0;
            Hours = 0;
            Minutes = 0;
            _isEditing = false;
        }

        //  TODO: Refactor this
        private bool ValidateInput()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Comment) &&
                           Difficulty >= 1 && Difficulty <= 5 &&
                           Rating >= 1 && Rating <= 5 &&
                           TotalDistance >= 0 &&
                           Hours >= 0 &&
                           Minutes >= 0 && Minutes < 60;

            if (isValid) return true;
            if (string.IsNullOrWhiteSpace(Comment)) Debug.WriteLine("- Comment is empty");
            if (Difficulty < 1 || Difficulty > 5) Debug.WriteLine("- Invalid Difficulty");
            if (Rating < 1 || Rating > 5) Debug.WriteLine("- Invalid Rating");
            if (TotalDistance < 0) Debug.WriteLine("- Invalid TotalDistance");
            if (Hours < 0) Debug.WriteLine("- Invalid Hours");
            if (Minutes < 0 || Minutes >= 60) Debug.WriteLine("- Invalid Minutes");

            return false;
        }

        private void CreateTourLog()
        {
            TourLog tourLog = new TourLog
            {
                Id = _isEditing ? _editingId : Guid.NewGuid(),
                Date = Date,
                Comment = Comment,
                Difficulty = Difficulty,
                Rating = Rating,
                TotalDistance = TotalDistance,
                TotalTime = new TimeSpan(Hours, Minutes, 0)
            };

            TourLogCreated?.Invoke(this, tourLog);

            if (!_isEditing)
            {
                ResetForm();
            }
        }

        private void Cancel()
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        public void LoadTourLog(TourLog tourLog)
        {
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
        }
    }
}
