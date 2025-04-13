using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        [ObservableProperty] string _userID = string.Empty;
        [ObservableProperty] bool _isVisibleImage = false;
        [ObservableProperty] bool _isVisibleUserBox = false;
        [ObservableProperty] bool _isVisibleText = false;
        [ObservableProperty] bool _isVisibleFilterDate = false;
        [ObservableProperty] bool _isVisibleSliderDate = false;
        [ObservableProperty] private Bitmap? _statsImage;
        [ObservableProperty] private string? _statsText;
        [ObservableProperty] private UserInfo? _userInfo;
        [ObservableProperty] private string _errorMessage = string.Empty;
        private DateTime? _startDate;
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
        [ObservableProperty] private DateTime? _endDate;
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
        public async Task LoadTopUsers()
        {
            CleanContent(false);
            await LoadStatsImageAsync("http://localhost:5005/users/stats/top3", "Ecco la top 3 degli utenti più attivi!", false, false);
        }

        [RelayCommand]
        public async Task LoadUsersByRegion()
        {
            CleanContent(false);
            await LoadStatsImageAsync("http://localhost:5005/users/stats/regions", "Il grafico mostra da dove provengono gli utenti che utilizzano CarRentalHub, rappresentandone la distribuzione per regione.", false, false);
        }

        [RelayCommand]
        public async Task LoadAvailableVehicles()
        {
            CleanContent(true);

            if (StartDate.HasValue && EndDate.HasValue)
            {
                string url = "http://localhost:5005/vehicles/stats/availability";

                var startDateFormatted = StartDate?.ToString("yyyy-MM-dd");
                var endDateFormatted = EndDate?.ToString("yyyy-MM-dd");

                url += $"?start_date={startDateFormatted}&end_date={endDateFormatted}";

                await LoadStatsImageAsync(url, "Il grafico mostra il rapporto tra veicoli disponibili e prenotati. Puoi anche decidere il periodo di riferimento!", false, false);
            }
            else
            {
                ErrorMessage = "Inserisci le date per la ricerca!";
            }

            IsVisibleFilterDate = true;
        }

        [RelayCommand]
        public async Task LoadRevenueTrend()
        {
            string url = "http://localhost:5005/bookings/stats/revenue/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            await LoadStatsImageAsync(url, "Il grafico mostra il trend per periodo rispetto ai guadagni complessivi", false, true);
        }

        [RelayCommand]
        public async Task LoadTopVehicles()
        {
            CleanContent(true);

            string url = "http://localhost:5005/vehicles/stats/top5";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            await LoadStatsImageAsync(url, "Il grafico mostra la top 5 delle auto più richieste dagli utenti. Scegli anche il periodo di riferimento", true, false);
        }

        [RelayCommand]
        public async Task LoadNumberBookings()
        {
            string url = "http://localhost:5005/bookings/stats/numbookings/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            await LoadStatsImageAsync(url, "Il grafico mostra il quantitativo di prenotazioni nel periodo di riferimento", false, true);
        }

        [RelayCommand]
        public async Task LoadRevenueByVehicle()
        {
            string url = "http://localhost:5005/bookings/stats/revenueforvehicle/trend";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                url += $"?start_date={StartDate.Value:yyyy-MM-dd}&end_date={EndDate.Value:yyyy-MM-dd}";
            }

            await LoadStatsImageAsync(url, "Il grafico mostra il guadagno relativo ad ogni auto nel periodo di riferimento", false, true);
        }

        [RelayCommand]
        public async Task LoadUserInfo()
        {
            CleanContent(false);
            if (UserID != string.Empty && MyRegex().IsMatch(UserID))
            {
                try
                {
                    int id = int.Parse(UserID);
                    var response = await _httpClient.GetAsync($"http://localhost:5005/users/stats/info/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var result = JsonSerializer.Deserialize<ErrorInfo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        ErrorMessage = result?.Error ?? "Errore nel recupero dei dati utente.";
                        return;
                    }

                    var user = JsonSerializer.Deserialize<UserInfo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

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

        private async Task LoadStatsImageAsync(string url, string message, bool showFilterDate = false, bool showSliderDate = false)
        {
            StatsMessage = message;
            IsVisibleFilterDate = showFilterDate;
            IsVisibleSliderDate = showSliderDate;

            try
            {
                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    StatsImage = new Bitmap(stream);
                    IsVisibleImage = true;
                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic? errorResponse = JsonSerializer.Deserialize<dynamic>(jsonResponse);
                    StatsText = $"Errore: {errorResponse?.error}";
                }
            }
            catch (Exception ex)
            {
                StatsText = $"Errore: {ex.Message}";
            }
        }

        private void CleanContent(bool noDates)
        {
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

        [RelayCommand]
        public void OnMenuItemSelected(string filterType)
        {
            CleanContent(false);
            if (!UpdateFilterCommand(filterType))
            {
                ExecuteCurrentFilterCommand();
            }
            else
            {
                ShowDatesPanel();
            }
        }

        [RelayCommand]
        public void ShowMenuTextBox()
        {
            CleanContent(false);
            IsVisibleUserBox = true;
        }

        public void ExecuteCurrentFilterCommand()
        {
            CurrentFilterCommand?.Execute(null);
        }

        [RelayCommand]
        private void ShowDatesPanel()
        {
            IsVisibleFilterDate = true;
        }

        [GeneratedRegex("^[0-9]{1,10}?$")]
        private static partial Regex MyRegex();
    }

    public record UserInfo(int UserId, string Username, int TotalBookings, double TotalSpent);
    public record ErrorInfo(string Error);
}
