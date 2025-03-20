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
		return nil, "Email già in uso", fmt.Errorf("")
	}

	// Verifica se lo username è già in uso
	if _, err := repositories.FindByUsername(user.Username); err == nil {
		return nil, "Username già in uso", fmt.Errorf("")
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
	return userDTO, "Registrazione effettuata!", nil
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
