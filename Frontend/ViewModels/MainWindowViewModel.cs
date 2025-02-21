namespace Frontend.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainWindowViewModel : ViewModelBase
{

    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        // Imposta la schermata iniziale (DashView)
        CurrentView = new HomeViewModel(this);//this);
    }

    public void ChangeViewModel(ViewModelBase newView)
    {
        CurrentView = newView;
    }
}
