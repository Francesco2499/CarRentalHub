using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Frontend.Models;

public record ShowroomModel(
    int Id,
    string Name,
    string Location, 
    float Latitude,
    float Longitude,
    List<VehicleModel> Vehicles
);

