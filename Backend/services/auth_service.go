package services

import (
	"Backend/helpers"
	"Backend/models"
	"Backend/repositories"
	"fmt"
	"regexp"
)

func isEmail(s string) bool {
	re := regexp.MustCompile(`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`)
	return re.MatchString(s)
}

func RegisterUser(user models.User) (*models.UserLoginResponseDTO, string, error) {
	if _, err := repositories.FindByEmail(user.Email); err == nil {
		return nil, "Email già in uso", fmt.Errorf("")
	}

	if _, err := repositories.FindByUsername(user.Username); err == nil {
		return nil, "Username già in uso", fmt.Errorf("")
	}

	if user.Role == "" {
		user.Role = "customer"
	}

	if err := repositories.SaveUser(&user); err != nil {
		return nil, "Error", err
	}

	userDTO := models.ToUserLoginResponseDTO(&user)
	return userDTO, "Registrazione effettuata!", nil
}

func AuthenticateUser(username, password string) (string, string, *models.UserLoginResponseDTO, error) {
	var user *models.User
	var err error

	if isEmail(username) {
		user, err = repositories.FindByEmail(username)
	} else {
		user, err = repositories.FindByUsername(username)
	}

	if err != nil {
		return "Utente non trovato", "", nil, fmt.Errorf("")
	}

	if user.Password != password {
		return "Password errata", "", nil, fmt.Errorf("")
	}

	token, err := helpers.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "Errore generazione token", "", nil, fmt.Errorf("")
	}

	userDTO := models.ToUserLoginResponseDTO(user)
	return "Login effettuato!", token, userDTO, nil
}

func GetUserIdByUsername(username string) (int, error) {

	user, err := repositories.FindByUsername(username)
	if err != nil {
		return 0, fmt.Errorf("")
	}
	return user.ID, nil
}
