package models

import "time"

type BookingDTO struct {
	ID           int       `json:"id"`
	Username     string    `json:"username"`
	VehicleID    int       `json:"vehicle_id"`
	VehicleModel string    `json:"vehicle_model"`
	StartDate    time.Time `json:"start_date"`
	EndDate      time.Time `json:"end_date"`
	CreatedAt    time.Time `json:"created_at"`
	UpdatedAt    time.Time `json:"updated_at"`
	TotalPrice   float64   `json:"total_price"`
}
