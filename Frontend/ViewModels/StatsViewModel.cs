using System;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class StatsViewModel : ViewModelBase
    {
        private readonly HttpClient _httpClient = new();
        [ObservableProperty] private ICommand? _currentFilterCommand;
        [ObservableProperty] string _statsMessage = string.Empty;
        [ObservableProperty] int _userID = 0;
        [ObservableProperty] bool _isVisibleImage = false;
        [ObservableProperty] bool _isVisibleUserBox = false;
        [ObservableProperty] bool _isVisibleText = false;
        [ObservableProperty] bool _isVisibleFilterDate = false;
        [ObservableProperty] bool _isVisibleSliderDate = false;
        [ObservableProperty] private Bitmap? _statsImage; // Immagine dei grafici
        [ObservableProperty] private string? _statsText; // Testo con statistiche numeriche
        [ObservableProperty] private UserInfo? _userInfo;
        [ObservableProperty] private string _errorMessage = string.Empty;
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

         private int _selectedRangeIndex;
        public int SelectedRangeIndex
        {
            get => _selectedRangeIndex;
            set
            {
                if (SetProperty(ref _selectedRangeIndex, value))
                {
                    UpdateDateRange();
                }
            }
        }

        public StatsViewModel()
        {
            _httpClient = new HttpClient();
            UpdateDateRange();
        }

        private void UpdateDateRange()
        {
            CleanContent(false);
            switch (SelectedRangeIndex)
            {
                case 0:
                    StartDate = DateTime.Today.AddMonths(-1);
                    EndDate = DateTime.Today;
                    break;
                case 1:
                    StartDate = DateTime.Today.AddMonths(-3);
                    EndDate = DateTime.Today;
                    break;
                case 2:
                    StartDate = null;
                    EndDate = null;
                    break;
            }
            ExecuteCurrentFilterCommand();
        }

        [RelayCommand]
        public void LoadTopUsers()
        {
            CleanContent(false);
            StatsMessage = "Ecco la top 3 degli utenti più attivi!";   
            LoadStatsImage("http://localhost:5005/users/stats/top3");
        }

        [RelayCommand]
        public void LoadUsersByRegion()
        {
            CleanContent(false);
            StatsMessage = "Il grafico mostra da dove provengono gli utenti che utilizzano CarRentalHub, rappresentandone la distribuzione per regione.";   
            LoadStatsImage("http://localhost:5005/users/stats/regions");
        }

        [RelayCommand]
        public void LoadAvailableVehicles()
        {
            CleanContent(true);

            if (StartDate.HasValue && EndDate.HasValue)
            {
                StatsMessage = "Il grafico mostra il rapporto tra veicoli disponibili e prenotati. Puoi anche decidere il periodo di riferimento!";   
                
                string url = "http://localhost:5005/vehicles/stats/availability";

                var startDateFormatted = StartDate?.ToString("yyyy-MM-dd");
                var endDateFormatted = EndDate?.ToString("yyyy-MM-dd");
                
                url += $"?start_date={startDateFormatted}&end_date={endDateFormatted}";
                
                LoadStatsImage(url);
                IsVisibleFilterDate = true;
            } else
            {
                ErrorMessage = "Inserisci le date per la ricerca!";
                IsVisibleFilterDate = true;
            }
        }

        [RelayCommand]
        public void LoadRevenueTrend()
        {
            IsVisibleSliderDate = true;

            StatsMessage = "Il grafico mostra il trend per periodo rispetto ai guadagni complessivi";   
        

            string url = "http://localhost:5005/bookings/stats/revenue/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Aggiungi le date come query parameters nell'URL
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            LoadStatsImage(url);
          
        }

        [RelayCommand]
        public void LoadTopVehicles()
        {
            CleanContent(true);

            IsVisibleFilterDate = true;

            StatsMessage = "Il grafico mostra la top 5 delle auto più richieste dagli utenti. Scegli anche il periodo di riferimento";   
           

            string url = "http://localhost:5005/vehicles/stats/top5";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Aggiungi le date come query parameters nell'URL
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            LoadStatsImage(url);
        }


        [RelayCommand]
        public void LoadNumberBookings()
        {
            IsVisibleSliderDate = true;

            StatsMessage = "Il grafico mostra il quantitativo di prenotazioni nel periodo di riferimento";   
           

            string url = "http://localhost:5005/bookings/stats/numbookings/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Aggiungi le date come query parameters nell'URL
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            LoadStatsImage(url);
        }

        [RelayCommand]
        public void LoadRevenueByVehicle()
        {
            IsVisibleSliderDate = true;

            StatsMessage = "Il grafico mostra il guadagno relativo ad ogni auto nel periodo di riferimento";   
           

            string url = "http://localhost:5005/bookings/stats/revenueforvehicle/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Aggiungi le date come query parameters nell'URL
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            LoadStatsImage(url);
        }

        [RelayCommand]
        public void LoadUserInfo()
        {
            CleanContent(false);
            if (UserID != 0) {
                try
                {
                    var response = _httpClient.GetAsync($"http://localhost:5005/users/stats/info/{UserID}").Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        ErrorMessage = "Errore nel recupero dei dati utente.";
                        return;
                    }

                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var user = JsonSerializer.Deserialize<UserInfo>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    if (user != null)
                    {
                        UserInfo = user;
                        IsVisibleText = true;
                    }
                    else
                    {
                        ErrorMessage = "Utente non trovato!";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Errore: {ex.Message}";
                }  
            }    
        }

        private void LoadStatsImage(string url)
        {
            try
            {
                using var response = _httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var stream = response.Content.ReadAsStream();
                    StatsImage = new Bitmap(stream);
                    IsVisibleImage = true;
                }
                else
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    dynamic? errorResponse = JsonSerializer.Deserialize<dynamic>(jsonResponse);
                    StatsText = $"Errore: {errorResponse?.error}";
                }
            }
            catch (Exception ex)
            {
                StatsText = $"Errore: {ex.Message}";
            }
        }

        private void CleanContent(bool noDates) {
            ErrorMessage = string.Empty;
            IsVisibleText = false;
            IsVisibleImage = false;
            IsVisibleUserBox = false;
            IsVisibleFilterDate = false;
            StatsMessage = string.Empty;
            IsVisibleSliderDate = false;
            if (!noDates) 
            {
                EndDate = null;
                StartDate = null;
            }
            
        }

        // Funzione per cambiare dinamicamente il comando del pulsante "Filtra per date"
        public bool UpdateFilterCommand(string filterType)
        {
            bool result = false;
            switch (filterType)
            {
                case "Available":
                    CurrentFilterCommand = LoadAvailableVehiclesCommand;
                    result = true;
                    break;
                case "Top5":
                    CurrentFilterCommand = LoadTopVehiclesCommand;
                    result = false;
                    break;
                case "AllRevenue":
                    CurrentFilterCommand = LoadRevenueTrendCommand;
                    SelectedRangeIndex = 2;
                    result = false;
                    break;
                case "NumberBookings":
                    CurrentFilterCommand = LoadNumberBookingsCommand;
                    SelectedRangeIndex = 2;
                    result = false;
                    break;
                case "RevenueVehicle":
                    CurrentFilterCommand = LoadRevenueByVehicleCommand;
                    SelectedRangeIndex = 2;
                    result = false;
                    break;
                
            }

            return result;
        }

        // Funzione per gestire il cambiamento di selezione nel MenuFlyout e eseguire il comando immediatamente
        [RelayCommand]
        public void OnMenuItemSelected(string filterType)
        {
            CleanContent(false);
            if (!UpdateFilterCommand(filterType))
            {
                ExecuteCurrentFilterCommand();
            } else {
                ShowDatesPanel();
            }
        }

        [RelayCommand]
        public void ShowMenuTextBox()
        {
            CleanContent(false);
            IsVisibleUserBox = true;
        }

        // Funzione per eseguire il comando attualmente associato al pulsante "Filtra per date"
        public void ExecuteCurrentFilterCommand()
        {
            CurrentFilterCommand?.Execute(null);  // Esegui il comando corrente
        }

        [RelayCommand]
        private void ShowDatesPanel() 
        {
            IsVisibleFilterDate = true;
        }
    }
    public record UserInfo(int UserId, string Username, int TotalBookings, double TotalSpent);

}
