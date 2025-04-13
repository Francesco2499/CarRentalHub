package controllers

import (
	"Backend/models"
	"Backend/services"
	"net/http"
	"fmt"
	"github.com/gin-gonic/gin"
)

func Register(c *gin.Context) {
	var user models.User

	if err := c.ShouldBindJSON(&user); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid data"})
		return
	}

	newUser, message, err := services.RegisterUser(user)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"message": message, "error": fmt.Errorf("")})
		return
	}

	c.JSON(http.StatusOK, gin.H{"message": message, "user": newUser})
}

func Login(c *gin.Context) {
	var user models.User

	if err := c.ShouldBindJSON(&user); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid data"})
		return
	}

	message, token, userResp, err := services.AuthenticateUser(user.Username, user.Password)
	if err != nil {
		c.JSON(http.StatusOK, gin.H{"message": message, "error": fmt.Errorf("")})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"message": message,
		"token":   token,
		"user":    userResp,
	})
}
