package repositories

import (
	"Backend/config"
	"Backend/models"

	"golang.org/x/crypto/bcrypt"
)

// FindByEmail cerca un utente per email
func FindByEmail(email string) (*models.User, error) {
	db := config.GetDB()
	var user models.User
	err := db.QueryRow(`SELECT id, username, email, password, role FROM users WHERE email = $1`, email).
		Scan(&user.ID, &user.Username, &user.Email, &user.Password, &user.Role)
	if err != nil {
		return nil, err
	}
	return &user, nil
}

// FindByUsername cerca un utente per username
func FindByUsername(username string) (*models.User, error) {
	db := config.GetDB()
	var user models.User
	err := db.QueryRow(`SELECT id, username, email, password, role FROM users WHERE username = $1`, username).
		Scan(&user.ID, &user.Username, &user.Email, &user.Password, &user.Role)
	if err != nil {
		return nil, err
	}
	return &user, nil
}

// SaveUser salva un nuovo utente nel database
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

	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(user.Password), bcrypt.DefaultCost)
	if err != nil {
		return err
	}
	user.Password = string(hashedPassword)

	query := `INSERT INTO users (username, email, password, role) VALUES ($1, $2, $3, $4) RETURNING id`
	err = db.QueryRow(query, user.Username, user.Email, user.Password, user.Role).Scan(&user.ID)
	if err != nil {
		return err
	}

	return nil
}
