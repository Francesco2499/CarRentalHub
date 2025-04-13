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

func GetAllBookings(userID int, isAdmin bool) ([]models.BookingDTO, error) {
	db := config.GetDB()
	var query string
	var rows *sql.Rows
	var err error

	if isAdmin {
		query = `
			SELECT 
				b.id, b.user_id, v.id, v.model, b.start_date, b.end_date, b.created_at, b.updated_at
			FROM bookings b
			JOIN vehicles v ON b.vehicle_id = v.id`
		rows, err = db.Query(query)
	} else {
		query = `
			SELECT 
				b.id, b.user_id, v.id, v.model, b.start_date, b.end_date, b.created_at, b.updated_at
			FROM bookings b
			JOIN vehicles v ON b.vehicle_id = v.id
			WHERE b.user_id = $1`
		rows, err = db.Query(query, userID)
	}

	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var bookings []models.BookingDTO
	for rows.Next() {
		var booking models.BookingDTO

		if err := rows.Scan(&booking.ID, &userID, &booking.VehicleID, &booking.VehicleModel, &booking.StartDate, &booking.EndDate, &booking.CreatedAt, &booking.UpdatedAt); err != nil {
			return nil, fmt.Errorf("data scan error: %w", err)
		}

		var username string
		err = db.QueryRow(`SELECT username FROM users WHERE id = $1`, userID).Scan(&username)
		if err != nil {
			return nil, fmt.Errorf("failed to fetch user details: %w", err)
		}
		booking.Username = username

		var priceForDay float64
		err = db.QueryRow(`SELECT price FROM vehicles WHERE id = $1`, booking.VehicleID).Scan(&priceForDay)
		if err != nil {
			return nil, fmt.Errorf("failed to fetch vehicle details: %w", err)
		}

		days := int(booking.EndDate.Truncate(24*time.Hour).Sub(booking.StartDate.Truncate(24*time.Hour)).Hours()/24) + 1
		if days < 1 {
			days = 1 
		}
		booking.TotalPrice = priceForDay * float64(days)

		bookings = append(bookings, booking)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return bookings, nil
}

func IsVehicleAvailable(vehicleID int, bookingID int, startDate, endDate time.Time) (bool, error) {
	db := config.GetDB()
	query := `SELECT COUNT(*) FROM bookings WHERE vehicle_id = $1 
		AND id != $2 
		AND	(start_date, end_date) OVERLAPS ($3::timestamp, $4::timestamp)`

	var count int
	err := db.QueryRow(query, vehicleID, bookingID, startDate, endDate).Scan(&count)
	if err != nil {
		return false, fmt.Errorf("error checking vehicle availability: %w", err)
	}
	return count == 0, nil
}

func CreateBooking(booking *models.Booking) (*models.BookingDTO, error) {
	db := config.GetDB()

	tx, err := db.Begin()
	if err != nil {
		return nil, fmt.Errorf("failed to start transaction: %w", err)
	}

	query := `
		INSERT INTO bookings (user_id, vehicle_id, start_date, end_date) 
		SELECT $1, $2, $3, $4
		WHERE NOT EXISTS (
			SELECT 1 FROM bookings 
			WHERE vehicle_id = $2 AND (start_date, end_date) OVERLAPS ($3, $4)
		)
		RETURNING id, user_id, vehicle_id, start_date, end_date, created_at, updated_at`

	var newBooking models.BookingDTO
	err = tx.QueryRow(query, booking.UserID, booking.VehicleID, booking.StartDate, booking.EndDate).
		Scan(&newBooking.ID, &booking.UserID, &newBooking.VehicleID, &newBooking.StartDate, &newBooking.EndDate, &newBooking.CreatedAt, &newBooking.UpdatedAt)

	if err != nil {
		tx.Rollback()
		if errors.Is(err, sql.ErrNoRows) {
			return nil, fmt.Errorf("vehicle is no longer available, booking rejected")
		}
		return nil, fmt.Errorf("booking entry error: %w", err)
	}

	var priceForDay float64
	err = db.QueryRow(`SELECT model, price FROM vehicles WHERE id = $1`, booking.VehicleID).Scan(&newBooking.VehicleModel, &priceForDay)
	if err != nil {
		tx.Rollback()
		return nil, fmt.Errorf("failed to fetch vehicle details: %w", err)
	}

	days := int(newBooking.EndDate.Truncate(24*time.Hour).Sub(newBooking.StartDate.Truncate(24*time.Hour)).Hours()/24) + 1
	if days < 1 {
		days = 1 
	}
	newBooking.TotalPrice = priceForDay * float64(days)

	var username string
	err = db.QueryRow(`SELECT username FROM users WHERE id = $1`, booking.UserID).Scan(&username)
	if err != nil {
		tx.Rollback()
		return nil, fmt.Errorf("failed to fetch user details: %w", err)
	}
	newBooking.Username = username

	err = tx.Commit()
	if err != nil {
		return nil, fmt.Errorf("failed to commit transaction: %w", err)
	}

	log.Printf("Booking transaction completed successfully")
	return &newBooking, nil
}

func UpdateBooking(booking *models.Booking) error {
	db := config.GetDB()

	tx, err := db.Begin()
	if err != nil {
		return fmt.Errorf("failed to start start transaction: %w", err)
	}

	query := `UPDATE bookings SET vehicle_id = $1, start_date = $2, end_date = $3 WHERE id = $4`
	_, err = tx.Exec(query, booking.VehicleID, booking.StartDate, booking.EndDate, booking.ID)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("booking update error: %w", err)
	}

	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}

	return nil
}

func DeleteBooking(id int) error {
	db := config.GetDB()

	tx, err := db.Begin()
	if err != nil {
		return fmt.Errorf("failed to start start transaction: %w", err)
	}

	query := `DELETE FROM bookings WHERE id = $1`
	result, err := tx.Exec(query, id)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("booking delete error: %w", err)
	}
	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		tx.Rollback()
		return errors.New("booking not found")
	}

	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}

	return nil
}
