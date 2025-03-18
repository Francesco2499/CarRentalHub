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
            SelectBookingCommand = new RelayCommand(() => SelectItem("My bookings"));
            SelectCarCommand = new RelayCommand(() => SelectItem("Add booking"));
        }

        public RelayCommand SelectBookingCommand { get; }
        public RelayCommand SelectCarCommand { get; }

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