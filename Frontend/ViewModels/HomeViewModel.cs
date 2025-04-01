using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class HomeViewModel(MainWindowViewModel mainViewModel) : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel = mainViewModel;

        [RelayCommand]
        private void Login()
        {
            _mainViewModel.ChangeViewModel(new LoginViewModel(_mainViewModel));
        }

        [RelayCommand]
        private void Register()
        {
            _mainViewModel.ChangeViewModel(new RegisterViewModel(_mainViewModel));
        }
    }
}