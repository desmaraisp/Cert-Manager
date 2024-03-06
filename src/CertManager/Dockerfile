FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5156

ENV ASPNETCORE_URLS=http://+:5156

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS deps
WORKDIR /src

COPY ./src/CertManager/CertManager.csproj ./src/CertManager/
COPY ./src/CertManager.Database/CertManager.Database.csproj ./src/CertManager.Database/
COPY ./src/CertManager.Migrations.Postgresql/CertManager.Migrations.Postgresql.csproj ./src/CertManager.Migrations.Postgresql/
COPY ./src/CertManager.Migrations.SqlServer/CertManager.Migrations.SqlServer.csproj ./src/CertManager.Migrations.SqlServer/
RUN dotnet restore "./src/CertManager/CertManager.csproj"

FROM deps as build
COPY ./third-party-licenses ./third-party-licenses
COPY ./src/CertManager/ ./src/CertManager/
COPY ./src/CertManager.Database/ ./src/CertManager.Database/
COPY ./src/CertManager.Migrations.Postgresql/ ./src/CertManager.Migrations.Postgresql/
COPY ./src/CertManager.Migrations.SqlServer/ ./src/CertManager.Migrations.SqlServer/
RUN dotnet build "./src/CertManager/CertManager.csproj" -c Release

FROM build AS publish
RUN dotnet publish "./src/CertManager/CertManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CertManager.dll"]
