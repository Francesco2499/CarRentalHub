# CarRentalHub

CarRentalHub è un'applicazione progettata per gestire un sistema di noleggio auto. Il progetto è composto da tre moduli principali: Frontend, Backend e Stats, integrati tramite Docker per garantire un'esperienza di avvio e gestione semplice e modulare.

## Struttura del Progetto
Il codice sorgente è organizzato nella directory principale, con i moduli separati nelle relative sottodirectory. Il modulo Frontend è sviluppato in Avalonia (.NET 9.0), mentre i moduli Backend e Stats sono containerizzati tramite Docker e realizzati rispettivamente in Go e Python.

Per maggiori dettagli sulla struttura e sul funzionamento del progetto, si può consultare la documentazione presente nella directory `/docs`.

## Dipendenze
Per avviare correttamente il sistema è necessario:

- .NET SDK v9.0 per il modulo Frontend
- Docker per eseguire i container dei moduli Backend e Stats

### Modulo Frontend
Il modulo Frontend richiede la versione 9.0 di .NET. Durante la fase di build, tutte le dipendenze vengono automaticamente installate, inclusi i pacchetti:
- Avalonia
- CommunityToolkit.Mvvm
- MapsUI

### Moduli Backend e Stats
Questi moduli richiedono l'installazione di Docker e il lancio dei container tramite `docker-compose`. Le immagini utilizzate includono:
- PostgreSQL per la gestione del database
- Modulo Backend realizzato in Go e Gin
- Modulo Stats sviluppato in Python con Flask

Per il corretto funzionamento è necessario un file `.env` con le seguenti variabili d'ambiente da posizionare nella cartella `Backend`:
- POSTGRES_USER=postgres 
- POSTGRES_PASSWORD=password@db
- POSTGRES_DB=CarRentalHub
- DB_HOST=db DB_USER=postgres
- DB_PASSWORD=password@db DB_NAME=CarRentalHub

## Avvio del Sistema
Assicurarsi che Docker sia avviato e seguire i seguenti passaggi:
1. Spostarsi nella directory `./docker`.
2. Lanciare il comando per avviare i container: ```docker-compose up --build```
3. Una volta avviati i container, accedere alla directory `./Frontend` e lanciare il comando: ```dotnet run```

Il modulo Frontend sarà ora disponibile per l'interazione, mentre i moduli Backend e Stats saranno eseguiti nei rispettivi container Docker.

## Porte Utilizzate
- **Backend**: 8085
- **Stats**: 5005
- **Database (PostgreSQL)**: 5432

Assicurarsi che le porte siano libere prima dell'avvio.
