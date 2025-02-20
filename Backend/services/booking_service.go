package services

import (
	"Backend/models"
	"Backend/repositories"
	"errors"
	"fmt"
)

func GetAllBookings(userID int, isAdmin bool) ([]models.Booking, error) {
	return repositories.GetAllBookings(userID, isAdmin)
}

func GetBookingsByUser(userID int) ([]models.Booking, error) {
	return repositories.GetBookingsByUser(userID)
}

func GetBookingById(id int, userID int, isAdmin bool) (*models.Booking, error) {
	return repositories.GetBookingById(id, userID, isAdmin)
}

func CreateBooking(booking *models.Booking) error {
	return repositories.CreateBooking(booking)
}

func UpdateBooking(booking *models.Booking) error {
	// Recuperiamo il veicolo attuale della prenotazione
	currentVehicleID, err := repositories.GetVehicleIdFromBooking(booking.ID)
	if err != nil {
		return fmt.Errorf("Error retrieving current booking: %w", err)
	}

	// Se il veicolo è cambiato, controlliamo la disponibilità
	if booking.VehicleID != currentVehicleID {
		available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.ID, booking.StartDate, booking.EndDate)
		if err != nil {
			return fmt.Errorf("Error checking vehicle availability: %w", err)
		}
		if !available {
			return errors.New("Vehicle is not available for the selected dates")
		}
	}

	return repositories.UpdateBooking(booking)
}

func DeleteBooking(id int) error {
	return repositories.DeleteBooking(id)
}
