package repositories

import (
	"Backend/config"
	"Backend/models"
	"database/sql"
	"errors"
	"fmt"

	//"log"
	"time"
)

func GetAllVehicles() ([]models.Vehicle, error) {
	db := config.GetDB()
	query := `SELECT id, model, category, price, location FROM vehicles`
	rows, err := db.Query(query)
	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var vehicles []models.Vehicle
	for rows.Next() {
		var vehicle models.Vehicle
		if err := rows.Scan(&vehicle.ID, &vehicle.Model, &vehicle.Category, &vehicle.Price, &vehicle.Location); err != nil {
			return nil, fmt.Errorf("data scan error: %w", err)
		}
		vehicles = append(vehicles, vehicle)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return vehicles, nil
}

func GetVehicleById(id int) (*models.Vehicle, error) {
	db := config.GetDB()
	query := `SELECT id, model, category, price, location FROM vehicles WHERE id = $1`
	var vehicle models.Vehicle
	err := db.QueryRow(query, id).Scan(
		&vehicle.ID,
		&vehicle.Model,
		&vehicle.Category,
		&vehicle.Price,
		&vehicle.Location,
	)
	if err != nil {
		if errors.Is(err, sql.ErrNoRows) {
			return nil, fmt.Errorf("vehicle with ID %d not found", id)
		}
		return nil, fmt.Errorf("database query error: %w", err)
	}
	return &vehicle, nil
}

func CreateVehicle(vehicle *models.Vehicle) error {
	db := config.GetDB()

	tx, err := db.Begin()
	if err != nil {
		return fmt.Errorf("failed to start start transaction: %w", err)
	}

	query := `INSERT INTO vehicles (model, category, price, location) VALUES ($1, $2, $3, $4)`
	_, err = tx.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.Location)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("vehicle entry error: %w", err)
	}

	// Conferma la transazione
	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}
	return nil
}

func UpdateVehicle(vehicle *models.Vehicle) error {
	db := config.GetDB()
	tx, err := db.Begin()
	if err != nil {
		return fmt.Errorf("failed to start start transaction: %w", err)
	}

	query := `UPDATE vehicles SET model = $1, category = $2, price = $3, location = $4 WHERE id = $5`
	_, err = tx.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.Location, vehicle.ID)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("vehicle update error: %w", err)
	}

	// Conferma la transazione
	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}
	return nil
}

func DeleteVehicle(id int) error {
	db := config.GetDB()
	tx, err := db.Begin()
	if err != nil {
		return fmt.Errorf("failed to start start transaction: %w", err)
	}

	query := `DELETE FROM vehicles WHERE id = $1`
	result, err := tx.Exec(query, id)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("vehicle delete error: %w", err)
	}
	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		tx.Rollback()
		return errors.New("vehicle not found")
	}

	// Conferma la transazione
	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}
	return nil
}

func GetAvailableVehicles(startDate, endDate time.Time, location string) ([]models.Vehicle, error) {
	db := config.GetDB()
	query := `SELECT id, model, category, price, location 
		FROM vehicles 
		WHERE id NOT IN (
			SELECT vehicle_id FROM bookings 
			WHERE (start_date, end_date) OVERLAPS ($1, $2)
		) AND LOWER(location) = LOWER($3)`
	// overlaps = start_date <= param_end AND end_date >= param_start

	rows, err := db.Query(query, startDate, endDate, location)
	if err != nil {
		return nil, fmt.Errorf("error retrieving available vehicles: %w", err)
	}
	defer rows.Close()

	var vehicles []models.Vehicle
	for rows.Next() {
		var vehicle models.Vehicle
		if err := rows.Scan(&vehicle.ID, &vehicle.Model, &vehicle.Category, &vehicle.Price, &vehicle.Location); err != nil {
			return nil, fmt.Errorf("error scanning vehicle data: %w", err)
		}
		vehicles = append(vehicles, vehicle)
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return vehicles, nil
}
