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
    public partial class NewBookingViewModel : ViewModelBase
    {
        public SearchShowroomViewModel ShowroomSearch { get; } = new();
        public SearchVehicleViewModel VehicleSearch { get; } = new();

        [ObservableProperty] private VehicleModel? _selectedVehicle;
        [ObservableProperty] private ShowroomModel? _selectedShowroom;    
        [ObservableProperty] private string _errorMessage = string.Empty;  

        [ObservableProperty] private BookingModel? _booking;

        [ObservableProperty] private bool _isBookingSummaryVisible = false;

        [ObservableProperty] private bool _isVisibleSubTitle = true;

        [ObservableProperty] private string _firstErrorMessage = string.Empty;

        [ObservableProperty] private string _bookingMessage = string.Empty;

        [ObservableProperty] private bool _isVisibleSearchDate = true;

        [ObservableProperty] private string _location = string.Empty;

        [RelayCommand]
        private void SearchShowrooms()
        {
            // Resetta il messaggio di errore all'inizio
            FirstErrorMessage = string.Empty;

            // Verifica che entrambe le date siano selezionate
            if (ShowroomSearch.StartDate == null || ShowroomSearch.EndDate == null)
            {
                FirstErrorMessage = "Seleziona entrambe le date (inizio e fine).";
                return;
            }

            // Verifica che la data di inizio non sia nel passato
            if (ShowroomSearch.StartDate.Value.Date < DateTime.Today)
            {
                FirstErrorMessage = "La data di inizio non può essere nel passato.";
                return;
            }

            // Verifica che la data di fine non sia prima della data di inizio
            if (ShowroomSearch.EndDate.Value.Date < ShowroomSearch.StartDate.Value.Date)
            {
                FirstErrorMessage = "La data di fine non può essere prima della data di inizio.";
                return;
            }

            // Se tutte le verifiche sono superate, mostra la lista
            ShowroomSearch.LoadItems();

            if (ShowroomSearch.Items != null && ShowroomSearch.Items is { Count: > 0 })
            {
                ShowroomSearch.IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                BookingMessage = "Nelle date richieste on ci sono auto disponibili per l'autosalone selezionato";
            }
        }

        [RelayCommand]
        private void SearchVehicles()
        {
            if (SelectedShowroom == null)
            {
                ErrorMessage = "Seleziona entrambe le date (inizio e fine).";
                return;
            }

            ShowroomSearch.IsVisibleList = false;

            // Se tutte le verifiche sono superate, mostra la lista
            VehicleSearch.SetShowroom(SelectedShowroom);

            if (VehicleSearch.Items != null && VehicleSearch.Items is { Count: > 0 })
            {
                VehicleSearch.IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                BookingMessage = "Nelle date richieste on ci sono auto disponibili per l'autosalone selezionato";
            }
        }

    
        [RelayCommand]
        private void AddBooking()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo!";
                return;
            }  

            var bookingResponse = BookingService.AddBooking(SelectedVehicle.Id, ShowroomSearch.StartDate, ShowroomSearch.EndDate);
            
            if (bookingResponse?.Booking != null)
            {
                Booking = bookingResponse?.Booking;
                ErrorMessage = string.Empty;
                IsBookingSummaryVisible = true;
                IsVisibleSubTitle = false;
                ShowroomSearch.IsVisibleList = false;
                VehicleSearch.IsVisibleList = false;
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
            ShowroomSearch.IsVisibleList = false;
            VehicleSearch.IsVisibleList = false;            
            IsVisibleSearchDate = true;   
            IsVisibleSubTitle = true;
            IsBookingSummaryVisible = false;
            ShowroomSearch.ResetPagination();
            VehicleSearch.ResetPagination();
        }
    }
}
