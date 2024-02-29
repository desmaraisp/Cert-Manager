FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5156

ENV ASPNETCORE_URLS=http://+:5156

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CertManager/CertManager.csproj", "src/CertManager/"]
RUN dotnet restore "src/CertManager/CertManager.csproj"
COPY . .
WORKDIR "/src/src/CertManager"
RUN dotnet build "CertManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CertManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CertManager.dll"]
