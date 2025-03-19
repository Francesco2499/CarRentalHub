package services

import (
	"Backend/models"
	"Backend/repositories"
	"fmt"
)

func UpdateUser(userID int, req *models.UserUpdateRequestDTO) (*models.UserLoginResponseDTO, error) {
	user, err := repositories.FindUserByID(userID)
	if err != nil {
		return nil, fmt.Errorf("user not found")
	}

	// Verifica password attuale
	if req.Password != "" && req.NuovaPassword != "" {
		if user.Password != req.Password {
			return nil, fmt.Errorf("invalid current password")
		}
		// Password corretta, aggiorna con nuova hashata
		user.Password = req.NuovaPassword
	}

	// Aggiorna i campi modificabili
	user.Username = req.Username
	user.Email = req.Email
	user.Region = req.Region

	// Salva nel DB
	err = repositories.UpdateFullUser(user)
	if err != nil {
		return nil, fmt.Errorf("failed to update user")
	}

	return models.ToUserLoginResponseDTO(user), nil
}

func DeleteUser(userID int) error {
	return repositories.DeleteUserByID(userID)
}
