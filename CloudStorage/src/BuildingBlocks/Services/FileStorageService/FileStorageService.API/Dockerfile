# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Create directories for data and storage
RUN mkdir -p /app/data /app/storage && \
    chmod -R 777 /app/data /app/storage
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BuildingBlocks/Services/FileStorageService/FileStorageService.API/FileStorageService.API.csproj", "src/BuildingBlocks/Services/FileStorageService/FileStorageService.API/"]
RUN dotnet restore "./src/BuildingBlocks/Services/FileStorageService/FileStorageService.API/FileStorageService.API.csproj"
COPY . .
WORKDIR "/src/src/BuildingBlocks/Services/FileStorageService/FileStorageService.API"
RUN dotnet build "./FileStorageService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FileStorageService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Create directories for data and storage again in the final stage
RUN mkdir -p /app/data /app/storage && \
    chmod -R 777 /app/data /app/storage
ENTRYPOINT ["dotnet", "FileStorageService.API.dll"]