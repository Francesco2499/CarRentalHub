package controllers

import (
	"Backend/models"
	"Backend/services"
	"net/http"

	"github.com/gin-gonic/gin"
)

func UpdateCurrentUser(c *gin.Context) {
	userID := c.GetInt("userID")

	var req models.UserUpdateRequestDTO
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid request body"})
		return
	}

	userDTO, err := services.UpdateUser(userID, &req)
	if err != nil {
		c.JSON(http.StatusUnauthorized, gin.H{"error": err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{"message": "User updated successfully", "user": userDTO})
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
