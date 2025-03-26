using Avalonia.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling;
using Mapsui.Styles;

namespace Frontend.Views;

public partial class TileMapView : UserControl
{
    public TileMapView()
    {
        InitializeComponent();
        SetupMap();
    }

   private void SetupMap()
    {
        var map = new Mapsui.Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        var (x, y) = Mapsui.Projections.SphericalMercator.FromLonLat(11.2558, 43.7696);
        //italia -> 12.5, 42.0
        //roma -> 12.4964, 41.9028
        //perugi -> 12.3888, 43.1122 
        //terni -> 12.6437, 42.5636
        //firenze -> 11.2558, 43.7696
        //arezzo -> 11.8820, 43.4631
        var center = new Mapsui.MPoint(x, y);

        map.Home = n => n.CenterOnAndZoomTo(center, n.Resolutions[6]);

        var (x1, y2) = Mapsui.Projections.SphericalMercator.FromLonLat(12.4964, 41.9028);
        var rome = new Mapsui.MPoint(x1, y2);
        map.Layers.Add(CreateMarkerLayer(rome));

        MapView.Map = map;
    }

    private static MemoryLayer CreateMarkerLayer(MPoint position)
    {
        // Crea il marker e imposta SOLO il triangolo rosso come stile
        var feature = new PointFeature(position);
        feature.Styles.Clear(); // <- molto importante: pulisce stili predefiniti

        feature.Styles.Add(new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            SymbolScale = 0.3,
            Fill = new Brush(Color.FromArgb(255, 0, 122, 204)), //#007ACC -> 0,122,204 (255 è opaco)
            SymbolOffset = new Offset(0, 10)
        });

        return new MemoryLayer
        {
            Name = "Markers",
            IsMapInfoLayer = true,
            Features = new[] { feature },

            Style = null
        };
    }

}
