package repositories

import (
	"Backend/config"
	"Backend/models"
	"database/sql"
	"errors"
	"fmt"
	"time"
)

func GetAllVehicles() ([]models.VehicleDTO, error) {
	db := config.GetDB()
	query := `
		SELECT v.id, v.model, v.category, v.price, v.car_showroom_id, cs.name
		FROM vehicles v
		JOIN car_showrooms cs ON v.car_showroom_id = cs.id;
	`
	rows, err := db.Query(query)
	if err != nil {
		return nil, fmt.Errorf("query error: %w", err)
	}
	defer rows.Close()

	var vehicles []models.VehicleDTO
	for rows.Next() {
		var vehicle models.VehicleDTO
		var showroomName string

		if err := rows.Scan(&vehicle.ID, &vehicle.Model, &vehicle.Category, &vehicle.Price, &vehicle.CarShowroomID, &showroomName); err != nil {
			return nil, fmt.Errorf("data scan error: %w", err)
		}
		vehicle.CarShowroomName = showroomName
		vehicles = append(vehicles, vehicle)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("error after iteration: %w", err)
	}
	return vehicles, nil
}

func GetVehicleById(id int) (*models.Vehicle, error) {
	db := config.GetDB()
	query := `SELECT id, model, category, price,  car_showroom_id FROM vehicles WHERE id = $1`
	var vehicle models.Vehicle
	err := db.QueryRow(query, id).Scan(
		&vehicle.ID,
		&vehicle.Model,
		&vehicle.Category,
		&vehicle.Price,
		&vehicle.CarShowroomID,
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

	query := `INSERT INTO vehicles (model, category, price, car_showroom_id) VALUES ($1, $2, $3, $4)`
	_, err = tx.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.CarShowroomID)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("vehicle entry error: %w", err)
	}

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

	query := `UPDATE vehicles SET model = $1, category = $2, price = $3, car_showroom_id = $4 WHERE id = $5`
	_, err = tx.Exec(query, vehicle.Model, vehicle.Category, vehicle.Price, vehicle.CarShowroomID, vehicle.ID)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("vehicle update error: %w", err)
	}

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

	err = tx.Commit()
	if err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}
	return nil
}

func GetAvailableVehicles(startDate, endDate *time.Time) ([]models.CarShowroomVehiclesDTO, error) {

	db := config.GetDB()

	var rows *sql.Rows
	var err error

	if startDate != nil && endDate != nil {
		query := `SELECT cs.id, cs.name, cs.location, cs.latitude, cs.longitude,
						v.id, v.model, v.category, v.price, v.car_showroom_id
					FROM car_showrooms cs
					JOIN vehicles v ON cs.id = v.car_showroom_id
					WHERE v.id NOT IN (
							SELECT vehicle_id FROM bookings 
							WHERE (start_date, end_date) OVERLAPS ($1, $2)
						)
					ORDER BY cs.id;`
		rows, err = db.Query(query, *startDate, *endDate)
	} else {
		query := `SELECT cs.id, cs.name, cs.location, cs.latitude, cs.longitude,
						v.id, v.model, v.category, v.price, v.car_showroom_id
					FROM car_showrooms cs
					JOIN vehicles v ON cs.id = v.car_showroom_id
					ORDER BY cs.id;`
		rows, err = db.Query(query)
	}

	if err != nil {
		return nil, fmt.Errorf("error retrieving vehicles: %w", err)
	}
	defer rows.Close()

	showroomMap := make(map[int]*models.CarShowroomVehiclesDTO)

	for rows.Next() {
		var (
			csID        int
			csName      string
			csLocation  string
			csLat       float64
			csLong      float64
			vID         int
			vModel      string
			vCategory   string
			vPrice      float64
			vShowroomID int
		)

		if err := rows.Scan(&csID, &csName, &csLocation, &csLat, &csLong, &vID, &vModel, &vCategory, &vPrice, &vShowroomID); err != nil {
			return nil, fmt.Errorf("error scanning row: %w", err)
		}

		if showroomMap[csID] == nil {
			showroomMap[csID] = &models.CarShowroomVehiclesDTO{
				ID:        csID,
				Name:      csName,
				Location:  csLocation,
				Latitude:  csLat,
				Longitude: csLong,
				Vehicles:  []models.Vehicle{},
			}
		}

		showroomMap[csID].Vehicles = append(showroomMap[csID].Vehicles, models.Vehicle{
			ID:            vID,
			Model:         vModel,
			Category:      vCategory,
			Price:         vPrice,
			CarShowroomID: vShowroomID,
		})
	}

	result := make([]models.CarShowroomVehiclesDTO, 0, len(showroomMap))
	for _, showroom := range showroomMap {
		result = append(result, *showroom)
	}

	return result, nil
}
