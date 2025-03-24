using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.ViewModels;

public partial class UserMainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;
    private readonly MainWindowViewModel _mainViewModel;

    public ObservableCollection<ListItemTemplate> Items { get; }

    public UserMainViewModel(bool isAdmin, UserModel user, MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        var templates = !isAdmin
            ? new List<ListItemTemplate>
            {
                new(() => new UserHomePageViewModel(this), "HomeRegular", "Home"),
                new(() => new NewBookingViewModel(), "add_regular", "Nuova prenotazione"),
                new(() => new MyBookingsViewModel(), "book_search_regular", "Le mie prenotazioni"),
                new(() => new MyAreaViewModel(user), "person_regular", "My Area"),
                new(() => new TileMapViewModel(_mainViewModel), "map_regular", "Mappa"),
                new(() => new MainWindowViewModel(), "sign_out_regular", "Logout")  // Qui non c'è un ViewModel da creare
            }
            :
            [
                new(() => new AdminVehicleViewModel(), "vehicle_car_regular", "Gestisci veicoli"),
                new(() => new AdminBookingViewModel(), "book_search_regular", "Gestisci prenotazioni"),
                new(() => new RegisterViewModel(true), "personal_regular", "Aggiungi admin"),
                new(() => new StatsViewModel(), "personal_regular", "Visualizza stats")
            ];

        Items = [.. templates];
        SelectedListItem = Items.First();
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value != null)
        {
            if (value.Label == "Logout")
            {
                Logout();  // Esegui l'azione di logout
            }
            else
            {
                CurrentPage = value.CreateInstance();  // Crea la vista per gli altri elementi
            }
        }
    }

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
        private void Logout()
        {
            _mainViewModel.ChangeViewModel(new HomeViewModel(_mainViewModel));
        }
}
