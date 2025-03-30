package main

import (
	"Backend/config"
	"Backend/routes"
	"fmt"
	"io"
	"log"
	"os"

	"github.com/gin-gonic/gin"
)

func init() {
	// Crea la cartella logs se non esiste
	err := os.MkdirAll("logs", os.ModePerm)
	if err != nil {
		fmt.Println("Error creating logs folder:", err)
	}

	// Apre (o crea) il file di log
	logFile, err := os.OpenFile("logs/carrentalhub.log", os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)
	if err != nil {
		fmt.Println("Error opening log file:", err)
	} else {
		// Scrivi log sia su stdout (console) che su file
		multiWriter := io.MultiWriter(os.Stdout, logFile)
		log.SetOutput(multiWriter)

		gin.DefaultWriter = multiWriter
		gin.DefaultErrorWriter = multiWriter
	}
}

func main() {
	db, err := config.InitDB()
	if err != nil {
		fmt.Println("Error database connection:", err)
		return
	}
	defer db.Close()

	r := routes.SetupRoutes()
	r.SetTrustedProxies(nil)

	if err := r.Run(":8085"); err != nil {
		log.Println("Server startup error:", err)
	}
}
