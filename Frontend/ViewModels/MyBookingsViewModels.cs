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
    public partial class MyBookingsViewModel : ViewModelBase
    {
        private readonly BookingService _bookingService;

        [ObservableProperty]
        private int _currentPage = 1;

        private const int PageSize = 5;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _paginatedBookings = new();

        private int _totalBookingsCount = 0;

        [ObservableProperty]
        private bool _isPaginationVisible = false;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _allBookings = new();

        // Gestione dei pulsanti di paginazione
        [ObservableProperty]
        private bool _isPreviousPageEnabled = true;

        [ObservableProperty]
        private bool _isNextPageEnabled = true;

        // Opacità dei pulsanti di paginazione
        [ObservableProperty]
        private double _previousPageOpacity = 1.0;

        [ObservableProperty]
        private double _nextPageOpacity = 1.0;

        public MyBookingsViewModel()
        {
            _bookingService = new BookingService();
            LoadBookings();
        }

        private async Task LoadBookings()
        {
            var bookings = await _bookingService.GetAllBookings();
            AllBookings.Clear();

            if (bookings is { Count: > 0 })
            {
                foreach (var booking in bookings)
                {
                    AllBookings.Add(booking);
                }

                _totalBookingsCount = bookings.Count;
                UpdatePaginatedBookings();
            }
        }

        // Metodo per aggiornare la lista di prenotazioni paginata
        private void UpdatePaginatedBookings()
        {
            var skip = (CurrentPage - 1) * PageSize;
            var take = PageSize;

            var paginatedList = AllBookings.Skip(skip).Take(take).ToList();

            PaginatedBookings.Clear();
            foreach (var booking in paginatedList)
            {
                PaginatedBookings.Add(booking);
            }

            // Verifica se la paginazione è necessaria (se ci sono più prenotazioni rispetto alla page size)
            IsPaginationVisible = _totalBookingsCount > PageSize;

            // Abilita/disabilita i pulsanti di paginazione e aggiorna l'opacità
            IsPreviousPageEnabled = CurrentPage > 1;
            IsNextPageEnabled = CurrentPage * PageSize < _totalBookingsCount;

            PreviousPageOpacity = IsPreviousPageEnabled ? 1.0 : 0.5; // Riduci l'opacità se disabilitato
            NextPageOpacity = IsNextPageEnabled ? 1.0 : 0.5; // Riduci l'opacità se disabilitato
        }

        // Comando per andare alla pagina precedente
        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePaginatedBookings();
            }
        }

        // Comando per andare alla pagina successiva
        [RelayCommand]
        private void GoToNextPage()
        {
            if (CurrentPage * PageSize < _totalBookingsCount)
            {
                CurrentPage++;
                UpdatePaginatedBookings();
            }
        }
    }
}
