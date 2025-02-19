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
	query := `SELECT id, model, category, price, available, location FROM vehicles`
	rows, err := db.Query(query)
	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var vehicles []models.Vehicle
	for rows.Next() {
		var vehicle models.Vehicle
		if err := rows.Scan(&vehicle.ID, &vehicle.Model, &vehicle.Category, &vehicle.Price, &vehicle.Available, &vehicle.Location); err != nil {
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
	query := `SELECT id, model, category, price, available, location FROM vehicles WHERE id = $1`
	var vehicle models.Vehicle
	err := db.QueryRow(query, id).Scan(
		&vehicle.ID,
		&vehicle.Model,
		&vehicle.Category,
		&vehicle.Price,
		&vehicle.Available,
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
	query := `INSERT INTO vehicles (model, category, price, available, location) VALUES ($1, $2, $3, $4, $5)`
	_, err := db.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.Available, vehicle.Location)
	if err != nil {
		return fmt.Errorf("vehicle entry error: %w", err)
	}
	return nil
}

func UpdateVehicle(vehicle *models.Vehicle) error {
	db := config.GetDB()
	query := `UPDATE vehicles SET model = $1, category = $2, price = $3, available = $4, location = $5 WHERE id = $6`
	_, err := db.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.Available, vehicle.Location, vehicle.ID)
	if err != nil {
		return fmt.Errorf("vehicle update error: %w", err)
	}
	return nil
}

func DeleteVehicle(id int) error {
	db := config.GetDB()
	query := `DELETE FROM vehicles WHERE id = $1`
	result, err := db.Exec(query, id)
	if err != nil {
		return fmt.Errorf("vehicle delete error: %w", err)
	}
	rowsAffected, _ := result.RowsAffected()
	if rowsAffected == 0 {
		return errors.New("vehicle not found")
	}
	return nil
}

func GetAvailableVehicles(startDate, endDate time.Time) ([]models.Vehicle, error) {
	db := config.GetDB()
	query := `SELECT id, model, category, price, available, location 
		FROM vehicles 
		WHERE id NOT IN (
			SELECT vehicle_id FROM bookings 
			WHERE (start_date, end_date) OVERLAPS ($1, $2)
		)`

	rows, err := db.Query(query, startDate, endDate)
	if err != nil {
		return nil, fmt.Errorf("error retrieving available vehicles: %w", err)
	}
	defer rows.Close()

	var vehicles []models.Vehicle
	for rows.Next() {
		var vehicle models.Vehicle
		if err := rows.Scan(&vehicle.ID, &vehicle.Model, &vehicle.Category, &vehicle.Price, &vehicle.Available, &vehicle.Location); err != nil {
			return nil, fmt.Errorf("error scanning vehicle data: %w", err)
		}
		vehicles = append(vehicles, vehicle)
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return vehicles, nil
}
