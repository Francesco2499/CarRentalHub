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
    public partial class VeicoliViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPreviousPageEnabled = true;

        [ObservableProperty]
        private bool _isNextPageEnabled = true;

        [ObservableProperty]
        private double _previousPageOpacity = 1.0;

        [ObservableProperty]
        private double _nextPageOpacity = 1.0;

        private readonly VehicleService _vehicleService;
        private readonly BookingService _bookingService;

        // Proprietà per la pagina corrente (inizialmente 1)
        [ObservableProperty]
        private int _currentPage = 1;

        // Proprietà per la dimensione della pagina (6 veicoli per pagina)
        private const int PageSize = 2;

        // Proprietà per la lista di veicoli visibili (che verrà aggiornata a seconda della pagina)
        private List<VehicleModel> vehicles = new();

        // Proprietà per il numero totale di veicoli disponibili
        private int _totalVehiclesCount = 0;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBookingSummaryVisible = false;
        
        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private string _carId = string.Empty;

         [ObservableProperty]
        private string _dateSearchMessage = string.Empty;

        [ObservableProperty]
        private bool _isVisibleList = false;

        [ObservableProperty]
        private bool _isVisibleSearchDate = true;

// Proprietà per verificare se la paginazione è necessaria (cioè se ci sono più di 6 veicoli)
        [ObservableProperty]
        private bool _isPaginationVisible = false;

        [ObservableProperty]
        private ObservableCollection<VehicleModel> _veicoliDisponibili = new();

        [ObservableProperty]
        private BookingModel _booking;

        // Proprietà per la data di inizio (simulata)
        [ObservableProperty]
        private DateTime? _startDate = DateTime.Today.Date;

        // Proprietà per la data di fine (simulata)
        [ObservableProperty]
        private DateTime? _endDate = DateTime.Today.AddDays(1).Date;

        public VeicoliViewModel()
        {
            _vehicleService = new VehicleService();
            _bookingService = new BookingService();
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
            vehicles = await _vehicleService.GetVehiclesByDate(StartDate, EndDate);

            if (vehicles is { Count: > 0 })
            {
                // Imposta il numero totale di veicoli disponibili
                _totalVehiclesCount = vehicles.Count;
                updatePaginatedVehicles(null);
                IsVisibleList = true;
                IsVisibleSearchDate = false;    
            } else {
                DateSearchMessage = "Non ci sono auto disponibili per le date selezionate!";
            }
        }

            

        private void updatePaginatedVehicles(List<VehicleModel?> listVehicle)
        {
            var skip = (CurrentPage - 1) * PageSize;
            var take = PageSize;
            var results = vehicles;

            if (listVehicle != null && listVehicle.Any())
            {
                results = listVehicle;
            }

            // Ottieni la lista dei veicoli per la pagina corrente
            var paginatedList = results.Skip(skip).Take(take).ToList();

            // Imposta la lista dei veicoli
            VeicoliDisponibili.Clear();
            foreach (var vehicle in paginatedList)
            {
                VeicoliDisponibili.Add(vehicle);
            }

            // Verifica se la paginazione è necessaria
            IsPaginationVisible = _totalVehiclesCount > PageSize;

            // Abilita/disabilita i pulsanti di paginazione e modifica l'opacità
            IsPreviousPageEnabled = CurrentPage > 1;
            IsNextPageEnabled = CurrentPage * PageSize < _totalVehiclesCount;

            PreviousPageOpacity = IsPreviousPageEnabled ? 1.0 : 0.5; // Riduci l'opacità se disabilitato
            NextPageOpacity = IsNextPageEnabled ? 1.0 : 0.5; // Riduci l'opacità se disabilitato
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                updatePaginatedVehicles(null);
            }
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (CurrentPage * PageSize < _totalVehiclesCount)
            {
                CurrentPage++;
                updatePaginatedVehicles(null);
            }
        }

        // Comando per cercare i veicoli in base alla ricerca e intervallo di date
        [RelayCommand]
        private void CercaVeicoli()
    {       
            var risultati = vehicles.ToList();
            // Se la query di ricerca è vuota, non applicare alcun filtro, quindi mostra tutte le auto
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Filtro per nome, categoria e intervallo di date
                risultati = vehicles.Where(v =>
                    (v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) 
                ).ToList();
            }

            // Aggiorna la lista
            VeicoliDisponibili.Clear();
            updatePaginatedVehicles(risultati);
        }


        [RelayCommand]
         private async Task AddBooking(string carId)
        {
            if (string.IsNullOrEmpty(carId))
            {
                Console.WriteLine("ID veicolo non valido.");
                return;
            }

            // Trova il veicolo con l'ID fornito (in un'app reale, questo sarebbe un servizio API, database, ecc.)
            var veicolo = VeicoliDisponibili.FirstOrDefault(v => v.Id == int.Parse(carId));
            
            if (veicolo != null)
            {
                               

                var booking = await _bookingService.AddBooking(int.Parse(carId), StartDate, EndDate);
                
                if (booking != null)
                {
                    Booking = booking;
                    ErrorMessage = string.Empty;
                    IsBookingSummaryVisible = true;
                } else
                {
                    ErrorMessage = "Errore nella prenotazione. Riprova.";
                }           
            }
            else
            {
                ErrorMessage = $"Nessun veicolo trovato con l'ID: {carId}";
            }
        }
    }
}
