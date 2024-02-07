FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt update && apt install -y clang zlib1g-dev

WORKDIR /src
COPY ["src/CertManagerAgent/CertManagerAgent.csproj", "src/CertManagerAgent/"]
RUN dotnet restore "src/CertManagerAgent/CertManagerAgent.csproj"
COPY . .
WORKDIR "/src/src/CertManagerAgent"
RUN dotnet build "CertManagerAgent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CertManagerAgent.csproj" -o /app/publish /p:DebugType=None /p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:8.0-jammy-chiseled-aot AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/CertManagerAgent"]
