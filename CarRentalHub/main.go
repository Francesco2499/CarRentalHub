package main

import (
	"CarRentalHub/config"
	"CarRentalHub/routes"
	"fmt"
)

func main() {
	err := config.SetupDB()
	if err != nil {
		fmt.Println("Errore di connessione al database:", err)
		return
	}
	defer config.DB.Close()

	r := routes.SetupRoutes()
	r.Run(":8080")
}

// package main

// import (
// 	"fmt"
// 	"log"
// 	"net/http"

// 	"github.com/gin-gonic/gin"
// 	"github.com/jmoiron/sqlx"
// 	_ "github.com/lib/pq"
// )

// var db *sqlx.DB

// // Struttura del veicolo
// type Vehicle struct {
// 	ID           int     `json:"id"`
// 	Model        string  `json:"model"`
// 	Price        float64 `json:"price"`
// 	Availability bool    `json:"availability"`
// }

// func main() {
// 	// Connessione al database
// 	var err error
// 	// Sostituisci <HOST>, <USER>, <PASSWORD>, <DBNAME> con i tuoi dati di connessione PostgreSQL
// 	db, err = sqlx.Connect("postgres", "host=localhost port=5432 user=admin password=admin dbname=carrentalhub sslmode=disable")
// 	if err != nil {
// 		log.Fatal("Errore durante la connessione al database:", err)
// 	}

// 	// Creazione del router
// 	r := gin.Default()

// 	// Definizione degli endpoint
// 	r.GET("/vehicles", getVehicles)
// 	r.POST("/vehicles", createVehicle)
// 	r.PUT("/vehicles/:id", updateVehicle)
// 	r.DELETE("/vehicles/:id", deleteVehicle)

// 	// Avvio del server
// 	fmt.Println("Server in esecuzione su http://localhost:8080")
// 	r.Run(":8080")
// }

// // Funzione per ottenere tutti i veicoli
// func getVehicles(c *gin.Context) {
// 	var vehicles []Vehicle
// 	err := db.Select(&vehicles, "SELECT id, model, price, availability FROM vehicles")
// 	if err != nil {
// 		c.JSON(http.StatusInternalServerError, gin.H{"error": "Errore durante il recupero dei veicoli"})
// 		return
// 	}
// 	c.JSON(http.StatusOK, vehicles)
// }

// // Funzione per creare un nuovo veicolo
// func createVehicle(c *gin.Context) {
// 	var vehicle Vehicle
// 	if err := c.ShouldBindJSON(&vehicle); err != nil {
// 		c.JSON(http.StatusBadRequest, gin.H{"error": "Dati non validi"})
// 		return
// 	}

// 	// Inserimento nel database
// 	_, err := db.Exec("INSERT INTO vehicles (model, price, availability) VALUES ($1, $2, $3)", vehicle.Model, vehicle.Price, vehicle.Availability)
// 	if err != nil {
// 		c.JSON(http.StatusInternalServerError, gin.H{"error": "Errore durante l'inserimento del veicolo"})
// 		return
// 	}
// 	c.JSON(http.StatusOK, gin.H{"message": "Veicolo creato con successo"})
// }

// // Funzione per aggiornare un veicolo esistente
// func updateVehicle(c *gin.Context) {
// 	id := c.Param("id")
// 	var vehicle Vehicle
// 	if err := c.ShouldBindJSON(&vehicle); err != nil {
// 		c.JSON(http.StatusBadRequest, gin.H{"error": "Dati non validi"})
// 		return
// 	}

// 	// Aggiornamento nel database
// 	_, err := db.Exec("UPDATE vehicles SET model=$1, price=$2, availability=$3 WHERE id=$4", vehicle.Model, vehicle.Price, vehicle.Availability, id)
// 	if err != nil {
// 		c.JSON(http.StatusInternalServerError, gin.H{"error": "Errore durante l'aggiornamento del veicolo"})
// 		return
// 	}
// 	c.JSON(http.StatusOK, gin.H{"message": "Veicolo aggiornato con successo"})
// }

// // Funzione per eliminare un veicolo
// func deleteVehicle(c *gin.Context) {
// 	id := c.Param("id")
// 	_, err := db.Exec("DELETE FROM vehicles WHERE id=$1", id)
// 	if err != nil {
// 		c.JSON(http.StatusInternalServerError, gin.H{"error": "Errore durante l'eliminazione del veicolo"})
// 		return
// 	}
// 	c.JSON(http.StatusOK, gin.H{"message": "Veicolo eliminato con successo"})
// }
