package controllers

import (
	"Backend/models"
	"Backend/services"
	"log"
	"net/http"
	"strconv"
	"time"

	"github.com/gin-gonic/gin"
)

func GetAllVehicles(c *gin.Context) {
	log.Println("Received request to fetch all vehicles from the db")
	vehicles, err := services.GetAllVehicles()
	if err != nil {
		log.Printf("Error retrieving vehicles: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Successfully retrieved %d vehicles", len(vehicles))
	c.JSON(http.StatusOK, vehicles)
}

func GetVehicleById(c *gin.Context) {
	log.Println("Received request to fetch a vehicle from the db")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid vehicle ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid vehicle ID"})
		return
	}
	log.Printf("Vehicle ID %d", id)
	vehicle, err := services.GetVehicleById(id)
	if err != nil {
		log.Printf("Error retrieving vehicle: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, vehicle)
}

func CreateVehicle(c *gin.Context) {
	log.Println("Received request to create a new vehicle")
	var vehicle models.Vehicle
	if err := c.ShouldBindJSON(&vehicle); err != nil {
		log.Printf("Invalid request payload: %v", err)
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	if err := services.CreateVehicle(&vehicle); err != nil {
		log.Printf("Error saving vehicle to the database: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("New vehicle created successfully: %+v", vehicle)
	c.JSON(http.StatusCreated, vehicle)
}

func UpdateVehicle(c *gin.Context) {
	log.Println("Received request to update a vehicle")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid vehicle ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid vehicle ID"})
		return
	}
	var vehicle models.Vehicle
	if err := c.ShouldBindJSON(&vehicle); err != nil {
		log.Println("Error: Decode JSON failed")
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	vehicle.ID = id
	if err := services.UpdateVehicle(&vehicle); err != nil {
		log.Printf("Error while updating vehicle ID %d: %v", id, err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Vehicle with ID %d updated successfully: %+v", id, vehicle)
	c.JSON(http.StatusOK, vehicle)
}

func DeleteVehicle(c *gin.Context) {
	log.Println("Received request to delete a vehicle")
	id, err := strconv.Atoi(c.Param("id"))
	if err != nil {
		log.Println("Error: Invalid vehicle ID")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid vehicle ID"})
		return
	}
	if err := services.DeleteVehicle(id); err != nil {
		log.Printf("Error while deleting vehicle ID %d: %v", id, err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	log.Printf("Vehicle ID %d deleted successfully", id)
	c.Status(http.StatusNoContent)
}

func GetAvailableVehicles(c *gin.Context) {
	log.Println("Received request to fetch available vehicles")

	startDateStr := c.Query("start_date")
	endDateStr := c.Query("end_date")
	location := c.Query("location")

	if startDateStr == "" || endDateStr == "" {
		log.Println("Error: Missing start_date or end_date query parameters")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Missing start_date or end_date query parameters"})
		return
	}

	startDate, err := time.Parse("2006-01-02", startDateStr)
	if err != nil {
		log.Println("Error: Invalid start_date format")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid start_date format. Use YYYY-MM-DD."})
		return
	}

	endDate, err := time.Parse("2006-01-02", endDateStr)
	if err != nil {
		log.Println("Error: Invalid end_date format")
		c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid end_date format. Use YYYY-MM-DD."})
		return
	}

	vehicles, err := services.GetAvailableVehicles(startDate, endDate, location)
	if err != nil {
		log.Printf("Error retrieving available vehicles: %v", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}

	log.Printf("Successfully retrieved %d available vehicles in '%s' between %s and %s", len(vehicles), location, startDateStr, endDateStr)
	c.JSON(http.StatusOK, vehicles)
}
