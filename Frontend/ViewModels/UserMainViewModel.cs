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

    public UserMainViewModel(bool isAdmin)
    {
        var templates = !isAdmin
            ? new List<ListItemTemplate>
            {
                new(() => new NewBookingViewModel(), "add_regular", "Add booking"),
                new(() => new MyBookingsViewModel(), "book_search_regular", "My bookings")
            }
            :
            [
                new(() => new AdminVehicleViewModel(), "vehicle_car_regular", "Manage Vehicles"),
                new(() => new AdminBookingViewModel(), "book_search_regular", "Manage Bookings"),
                new(() => new RegisterViewModel(true), "personal_regular", "Add new admin"),
                new(() => new StatsViewModel(), "personal_regular", "Add new admin")
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
