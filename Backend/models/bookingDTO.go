package models

import "time"

/*type BookingDTO struct {
	ID           int       `json:"id"`
	UserID       int       `json:"user_id"`
	VehicleModel string    `json:"vehicle_model"`
	StartDate    time.Time `json:"start_date"`
	EndDate      time.Time `json:"end_date"`
	CreatedAt    time.Time `json:"created_at"`
	UpdatedAt    time.Time `json:"updated_at"`
	TotalPrice   float64   `json:"total_price"`
}*/

type BookingDTO struct {
	ID           int       `json:"id"`
	Username     string    `json:"username"`
	VehicleModel string    `json:"vehicle_model"`
	StartDate    time.Time `json:"start_date"`
	EndDate      time.Time `json:"end_date"`
	CreatedAt    time.Time `json:"created_at"`
	UpdatedAt    time.Time `json:"updated_at"`
	TotalPrice   float64   `json:"total_price"`
}

/*func ToBookingDTO(booking *Booking, username string, vehicleModel string, totalPrice float64) *BookingDTO {
	return &BookingDTO{
		ID:				booking.ID,
		Username:		username,
		VehicleModel: 	vehicleModel,
		StartDate: 		booking.StartDate,
		EndDate: 		booking.EndDate,
		CreatedAt: 		booking.CreatedAt,
		UpdatedAt: 		booking.UpdatedAt,
		TotalPrice: 	totalPrice,
	}
}*/
