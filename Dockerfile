# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY FacturacionAPI/*.csproj ./FacturacionAPI/
RUN dotnet restore ./FacturacionAPI/FacturacionAPI.csproj

COPY FacturacionAPI/ ./FacturacionAPI/
RUN dotnet publish ./FacturacionAPI/FacturacionAPI.csproj -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FacturacionAPI.dll"]
