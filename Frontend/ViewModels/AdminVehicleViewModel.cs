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
    public partial class AdminVehicleViewModel : SearchableViewModel<VehicleModel>
    {
        private readonly VehicleService _vehicleService;

        [ObservableProperty]
        private VehicleModel? _selectedVehicle;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isFormVisible = false;

        [ObservableProperty]
        private bool _isGridVisible = true;

        [ObservableProperty]
        private VehicleModel _editingVehicle = new(0, "", "", 0, "", 0, 0); 

        [RelayCommand]
        private void ShowAddVehicleForm()
        {
            EditingVehicle = new VehicleModel(0, "", "", 0, "", 0, 0);
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
        private async Task SaveVehicle()
        {
            if (string.IsNullOrWhiteSpace(EditingVehicle.Model) || string.IsNullOrWhiteSpace(EditingVehicle.Category))
            {
                ErrorMessage = "Tutti i campi sono obbligatori!";
                return;
            }

            try {
                if (EditingVehicle.Id == 0) // Aggiunta di un nuovo veicolo
                {
                    await _vehicleService.AddVehicle(EditingVehicle);
                }
                else
                {
                    await _vehicleService.EditVehicle(EditingVehicle);
                }
                ErrorMessage = "";
                await LoadItems();
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
            LoadItems();
        }

        protected override async Task<List<VehicleModel>?> LoadAllItemsAsync()
        {
            return await _vehicleService.GetAllVehicles();
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            return [.. items.Where(v =>
                    (v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) 
                )];
        }

        [RelayCommand]
        private async Task DeleteVehicle()
        {
            ErrorMessage = "";
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un'auto da eliminare.";
                return;
            }

            await _vehicleService.DeleteVehicle(SelectedVehicle.Id);
            await LoadItems();
        }
    }
}
