using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using WineTracker.Models;
using WineTracker.Services;
using WineTracker.Views;

namespace WineTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IWineStorageService _storage;
        private readonly ObservableCollection<Wine> _wines;
        private readonly ICollectionView _winesView;

        private Wine? _selectedWine;
        private string _searchText = string.Empty;

        public MainViewModel() : this(new WineStorageService()) { }

        public MainViewModel(IWineStorageService storage)
        {
            _storage = storage;
            _wines = new ObservableCollection<Wine>(_storage.LoadAll());

            _winesView = CollectionViewSource.GetDefaultView(_wines);
            _winesView.Filter = FilterWine;

            AddCommand = new RelayCommand(OnAdd);
            EditCommand = new RelayCommand(OnEdit, () => SelectedWine != null);
            DeleteCommand = new RelayCommand(OnDelete, () => SelectedWine != null);
            OpenLinkCommand = new RelayCommand(OnOpenLink, () =>
                SelectedWine != null && !string.IsNullOrWhiteSpace(SelectedWine.Link));
        }

        // ── Properties ─────────────────────────────────────────────────────

        public ICollectionView WinesView => _winesView;

        public Wine? SelectedWine
        {
            get => _selectedWine;
            set
            {
                _selectedWine = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedWine));
            }
        }

        public bool HasSelectedWine => _selectedWine != null;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                _winesView.Refresh();
            }
        }

        // ── Commands ───────────────────────────────────────────────────────

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand OpenLinkCommand { get; }

        // ── Command Handlers ───────────────────────────────────────────────

        private void OnAdd()
        {
            var vm = new WineEditViewModel();
            var dialog = new WineEditWindow(vm) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog() == true)
            {
                Wine wine = vm.ToWine();
                _wines.Add(wine);
                _storage.Save(_wines);
                SelectedWine = wine;
            }
        }

        private void OnEdit()
        {
            if (SelectedWine == null) return;

            var vm = new WineEditViewModel();
            vm.LoadFromWine(SelectedWine);

            var dialog = new WineEditWindow(vm) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog() == true)
            {
                Wine updated = vm.ToWine();
                int index = _wines.IndexOf(SelectedWine);
                if (index >= 0)
                    _wines[index] = updated;

                _storage.Save(_wines);
                SelectedWine = updated;
            }
        }

        private void OnDelete()
        {
            if (SelectedWine == null) return;

            var result = MessageBox.Show(
                $"Delete \"{SelectedWine.Name}\"?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _wines.Remove(SelectedWine);
                _storage.Save(_wines);
                SelectedWine = null;
            }
        }

        private void OnOpenLink()
        {
            if (SelectedWine == null || string.IsNullOrWhiteSpace(SelectedWine.Link)) return;

            try
            {
                Process.Start(new ProcessStartInfo(SelectedWine.Link) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open link:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Filter ─────────────────────────────────────────────────────────

        private bool FilterWine(object obj)
        {
            if (obj is not Wine wine) return false;
            if (string.IsNullOrWhiteSpace(_searchText)) return true;

            string q = _searchText.Trim();
            return wine.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                || wine.Wineyard.Contains(q, StringComparison.OrdinalIgnoreCase)
                || wine.City.Contains(q, StringComparison.OrdinalIgnoreCase);
        }

        // ── INotifyPropertyChanged ──────────────────────────────────────────

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
