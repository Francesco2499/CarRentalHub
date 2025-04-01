namespace Frontend.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainWindowViewModel : ViewModelBase
{

    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        CurrentView = new HomeViewModel(this);
    }

    public void ChangeViewModel(ViewModelBase newView)
    {
        CurrentView = newView;
    }
}
