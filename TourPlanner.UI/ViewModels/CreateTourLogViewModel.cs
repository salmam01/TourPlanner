using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Utils.Validators;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;

namespace TourPlanner.UI.ViewModels
{
    public class CreateTourLogViewModel : BaseFormViewModel {
        private DateTime? _date;
        private string _comment;
        private int? _difficulty, _hours, _minutes;
        private double? _rating, _totalDistance;
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
        public event EventHandler<TourLog> TourLogEdited;
        public event EventHandler Cancelled;

        public DateTime? Date
        {
            get => _date;
            set
            {
                if (_date == value) return;
                _date = value;
                ValidateDate();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(DateError));
            }
        }
        public string DateError => GetFirstError(nameof(Date));

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment == value) return;
                _comment = value;
                ValidateComment();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(CommentError));
            }
        }
        public string CommentError => GetFirstError(nameof(Comment));

        public int? Difficulty
        {
            get => _difficulty;
            set
            {
                if (_difficulty == value) return;
                _difficulty = value;
                ValidateDifficulty();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(DifficultyError));
            }
        }
        public string DifficultyError => GetFirstError(nameof(Difficulty));

        public double? Rating
        {
            get => _rating;
            set
            {
                if (_rating == value) return;
                _rating = value;
                ValidateRating();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(RatingError));
            }
        }
        public string RatingError => GetFirstError(nameof(Rating));

        public double? TotalDistance
        {
            get => _totalDistance;
            set
            {
                if (_totalDistance == value) return;
                _totalDistance = value;
                ValidateTotalDistance();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(TotalDistanceError));
            }
        }
        public string TotalDistanceError => GetFirstError(nameof(TotalDistance));

        public int? Hours
        {
            get => _hours;
            set
            {
                if (_hours == value) return;
                _hours = value;
                ValidateTotalTime();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(TotalTimeError));
            }
        }

        public int? Minutes
        {
            get => _minutes;
            set
            {
                if (_minutes == value) return;
                _minutes = value;
                ValidateTotalTime();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(TotalTimeError));
            }
        }
        // Shows TotalTime-related errors for both time inputs (Hours & Minutes)
        public string TotalTimeError => GetFirstError(nameof(Hours)) ?? GetFirstError(nameof(Minutes));

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

            ClearErrors(nameof(Date));
            ClearErrors(nameof(Comment));
            ClearErrors(nameof(Difficulty));
            ClearErrors(nameof(Rating));
            ClearErrors(nameof(TotalDistance));
            ClearErrors(nameof(Hours));
            ClearErrors(nameof(Minutes));
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void CreateTourLog() {
            ValidateDate();
            ValidateComment();
            ValidateDifficulty();
            ValidateRating();
            ValidateTotalDistance();
            ValidateTotalTime();

            if (!ValidateInput()) return;

            DateTime utcDate = DateTime.SpecifyKind(Date ?? DateTime.UtcNow, DateTimeKind.Utc);

            TourLog tourLog = new TourLog
            {
                Id = _isEditing ? _editingId : Guid.NewGuid(),
                Date = utcDate,
                Comment = Comment,
                Difficulty = Difficulty ?? 1,
                Rating = Rating ?? 1,
                TotalDistance = TotalDistance ?? 0,
                TotalTime = new TimeSpan(Hours ?? 0, Minutes ?? 0, 0)
            };

            if (_isEditing) {
                TourLogEdited?.Invoke(this, tourLog);
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
            ResetForm();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
        
        private bool ValidateInput()
        {
            return
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Date))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Comment))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Difficulty))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Rating))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(TotalDistance))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Hours))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Minutes)));
        }

        private string GetFirstError(string propertyName)
        {
            var errors = GetErrors(propertyName) as System.Collections.Generic.List<string>;
            return errors != null && errors.Any() ? errors.First() : null;
        }

        private void ValidateDate()
        {
            var error = TourLogValidator.ValidateDate(Date);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Date), error);
            else
                ClearErrors(nameof(Date));
        }

        private void ValidateComment()
        {
            var error = TourLogValidator.ValidateComment(Comment);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Comment), error);
            else
                ClearErrors(nameof(Comment));
        }

        private void ValidateDifficulty()
        {
            var error = TourLogValidator.ValidateDifficulty(Difficulty);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Difficulty), error);
            else
                ClearErrors(nameof(Difficulty));
        }

        private void ValidateRating()
        {
            var error = TourLogValidator.ValidateRating(Rating.HasValue ? (int?)Rating : null);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Rating), error);
            else
                ClearErrors(nameof(Rating));
        }

        private void ValidateTotalDistance()
        {
            var error = TourLogValidator.ValidateTotalDistance(TotalDistance);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(TotalDistance), error);
            else
                ClearErrors(nameof(TotalDistance));
        }

        private void ValidateTotalTime()
        {
            var time = new TimeSpan(Hours ?? 0, Minutes ?? 0, 0);
            var error = TourLogValidator.ValidateTotalTime(time);
            if (!string.IsNullOrEmpty(error))
            {
                SetError(nameof(Hours), error);
                SetError(nameof(Minutes), error);
            }
            else
            {
                ClearErrors(nameof(Hours));
                ClearErrors(nameof(Minutes));
            }
        }

    }
}
