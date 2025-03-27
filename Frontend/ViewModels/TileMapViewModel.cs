using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling;
using Mapsui.Styles;
using Mapsui.Extensions;
using Mapsui.Providers;
using Mapsui.Projections;
using System.Linq;
using Frontend.Services;
using Frontend.Models;

namespace Frontend.ViewModels;
public class TileMapViewModel : ViewModelBase
{
    public Map MapView { get; private set; }

    public TileMapViewModel()
    {
        MapView = new Map(); // Crea la mappa nel ViewModel
        SetupMap();
    }

    private void SetupMap()
    {

        MapView.Layers.Add(OpenStreetMap.CreateTileLayer());



        var (x1, y2) = SphericalMercator.FromLonLat(12.4964, 41.9028);
        var centerPoint = new MPoint(x1, y2);
        var vehicles = VehicleService.GetAllVehicles();

            // Per ogni veicolo, crea un marker sulla mappa
        foreach (var vehicle in vehicles)
        {
            var (x, y) = SphericalMercator.FromLonLat(vehicle.Longitude, vehicle.Latitude);
            var point = new MPoint(x, y);

            // Aggiungi il marker per ogni veicolo
            MapView.Layers.Add(CreateMarkerLayer(point, vehicle));
        }
        MapView.Info += MapOnInfo;

        MapView.Home = n => n.CenterOnAndZoomTo(MapView.Layers[1].Extent!.Centroid, n.Resolutions[7]);

    }

    private static void MapOnInfo(object? sender, MapInfoEventArgs e)
    {
        var calloutStyle = e.MapInfo?.Feature?.Styles.Where(s => s is CalloutStyle).Cast<CalloutStyle>().FirstOrDefault();
        if (calloutStyle != null)
        {
            calloutStyle.Enabled = !calloutStyle.Enabled;
            e.MapInfo?.Layer?.DataHasChanged();
        }
    }

private static MemoryLayer CreateMarkerLayer(MPoint position, VehicleModel vehicle)
        {
            var feature = new PointFeature(position);
            feature.Styles.Clear();

            // Aggiungi le informazioni del veicolo
            feature[nameof(VehicleModel.Id)] = vehicle.Id.ToString();
            feature[nameof(VehicleModel.Model)] = vehicle.Model;
            feature[nameof(VehicleModel.Category)] = vehicle.Category;
            feature[nameof(VehicleModel.Price)] = vehicle.Price.ToString("C"); // Formatta il prezzo come valuta

            // Aggiungi lo stile del callout
            feature.Styles.Add(CreateCalloutStyle(vehicle));

            return new MemoryLayer
            {
                Name = "VehicleMarkers",
                IsMapInfoLayer = true,
                Features = new MemoryProvider(feature).Features,
                Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7) // Puoi cambiare l'icona del marker se lo desideri
            };
        }
        private static CalloutStyle CreateCalloutStyle(VehicleModel vehicle)
        {
            string price = vehicle.Price.ToString("C");
            string id = vehicle.Id.ToString("");
            // Crea un callout che mostra le informazioni del veicolo
            return new CalloutStyle
            {
                Title = $"Modello: {vehicle.Model}\nCategoria:{vehicle.Category}\nID: {vehicle.Id}\nPrice: {vehicle.Price:C}",
                TitleFont = { FontFamily = null, Size = 14, Italic = false, Bold = true }, // Font più grande
                TitleFontColor = Color.FromArgb(255, 40, 167, 69), // Testo più visibile
                Color = Color.FromArgb(255, 40, 167, 69),
                MaxWidth = 200, // Più spazio per il testo
                RectRadius = 10,
                ShadowWidth = 5,
                Enabled = false,  // Disabilita per renderlo visibile solo quando richiesto
                SymbolOffset = new Offset(0, 30), // Posiziona il callout sopra il marker
            };
        }

    private class City
    {
        public string? Country { get; set; }
        public string? Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}

