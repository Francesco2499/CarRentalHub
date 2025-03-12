using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Frontend.ViewModels;


public record AvailabilityStats
(
    int available,
    int rented
);

public partial class StatsViewModel : ViewModelBase
{   [ObservableProperty] private bool _isBackButtonVisible = false;

    // Proprietà per controllare la visibilità dei bottoni principali
    [ObservableProperty] private bool _areMainButtonsVisible = true;

    // Proprietà per controllare la visibilità dei bottoni principali singoli
    [ObservableProperty] private bool _isVeicoliButtonVisible = true;
    [ObservableProperty] private string _availabilityMessage = string.Empty;

    [ObservableProperty] private bool _isUtentiButtonVisible = true;

    [ObservableProperty] private bool _isPrenotazioniButtonVisible = true;
    [ObservableProperty] public AvailabilityStats? _availabilityStats;
    [ObservableProperty] public Bitmap? _top5Image;
    [ObservableProperty] private bool _isVisibleTop5 = false;
    [ObservableProperty] private bool _isVisibleVehicleStats = false;
    [ObservableProperty] private bool _isVisibleAvailability = false;
    private readonly HttpClient _httpClient;
    [ObservableProperty] private string errorMessage = string.Empty;
    public StatsViewModel()
    {
        _httpClient = new HttpClient();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    [RelayCommand]
    public void ShowButton(string buttonName)
    {
        IsVeicoliButtonVisible = false;
        IsUtentiButtonVisible = false;
        IsPrenotazioniButtonVisible = false;

        switch (buttonName)
        {
            case "Veicoli":
                IsVeicoliButtonVisible = true;
                IsVisibleVehicleStats = true;
                break;
            case "Utenti":
                IsUtentiButtonVisible = true;
                HideVehicleStats();
                break;
            case "Prenotazioni":
                IsPrenotazioniButtonVisible = true;
                HideVehicleStats();
                break;
        }

        IsBackButtonVisible = true;
    }

    [RelayCommand]
    public void GoBack()
    {
        IsVeicoliButtonVisible = true;
        IsUtentiButtonVisible = true;
        IsPrenotazioniButtonVisible = true;

        IsBackButtonVisible = false;
        HideVehicleStats();
    }

    public void HideVehicleStats() {
        IsVisibleVehicleStats = false;
        IsVisibleTop5 = false;
        IsVisibleAvailability = false;
    }


    [RelayCommand]
    public  async Task ShowTopRented()
    {
        HideVehicleStats();
        try
        {
            // Leggi il corpo della risposta
            byte[] imageBytes = await _httpClient.GetByteArrayAsync("http://localhost:5005/vehicle/stats/top5");

            // Crea un Bitmap da questi byte
            var bitmap = new Bitmap(new MemoryStream(imageBytes));

            // Imposta l'immagine come sorgente nel controllo Image
            Top5Image = bitmap; 
            IsVisibleTop5 = true;//
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Si è verificato un errore: {ex.Message}";
        }
    }

    // Comando per mostrare la disponibilità delle auto
    [RelayCommand]
    public async Task ShowAvailability()
    {
        HideVehicleStats();
        try
        {
            // Effettua la richiesta GET alla rotta /vehicle_stats
            var response = await _httpClient.GetAsync("http://localhost:5005/vehicle/stats/availability");

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Errore nella risposta del server";
                return;
            }

            // Leggi il corpo della risposta
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);  // Aggiungi un debug per vedere la risposta

            // Deserialize il JSON restituito dal server
            var stats = JsonSerializer.Deserialize<AvailabilityStats>(responseBody, JsonOptions);

            if (stats != null)
            {
                AvailabilityStats = stats;
                AvailabilityMessage = $"Veicoli attualmente disponibili: {AvailabilityStats?.available.ToString()} \nVeicoli attualmente noleggiati: {AvailabilityStats?.rented.ToString()}";
                IsVisibleAvailability = true;
            }
            else
            {
                ErrorMessage = "Errore nel deserializzare i dati.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Si è verificato un errore: {ex.Message}";
        }
    }

}
