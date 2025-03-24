/*using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Frontend.Models;

namespace Frontend.Views;

public partial class TileMapView : UserControl
{
    private readonly int _zoom = 5;
    private readonly double _centerLat = 41.8719;  // centro Italia
    private readonly double _centerLon = 12.5674;
    private readonly int _gridSize = 5;

    private Grid _mapGrid;
    private Canvas _markerCanvas;

    public TileMapView()
    {
        InitializeComponent();

        _mapGrid = this.FindControl<Grid>("MapGrid");
        _markerCanvas = this.FindControl<Canvas>("MarkerCanvas");

        LoadMapWithMarkersAsync();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LoadMapWithMarkersAsync()
    {
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (AvaloniaApp)");

            var centerX = LonToTileX(_centerLon, _zoom);
            var centerY = LatToTileY(_centerLat, _zoom);
            var half = _gridSize / 2;

            _mapGrid.RowDefinitions.Clear();
            _mapGrid.ColumnDefinitions.Clear();
            _mapGrid.Children.Clear();
            _markerCanvas.Children.Clear();

            for (int i = 0; i < _gridSize; i++)
            {
                //_mapGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                //_mapGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                _mapGrid.RowDefinitions.Add(new RowDefinition(new GridLength(256)));
                _mapGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(256)));
            }

            for (int dy = -half; dy <= half; dy++)
            {
                for (int dx = -half; dx <= half; dx++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;
                    int col = dx + half;
                    int row = dy + half;

                    string tileUrl = $"https://tile.openstreetmap.org/{_zoom}/{x}/{y}.png";

                    try
                    {
                        var bytes = await client.GetByteArrayAsync(tileUrl);
                        using var stream = new MemoryStream(bytes);
                        var bitmap = new Bitmap(stream);

                        var img = new Image
                        {
                            Source = bitmap,
                            Width = 256,
                            Height = 256
                        };

                        Grid.SetRow(img, row);
                        Grid.SetColumn(img, col);
                        _mapGrid.Children.Add(img);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore tile {x}/{y}: {ex.Message}");
                    }
                }
            }

            DrawMarkers(centerX, centerY);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRORE: {ex.Message}");
        }
    }

    private void DrawMarkers(int centerTileX, int centerTileY)
    {
        var markers = GetSampleMarkers();
        foreach (var m in markers)
        {
            int tileX = LonToTileX(m.Longitude, _zoom);
            int tileY = LatToTileY(m.Latitude, _zoom);

            int dx = tileX - centerTileX + _gridSize / 2;
            int dy = tileY - centerTileY + _gridSize / 2;

            if (dx < 0 || dy < 0 || dx >= _gridSize || dy >= _gridSize)
                continue;

            double offsetX = 256 * dx + 128;
            double offsetY = 256 * dy + 128;

            var ellipse = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(ellipse, offsetX - 6);
            Canvas.SetTop(ellipse, offsetY - 6);

            _markerCanvas.Children.Add(ellipse);
        }
    }

    private List<VehicleMarker> GetSampleMarkers() => new()
    {
        new() { Latitude = 45.4642, Longitude = 9.19 },   // Milano
        new() { Latitude = 41.9028, Longitude = 12.4964 }, // Roma
        new() { Latitude = 40.8518, Longitude = 14.2681 }  // Napoli
    };

    private int LonToTileX(double lon, int zoom) =>
        (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2, zoom));

    private int LatToTileY(double lat, int zoom)
    {
        var rad = lat * Math.PI / 180.0;
        return (int)Math.Floor(
            (1.0 - Math.Log(Math.Tan(rad) + 1.0 / Math.Cos(rad)) / Math.PI) / 2.0 * Math.Pow(2, zoom)
        );
    }
}
*/
/* funzionante con bande larghe ai lati
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls.Shapes;
using Avalonia;
using Avalonia.Platform;

namespace Frontend.Views;

public partial class TileMapView : UserControl
{
    private const int Zoom = 5;
    private const int TileSize = 256;
    private const int GridSize = 3; // 3x3 tile
    private const double CenterLat = 42.5;   // Centro Italia
    private const double CenterLon = 13.5;

    private Image _mapImage;
    private Canvas _markerCanvas;

    public TileMapView()
    {
        InitializeComponent();
        _mapImage = this.FindControl<Image>("MapImage");
        _markerCanvas = this.FindControl<Canvas>("MarkerCanvas");

        _ = LoadMapWithMarkersAsync();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async Task LoadMapWithMarkersAsync()
    {
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("AvaloniaApp");

            int centerX = LonToTileX(CenterLon, Zoom);
            int centerY = LatToTileY(CenterLat, Zoom);
            int half = GridSize / 2;

            var bitmap = new RenderTargetBitmap(new PixelSize(GridSize * TileSize, GridSize * TileSize));
            var drawingContext = bitmap.CreateDrawingContext();

            for (int dx = -half; dx <= half; dx++)
            {
                for (int dy = -half; dy <= half; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    var url = $"https://tile.openstreetmap.org/{Zoom}/{x}/{y}.png";
                    var bytes = await client.GetByteArrayAsync(url);
                    using var stream = new MemoryStream(bytes);
                    var tile = new Bitmap(stream);

                    drawingContext.DrawImage(tile,
                        sourceRect: new Avalonia.Rect(0, 0, TileSize, TileSize),
                        destRect: new Avalonia.Rect((dx + half) * TileSize, (dy + half) * TileSize, TileSize, TileSize));

                }
            }

            drawingContext.Dispose();
            _mapImage.Source = bitmap;

            DrawMarkers(centerX, centerY);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Errore mappa: {ex.Message}");
        }
    }

    private void DrawMarkers(int centerTileX, int centerTileY)
    {
        var markers = new List<(double Lat, double Lon)>
        {
            (41.9028, 12.4964), // Roma
            (45.4642, 9.1900),  // Milano
            (40.8518, 14.2681)  // Napoli
        };

        int half = GridSize / 2;

        foreach (var (lat, lon) in markers)
        {
            int tileX = LonToTileX(lon, Zoom);
            int tileY = LatToTileY(lat, Zoom);

            double pixelX = (tileX - centerTileX + half) * TileSize + TileSize / 2;
            double pixelY = (tileY - centerTileY + half) * TileSize + TileSize / 2;

            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(ellipse, pixelX - 5);
            Canvas.SetTop(ellipse, pixelY - 5);
            _markerCanvas.Children.Add(ellipse);
        }
    }

    private int LonToTileX(double lon, int zoom) =>
        (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2, zoom));

    private int LatToTileY(double lat, int zoom)
    {
        double rad = lat * Math.PI / 180.0;
        return (int)Math.Floor((1.0 - Math.Log(Math.Tan(rad) + 1 / Math.Cos(rad)) / Math.PI) / 2.0 * Math.Pow(2, zoom));
    }
}*/

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Frontend.Views
{
    public partial class TileMapView : UserControl
    {
        private ScaleTransform _scaleTransform;

        public TileMapView()
        {
            InitializeComponent();

            // Inizializza trasformazione
            _scaleTransform = new ScaleTransform(1, 1);

            // Applica trasformazione alla Canvas
            var canvas = this.FindControl<Canvas>("MainCanvas");
            canvas.RenderTransform = _scaleTransform;

            // Gestione zoom
            var image = this.FindControl<Image>("MapImage");
            if (image != null)
                image.PointerWheelChanged += OnZoom;
        }

        private void OnZoom(object? sender, PointerWheelEventArgs e)
        {
            var zoomFactor = e.Delta.Y > 0 ? 1.1 : 0.9;
            _scaleTransform.ScaleX *= zoomFactor;
            _scaleTransform.ScaleY *= zoomFactor;
        }
    }
}
