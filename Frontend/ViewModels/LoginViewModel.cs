using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Frontend.Services;

namespace Frontend.ViewModels;

public partial class LoginViewModel(MainWindowViewModel mainViewModel) : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel = mainViewModel;
    [ObservableProperty] private string emailOrUsername = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string loginMessage = string.Empty;

    [RelayCommand]
    public void SubmitLogin()
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            LoginMessage = "Compila tutti i campi!";
            return;
        }        
        var response = UserService.Authenticate(EmailOrUsername, Password);

        if (response != null) {
            if (response.Error == null && response.User != null) 
            {
                _mainViewModel.ChangeViewModel(new UserMainViewModel(response.User.Role == "admin", response.User, _mainViewModel));
            } else {
                LoginMessage = response.Message ?? "Errore login!";
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
