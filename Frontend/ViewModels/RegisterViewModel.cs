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
    private readonly UserService _userService;

    [ObservableProperty]
    private bool _showSubmitForm = true;


    [ObservableProperty]
    private bool _showBack= true;


    [ObservableProperty]
    private bool _showGoToLogin = false;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string registerMessage = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string registerTitle = "Registrazioe";

    [ObservableProperty]
    private bool _isAdmin = false;

    [ObservableProperty]
    private ObservableCollection<string>? _regions;

    [ObservableProperty]
    private string? _selectedRegion;

    public RegisterViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _userService = new UserService();
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
        _userService = new UserService();
    }

    [RelayCommand]
    public void SubmitRegistration()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Compila tutti i campi!";
            return;
        }        
        var response = UserService.Register(Email, Username, Password, IsAdmin);

        
        if (response != null) 
        {
            if (IsAdmin) {
                if (response.User != null) {
                    ErrorMessage = response.Message;
                } else {
                    RegisterMessage = "Aggiunto nuovo amministratore!";
                }
            } else {
                ErrorMessage = response.Message;

                if (response.User != null)
                {
                    ShowSubmitForm = false;
                    ShowGoToLogin = true;
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
