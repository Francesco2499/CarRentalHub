using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Frontend.Models;

namespace Frontend.Services
{
    public class VehicleService
    {

        // Recupera i veicoli disponibili in una data specifica (restituisce una lista vuota in caso di errore)
        public static List<VehicleModel> GetVehiclesByDate(DateTime? startDate, DateTime? endDate)
        {
            string url = $"http://localhost:8085/api/v1/vehicle/getAllAvailable?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
            
            var vehicles = HttpService.Get<List<VehicleModel>>(url);

            return vehicles ?? [];
        }

        // Recupera tutti i veicoli (restituisce una lista vuota in caso di errore)
        public static List<VehicleModel> GetAllVehicles()
        {
            var vehicles = HttpService.Get<List<VehicleModel>>("http://localhost:8085/api/v1/vehicle/getAll");

            return vehicles ?? [];
        }

        // Aggiunge un veicolo e restituisce l'oggetto creato oppure null in caso di errore
        public static VehicleModel? AddVehicle(VehicleModel vehicle)
        {
            var requestBody = new
            {
                model = vehicle.Model,
                category = vehicle.Category,
                price = vehicle.Price,
                location = "Napoli"
            };

            var result = HttpService.Post<VehicleModel>("http://localhost:8085/api/v1/vehicle/new", requestBody);
           
            return result;
        }

        // Modifica un veicolo esistente e restituisce il veicolo aggiornato oppure null in caso di errore
        public static VehicleModel? EditVehicle(VehicleModel vehicle)
        {
            var requestBody = new
            {
                model = vehicle.Model,
                category = vehicle.Category,
                price = vehicle.Price,
                location = "Napoli"
            };

            var updatedVehicle = HttpService.Put<VehicleModel>($"http://localhost:8085/api/v1/vehicle/update/{vehicle.Id}", requestBody);

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
}
