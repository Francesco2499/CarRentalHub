using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.ViewModels;

public partial class MyAreaViewModel : ViewModelBase
{
    private readonly UserModel _originalUser;

    [ObservableProperty] private UserModel _editedUser;
    [ObservableProperty] private bool isSaveEnabled;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private string _newPassword;
    [ObservableProperty] private string _successMessage;
    [ObservableProperty] private bool _showEditPasswordBtn;
    [ObservableProperty] private bool _showEditPasswordForm;

    public ObservableCollection<string> Regions { get; } = new()
    {
        "Lombardia", "Lazio", "Sicilia", "Campania", "Veneto"
    };

    public MyAreaViewModel(UserModel User)
    {
        _originalUser = User;
        EditedUser = User;
    }

    partial void OnEditedUserChanged(UserModel value) => CheckModified();

    private void CheckModified()
    {
        ErrorMessage = "";
        IsSaveEnabled = EditedUser.Username != _originalUser.Username ||
                        EditedUser.Email != _originalUser.Email ||
                        EditedUser.Region != _originalUser.Region ||
                        EditedUser.Password != _originalUser.Password;    
    }

    [RelayCommand]
    private void ShowEditPassword()
    {
        ShowEditPasswordBtn = false;
        ShowEditPasswordForm = true;
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(EditedUser.Email) 
        || string.IsNullOrWhiteSpace(EditedUser.Region) 
        || string.IsNullOrWhiteSpace(EditedUser.Username) 
        || (ShowEditPasswordForm && (string.IsNullOrWhiteSpace(EditedUser.Password) || string.IsNullOrWhiteSpace(NewPassword))))
        {
            ErrorMessage = "Inserisci un valore per tutti i campi!";
        }

        var editResponse = UserService.EditProfile(EditedUser, NewPassword);

        if (editResponse?.User != null && !string.IsNullOrEmpty(editResponse.Message)) {
            SuccessMessage = editResponse.Message;
        } else {
            ErrorMessage = editResponse?.Message ?? "Errore nella modifica del profilo!";
        }

        
    }

    [RelayCommand]
    private void Cancel()
    {
        ShowEditPasswordBtn = true;
        ShowEditPasswordForm = false;
    }
}
