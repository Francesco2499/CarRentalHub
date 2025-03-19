package repositories

import (
	"Backend/config"
	"Backend/models"
)

func FindUserByID(userID int) (*models.User, error) {
	db := config.GetDB()
	var user models.User
	err := db.QueryRow(`SELECT id, username, email, password, region, role FROM users WHERE id = $1`, userID).
		Scan(&user.ID, &user.Username, &user.Email, &user.Password, &user.Region, &user.Role)
	if err != nil {
		return nil, err
	}
	return &user, nil
}

func UpdateFullUser(user *models.User) error {
	db := config.GetDB()
	query := `UPDATE users SET username = $1, email = $2, password = $3, region = $4 WHERE id = $5`
	_, err := db.Exec(query, user.Username, user.Email, user.Password, user.Region, user.ID)
	return err
}

func DeleteUserByID(userID int) error {
	db := config.GetDB()
	query := `DELETE FROM users WHERE id = $1`
	_, err := db.Exec(query, userID)
	return err
}
