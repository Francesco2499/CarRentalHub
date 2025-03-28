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
using System.Collections.Generic;
using Mapsui.Widgets;
using System.Reflection.Metadata.Ecma335;
using System;

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

        MapView.Layers.Add(CreateMarkerLayer());
        
        MapView.Info += MapOnInfo;

        MapView.Home = n => n.CenterOnAndZoomTo(MapView.Layers[1].Extent!.Centroid, n.Resolutions[6]);

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

    private static MemoryLayer CreateMarkerLayer()
    {
        return new MemoryLayer
        {
            Name = "VehicleMarkers",
            IsMapInfoLayer = true,
            Features = new MemoryProvider(GetLocationFromShowrooms()).Features,
            Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7) // Puoi cambiare l'icona del marker se lo desideri
        };
    }

    private static IEnumerable<IFeature> GetLocationFromShowrooms()
    {
        var showrooms = VehicleService.GetAvailableShowrooms(null, null);

        return showrooms.Select(c =>
        {
            var feature = new PointFeature(SphericalMercator.FromLonLat(c.Longitude, c.Latitude).ToMPoint());
            feature.Styles.Add(CreateCalloutStyle(c.Name + "\n\n", "Lista veicoli:\n" + string.Join(", ", c.Vehicles.Select(v => $"\n{v.Model} ({v.Category}, {v.Price})"))));
            return feature;
        });
    }

    private static CalloutStyle CreateCalloutStyle(string title, string content)
    {
        // Crea un callout che mostra le informazioni del veicolo
        return new CalloutStyle
        {
            Type = CalloutType.Detail,
            Title = title,
            Subtitle = content,
            TitleTextAlignment = Alignment.Center,
            SubtitleFont = { FontFamily = null, Size = 14, Italic = false, Bold = false }, // Font più grande
            TitleFont = { FontFamily = null, Size = 14, Italic = false, Bold = true }, // Font più grande
            TitleFontColor = Color.White, // Testo più visibile
            SubtitleFontColor = Color.White,
            BackgroundColor = Color.FromArgb(255, 78, 78, 78),
            MaxWidth = 250,
            RectRadius = 10,
            ShadowWidth = 5,
            Enabled = false,  // Disabilita per renderlo visibile solo quando richiesto
            SymbolOffset = new Offset(0, 30), // Posiziona il callout sopra il marker
        };
    }
}

