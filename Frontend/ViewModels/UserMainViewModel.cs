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

    public ObservableCollection<ListItemTemplate> Items { get; }

    public UserMainViewModel(bool isAdmin, UserModel user)
    {
        var templates = !isAdmin
            ? new List<ListItemTemplate>
            {
                new(() => new UserHomePageViewModel(this), "HomeRegular", "Home"),
                new(() => new NewBookingViewModel(), "add_regular", "Nuova prenotazione"),
                new(() => new MyBookingsViewModel(), "book_search_regular", "Le mie prenotazioni"),
                new(() => new MyAreaViewModel(user), "book_search_regular", "My Area")
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
            CurrentPage = value.CreateInstance();
        }
    }

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
