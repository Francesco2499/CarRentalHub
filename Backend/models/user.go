package models

type User struct {
	ID       int    `json:"id"`
	Username string `json:"username"`
	Email    string `json:"email"`
	Password string `json:"password"`
	Region   string `json: "region"`
	Role     string `json:"role"` // "admin" o "customer"
}
