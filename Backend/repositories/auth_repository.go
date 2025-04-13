package repositories

import (
	"Backend/config"
	"Backend/models"
)

func FindByEmail(email string) (*models.User, error) {
	db := config.GetDB()
	var user models.User
	err := db.QueryRow(`SELECT id, username, email, password, region, role FROM users WHERE email = $1`, email).
		Scan(&user.ID, &user.Username, &user.Email, &user.Password, &user.Region, &user.Role)
	if err != nil {
		return nil, err
	}
	return &user, nil
}

func FindByUsername(username string) (*models.User, error) {
	db := config.GetDB()
	var user models.User
	err := db.QueryRow(`SELECT id, username, email, password, region, role FROM users WHERE username = $1`, username).
		Scan(&user.ID, &user.Username, &user.Email, &user.Password, &user.Region, &user.Role)
	if err != nil {
		return nil, err
	}
	return &user, nil
}

func SaveUser(user *models.User) error {
	db := config.GetDB()

	tx, err := db.Begin()
	if err != nil {
		return err
	}

	defer func() {
		if err != nil {
			tx.Rollback()
		}
	}()

	query := `INSERT INTO users (username, email, password, region, role) VALUES ($1, $2, $3, $4, $5) RETURNING id`
	err = db.QueryRow(query, user.Username, user.Email, user.Password, user.Region, user.Role).Scan(&user.ID)
	if err != nil {
		return err
	}

	return nil
}
