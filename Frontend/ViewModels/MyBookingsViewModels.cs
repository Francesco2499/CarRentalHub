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

        [ObservableProperty] private string? _bookingMessage;

        [ObservableProperty] private bool _isVisibleList = true;

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

        public MyBookingsViewModel()
        {
            LoadItems();
        }

        protected override List<BookingModel>? LoadAllItems()
        {
            var bookings = BookingService.GetAllBookings();
            if (bookings == null || bookings.Count == 0)
            {
                BookingMessage = "Non hai ancora effettuata nessuna prenotazione";
                IsVisibleList = false;
                return [];
            }
            IsVisibleList = true;
            return bookings;
        }

        protected override List<BookingModel> ApplySearch(List<BookingModel> items, string query)
        {
            return [.. items.Where(b => b.VehicleModel.Contains(query, StringComparison.OrdinalIgnoreCase))];
        }

        [RelayCommand]
        private void SearchByDate()
        {
            if (StartDate == null || EndDate == null)
                return;

            if(_allItems != null) {
               EnableShowAll = true;
                var bookings = new List<BookingModel>(_allItems.Where(b => b.StartDate >= StartDate.Value && b.EndDate <= EndDate.Value)    );
                UpdatePaginatedItems(bookings, false);
                if (bookings.Count == 0) {
                    ErrorMessage = "Nessun risultato trovato. Cambia i parametri di ricerca.";
                } 
            }
        }
    }
}
