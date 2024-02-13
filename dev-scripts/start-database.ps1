Push-Location "$PSScriptRoot/.."
docker compose up --build --force-recreate
Pop-Location

Push-Location "$PSScriptRoot/../src/CertManager/"
dotnet ef database update -- --DatabaseType MSSQL
Pop-Location