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
    public partial class AdminVehicleViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPaginationVisible = false;

        [ObservableProperty]
        private double _previousPageOpacity = 1.0;

        [ObservableProperty]
        private double _nextPageOpacity = 1.0;

        private int _totalVehiclesCount = 0;

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

        [ObservableProperty]
        private bool _isFormVisible = false;

        [ObservableProperty]
        private bool _isGridVisible = true;

        [ObservableProperty]
        private VehicleModel _editingVehicle = new(0, "", "", 0, true, "", 0, 0); 

        [RelayCommand]
        private void ShowAddVehicleForm()
        {
            EditingVehicle = new VehicleModel(0, "", "", 0, true, "", 0, 0);
            IsGridVisible = false;
            IsFormVisible = true;
        }

        [RelayCommand]
        private void ShowEditVehicleForm()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo da modificare.";
                return;
            }

            EditingVehicle = SelectedVehicle with { };
            IsGridVisible = false;
            IsFormVisible = true;
        }

        [RelayCommand]
        private void SaveVehicle()
        {
            if (string.IsNullOrWhiteSpace(EditingVehicle.Model) || string.IsNullOrWhiteSpace(EditingVehicle.Category))
            {
                ErrorMessage = "Tutti i campi sono obbligatori!";
                return;
            }

            try {
                if (EditingVehicle.Id == 0) // Aggiunta di un nuovo veicolo
                {
                    _vehicleService.AddVehicle(EditingVehicle);
                }
                else // Modifica esistente
                {
                    // var existingVehicle = allVehicles.FirstOrDefault(v => v.Id == EditingVehicle.Id);
                    // if (existingVehicle != null)
                    // {
                    //     existingVehicle.Model = EditingVehicle.Model;
                    //     existingVehicle.Category = EditingVehicle.Category;
                    //     existingVehicle.Price = EditingVehicle.Price;
                    // }
                }

                LoadVehicles();
                IsFormVisible = false;  
                IsGridVisible = true;
            } catch (Exception ex)
            {
                ErrorMessage = $"Si è verificato un errore imprevisto: {ex.Message}";
            } 
        }

        public AdminVehicleViewModel()
        {
            _vehicleService = new VehicleService();
            LoadVehicles();
        }

        private async Task LoadVehicles()
        {
            allVehicles = await _vehicleService.GetAllVehicles();
            _totalVehiclesCount = allVehicles.Count;

            UpdatePaginatedVehicles();
        }

        private void UpdatePaginatedVehicles()
        {
            var skip = (CurrentPage - 1) * PageSize;
            var filteredVehicles = allVehicles;
                // .Where(c => string.IsNullOrWhiteSpace(SearchQuery) ||
                //             c.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                //             c.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                // .ToList();

            Vehicles.Clear();
            foreach (var vehicle in filteredVehicles.Skip(skip).Take(PageSize))
            {
                Vehicles.Add(vehicle);
            }

            // Gestione paginazione
            IsPreviousPageEnabled = CurrentPage > 1;
            IsNextPageEnabled = CurrentPage * PageSize < filteredVehicles.Count;

            IsPaginationVisible = _totalVehiclesCount > PageSize;
            PreviousPageOpacity = IsPreviousPageEnabled ? 1.0 : 0.5; // Riduci l'opacità se disabilitato
            NextPageOpacity = IsNextPageEnabled ? 1.0 : 0.5; 
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
