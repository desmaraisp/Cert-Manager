FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/CertRenewer/CertRenewer.csproj", "src/CertRenewer/"]
RUN dotnet restore "src/CertRenewer/CertRenewer.csproj"
COPY . .
WORKDIR "/src/src/CertRenewer"
RUN dotnet build "CertRenewer.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CertRenewer.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CertRenewer.dll"]
