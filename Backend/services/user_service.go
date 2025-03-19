package services

import (
	"Backend/models"
	"Backend/repositories"
)

func UpdateUser(userID int, updatedUser *models.User) error {
	return repositories.UpdateUserByID(userID, updatedUser)
}

func DeleteUser(userID int) error {
	return repositories.DeleteUserByID(userID)
}
