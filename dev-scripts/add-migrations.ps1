[CmdletBinding()]
param (
	[Parameter(mandatory = $true)]
	[string]
	$MigrationName
)

Push-Location "$PSScriptRoot/../src/CertManager"

dotnet ef migrations add $MigrationName --project ../Migrations.MSSQL -- --DatabaseType SqlServer
dotnet ef migrations add $MigrationName --project ../Migrations.Postgresql -- --DatabaseType Postgresql

Pop-Location