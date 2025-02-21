using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;


namespace Frontend.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _prova = string.Empty;

    public MainViewModel()
    {
        Items = new ObservableCollection<ListItemTemplate>(_templates);
        Prova = TokenService.GetToken();;
        SelectedListItem = Items.First(vm => vm.ModelType == typeof(VeicoliViewModel));
    }

    
    private readonly List<ListItemTemplate> _templates =
    [
        new ListItemTemplate(typeof(VeicoliViewModel), "add_regular", "Add booking"),
    ];


    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private ViewModelBase _currentPage = new HomeViewModel();

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
       CurrentPage = Activator.CreateInstance(value.ModelType) as ViewModelBase;
    }


    public ObservableCollection<ListItemTemplate> Items { get; }

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
