package repositories

import (
	"Backend/config"
	"Backend/models"
	"database/sql"
	"errors"
	"fmt"
	"log"
	"time"
)

func GetAllBookings(userID int, isAdmin bool) ([]models.Booking, error) {
	db := config.GetDB()
	var query string
	var rows *sql.Rows
	var err error

	if isAdmin {
		query = `SELECT id, user_id, vehicle_id, start_date, end_date, created_at, updated_at FROM bookings`
		log.Printf("Executing query: %s", query)
		rows, err = db.Query(query)
	} else {
		query = `SELECT id, user_id, vehicle_id, start_date, end_date, created_at, updated_at FROM bookings WHERE user_id = $1`
		log.Printf("Executing query: %s with userID: %d", query, userID)
		rows, err = db.Query(query, userID)
	}

	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var bookings []models.Booking
	for rows.Next() {
		var booking models.Booking
		if err := rows.Scan(&booking.ID, &booking.UserID, &booking.VehicleID, &booking.StartDate, &booking.EndDate, &booking.CreatedAt, &booking.UpdatedAt); err != nil {
			return nil, fmt.Errorf("data scan error: %w", err)
		}
		bookings = append(bookings, booking)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return bookings, nil
}

func GetBookingsByUser(userID int) ([]models.Booking, error) {
	db := config.GetDB()
	query := `SELECT id, user_id, vehicle_id, start_date, end_date, created_at, updated_at FROM bookings WHERE user_id = $1`
	rows, err := db.Query(query, userID)
	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var bookings []models.Booking
	for rows.Next() {
		var booking models.Booking
		if err := rows.Scan(&booking.ID, &booking.UserID, &booking.VehicleID, &booking.StartDate, &booking.EndDate, &booking.CreatedAt, &booking.UpdatedAt); err != nil {
			return nil, fmt.Errorf("data scan error: %w", err)
		}
		bookings = append(bookings, booking)
	}
	return bookings, nil
}

func GetBookingById(id int, userID int, isAdmin bool) (*models.Booking, error) {
	db := config.GetDB()
	var query string
	var booking models.Booking
	var err error

	if isAdmin {
		query = `SELECT id, user_id, vehicle_id, start_date, end_date, created_at, updated_at FROM bookings WHERE id = $1`
		err = db.QueryRow(query, id).Scan(
			&booking.ID, &booking.UserID, &booking.VehicleID, &booking.StartDate,
			&booking.EndDate, &booking.CreatedAt, &booking.UpdatedAt,
		)
	} else {
		query = `SELECT id, user_id, vehicle_id, start_date, end_date, created_at, updated_at FROM bookings WHERE id = $1 AND user_id = $2`
		err = db.QueryRow(query, id, userID).Scan(
			&booking.ID, &booking.UserID, &booking.VehicleID, &booking.StartDate, &booking.EndDate, &booking.CreatedAt, &booking.UpdatedAt)
	}

	if err != nil {
		if errors.Is(err, sql.ErrNoRows) {
			return nil, fmt.Errorf("booking with ID %d not found", id)
		}
		return nil, fmt.Errorf("database query error: %w", err)
	}
	return &booking, nil
}

func IsVehicleAvailable(vehicleID int, bookingID int, startDate, endDate time.Time) (bool, error) {
	db := config.GetDB()
	query := `SELECT COUNT(*) FROM bookings WHERE vehicle_id = $1 
		AND id != $2 
		AND	(start_date, end_date) OVERLAPS ($3::timestamp, $4::timestamp)`
	//AND (
	//(start_date <= $3 AND end_date >= $2) -- La nuova prenotazione inizia dentro un'altra
	//)

	var count int
	err := db.QueryRow(query, vehicleID, bookingID, startDate, endDate).Scan(&count)
	if err != nil {
		return false, fmt.Errorf("error checking vehicle availability: %w", err)
	}
	return count == 0, nil
}

func CreateBooking(booking *models.Booking) error {
	db := config.GetDB()
	query := `INSERT INTO bookings (user_id, vehicle_id, start_date, end_date) VALUES ($1, $2, $3, $4)`
	_, err := db.Exec(query, booking.UserID, booking.VehicleID, booking.StartDate, booking.EndDate)
	if err != nil {
		return fmt.Errorf("booking entry error: %w", err)
	}
	return nil
}

func UpdateBooking(booking *models.Booking) error {
	db := config.GetDB()
	query := `UPDATE bookings SET vehicle_id = $1, start_date = $2, end_date = $3 WHERE id = $4`
	_, err := db.Exec(query, booking.VehicleID, booking.StartDate, booking.EndDate, booking.ID)
	if err != nil {
		return fmt.Errorf("booking update error: %w", err)
	}
	return nil
}

func DeleteBooking(id int) error {
	db := config.GetDB()
	query := `DELETE FROM bookings WHERE id = $1`
	result, err := db.Exec(query, id)
	if err != nil {
		return fmt.Errorf("booking delete error: %w", err)
	}
	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		return errors.New("booking not found")
	}
	return nil
}

func GetVehicleIdFromBooking(bookingID int) (int, error) {
	db := config.GetDB()
	query := `SELECT vehicle_id FROM bookings WHERE id = $1`

	var vehicleID int
	err := db.QueryRow(query, bookingID).Scan(&vehicleID)
	if err != nil {
		if errors.Is(err, sql.ErrNoRows) {
			return 0, fmt.Errorf("booking with ID %d not found", bookingID)
		}
		return 0, fmt.Errorf("database query error: %w", err)
	}
	return vehicleID, nil
}
