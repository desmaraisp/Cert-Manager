Push-Location "$PSScriptRoot/.."
docker compose up --build --force-recreate
Pop-Location