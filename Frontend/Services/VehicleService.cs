using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Models;

namespace Frontend.Services
{
    public static class VehicleService
    {
        public static async Task<List<ShowroomModel>> GetAvailableShowrooms(DateTime? startDate, DateTime? endDate)
        {
            string url = "http://localhost:8085/api/v1/vehicle/getAllAvailable";

            if (startDate != null && endDate != null)
            {
                url += $"?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
            }

            var showrooms = await HttpService.GetAsync<List<ShowroomModel>>(url);
            return showrooms ?? [];
        }

        public static async Task<List<VehicleModel>> GetAllVehicles()
        {
            var vehicles = await HttpService.GetAsync<List<VehicleModel>>("http://localhost:8085/api/v1/vehicle/getAll");
            return vehicles ?? [];
        }

        public static async Task<VehicleResponse?> AddVehicle(VehicleModel vehicle)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "car_showroom_id", vehicle.CarShowroomID }
            };

            return await HttpService.PostAsync<VehicleResponse>("http://localhost:8085/api/v1/vehicle/new", requestBody);
        }

        public static async Task<VehicleResponse?> EditVehicle(VehicleModel vehicle)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", vehicle.Model },
                { "category", vehicle.Category },
                { "price", vehicle.Price },
                { "car_showroom_id", vehicle.CarShowroomID }
            };

            return await HttpService.PutAsync<VehicleResponse>($"http://localhost:8085/api/v1/vehicle/update/{vehicle.Id}", requestBody);
        }

        public static async Task DeleteVehicle(int vehicleId)
        {
            try
            {
                await HttpService.DeleteAsync<dynamic>($"http://localhost:8085/api/v1/vehicle/delete/{vehicleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting vehicle {vehicleId}: {ex.Message}");
            }
        }
    }

    public record VehicleResponse(VehicleModel? Vehicle, string? Message, object? Error);
}
