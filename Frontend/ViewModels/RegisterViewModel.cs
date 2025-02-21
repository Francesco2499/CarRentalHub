using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Net.Http;
using Frontend.Services;

namespace Frontend.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly UserService _userService;

    [ObservableProperty]
    private bool _showSubmitForm = true;

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

    public RegisterViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _userService = new UserService();
    }

    [RelayCommand]
    public async Task SubmitRegistration()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Password))
        {
            RegisterMessage = "Compila tutti i campi!";
            return;
        }        
        var response = await _userService.Register(Email, Username, Password);

        if (response != null) 
        {
            RegisterMessage = response.Message;

            if (response.User != null)
            {
                ShowSubmitForm = false;
                ShowGoToLogin = true;
            }
        }
    }
    
    [RelayCommand]
    private void GoToLogin()
    {
        _mainViewModel.ChangeViewModel(new LoginViewModel(_mainViewModel));
    }

    [RelayCommand]
    private void Back()
    {
        _mainViewModel.ChangeViewModel(new HomeViewModel(_mainViewModel));
    }
}
