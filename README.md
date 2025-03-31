# GamingPlatform

## Prerequisites
- Docker
- Docker Compose

## Installation
1. Clone the repository:
2. Run Docker Compose:
## Usage
1. Register in the Hub:
   Open [Swagger UI](http://localhost:5000/swagger/index.html) and register a new user.

2. Use GamingPlatformClient:
   With the Username and password, you can now use the GamingPlatformClient to interact with the platform.

4. Trigger Leaderboard Processing:
   You can manually trigger leaderboard processing without waiting for the scheduled interval:
   for that authenticate to Hub and use token to open [Leaderboard Swagger UI](http://localhost:8080/swagger/index.html) and trigger the process.

## API Documentation
- Hub API: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)
- Leaderboard API: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
- Game API: [http://localhost:7000/swagger/index.html](http://localhost:7000/swagger/index.html)