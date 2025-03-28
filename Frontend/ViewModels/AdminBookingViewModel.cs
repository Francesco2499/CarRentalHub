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
    public partial class AdminBookingViewModel : SearchableViewModel<BookingModel>
    {
        [ObservableProperty] private BookingModel? _selectedBooking = null;
        [ObservableProperty] public List<(string VehicleModel, int VehicleId)> _allVehicles = [];

        [ObservableProperty] private List<string> _vehicleNameList = [];
        
               
        private string? _selectedVehicleModel;
        public int SelectedVehicleId { get; set; }

        public DateTime _newStartDate;

        public DateTime NewStartDate
        {
            get => _newStartDate;
            set
            {
                if (SelectedBooking != null) {
                    
                    SetProperty(ref _newStartDate, value);
                    EditingBooking = SelectedBooking with {StartDate = value, EndDate = NewEndDate};
                    AllVehicles = GetVehicles();
                    
                }

            }
        }

        public DateTime _newEndDate;

        public DateTime NewEndDate
        {
            get => _newEndDate;
            set
            {
                if (SelectedBooking != null) {
                    SetProperty(ref _newEndDate, value);
                    EditingBooking = SelectedBooking with {StartDate = NewStartDate, EndDate = value};
                    AllVehicles = GetVehicles();
                }
            }
        }

        // Proprietà che aggiorna l'ID quando viene selezionato un modello
        public string? SelectedVehicleModel
        {
            get => _selectedVehicleModel;
            set
            {
                if (SetProperty(ref _selectedVehicleModel, value))
                {
                    // Quando il valore cambia, aggiorna l'ID corrispondente
                    var selectedVehicle = AllVehicles?.FirstOrDefault(v => v.VehicleModel == value);
                    SelectedVehicleId = selectedVehicle?.VehicleId ?? 0;
                }
            }
        }
        [ObservableProperty] private bool _isConfirmationModalVisible = false;
        [ObservableProperty] private string? _successMessage;
        [ObservableProperty] private bool _isFormVisible = false;
        [ObservableProperty] private BookingModel _editingBooking = new(0, 0, "", 0, "", 0, DateTime.Now, DateTime.Now, DateTime.Now);

        public AdminBookingViewModel()
        {
            AllVehicles = GetVehicles();
            IsVisibleList = true;
            LoadItems();
        }

        protected List<(string VehicleModel, int VehicleId)> GetVehicles()
        {
                var allVehicles = new List<(string VehicleModel, int VehicleId)>();

                // Itera su tutti gli showroom
                foreach (var showroom in VehicleService.GetAvailableShowrooms(EditingBooking.StartDate, EditingBooking.EndDate))
                {
                    // Aggiungi ogni veicolo come una tupla alla lista
                    foreach (var vehicle in showroom.Vehicles) // 'Vehicles' contiene i veicoli dello showroom
                    {
                        allVehicles.Add((vehicle.Model, vehicle.Id));
                    }
                }

                VehicleNameList = [.. allVehicles.Select((v) => { return v.VehicleModel;})];    

                return allVehicles;
        }

        protected override List<BookingModel>? LoadAllItems()
        {
            return BookingService.GetAllBookings();
        }
       
        protected override List<BookingModel> ApplySearch(List<BookingModel> items, string query)
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
            return [.. items.Where(b => b.VehicleModel.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                b.Username.Contains(query, StringComparison.OrdinalIgnoreCase)
            )];
        }

        protected override List<BookingModel> ApplySearchByBookingId(List<BookingModel> items, string query)
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
            return [.. items.Where(b => b.Id.ToString().Contains(query))];
        }
        

        [RelayCommand]
        private void ShowEditBookingForm()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;


            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da modificare.";
                return;
            }

            NewStartDate = SelectedBooking.StartDate;
            NewEndDate = SelectedBooking.EndDate;

            AllVehicles.Add((SelectedBooking.VehicleModel, SelectedBooking.VehicleId));
            VehicleNameList = [.. AllVehicles.Select((v) => { return v.VehicleModel;})];    
            SelectedVehicleModel = SelectedBooking.VehicleModel;
        

            EditingBooking = SelectedBooking with { };
            IsVisibleList = false;
            IsFormVisible = true;
        }

        [RelayCommand]
        private void SaveBooking()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            if (SelectedBooking == null)
            {
                ErrorMessage = "Nessuna prenotazione selezionata.";
                return;
            }

            try
            {
                var bookingResponse = BookingService.EditBooking(EditingBooking, SelectedVehicleId);
                if (bookingResponse?.Booking != null) {
                    LoadItems();
                    SuccessMessage = bookingResponse?.Message;
                    IsFormVisible = false;
                    IsVisibleList = true;  
                } else {
                    ErrorMessage = bookingResponse?.Message ?? "Errore nella modifica della prenotazione!";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore: {ex.Message}";
            }
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

            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da eliminare.";
                return;
            }
            
            IsConfirmationModalVisible = true;
        }

        [RelayCommand]
        private void ConfirmDelete()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da eliminare.";
                return;
            }

            try
            {
                VehicleService.DeleteVehicle(SelectedBooking.Id);
                SuccessMessage = $"Prenotazione con ID '{SelectedBooking.Id}' eliminata correttamente!";

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
