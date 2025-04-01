using System;
using System.Collections.Generic;
using Frontend.Models;

namespace Frontend.Services
{
    public class VehicleService
    {
        public static List<ShowroomModel> GetAvailableShowrooms(DateTime? startDate, DateTime? endDate)
        {
            string url = "http://localhost:8085/api/v1/vehicle/getAllAvailable";
            
            if (startDate != null && endDate != null) {
                url += $"?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
            }
            
            var showrooms = HttpService.Get<List<ShowroomModel>>(url);

            return showrooms ?? [];
        }

        public static List<VehicleModel> GetAllVehicles()
        {
            var vehicles = HttpService.Get<List<VehicleModel>>("http://localhost:8085/api/v1/vehicle/getAll");

            return vehicles ?? [];
        }

        public static VehicleResponse? AddVehicle(VehicleModel vehicle)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "car_showroom_id", vehicle.CarShowroomID }
            };

            var result = HttpService.Post<VehicleResponse>("http://localhost:8085/api/v1/vehicle/new", requestBody);

            return result;
        }

        public static VehicleResponse? EditVehicle(VehicleModel vehicle)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "car_showroom_id", vehicle.CarShowroomID }
            };

            var updatedVehicle = HttpService.Put<VehicleResponse>($"http://localhost:8085/api/v1/vehicle/update/{vehicle.Id}", requestBody);

            return updatedVehicle;
        }

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

    public record VehicleResponse(VehicleModel? Vehicle, string? Message, object? Error);

}
