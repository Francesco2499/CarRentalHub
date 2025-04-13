using System;
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
        private async Task SearchShowrooms()
        {
            FirstErrorMessage = string.Empty;

            if (ShowroomSearch.StartDate == null || ShowroomSearch.EndDate == null)
            {
                FirstErrorMessage = "Seleziona entrambe le date (inizio e fine).";
                return;
            }

            if (ShowroomSearch.StartDate.Value.Date < DateTime.Today)
            {
                FirstErrorMessage = "La data di inizio non può essere nel passato.";
                return;
            }

            if (ShowroomSearch.EndDate.Value.Date < ShowroomSearch.StartDate.Value.Date)
            {
                FirstErrorMessage = "La data di fine non può essere prima della data di inizio.";
                return;
            }

            await ShowroomSearch.LoadItems();

            if (ShowroomSearch.Items != null && ShowroomSearch.Items.Count > 0)
            {
                ShowroomSearch.IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                BookingMessage = "Nelle date richieste non ci sono auto disponibili per l'autosalone selezionato";
            }
        }

        [RelayCommand]
        private async Task SearchVehicles()
        {
            if (SelectedShowroom == null)
            {
                ErrorMessage = "Seleziona un autosalone!";
                return;
            }

            ShowroomSearch.IsVisibleList = false;

            VehicleSearch.SetShowroom(SelectedShowroom);

            await VehicleSearch.LoadItems();

            if (VehicleSearch.Items != null && VehicleSearch.Items.Count > 0)
            {
                VehicleSearch.IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                BookingMessage = "Nelle date richieste non ci sono veicoli disponibili per l'autosalone selezionato";
            }
        }

        [RelayCommand]
        private async Task AddBooking()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Seleziona un veicolo!";
                return;
            }  

            var bookingResponse = await BookingService.AddBooking(SelectedVehicle.Id, ShowroomSearch.StartDate, ShowroomSearch.EndDate);
            
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
