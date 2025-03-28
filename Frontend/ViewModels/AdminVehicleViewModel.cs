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
        [ObservableProperty] private Dictionary<int, string>  _showroomsList  = VehicleService.GetAvailableShowrooms(null, null)?.Select((showroom, index) => new { showroom.Name, showroom.Id }).ToDictionary(v => v.Id, v => v.Name) ?? [];
        public List<string> Showrooms => ShowroomsList?.Values.ToList() ?? [];
        private string? _selectedShowroom;
        public int SelectedShowroomId{ get; set; }

        // Proprietà che aggiorna l'ID quando viene selezionato un modello
        public string? SelectedShowroom
        {
            get => _selectedShowroom;
            set
            {
                if (SetProperty(ref _selectedShowroom, value))
                {
                    var selectedVehicle = ShowroomsList?.FirstOrDefault(v => v.Value == value);
                    SelectedShowroomId = selectedVehicle?.Key ?? 0;
                }
            }
        }
        [ObservableProperty] private VehicleModel? _selectedVehicle;
        [ObservableProperty] private string? _successMessage;
        [ObservableProperty] private bool _isConfirmationModalVisible = false;
        [ObservableProperty] private bool _isFormVisible = false;
        [ObservableProperty] private VehicleModel _editingVehicle = new(0, "", "", 0, 0,""); 
    
        public AdminVehicleViewModel()
        {
            IsVisibleList = true;
            LoadItems();
        }

        [RelayCommand]
        private void ShowAddVehicleForm()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
            SelectedShowroom = "";
            EditingVehicle = new VehicleModel(0, "", "", 0, 0, "");
            IsVisibleList = false;
            IsFormVisible = true;
        }

        [RelayCommand]
        private void ShowEditVehicleForm()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            SelectedShowroom = SelectedVehicle?.ShowroomName;


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
                VehicleResponse? vehicleResponse;
                string? msg;

                EditingVehicle = EditingVehicle with {CarShowroomID = SelectedShowroomId};

                if (EditingVehicle.Id == 0) // Aggiunta di un nuovo veicolo
                {
                    vehicleResponse = VehicleService.AddVehicle(EditingVehicle);
                    msg = "Errore nell'aggiunta del veicolo!";
                }
                else
                {
                    vehicleResponse = VehicleService.EditVehicle(EditingVehicle);
                    msg = "Errore nella modifica del veicolo!";
                }

                ErrorMessage = "";

                if (vehicleResponse?.Vehicle != null) {
                    LoadItems();
                    SuccessMessage = vehicleResponse?.Message;
                    IsFormVisible = false;  
                    IsVisibleList = true; 
                } else {
                    ErrorMessage = vehicleResponse?.Message ?? msg;
                }
               
            } catch (Exception ex)
            {
                ErrorMessage = $"Si è verificato un errore imprevisto: {ex.Message}";
            } 
        }

        protected override List<VehicleModel>? LoadAllItems()
        {
            return VehicleService.GetAllVehicles();
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
            return [.. items.Where(v =>
                    v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                )];
        }


        [RelayCommand]
        private void GoBack()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
            SearchQuery = string.Empty;
            IsVisibleList = true;
            IsFormVisible = false;
        }

        [RelayCommand]
        private void ShowConfirmationModal()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            if (SelectedVehicle== null)
            {
                ErrorMessage = "Seleziona un veicolo da eliminare.";
                return;
            }
            
            // Mostra la modale di conferma
            IsConfirmationModalVisible = true;
        }

        [RelayCommand]
        private void ConfirmDelete()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo da eliminare.";
                return;
            }

            try
            {
                VehicleService.DeleteVehicle(SelectedVehicle.Id);
                SuccessMessage = $"Veicolo '{SelectedVehicle.Model}' eliminato correttamente!";

                LoadItems();
                
                IsConfirmationModalVisible = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CancelDelete()
        {
            IsConfirmationModalVisible = false;
        }
    }
}
