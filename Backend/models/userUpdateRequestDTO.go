package models

type UserUpdateRequestDTO struct {
	Username    string `json:"username"`
	Email       string `json:"email"`
	Region      string `json:"region"`
	Password    string `json:"password"`     
	NewPassword string `json:"new_password"`
}
