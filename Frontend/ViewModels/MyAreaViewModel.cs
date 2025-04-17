using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;
using System.Threading.Tasks;
using System;

namespace Frontend.ViewModels;

public partial class MyAreaViewModel : ViewModelBase
{
    private readonly UserModel _originalUser;
    [ObservableProperty] private UserModel _editedUser;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string? _newPassword;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private bool _showEditPasswordBtn = true;
    [ObservableProperty] private bool _showEditPasswordForm = false;

    public static ObservableCollection<string> Regions { get; } =
    [
        "Abruzzo", "Basilicata", "Calabria", "Campania", "Emilia-Romagna",
        "Friuli-Venezia Giulia", "Lazio", "Liguria", "Lombardia", "Marche",
        "Molise", "Piemonte", "Puglia", "Sardegna", "Sicilia",
        "Toscana", "Trentino-Alto Adige", "Umbria", "Valle d'Aosta", "Veneto"
    ];

    public MyAreaViewModel(UserModel User)
    {
        _originalUser = User with { };
        EditedUser = User;
    }

    [RelayCommand]
    private void ShowEditPassword()
    {
        ShowEditPasswordBtn = false;
        ShowEditPasswordForm = true;
    }

    [RelayCommand]
    private async Task Save()
    {
        SuccessMessage = "";
        ErrorMessage = "";

        if (string.IsNullOrWhiteSpace(EditedUser.Email) 
            || string.IsNullOrWhiteSpace(EditedUser.Region) 
            || string.IsNullOrWhiteSpace(EditedUser.Username) 
            || (ShowEditPasswordForm && (string.IsNullOrWhiteSpace(EditedUser.Password) || string.IsNullOrWhiteSpace(NewPassword))))
        {
            ErrorMessage = "Inserisci un valore per tutti i campi!";
            return;
        }

        if (EditedUser == _originalUser) {
            ErrorMessage = "Modifica uno dei campi!";
            return;
        }

        try
        {
            var editResponse = await UserService.EditProfile(EditedUser, NewPassword);

            if (editResponse?.User != null && !string.IsNullOrEmpty(editResponse.Message))
            {
                SuccessMessage = editResponse.Message;
            }
            else
            {
                ErrorMessage = editResponse?.Message ?? "Errore nella modifica del profilo!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Errore: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        ShowEditPasswordBtn = true;
        ShowEditPasswordForm = false;
    }
}
