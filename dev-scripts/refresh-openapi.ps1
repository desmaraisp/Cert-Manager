Push-Location "$PSScriptRoot/../src/CertManagerClient"
dotnet openapi refresh http://localhost:5156/swagger/v1/swagger.json
dotnet build
Pop-Location

Push-Location "$PSScriptRoot/../src/react-app"
npm run generate-client
Pop-Location