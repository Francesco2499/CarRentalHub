package services

import (
	"Backend/models"
	"Backend/repositories"
	"time"
)

func CreateVehicle(vehicle *models.Vehicle) error {
	return repositories.CreateVehicle(vehicle)
}

func UpdateVehicle(vehicle *models.Vehicle) error {
	return repositories.UpdateVehicle(vehicle)
}

func DeleteVehicle(id int) error {
	return repositories.DeleteVehicle(id)
}

func GetAllVehicles() ([]models.Vehicle, error) {
	return repositories.GetAllVehicles()
}

func GetVehicleById(id int) (*models.Vehicle, error) {
	return repositories.GetVehicleById(id)
}

func GetAvailableVehicles(startDate, endDate time.Time) ([]models.Vehicle, error) {
	return repositories.GetAvailableVehicles(startDate, endDate)
}
