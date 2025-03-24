using Frontend.ViewModels;

namespace Frontend.ViewModels
{
    public class TileMapViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _main;

        public TileMapViewModel(MainWindowViewModel main)
        {
            _main = main;
        }
    }
}
