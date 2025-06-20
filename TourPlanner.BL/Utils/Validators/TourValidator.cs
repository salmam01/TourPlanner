using System;
using System.Collections.Generic;

namespace TourPlanner.BL.Utils.Validators
{
    public static class TourValidator
    {
        public static string ValidateName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "Name is required" : null;
        }

        public static string ValidateDescription(string description)
        {
            return string.IsNullOrWhiteSpace(description) ? "Description is required" : null;
        }

        public static string ValidateTransportType(string transportType)
        {
            return string.IsNullOrWhiteSpace(transportType) ? "TransportType is required" : null;
        }

        public static string ValidateFrom(string from)
        {
            return string.IsNullOrWhiteSpace(from) ? "From is required" : null;
        }

        public static string ValidateTo(string to)
        {
            return string.IsNullOrWhiteSpace(to) ? "To is required" : null;
        }

        public static string ValidateDate(DateTime date)
        {
            return date <= DateTime.UtcNow ? "Date must be in the future" : null;
        }

        public static Dictionary<string, string> ValidateAll(string name, string description, string transportType, string from, string to, DateTime date)
        {
            var errors = new Dictionary<string, string>
            {
                { nameof(name), ValidateName(name) },
                { nameof(description), ValidateDescription(description) },
                { nameof(transportType), ValidateTransportType(transportType) },
                { nameof(from), ValidateFrom(from) },
                { nameof(to), ValidateTo(to) },
                { nameof(date), ValidateDate(date) }
            };
            return errors;
        }
    }
}