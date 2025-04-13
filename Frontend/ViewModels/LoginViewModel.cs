using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Services;
using System;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public partial class LoginViewModel(MainWindowViewModel mainViewModel) : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel = mainViewModel;
        
        [ObservableProperty] private string _emailOrUsername = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _loginMessage = string.Empty;

        [RelayCommand]
        public async Task SubmitLogin()
        {
            if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
            {
                LoginMessage = "Compila tutti i campi!";
                return;
            }

            try
            {
                var response = await UserService.Authenticate(EmailOrUsername, Password);

                if (response != null)
                {
                    if (response.Error == null && response.User != null)
                    {
                        _mainViewModel.ChangeViewModel(new UserMainViewModel(response.User.Role == "admin", response.User, _mainViewModel));
                    }
                    else
                    {
                        LoginMessage = response.Message ?? "Errore login!";
                    }
                }
                else
                {
                    LoginMessage = "Errore nel login! Nessuna risposta dal server.";
                }
            }
            catch (Exception ex)
            {
                LoginMessage = $"Si è verificato un errore: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Back()
        {
            _mainViewModel.ChangeViewModel(new HomeViewModel(_mainViewModel));
        }
    }
}
