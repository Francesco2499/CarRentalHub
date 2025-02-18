# Usa l'immagine ufficiale di Go
FROM golang:1.23.4

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

#COPY . .
COPY ../ .

# Compila il codice
RUN go build -o main .

# Esponi la porta del server
EXPOSE 8085

# Comando di default per avviare l'app
CMD ["/app/main"]
