@echo off
echo CloudStorage servislerini baslatiliyor...

:: Docker Compose ile servisleri baslat
docker-compose up -d

echo.
echo Servisler baslatildi!
echo API Gateway: http://localhost:5000
echo Auth Service: http://localhost:5024
echo File Metadata Service: http://localhost:5026
echo File Storage Service: http://localhost:5025
echo Web App: http://localhost:5002
echo Swagger UI: http://localhost:5000/swagger
echo.

:: Servislerin durumunu goster
echo Servislerin durumu:
docker-compose ps

pause