using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Frontend.Models;

namespace Frontend.Services
{
    public class VehicleService
    {
        private readonly HttpService _httpService;

        public VehicleService()
        {
            _httpService = new HttpService();
        }

        public async Task<List<VehicleModel>?> GetVehiclesByDate(DateTime? startDate, DateTime? endDate)
        {
            string url = $"http://localhost:8085/api/v1/vehicle/getAllAvailable?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";

            return await _httpService.GetAsync<List<VehicleModel>>(url);
        }

        public async Task<List<VehicleModel>?> GetAllVehicles()
        {
            return await _httpService.GetAsync<List<VehicleModel>>("http://localhost:8085/api/v1/vehicle/getAll");
        }

        public async Task<VehicleModel> AddVehicle(VehicleModel vehicle)
        {
            var requestBody = new
            {
                model = vehicle.Model,
                category = vehicle.Category,
                price = vehicle.Price,
                location = "Napoli"
            };

            return await _httpService.PostAsync<VehicleModel>("http://localhost:8085/api/v1/vehicle/new", requestBody);
        }

        public async Task<VehicleModel> EditVehicle(VehicleModel vehicle)
        {
            var requestBody = new
            {
                model = vehicle.Model,
                category = vehicle.Category,
                price = vehicle.Price,
                location = "Napoli"
            };

            return await _httpService.PutAsync<VehicleModel>($"http://localhost:8085/api/v1/vehicle/update/{vehicle.Id}", requestBody);
        }

        public async Task<VehicleModel> DeleteVehicle(int vehicleId)
        {
            return await _httpService.DeleteAsync<VehicleModel>($"http://localhost:8085/api/v1/vehicle/delete/{vehicleId}");
        }
    }
}