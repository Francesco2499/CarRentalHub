using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Net.Http;
using Frontend.Services;
using System.Collections.ObjectModel;

namespace Frontend.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;
    [ObservableProperty] private bool _showSubmitForm = true;
    [ObservableProperty] private bool _showBack= true;
    [ObservableProperty] private bool _showGoToLogin = false;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string registerMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private string registerTitle = "Registrazioe";
    [ObservableProperty] private bool _isAdmin = false;
    [ObservableProperty] private ObservableCollection<string>? _regions;
    [ObservableProperty] private string? _selectedRegion;
    [ObservableProperty] private string _backgroundColor = "Black";

    public RegisterViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        Regions =
        [
            "Abruzzo", "Basilicata", "Calabria", "Campania", "Emilia-Romagna",
            "Friuli-Venezia Giulia", "Lazio", "Liguria", "Lombardia", "Marche",
            "Molise", "Piemonte", "Puglia", "Sardegna", "Sicilia",
            "Toscana", "Trentino-Alto Adige", "Umbria", "Valle d'Aosta", "Veneto"
        ];
    }

    public RegisterViewModel(bool isAdmin)
    {
        IsAdmin = isAdmin;
        ShowBack = !isAdmin;
        RegisterTitle = "Registrazione nuovi admin";
        BackgroundColor = "2d2d2d";

    }

    [RelayCommand]
    public void SubmitRegistration()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Compila tutti i campi!";
            return;
        }      

        var response = UserService.Register(Email, Username, Password, IsAdmin, SelectedRegion ?? "");
        
        if (response != null) 
        {
            if (IsAdmin) {
                if (response.Error != null) {
                    ErrorMessage = response.Message ?? "Errore nella registrazione!";
                } else {
                    RegisterMessage = "Aggiunto nuovo amministratore!";
                }
            } else {

                if (response.Error == null && !string.IsNullOrWhiteSpace(response.Message))
                {
                    ShowSubmitForm = false;
                    ShowGoToLogin = true;
                    RegisterMessage = response.Message;
                } else {
                    ErrorMessage = response.Message ?? "Errore nella registrazione!";
                }
            }
            
        }
    }
    
    [RelayCommand]
    private void GoToLogin()
    {
        _mainViewModel?.ChangeViewModel(new LoginViewModel(_mainViewModel));
    }

    [RelayCommand]
    private void Back()
    {
        _mainViewModel?.ChangeViewModel(new HomeViewModel(_mainViewModel));
    }
}
