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
        [ObservableProperty] private VehicleModel? _selectedVehicle;

        [ObservableProperty] private bool _isFormVisible = false;

        [ObservableProperty] private VehicleModel _editingVehicle = new(0, "", "", 0, ""); 
        
        public AdminVehicleViewModel()
        {
            IsVisibleList = true;
            LoadItems();
        }

        [RelayCommand]
        private void ShowAddVehicleForm()
        {
            EditingVehicle = new VehicleModel(0, "", "", 0, "");
            IsVisibleList = false;
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
            IsVisibleList = false;
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
                    VehicleService.AddVehicle(EditingVehicle);
                }
                else
                {
                    VehicleService.EditVehicle(EditingVehicle);
                }
                ErrorMessage = "";
                LoadItems();
                IsFormVisible = false;  
                IsVisibleList = true;
            } catch (Exception ex)
            {
                ErrorMessage = $"Si è verificato un errore imprevisto: {ex.Message}";
            } 
        }

        protected override List<VehicleModel>? LoadAllItemsAsync()
        {
            return VehicleService.GetAllVehicles();
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            return [.. items.Where(v =>
                    v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                )];
        }

        [RelayCommand]
        private void DeleteVehicle()
        {
            ErrorMessage = "";
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un'auto da eliminare.";
                return;
            }

            VehicleService.DeleteVehicle(SelectedVehicle.Id);
            LoadItems();
        }

        [RelayCommand]
        private void GoBack()
        {
            SearchQuery = string.Empty;
            IsVisibleList = true;
            IsFormVisible = false;
        }
    }
}
