using System;

namespace Frontend.Models;

public record VehicleModel(
    int Id,
    string Model,
    string Category,
    decimal Price,
    bool Available,
    string Location,
    double Lat,  // Aggiunto campo per latitudine
    double Lon 
);

