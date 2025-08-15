# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/VeniceOrders.API/VeniceOrders.API.csproj", "src/VeniceOrders.API/"]
COPY ["src/VeniceOrders.Application/VeniceOrders.Application.csproj", "src/VeniceOrders.Application/"]
COPY ["src/VeniceOrders.Domain/VeniceOrders.Domain.csproj", "src/VeniceOrders.Domain/"]
COPY ["src/VeniceOrders.Infrastructure/VeniceOrders.Infrastructure.csproj", "src/VeniceOrders.Infrastructure/"]

RUN dotnet restore "src/VeniceOrders.API/VeniceOrders.API.csproj"

COPY . .
WORKDIR "/src/src/VeniceOrders.API"
RUN dotnet publish "VeniceOrders.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VeniceOrders.API.dll"]
