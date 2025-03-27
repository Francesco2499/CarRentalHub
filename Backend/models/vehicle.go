package models

type Vehicle struct {
	ID        int     `json:"id"`
	Model     string  `json:"model"`
	Category  string  `json:"category"`
	Price     float64 `json:"price"`
	Location  string  `json:"location"`
	Latitude  float64 `json:"latitude"`
	Longitude float64 `json:"longitude"`
}
