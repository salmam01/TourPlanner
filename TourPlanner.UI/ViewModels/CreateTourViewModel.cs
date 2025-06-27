using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.API;
using TourPlanner.BL.Utils.Validators;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class CreateTourViewModel : BaseFormViewModel {
        private OpenRouteService _openRouteService;
        private readonly static int _minQueryLength = 3;

        private string _name;
        private DateTime _date;
        private string _description;
        private string _transportType;
        private string _from;
        private string _selectedFromSuggestion;
        private string _to;
        private string _selectedToSuggestion;
        private Tour _editingTour;

        public EventAggregator _eventAggretator;
        public EventHandler<Tour> TourCreated;
        public EventHandler<Tour> TourUpdated;
        public event EventHandler Cancelled;
        public bool _isEditing;
        public string SubmitButtonText => _isEditing ? "Save Tour" : "Create Tour";
        public bool CanCreate => ValidateInput();

        public ICommand CreateTourCommand => new RelayCommand(
            execute => CreateTour(),
            canExecute => CanCreate
        );
        public ICommand CancelCommand => new RelayCommand(
            execute => Cancel()
        );

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                ValidateName();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(NameError));
            }
        }
        public string NameError => GetFirstError(nameof(Name));

        public DateTime Date
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

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                ValidateDescription();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(DescriptionError));
            }
        }
        public string DescriptionError => GetFirstError(nameof(Description));

        public string TransportType
        {
            get => _transportType;
            set
            {
                if (_transportType == value) return;
                _transportType = value;
                ValidateTransportType();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(TransportTypeError));
            }
        }
        public string TransportTypeError => GetFirstError(nameof(TransportType));

        public string From
        {
            get => _from;
            set
            {
                if (_from == value) return;
                _from = value;
                ValidateFrom();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(FromError));
                _ = OnFromParamsChanged();
            }
        }

        public string SelectedFromSuggestion
        {
            get => _selectedFromSuggestion;
            set
            {
                _selectedFromSuggestion = value;
                From = value;
                OnPropertyChanged();
            }
        }

        public string FromError => GetFirstError(nameof(From));

        public string To
        {
            get => _to;
            set
            {
                if (_to == value) return;
                _to = value;
                ValidateTo();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(ToError));
                _ = OnToParamsChanged();
            }
        }
        public string SelectedToSuggestion
        {
            get => _selectedToSuggestion;
            set
            {
                _selectedToSuggestion = value;
                To = value;
                OnPropertyChanged();
            }
        }

        public string ToError => GetFirstError(nameof(To));

        public List<string> TransportTypeOptions { get; } = new List<string>
        {
            "Plane",
            "Bus",
            "Train",
            "Car"
        };

        public List<string> FromLocationSuggestions { get; set; } = [];
        public List<string> ToLocationSuggestions { get; set; } = [];

        public CreateTourViewModel(OpenRouteService openRouteService) {
            _openRouteService = openRouteService;
            ResetForm();
        }

        private void ResetForm() {
            _name = "";
            _date = DateTime.UtcNow;
            _description = "";
            _transportType = "";
            _from = "";
            _to = "";
            _isEditing = false;

            ClearErrors(nameof(Name));
            ClearErrors(nameof(Description));
            ClearErrors(nameof(TransportType));
            ClearErrors(nameof(From));
            ClearErrors(nameof(To));
            ClearErrors(nameof(Date));

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TransportType));
            OnPropertyChanged(nameof(From));
            OnPropertyChanged(nameof(To));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void CreateTour() {
            ValidateName();
            ValidateDescription();
            ValidateTransportType();
            ValidateFrom();
            ValidateTo();
            ValidateDate();

            if (!ValidateInput())
            {
                MessageBox.Show("Please fill in all required fields correctly!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dateUtc = DateTime.SpecifyKind(_date.Date, DateTimeKind.Utc);

            if (_isEditing && _editingTour != null) {
                _editingTour.Name = _name;
                _editingTour.Date = dateUtc;
                _editingTour.Description = _description;
                _editingTour.TransportType = _transportType;
                _editingTour.From = _from;
                _editingTour.To = _to;

                TourUpdated?.Invoke(this, _editingTour);
            }
            else {
                Tour tour = new Tour(
                    _name,
                    dateUtc,
                    _description,
                    _transportType,
                    _from,
                    _to
                );

                TourCreated?.Invoke(this, tour);
            }
            ResetForm();
        }

        public void LoadTour(Tour tour) {
            _editingTour = tour;
            _name = tour.Name;
            _date = tour.Date;
            _description = tour.Description;
            _transportType = tour.TransportType;
            _from = tour.From;
            _to = tour.To;
            _isEditing = true;
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private bool ValidateInput()
        {
            return
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Name))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Description))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(TransportType))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(From))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(To))) &&
                string.IsNullOrWhiteSpace(GetFirstError(nameof(Date)));
        }

        private void Cancel()
        {
            ResetForm();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private async Task OnFromParamsChanged()
        {
            if (!string.IsNullOrEmpty(From) && From.Length >= _minQueryLength && From.Length % _minQueryLength == 0)
            {
                FromLocationSuggestions = await _openRouteService.GetLocationSuggestionsAsync(From);
                OnPropertyChanged(nameof(FromLocationSuggestions));
            }
        }

        private async Task OnToParamsChanged()
        {
            if (!string.IsNullOrEmpty(To) && To.Length >= _minQueryLength && To.Length % _minQueryLength == 0)
            {
                ToLocationSuggestions = await _openRouteService.GetLocationSuggestionsAsync(To);
                OnPropertyChanged(nameof(ToLocationSuggestions));
            }
        }

        private void ValidateName()
        {
            string error = TourValidator.ValidateName(Name);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Name), error);
            else
                ClearErrors(nameof(Name));
        }
        private void ValidateDescription()
        {
            string error = TourValidator.ValidateDescription(Description);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Description), error);
            else
                ClearErrors(nameof(Description));
        }
        private void ValidateTransportType()
        {
            string error = TourValidator.ValidateTransportType(TransportType);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(TransportType), error);
            else
                ClearErrors(nameof(TransportType));
        }
        private void ValidateFrom()
        {
            string error = TourValidator.ValidateFrom(From);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(From), error);
            else
                ClearErrors(nameof(From));
        }
        private void ValidateTo()
        {
            string error = TourValidator.ValidateTo(To);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(To), error);
            else
                ClearErrors(nameof(To));
        }
        private void ValidateDate()
        {
            string error = TourValidator.ValidateDate(Date);
            if (!string.IsNullOrEmpty(error))
                SetError(nameof(Date), error);
            else
                ClearErrors(nameof(Date));
        }

        private string GetFirstError(string propertyName)
        {
            List<string> errors = GetErrors(propertyName) as List<string>;
            return errors != null && errors.Any() ? errors.First() : null;
        }

    } 
 }