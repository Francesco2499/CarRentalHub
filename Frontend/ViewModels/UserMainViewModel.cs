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
    private ViewModelBase _currentPage = new HomeViewModel();

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;

    public ObservableCollection<ListItemTemplate> Items { get; }

    public UserMainViewModel(bool isAdmin)
    {
        var templates = !isAdmin
            ? new List<ListItemTemplate>
            {
                new(typeof(VeicoliViewModel), "add_regular", "Add booking"),
                new(typeof(MyBookingsViewModel), "book_search_regular", "My bookings")
            }
            : new List<ListItemTemplate>
            {
                new(typeof(AdminVehicleViewModel), "admin_panel_settings", "Manage Vehicles")
            };

        Items = new ObservableCollection<ListItemTemplate>(templates);
        SelectedListItem = Items.First();
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value != null)
        {
            CurrentPage = Activator.CreateInstance(value.ModelType) as ViewModelBase;
        }
    }

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
