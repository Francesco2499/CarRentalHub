using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Net.Http;
using Frontend.Services;

namespace Frontend.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly UserService _userService;

    [ObservableProperty]
    private string emailOrUsername = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string loginMessage = string.Empty;

    public LoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _userService = new UserService();
    }

    [RelayCommand]
    public async Task SubmitLogin()
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            LoginMessage = "Compila tutti i campi!";
            return;
        }        
        var response = await _userService.Authenticate(EmailOrUsername, Password);

        if (response != null) {
            if (!string.IsNullOrEmpty(response.Token)) 
            {
                _mainViewModel.ChangeViewModel(new UserMainViewModel(response.IsAdmin));
            } else {
                LoginMessage = response.Message;
                return;
            }
        }
    }

    [RelayCommand]
    private void Back()
    {
        _mainViewModel.ChangeViewModel(new HomeViewModel(_mainViewModel));
    }
}
