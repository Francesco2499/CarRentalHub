using Avalonia.Controls;
using Frontend.ViewModels;

namespace Frontend.Views;

public partial class TileMapView : UserControl
{

        public TileMapView()
        {
            InitializeComponent();
            
            this.DataContext = new TileMapViewModel();

            SetupMap();
        }

        private void SetupMap()
        {
            if (this.DataContext != null) {
                var viewModel = (TileMapViewModel)this.DataContext;
                
                if (viewModel != null) {
                    MapView.Map = viewModel.MapView;
                }
            }
        }
    }
