using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public class UserHomePageViewModel : ViewModelBase
    {
        private readonly UserMainViewModel _mainViewModel;

        public UserHomePageViewModel(UserMainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SelectBookingCommand = new RelayCommand(() => SelectItem("Le mie prenotazioni"));
            SelectCarCommand = new RelayCommand(() => SelectItem("Nuova prenotazione"));
            GoToMyAreaCommand = new RelayCommand(() => SelectItem("My Area"));
        }

        public RelayCommand SelectBookingCommand { get; }
        public RelayCommand SelectCarCommand { get; }

        public RelayCommand GoToMyAreaCommand { get; }

        private void SelectItem(string label)
        {
            var item = _mainViewModel.Items.FirstOrDefault(i => i.Label == label);
            if (item != null)
            {
                _mainViewModel.SelectedListItem = item;
            }
        }
    }
}