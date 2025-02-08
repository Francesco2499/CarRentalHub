package models

import "time"

type Booking struct {
	ID        int       `json:"id"`
	UserID    int       `json:"user_id"`
	VehicleID int       `json:"vehicle_id"`
	Date      time.Time `json:"date"`
}
