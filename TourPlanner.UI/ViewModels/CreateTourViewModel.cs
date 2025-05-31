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
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;

namespace TourPlanner.UI.ViewModels
{
    public class CreateTourViewModel : BaseViewModel {
        private string _name;
        private DateTime _date;
        private string _description;
        private string _transportType;
        private string _from;
        private string _to;
        private Tour _editingTour;

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
            set {
                _name = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public DateTime Date
        {
            get => _date;
            set {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Description
        {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string TransportType
        {
            get => _transportType;
            set {
                _transportType = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string From
        {
            get => _from;
            set {
                _from = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string To
        {
            get => _to;
            set {
                _to = value;
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public List<string> TransportTypeOptions { get; } = new List<string>
        {
            "Plane",
            "Bus",
            "Train",
            "Car"
        };

        public CreateTourViewModel() {
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
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void CreateTour() {
            DateTime dateUtc = DateTime.SpecifyKind(_date.Date, DateTimeKind.Utc);

            if (_isEditing && _editingTour != null) {
                _editingTour.Name = _name;
                _editingTour.Date = dateUtc;
                _editingTour.Description = _description;
                _editingTour.TransportType = _transportType;
                _editingTour.From = _from;
                _editingTour.To = _to;

                TourUpdated?.Invoke(this, _editingTour);
                //Log.Information("Tour edited => {@_editingTour}", _editingTour);
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
                //Log.Information("Tour created => {@tour}", tour);
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
        
        
        private bool ValidateInput() {
            (bool IsValid, string Message)[] errors = new (bool IsValid, string Message)[]
            {
                (!string.IsNullOrWhiteSpace(Name), "Name is required"),
                (!string.IsNullOrWhiteSpace(Description), "Description is required"),
                (!string.IsNullOrWhiteSpace(TransportType), "TransportType is required"),
                (!string.IsNullOrWhiteSpace(From), "From is required"),
                (!string.IsNullOrWhiteSpace(To), "To is required"),
                (Date > DateTime.UtcNow, "Date must be in the future")
            };

            foreach (var (isValid, message) in errors.Where(e => !e.IsValid))
                Debug.WriteLine($"- {message}");

            return errors.All(e => e.IsValid);
        }

        private void Cancel()
        {
            ResetForm();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    } 
 }
