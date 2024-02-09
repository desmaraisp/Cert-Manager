# Cert-Manager
Central certificate api
 
docker compose up --build --force-recreate
cd .\src\CertManager\
dotnet ef database update
dotnet openapi refresh http://localhost:5156/swagger/v1/swagger.json