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
        [ObservableProperty] private bool _isFormVisible = false;
        [ObservableProperty] private bool _isGridVisible = true;
        [ObservableProperty] private BookingModel _editingBooking = new(0, 0, 0, "", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
        private readonly BookingService _bookingService;
        public AdminBookingViewModel()
        {
            _bookingService = new BookingService();
            LoadItems();
        }

        protected override List<BookingModel>? LoadAllItemsAsync()
        {
            return BookingService.GetAllBookings();
        }
       
        protected override List<BookingModel> ApplySearch(List<BookingModel> items, string query)
        {
            return [.. items.Where(b => b.vehicle_model.Contains(query, StringComparison.OrdinalIgnoreCase))];
        }

        protected override List<BookingModel> ApplySearchByUserId(List<BookingModel> items, int query)
        {
            return [.. items.Where(b => b.user_id == query)];
        }

        protected override List<BookingModel> ApplySearchByBookingId(List<BookingModel> items, int query)
        {
            return [.. items.Where(b => b.Id == query)];
        }
        

        [RelayCommand]
        private void ShowEditBookingForm()
        {
            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da modificare.";
                return;
            }

            EditingBooking = SelectedBooking with { };
            IsGridVisible = false;
            IsFormVisible = true;
        }

        [RelayCommand]
        private void SaveBooking()
        {
            if (SelectedBooking == null)
            {
                ErrorMessage = "Nessuna prenotazione selezionata.";
                return;
            }

            try
            {
                var bookingResponse = BookingService.EditBooking(EditingBooking);
                if (bookingResponse?.Booking != null) {
                    LoadItems();
                    IsFormVisible = false;
                    IsGridVisible = true;  
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
        private void DeleteBooking()
        {
            if (SelectedBooking == null)
            {
                ErrorMessage = "Seleziona una prenotazione da eliminare.";
                return;
            }

            BookingService.DeleteBooking(SelectedBooking.Id);
            LoadItems();
        }

    }
}
