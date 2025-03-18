using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;
using Avalonia.Automation.Peers;

namespace Frontend.ViewModels
{
    public partial class MyBookingsViewModel : SearchableViewModel<BookingModel>
    {
        private readonly BookingService _bookingService;

        [ObservableProperty] private string? _bookingMessage;

        [ObservableProperty] private bool _isVisibleList = true;

        public MyBookingsViewModel()
        {
            _bookingService = new BookingService();
            LoadItems();
        }

        protected override List<BookingModel>? LoadAllItemsAsync()
        {
            var bookings = BookingService.GetAllBookings();
            if (bookings == null || bookings.Count == 0)
            {
                BookingMessage = "Non hai ancora effettuata nessuna prenotazione";
                IsVisibleList = false;
                return []; // Restituisce una lista vuota invece di null
            }
            IsVisibleList = true;
            return bookings;
        }

        protected override List<BookingModel> ApplySearch(List<BookingModel> items, string query)
        {
            return [.. items.Where(b => b.vehicle_model.Contains(query, StringComparison.OrdinalIgnoreCase))];
        }

        protected override List<BookingModel> ApplySearchByBookingId(List<BookingModel> items, int query)
        {
            return [.. items.Where(b => b.Id == query)];
        }
    }
}
