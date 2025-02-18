package main

import (
	"Backend/config"
	"Backend/routes"
	"fmt"
)

func main() {
	db, err := config.InitDB()
	if err != nil {
		fmt.Println("Error database connection:", err)
		return
	}
	defer db.Close()

	r := routes.SetupRoutes()
	r.Run(":8085")
}
