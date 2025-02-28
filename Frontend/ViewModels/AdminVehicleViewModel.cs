using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public partial class AdminVehicleViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;

        // Numero di elementi per pagina
        private const int PageSize = 5;

        private List<VehicleModel> allVehicles = new();

        [ObservableProperty]
        private ObservableCollection<VehicleModel> _vehicles = new();

        [ObservableProperty]
        private VehicleModel? _selectedVehicle;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private bool _isPreviousPageEnabled = false;

        [ObservableProperty]
        private bool _isNextPageEnabled = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public AdminVehicleViewModel()
        {
            _vehicleService = new VehicleService();
            LoadVehicles();
        }

        private async Task LoadVehicles()
        {
            allVehicles = await _vehicleService.GetAllVehicles();
            UpdatePaginatedVehicles();
        }

        private void UpdatePaginatedVehicles()
        {
            var skip = (CurrentPage - 1) * PageSize;
            var filteredVehicles = allVehicles
                .Where(c => string.IsNullOrWhiteSpace(SearchQuery) ||
                            c.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                            c.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Vehicles.Clear();
            foreach (var vehicle in filteredVehicles.Skip(skip).Take(PageSize))
            {
                Vehicles.Add(vehicle);
            }

            // Gestione paginazione
            IsPreviousPageEnabled = CurrentPage > 1;
            IsNextPageEnabled = CurrentPage * PageSize < filteredVehicles.Count;
        }

        [RelayCommand]
        private void SearchVehicles()
        {
            CurrentPage = 1;
            UpdatePaginatedVehicles();
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (IsPreviousPageEnabled)
            {
                CurrentPage--;
                UpdatePaginatedVehicles();
            }
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (IsNextPageEnabled)
            {
                CurrentPage++;
                UpdatePaginatedVehicles();
            }
        }

        [RelayCommand]
        private async Task AddVehicle()
        {
            //var newVehicle = new VehicleModel
           //
            //UpdatePaginatedCars();
        }

        [RelayCommand]
        private async Task EditVehicle()
        {
            // if (SelectedCar == null)
            // {
            //     ErrorMessage = "Seleziona un'auto da modificare.";
            //     return;
            // }

            // SelectedCar.Model = "Modificato";
            // UpdatePaginatedCars();
        }

        [RelayCommand]
        private async Task DeleteVehicle()
        {
            // if (SelectedCar == null)
            // {
            //     ErrorMessage = "Seleziona un'auto da eliminare.";
            //     return;
            // }

            // allCars.Remove(SelectedCar);
            // SelectedCar = null;
            // UpdatePaginatedCars();
        }
    }
}
