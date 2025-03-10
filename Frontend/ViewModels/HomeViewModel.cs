using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;

        public HomeViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void Login()
        {
            // Cambia schermata alla vista di login
            _mainViewModel.ChangeViewModel(new LoginViewModel(_mainViewModel));
        }

        [RelayCommand]
        private void Register()
        {
            // Cambia schermata alla vista di registrazione
            _mainViewModel.ChangeViewModel(new RegisterViewModel(_mainViewModel));
        }
    }
}