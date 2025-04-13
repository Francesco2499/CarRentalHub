package services

import (
	"Backend/models"
	"Backend/repositories"
	"fmt"
)

func UpdateUser(userID int, req *models.UserUpdateRequestDTO) (*models.UserLoginResponseDTO, string, error) {
	user, err := repositories.FindUserByID(userID)
	if err != nil {
		return nil, "Utente non trovato!", fmt.Errorf("")
	}

	if req.Password != "" && req.NewPassword != "" {
		if user.Password != req.Password {
			return nil, "Password errata!", fmt.Errorf("")
		}
		user.Password = req.NewPassword
	}

	user.Username = req.Username
	user.Email = req.Email
	user.Region = req.Region

	err = repositories.UpdateFullUser(user)
	if err != nil {
		return nil, "Errore nella modifica dell'utente", fmt.Errorf("")
	}

	return models.ToUserLoginResponseDTO(user), "Modifica effettuata correttamente!", nil
}

func DeleteUser(userID int) error {
	return repositories.DeleteUserByID(userID)
}
