using Avalonia.Controls;
using Frontend.ViewModels;

namespace Frontend.Views;

public partial class TileMapView : UserControl
{

        public TileMapView()
        {
            InitializeComponent();
            
            // Imposta il DataContext manualmente
            this.DataContext = new TileMapViewModel();

            // Configura la mappa
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
