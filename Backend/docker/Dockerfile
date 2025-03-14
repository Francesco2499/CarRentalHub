# Fase 1: build del binario Go
FROM golang:1.23.4 AS builder

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . .
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o carrentalhub .

# Fase 2: container finale con Ubuntu
FROM ubuntu:22.04

WORKDIR /app

RUN apt-get update && apt-get install -y \
    curl \
    bash \
    ca-certificates \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app/carrentalhub .

RUN chmod +x /app/carrentalhub

EXPOSE 8085

CMD ["./carrentalhub"]


