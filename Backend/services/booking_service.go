package services

import (
	"Backend/models"
	"Backend/repositories"
	"errors"
	"fmt"
)

func GetAllBookings(userID int, isAdmin bool) ([]models.BookingWithVehicleDTO, error) {
	return repositories.GetAllBookings(userID, isAdmin)
}

/*func GetBookingsByUser(userID int) ([]models.BookingWithVehicleDTO, error) {
	return repositories.GetBookingsByUser(userID)
}*/

/*func GetBookingById(id int, userID int, isAdmin bool) (*models.BookingWithVehicleDTO, error) {
	return repositories.GetBookingById(id, userID, isAdmin)
}*/

func CreateBooking(booking *models.Booking) error {
	return repositories.CreateBooking(booking)
}

func UpdateBooking(booking *models.Booking) error {

	available, err := repositories.IsVehicleAvailable(booking.VehicleID, booking.ID, booking.StartDate, booking.EndDate)
	if err != nil {
		return fmt.Errorf("Error checking vehicle availability: %w", err)
	}
	if !available {
		return errors.New("Vehicle is not available for the selected dates")
	}

	return repositories.UpdateBooking(booking)
}

func DeleteBooking(id int) error {
	return repositories.DeleteBooking(id)
}
