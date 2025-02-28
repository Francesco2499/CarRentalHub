package models

import "time"

type BookingWithVehicleDTO struct {
	ID           int       `json:"id"`
	UserID       int       `json:"user_id"`
	VehicleModel string    `json:"vehicle_model"`
	StartDate    time.Time `json:"start_date"`
	EndDate      time.Time `json:"end_date"`
	CreatedAt    time.Time `json:"created_at"`
	UpdatedAt    time.Time `json:"updated_at"`
}
