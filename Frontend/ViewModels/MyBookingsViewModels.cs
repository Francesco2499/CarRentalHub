using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public partial class MyBookingsViewModel : SearchableViewModel<BookingModel>
    {
        [ObservableProperty] private string? _bookingMessage;
        [ObservableProperty] private bool _isVisibleList = true;
        private DateTime? _startDate;
        [ObservableProperty] private DateTime? _endDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                
                if (value.HasValue)
                {
                    EndDate = value.Value.AddDays(1);
                }
            }
        }

        public MyBookingsViewModel()
        {
            _ = LoadItems();
        }

        protected override async Task<List<BookingModel>?> LoadAllItems()
        {
            try
            {
                var bookings = await BookingService.GetAllBookings();  
                if (bookings == null || bookings.Count == 0)
                {
                    BookingMessage = "Non hai ancora effettuato nessuna prenotazione";
                    IsVisibleList = false;
                    return [];
                }
                IsVisibleList = true;
                return bookings;
            }
            catch (Exception ex)
            {
                BookingMessage = $"Si è verificato un errore: {ex.Message}";
                IsVisibleList = false;
                return [];
            }
        }

        protected override List<BookingModel> ApplySearch(List<BookingModel> items, string query)
        {
            return items.Where(b => b.VehicleModel.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        [RelayCommand]
        private void SearchByDate()
        {
            if (StartDate == null || EndDate == null)
                return;

            if (_allItems != null)
            {
                EnableShowAll = true;

                var bookings = _allItems.Where(b => b.StartDate >= StartDate.Value && b.EndDate <= EndDate.Value).ToList();

                UpdatePaginatedItems(bookings, false);

                if (bookings.Count == 0)
                {
                    ErrorMessage = "Nessun risultato trovato. Cambia i parametri di ricerca.";
                }
            }
        }
    }
}
