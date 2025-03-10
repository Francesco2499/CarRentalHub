using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;


namespace Frontend.ViewModels
{
    public partial class NewBookingViewModel : SearchableViewModel<VehicleModel>
    {

        private readonly VehicleService _vehicleService;
        private readonly BookingService _bookingService;
        [ObservableProperty]
        private BookingModel? _booking;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isBookingSummaryVisible = false;

        [ObservableProperty]
        private string _dateSearchMessage = string.Empty;

        [ObservableProperty]
        private bool _isVisibleList = false;

        [ObservableProperty]
        private bool _isVisibleSearchDate = true;

        [ObservableProperty]
        private VehicleModel? _selectedVehicle;

        // Proprietà per la data di inizio (simulata)
        [ObservableProperty]
        private DateTime? _startDate = DateTime.Today.Date;

        // Proprietà per la data di fine (simulata)
        [ObservableProperty]
        private DateTime? _endDate = DateTime.Today.AddDays(1).Date;

        public NewBookingViewModel()
        {
            _vehicleService = new VehicleService();
            _bookingService = new BookingService();
        }

        protected override async Task<List<VehicleModel>?> LoadAllItemsAsync()
        {
            return await _vehicleService.GetVehiclesByDate(StartDate, EndDate);
        }

        [RelayCommand]
        private async Task CercaVeicoliTest()
        {
            // Resetta il messaggio di errore all'inizio
            DateSearchMessage = string.Empty;

            // Verifica che entrambe le date siano selezionate
            if (StartDate == null || EndDate == null)
            {
                DateSearchMessage = "Seleziona entrambe le date (inizio e fine).";
                return;
            }

            // Verifica che la data di inizio non sia nel passato
            if (StartDate.Value.Date < DateTime.Today)
            {
                DateSearchMessage = "La data di inizio non può essere nel passato.";
                return;
            }

            // Verifica che la data di fine non sia prima della data di inizio
            if (EndDate.Value.Date < StartDate.Value.Date)
            {
                DateSearchMessage = "La data di fine non può essere prima della data di inizio.";
                return;
            }

            // Se tutte le verifiche sono superate, mostra la lista
            await LoadItems();
            
            if (Items != null && Items is { Count: > 0 })
            {
                IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                DateSearchMessage = "Non ci sono auto disponibili per le date selezionate!";
            }
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            return [.. items.Where(v =>
                    (v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) 
                )];
        }


        [RelayCommand]
        private async Task AddBooking()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo!";
                return;
            }  

            var booking = await _bookingService.AddBooking(SelectedVehicle.Id, StartDate, EndDate);
            
            if (booking != null)
            {
                Booking = booking;
                ErrorMessage = string.Empty;
                IsBookingSummaryVisible = true;
            } else
            {
                ErrorMessage = "Errore nella prenotazione. Riprova.";
                return;
            }           
        }
    }
}
