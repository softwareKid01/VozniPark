# Faza 1: Build (Korištenje .NET SDK za kompajliranje)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiranje csproj fajla i preuzimanje paketa
COPY ["VozniPark.csproj", "./"]
RUN dotnet restore "VozniPark.csproj"

# Kopiranje ostatka koda i build aplikacije
COPY . .
RUN dotnet build "VozniPark.csproj" -c Release -o /app/build
RUN dotnet publish "VozniPark.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Faza 2: Run (Laka slika samo za pokretanje)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port koji Docker koristi
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VozniPark.dll"]