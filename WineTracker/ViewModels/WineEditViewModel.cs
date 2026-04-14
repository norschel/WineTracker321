using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WineTracker.Models;

namespace WineTracker.ViewModels
{
    public class WineEditViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _name = string.Empty;
        private string _description = string.Empty;
        private int _rate = 3;
        private string _rateDescription = string.Empty;
        private string _wineyard = string.Empty;
        private string _city = string.Empty;
        private string _link = string.Empty;

        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsNew { get; set; } = true;

        public string WindowTitle => IsNew ? "Add Wine" : "Edit Wine";

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); ValidateRequired(value); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public int Rate
        {
            get => _rate;
            set
            {
                _rate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRate1));
                OnPropertyChanged(nameof(IsRate2));
                OnPropertyChanged(nameof(IsRate3));
                OnPropertyChanged(nameof(IsRate4));
                OnPropertyChanged(nameof(IsRate5));
            }
        }

        public string RateDescription
        {
            get => _rateDescription;
            set { _rateDescription = value; OnPropertyChanged(); }
        }

        public string Wineyard
        {
            get => _wineyard;
            set { _wineyard = value; OnPropertyChanged(); ValidateRequired(value); }
        }

        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); ValidateRequired(value); }
        }

        public string Link
        {
            get => _link;
            set { _link = value; OnPropertyChanged(); }
        }

        // ── Star-rating helpers (two-way bound to radio buttons) ────────────

        public bool IsRate1 { get => _rate == 1; set { if (value) Rate = 1; } }
        public bool IsRate2 { get => _rate == 2; set { if (value) Rate = 2; } }
        public bool IsRate3 { get => _rate == 3; set { if (value) Rate = 3; } }
        public bool IsRate4 { get => _rate == 4; set { if (value) Rate = 4; } }
        public bool IsRate5 { get => _rate == 5; set { if (value) Rate = 5; } }

        public bool IsValid => !HasErrors;

        // ── INotifyDataErrorInfo ────────────────────────────────────────────

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && _errors.TryGetValue(propertyName, out var errors))
                return errors;
            return Array.Empty<string>();
        }

        private void ValidateRequired(string value, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null) return;
            if (string.IsNullOrWhiteSpace(value))
                AddError(propertyName, "This field is required.");
            else
                ClearErrors(propertyName);
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        // ── Conversion ─────────────────────────────────────────────────────

        public void LoadFromWine(Wine wine)
        {
            Id = wine.Id;
            IsNew = false;
            Name = wine.Name;
            Description = wine.Description;
            Rate = wine.Rate;
            RateDescription = wine.RateDescription;
            Wineyard = wine.Wineyard;
            City = wine.City;
            Link = wine.Link;
        }

        public Wine ToWine() => new Wine
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Rate = Rate,
            RateDescription = RateDescription,
            Wineyard = Wineyard,
            City = City,
            Link = Link
        };

        // ── INotifyPropertyChanged ──────────────────────────────────────────

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
