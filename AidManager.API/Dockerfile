# Etapa 1: Imagen base para el entorno de ejecución final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa 2: Imagen con el SDK para construir y probar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia los archivos de proyecto (.csproj) para optimizar el cache de Docker
COPY ["AidManager.API/AidManager.API.csproj", "AidManager.API/"]
COPY ["AidManager.API/Tests/UnitTests/AidManager.API.Tests.UnitTests.csproj", "AidManager.API/Tests/UnitTests/"]
COPY ["AidManager.API/Tests/IntegrationTests/AidManager.API.Tests.IntegrationTests.csproj", "AidManager.API/Tests/IntegrationTests/"]

# Restaura las dependencias de todos los proyectos
RUN dotnet restore "AidManager.API/AidManager.API.csproj"
RUN dotnet restore "AidManager.API/Tests/UnitTests/AidManager.API.Tests.UnitTests.csproj"
RUN dotnet restore "AidManager.API/Tests/IntegrationTests/AidManager.API.Tests.IntegrationTests.csproj"

# Copia el resto del código fuente
COPY . .

# Construye el proyecto principal
WORKDIR "/src/AidManager.API"
RUN dotnet build "AidManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa 3: Ejecutar las pruebas
FROM build AS test
WORKDIR "/src"
# Ejecuta las pruebas unitarias
RUN dotnet test "AidManager.API/Tests/UnitTests/AidManager.API.Tests.UnitTests.csproj" --no-build -c $BUILD_CONFIGURATION
# Ejecuta las pruebas de integración
RUN dotnet test "AidManager.API/Tests/IntegrationTests/AidManager.API.Tests.IntegrationTests.csproj" --no-build -c $BUILD_CONFIGURATION

# Etapa 4: Publicar la aplicación
FROM build AS publish
WORKDIR "/src/AidManager.API"
RUN dotnet publish "AidManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa 5: Imagen final y optimizada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AidManager.API.dll"]
