# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY Lab11-MiguelQuispe.WebApi/Lab11-MiguelQuispe.WebApi.csproj Lab11-MiguelQuispe.WebApi/
COPY Lab11-MiguelQuispe.Application/Lab11-MiguelQuispe.Application.csproj Lab11-MiguelQuispe.Application/
COPY Lab11-MiguelQuispe.Infraestructure/Lab11-MiguelQuispe.Infraestructure.csproj Lab11-MiguelQuispe.Infraestructure/
COPY Lab11-MiguelQuispe.Domain/Lab11-MiguelQuispe.Domain.csproj Lab11-MiguelQuispe.Domain/

RUN dotnet restore Lab11-MiguelQuispe.WebApi/Lab11-MiguelQuispe.WebApi.csproj

# Copiar el resto del código y publicar
COPY . .
WORKDIR /src/Lab11-MiguelQuispe.WebApi
RUN dotnet publish Lab11-MiguelQuispe.WebApi.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
# Render usa la variable PORT para exponer el servicio
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000
ENTRYPOINT ["dotnet", "Lab11-MiguelQuispe.WebApi.dll"]
