package models

type UserUpdateRequestDTO struct {
	Username    string `json:"username"`
	Email       string `json:"email"`
	Region      string `json:"region"`
	Password    string `json:"password"`     // password attuale da confermare
	NewPassword string `json:"new_password"` // nuova password da impostare (opzionale)
}
