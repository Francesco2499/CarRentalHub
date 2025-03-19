package repositories

import (
	"Backend/config"
	"Backend/models"

	"golang.org/x/crypto/bcrypt"
)

func UpdateUserByID(userID int, updatedUser *models.User) error {
	db := config.GetDB()

	// Se la password è valorizzata, va criptata e aggiornata
	if updatedUser.Password != "" {
		hashedPassword, err := bcrypt.GenerateFromPassword([]byte(updatedUser.Password), bcrypt.DefaultCost)
		if err != nil {
			return err
		}
		updatedUser.Password = string(hashedPassword)

		query := `UPDATE users SET username = $1, email = $2, region = $3, password = $4 WHERE id = $5`
		_, err = db.Exec(query, updatedUser.Username, updatedUser.Email, updatedUser.Region, updatedUser.Password, userID)
		return err
	}

	query := `UPDATE users SET username = $1, email = $2, region = $3 WHERE id = $4`
	_, err := db.Exec(query, updatedUser.Username, updatedUser.Email, updatedUser.Region, userID)
	return err
}

func DeleteUserByID(userID int) error {
	db := config.GetDB()
	query := `DELETE FROM users WHERE id = $1`
	_, err := db.Exec(query, userID)
	return err
}
