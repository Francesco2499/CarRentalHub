package models

type UserUpdateRequestDTO struct {
	Username      string `json:"username"`
	Email         string `json:"email"`
	Region        string `json:"region"`
	Password      string `json:"password"`       // password attuale da confermare
	NuovaPassword string `json:"nuovaPassword"`  // nuova password da impostare (opzionale)
}