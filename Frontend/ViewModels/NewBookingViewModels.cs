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
        [ObservableProperty] private BookingModel? _booking;

        [ObservableProperty] private bool _isBookingSummaryVisible = false;

        [ObservableProperty] private bool _isVisibleSubTitle = true;

        [ObservableProperty] private string _firstErrorMessage = string.Empty;

        [ObservableProperty] private string _bookingMessage = string.Empty;

        [ObservableProperty] private bool _isVisibleSearchDate = true;

        [ObservableProperty] private string _location = string.Empty;

        [ObservableProperty] private VehicleModel? _selectedVehicle;

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                
                // Imposta EndDate al giorno successivo, solo se StartDate è selezionata
                if (value.HasValue)
                {
                    EndDate = value.Value.AddDays(1);
                }
            }
        }

        [ObservableProperty]
        private DateTime? _endDate;
        protected override List<VehicleModel>? LoadAllItemsAsync()
        {
            return VehicleService.GetVehiclesByDate(StartDate, EndDate, Location);
        }

        [RelayCommand]
        private void SearchVehicles()
        {
            // Resetta il messaggio di errore all'inizio
            FirstErrorMessage = string.Empty;

            // Verifica che entrambe le date siano selezionate
            if (StartDate == null || EndDate == null)
            {
                FirstErrorMessage = "Seleziona entrambe le date (inizio e fine).";
                return;
            }

            // Verifica che la data di inizio non sia nel passato
            if (StartDate.Value.Date < DateTime.Today)
            {
                FirstErrorMessage = "La data di inizio non può essere nel passato.";
                return;
            }

            // Verifica che la data di fine non sia prima della data di inizio
            if (EndDate.Value.Date < StartDate.Value.Date)
            {
                FirstErrorMessage = "La data di fine non può essere prima della data di inizio.";
                return;
            }

            if (Location == string.Empty)
            {
                FirstErrorMessage = "Inserisci un comune per la tua ricerca!";
                return;
            }

            // Se tutte le verifiche sono superate, mostra la lista
            LoadItems();
            
            if (Items != null && Items is { Count: > 0 })
            {
                IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                BookingMessage = "Non ci sono auto disponibili per i parametri selezionati!";
            }
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            return [.. items.Where(v =>
                    v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) 
                )];
        }


        [RelayCommand]
        private void AddBooking()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo!";
                return;
            }  

            var bookingResponse = BookingService.AddBooking(SelectedVehicle.Id, StartDate, EndDate);
            
            if (bookingResponse?.Booking != null)
            {
                Booking = bookingResponse?.Booking;
                ErrorMessage = string.Empty;
                IsBookingSummaryVisible = true;
                IsVisibleSubTitle = false;
                IsVisibleList = false;
            } else
            {
                ErrorMessage = bookingResponse?.Message ?? "Errore nella prenotazione!";
                return;
            }           
        }

        [RelayCommand]
        private void GoBack()
        {
            BookingMessage = string.Empty;
            IsVisibleList = false;
            IsVisibleSearchDate = true;   
            IsVisibleSubTitle = true;
            IsBookingSummaryVisible = false;

        }
    }
}
