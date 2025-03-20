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

	// Verifica password attuale
	if req.Password != "" && req.NuovaPassword != "" {
		if user.Password != req.Password {
			return nil, "Password errata!", fmt.Errorf("")
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
		return nil, "Errore nella modifica dell'utente", fmt.Errorf("")
	}

	return models.ToUserLoginResponseDTO(user), "Modifica effettuata correttamente!", nil
}

func DeleteUser(userID int) error {
	return repositories.DeleteUserByID(userID)
}
