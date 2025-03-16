package services

import (
	"Backend/cache"
	"Backend/models"
	"Backend/repositories"
	"fmt"
	"log"
	"time"
)

// Cache per le prenotazioni (TTL di 30 secondi)
//var vehicleCache = cache.NewCache(30 * time.Second)

func CreateVehicle(vehicle *models.Vehicle) error {
	err := repositories.CreateVehicle(vehicle)
	if err != nil {
		return err
	}

	//invalida la cache
	cache.VehicleCache.Invalidate()

	return nil
}

func UpdateVehicle(vehicle *models.Vehicle) error {
	err := repositories.UpdateVehicle(vehicle)
	if err != nil {
		return err
	}

	//invalida la cache
	cache.VehicleCache.Invalidate()

	return nil
}

func DeleteVehicle(id int) error {
	err := repositories.DeleteVehicle(id)
	if err != nil {
		return err
	}

	//invalida la cache
	cache.VehicleCache.Invalidate()

	return nil
}

func GetAllVehicles() ([]models.Vehicle, error) {
	cacheKey := "all_vehicles"

	if cachedData, found := cache.VehicleCache.Get(cacheKey); found {
		log.Println("All vehicles found in cache")
		return cachedData.([]models.Vehicle), nil
	}

	vehicles, err := repositories.GetAllVehicles()
	if err != nil {
		return nil, err
	}

	cache.VehicleCache.Set(cacheKey, vehicles)
	return vehicles, nil

}

func GetVehicleById(id int) (*models.Vehicle, error) {

	cacheKey := fmt.Sprintf("vehicle_id_%d", id)

	if cachedData, found := cache.VehicleCache.Get(cacheKey); found {
		log.Printf("Vehicle with id: %d found in cache", id)
		return cachedData.(*models.Vehicle), nil
	}

	vehicle, err := repositories.GetVehicleById(id)
	if err != nil {
		return nil, err
	}

	// Salva in cache
	cache.VehicleCache.Set(cacheKey, vehicle)
	return vehicle, nil
}

func GetAvailableVehicles(startDate, endDate time.Time) ([]models.Vehicle, error) {
	cacheKey := fmt.Sprintf("available_vehicles_%s_%s", startDate.Format("2006-01-02"), endDate.Format("2006-01-02"))

	if cachedData, found := cache.VehicleCache.Get(cacheKey); found {
		log.Println("Available vehicles found in cache")
		return cachedData.([]models.Vehicle), nil
	}

	vehicles, err := repositories.GetAvailableVehicles(startDate, endDate)
	if err != nil {
		return nil, err
	}

	// Salva in cache
	cache.VehicleCache.Set(cacheKey, vehicles)
	return vehicles, nil
}
