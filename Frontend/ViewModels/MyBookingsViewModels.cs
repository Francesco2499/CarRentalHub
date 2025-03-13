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
    public partial class MyBookingsViewModel : SearchableViewModel<BookingModel>
    {
        private readonly BookingService _bookingService;

        public MyBookingsViewModel()
        {
            _bookingService = new BookingService();
            LoadItems();
        }

        protected override async Task<List<BookingModel>?> LoadAllItemsAsync()
        {
            return await _bookingService.GetAllBookings();
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
