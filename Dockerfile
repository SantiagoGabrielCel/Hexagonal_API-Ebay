# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# RUTA CORREGIDA ⬇️
COPY Api_EbayStocks/Api_EbayStocks.csproj .
RUN dotnet restore

COPY Api_EbayStocks/. .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Api_EbayStocks.dll"]
