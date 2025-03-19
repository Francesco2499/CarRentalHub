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

// RegisterUser registra un nuovo utente utilizzando il repository
func RegisterUser(user models.User) (*models.UserLoginResponseDTO, string, error) {
	// Verifica se l'email è già in uso
	if _, err := repositories.FindByEmail(user.Email); err == nil {
		return nil, "Email already in use", fmt.Errorf("email already in use")
	}

	// Verifica se lo username è già in uso
	if _, err := repositories.FindByUsername(user.Username); err == nil {
		return nil, "Username already in use", fmt.Errorf("Username already in use")
	}

	// Se il ruolo non è specificato, assegna il valore di default
	if user.Role == "" {
		user.Role = "customer"
	}

	// Salva il nuovo utente nel database
	if err := repositories.SaveUser(&user); err != nil {
		return nil, "Error", err
	}

	userDTO := models.ToUserLoginResponseDTO(&user)
	//return &user, "Registration done!", nil
	return userDTO, "Registration done!", nil
}

// AuthenticateUser esegue l'autenticazione dell'utente e restituisce un token JWT.
func AuthenticateUser(username, password string) (string, string, *models.UserLoginResponseDTO, error) {
	var user *models.User
	var err error

	if isEmail(username) {
		user, err = repositories.FindByEmail(username)
	} else {
		user, err = repositories.FindByUsername(username)
	}

	if err != nil {
		return "User not found", "", nil, fmt.Errorf("error: %v", err)
	}

	if user.Password != password {
		return "Invalid password", "", nil, fmt.Errorf("password error")
	}

	token, err := helpers.GenerateJWT(user.ID, user.Role)
	if err != nil {
		return "Error generating token", "", nil, fmt.Errorf("error generating token: %v", err)
	}

	userDTO := models.ToUserLoginResponseDTO(user)
	return "Login done", token, userDTO, nil
}

func GetUserIdByUsername(username string) (int, error) {

	user, err := repositories.FindByUsername(username)
	if err != nil {
		return 0, fmt.Errorf("user %s not found: %w", username, err)
	}
	return user.ID, nil
}
