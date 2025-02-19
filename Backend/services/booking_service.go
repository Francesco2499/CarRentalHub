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
	// Controllo se il veicolo è disponibile nel range di date
	/*available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.StartDate, booking.EndDate)
	if err != nil {
		return fmt.Errorf("error checking vehicle availability: %w", err)
	}
	if !available {
		return errors.New("vehicle is not available for the selected dates")
	}*/
	return repositories.CreateBooking(booking)
}

func UpdateBooking(booking *models.Booking) error {
	// Controllo se il veicolo è disponibile nel nuovo range di date
	available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.StartDate, booking.EndDate)
	if err != nil {
		return fmt.Errorf("error checking vehicle availability: %w", err)
	}
	if !available {
		return errors.New("vehicle is not available for the selected dates")
	}
	return repositories.UpdateBooking(booking)
}

func DeleteBooking(id int) error {
	return repositories.DeleteBooking(id)
}
