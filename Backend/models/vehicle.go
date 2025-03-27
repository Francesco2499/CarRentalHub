package models

type Vehicle struct {
	ID            int     `json:"id"`
	Model         string  `json:"model"`
	Category      string  `json:"category"`
	Price         float64 `json:"price"`
	CarShowroomID int     `json:"car_showroom_id"`
}
