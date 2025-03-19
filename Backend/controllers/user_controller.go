package controllers

import (
	"Backend/models"
	"Backend/services"
	"net/http"

	"github.com/gin-gonic/gin"
)

func UpdateCurrentUser(c *gin.Context) {
	userID := c.GetInt("userID")

	var updatedUser models.User
	if err := c.ShouldBindJSON(&updatedUser); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid request body"})
		return
	}

	err := services.UpdateUser(userID, &updatedUser)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to update user"})
		return
	}

	c.JSON(http.StatusOK, gin.H{"message": "User updated successfully"})
}

func DeleteCurrentUser(c *gin.Context) {
	userID := c.GetInt("userID")

	err := services.DeleteUser(userID)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to delete user"})
		return
	}

	c.JSON(http.StatusOK, gin.H{"message": "User deleted successfully"})
}
