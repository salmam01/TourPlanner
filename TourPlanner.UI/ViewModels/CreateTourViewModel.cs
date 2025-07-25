﻿using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.UI.API;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;
using TourPlanner.UI.Validators;

namespace TourPlanner.UI.ViewModels
{
    public class CreateTourViewModel : BaseFormViewModel
    {
        private OpenRouteService _openRouteService;

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
                if (_selectedFromSuggestion == value) return;
                _selectedFromSuggestion = value;
                _from = value;
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
                if (_selectedToSuggestion == value) return;
                _selectedToSuggestion = value;
                _to = value;
                OnPropertyChanged();
            }
        }
        public List<string> FromLocationSuggestions { get; set; } = new List<string>();
        public List<string> ToLocationSuggestions { get; set; } = new List<string>();

        public string ToError => GetFirstError(nameof(To));

        public List<string> TransportTypeOptions { get; } = new List<string>
        {
            "Walking",
            "Hiking (Trails)",
            "Bicycle",
            "Road Bike",
            "Mountain Bike",
            "E-Bike",
            "Car"
        };

        public CreateTourViewModel(OpenRouteService openRouteService)
        {
            _openRouteService = openRouteService;
            ResetForm();
        }

        private void ResetForm()
        {
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

            FromLocationSuggestions = new List<string>();
            ToLocationSuggestions = new List<string>();

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TransportType));
            OnPropertyChanged(nameof(From));
            OnPropertyChanged(nameof(To));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private async void CreateTour()
        {
            ValidateName();
            ValidateDescription();
            ValidateTransportType();
            ValidateFrom();
            ValidateTo();
            OnPropertyChanged(nameof(ToError));
            ValidateDate();

            if (!ValidateInput())
            {
                MessageBox.Show("Please fill in all required fields correctly!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate addresses if they are not in suggestions
            bool fromInSuggestions = FromLocationSuggestions.Any(s => string.Equals(s, From, StringComparison.OrdinalIgnoreCase));
            bool toInSuggestions = ToLocationSuggestions.Any(s => string.Equals(s, To, StringComparison.OrdinalIgnoreCase));

            if (!fromInSuggestions)
            {
                bool fromValid = await ValidateAddress(From);
                if (!fromValid)
                {
                    SetError(nameof(From), "Please select a valid address.");
                    return;
                }
            }

            if (!toInSuggestions)
            {
                bool toValid = await ValidateAddress(To);
                if (!toValid)
                {
                    SetError(nameof(To), "Please select a valid address.");
                    return;
                }
            }

            DateTime dateUtc = DateTime.SpecifyKind(_date.Date, DateTimeKind.Utc);

            if (_isEditing && _editingTour != null)
            {
                _editingTour.Name = _name;
                _editingTour.Date = dateUtc;
                _editingTour.Description = _description;
                _editingTour.TransportType = _transportType;
                _editingTour.From = _from;
                _editingTour.To = _to;

                TourUpdated?.Invoke(this, _editingTour);
            }
            else
            {
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

        public void LoadTour(Tour tour)
        {
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
            if (!string.IsNullOrEmpty(From))
            {
                Result result = await _openRouteService.GetLocationSuggestionsAsync(From);
                FromLocationSuggestions = (List<string>)result.Data ?? new List<string>();
                OnPropertyChanged(nameof(FromLocationSuggestions));
            }
        }

        private async Task OnToParamsChanged()
        {
            if (!string.IsNullOrEmpty(To))
            {
                Result result = await _openRouteService.GetLocationSuggestionsAsync(To);
                ToLocationSuggestions = (List<string>)result.Data ?? new List<string>();
                OnPropertyChanged(nameof(ToLocationSuggestions));
            }
        }

        private async Task<bool> ValidateAddress(string address)
        {
            Result result = await _openRouteService.CheckIfAddressExists(address);

            switch (result.Code)
            {
                case Result.ResultCode.Success:
                    return true;
                case Result.ResultCode.InvalidAddress:
                    MessageBox.Show($"The address '{address}' could not be found. Please enter a valid address.",
                        "Invalid Address", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                case Result.ResultCode.ApiError:
                    MessageBox.Show("Network error while validating address. Please check your internet connection.",
                        "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                case Result.ResultCode.ParseError:
                    MessageBox.Show("Error processing address validation. Please try again.",
                        "Processing Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                default:
                    MessageBox.Show("Unknown error occurred while validating address. Please try again.",
                        "Unknown Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
            }
        }

        private void ValidateDate()
        {
            string error = TourValidator.ValidateDate(Date);
            if (!string.IsNullOrWhiteSpace(error))
                SetError(nameof(Date), error);
            else
                ClearErrors(nameof(Date));
        }

        private void ValidateName()
        {
            string error = TourValidator.ValidateName(Name);
            if (!string.IsNullOrWhiteSpace(error))
                SetError(nameof(Name), error);
            else
                ClearErrors(nameof(Name));
        }
        private void ValidateDescription()
        {
            string error = TourValidator.ValidateDescription(Description);
            if (!string.IsNullOrWhiteSpace(error))
                SetError(nameof(Description), error);
            else
                ClearErrors(nameof(Description));
        }
        private void ValidateTransportType()
        {
            string error = TourValidator.ValidateTransportType(TransportType);
            if (!string.IsNullOrWhiteSpace(error))
                SetError(nameof(TransportType), error);
            else
                ClearErrors(nameof(TransportType));
        }
        private void ValidateFrom()
        {
            string error = TourValidator.ValidateFrom(From);

            if (!string.IsNullOrWhiteSpace(error))
            {
                SetError(nameof(From), error);
                return;
            }

            bool foundInSuggestions = FromLocationSuggestions
                .Any(s => string.Equals(s, From, StringComparison.OrdinalIgnoreCase));
            if (foundInSuggestions)
            {
                ClearErrors(nameof(From));
                return;
            }

            // For now, just clear errors if basic validation passes
            // Address validation can be done when user submits the form
            ClearErrors(nameof(From));
        }

        private void ValidateTo()
        {
            string error = TourValidator.ValidateTo(To);

            if (!string.IsNullOrWhiteSpace(error))
            {
                SetError(nameof(To), error);
                return;
            }

            bool foundInSuggestions = ToLocationSuggestions
                .Any(s => string.Equals(s, To, StringComparison.OrdinalIgnoreCase));
            if (foundInSuggestions)
            {
                ClearErrors(nameof(To));
                return;
            }

            // For now, just clear errors if basic validation passes
            // Address validation can be done when user submits the form
            ClearErrors(nameof(To));
        }

        private string GetFirstError(string propertyName)
        {
            List<string> errors = GetErrors(propertyName) as List<string>;
            return errors != null && errors.Any() ? errors.First() : null;
        }

    }
}