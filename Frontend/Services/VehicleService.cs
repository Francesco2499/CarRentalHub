using System;
using System.Collections.Generic;
using Frontend.Models;

namespace Frontend.Services
{
    public class VehicleService
    {
        // Recupera i veicoli disponibili in una data specifica (restituisce una lista vuota in caso di errore)
        public static List<VehicleModel> GetVehiclesByDate(DateTime? startDate, DateTime? endDate, string location)
        {
            string url = $"http://localhost:8085/api/v1/vehicle/getAllAvailable?location={location}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
            
            var vehicles = HttpService.Get<List<VehicleModel>>(url);

            return vehicles ?? [];  // Se la risposta è nulla, ritorna una lista vuota
        }

        // Recupera tutti i veicoli (restituisce una lista vuota in caso di errore)
        public static List<VehicleModel> GetAllVehicles()
        {
            var vehicles = HttpService.Get<List<VehicleModel>>("http://localhost:8085/api/v1/vehicle/getAll");

            return vehicles ?? [];  // Se la risposta è nulla, ritorna una lista vuota
        }

        // Aggiunge un veicolo e restituisce l'oggetto creato oppure null in caso di errore
        public static VehicleResponse? AddVehicle(VehicleModel vehicle)
        {
            // Creazione del dizionario per i parametri della richiesta
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "location", vehicle.Location }
            };

            // Invia la richiesta POST per aggiungere un nuovo veicolo
            var result = HttpService.Post<VehicleResponse>("http://localhost:8085/api/v1/vehicle/new", requestBody);

            return result;
        }

        // Modifica un veicolo esistente e restituisce il veicolo aggiornato oppure null in caso di errore
        public static VehicleResponse? EditVehicle(VehicleModel vehicle)
        {
            // Creazione del dizionario per i parametri della richiesta
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "location", vehicle.Location }
            };

            // Invia la richiesta PUT per aggiornare un veicolo
            var updatedVehicle = HttpService.Put<VehicleResponse>($"http://localhost:8085/api/v1/vehicle/update/{vehicle.Id}", requestBody);

            return updatedVehicle;
        }

        // Elimina un veicolo e gestisce eventuali errori
        public static void DeleteVehicle(int vehicleId)
        {
            try
            {
                HttpService.Delete<dynamic>($"http://localhost:8085/api/v1/vehicle/delete/{vehicleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting vehicle {vehicleId}: {ex.Message}");
            }
        }
    }

    public record VehicleResponse(VehicleModel? Vehicle, string? Message, string? Error);

}
