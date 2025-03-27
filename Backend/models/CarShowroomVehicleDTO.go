package models

type CarShowroomVehiclesDTO struct {
	ID        int      `json:"id"`
	Name      string   `json:"name"`
	Location  string   `json:"location"`
	Latitude  float64  `json:"latitude"`
	Longitude float64  `json:"longitude"`
	Vehicles  []Vehicle `json:"vehicles"`
}
