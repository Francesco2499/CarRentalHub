package models

type UserLoginResponseDTO struct {
	ID       int    `json:"id"`
	Username string `json:"username"`
	Email    string `json:"email"`
	Region   string `json:"region"`
	Role     string `json:"role"`
}

func ToUserLoginResponseDTO(user *User) *UserLoginResponseDTO {
	return &UserLoginResponseDTO{
		ID:       user.ID,
		Username: user.Username,
		Email:    user.Email,
		Region:   user.Region,
		Role:     user.Role,
	}
}
