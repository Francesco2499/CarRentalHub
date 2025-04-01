using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Helpers;

namespace Frontend.ViewModels;

public partial class UserMainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isPaneOpen;
    [ObservableProperty] private ViewModelBase? _currentPage;
    [ObservableProperty] private ListItemTemplate? _selectedListItem;
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
                new(() => new TileMapViewModel(), "map_regular", "Mappa"),
                new(() => new MainWindowViewModel(), "sign_out_regular", "Logout")
            }
            :
            [
                new(() => new AdminVehicleViewModel(), "vehicle_car_regular", "Gestisci veicoli"),
                new(() => new AdminBookingViewModel(), "book_search_regular", "Gestisci prenotazioni"),
                new(() => new RegisterViewModel(true), "person_regular", "Aggiungi admin"),
                new(() => new StatsViewModel(), "document_catch_up_regular", "Visualizza statistiche"),
                new(() => new MainWindowViewModel(), "sign_out_regular", "Logout")
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
                TokenHelper.RemoveToken();
                Logout();
            }
            else
            {
                CurrentPage = value.CreateInstance();
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
