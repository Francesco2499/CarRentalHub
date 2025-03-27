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
        [ObservableProperty] private BookingModel? _selectedBooking;
        [ObservableProperty] private Dictionary<int, string>  _vehicles = VehicleService.GetAllVehicles()?.Select((vehicle, index) => new { vehicle.Model, vehicle.Id }).ToDictionary(v => v.Id, v => v.Model) ?? [];
        public List<string> VehicleModels => Vehicles?.Values.ToList() ?? [];
        private string? _selectedVehicleModel;
        public int SelectedVehicleId { get; set; }

        // Proprietà che aggiorna l'ID quando viene selezionato un modello
        public string? SelectedVehicleModel
        {
            get => _selectedVehicleModel;
            set
            {
                if (SetProperty(ref _selectedVehicleModel, value))
                {
                    // Quando il valore cambia, aggiorna l'ID corrispondente
                    var selectedVehicle = Vehicles?.FirstOrDefault(v => v.Value == value);
                    SelectedVehicleId = selectedVehicle?.Key ?? 0;
                }
            }
        }
        [ObservableProperty] private bool _isConfirmationModalVisible = false;
        [ObservableProperty] private string? _successMessage;
        [ObservableProperty] private bool _isFormVisible = false;
        [ObservableProperty] private BookingModel _editingBooking = new(0, 0, "", 0, "", 0, DateTime.Now, DateTime.Now, DateTime.Now);

        public AdminBookingViewModel()
        {
            IsVisibleList = true;
            LoadItems();
        }

        protected override List<BookingModel>? LoadAllItemsAsync()
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
            Console.WriteLine(query);
            return [.. items.Where(b => b.Id.ToString().Contains(query))];
        }
        

        [RelayCommand]
        private void ShowEditBookingForm()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            SelectedVehicleModel = SelectedBooking?.VehicleModel;

            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da modificare.";
                return;
            }
            

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
                Console.WriteLine(SelectedVehicleId);
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
