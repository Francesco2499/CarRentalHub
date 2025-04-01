using System.Collections.Generic;

namespace Frontend.Models;

public record ShowroomModel(
    int Id,
    string Name,
    string Location, 
    float Latitude,
    float Longitude,
    List<VehicleModel> Vehicles
);

