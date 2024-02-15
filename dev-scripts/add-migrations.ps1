[CmdletBinding()]
param (
	[Parameter(mandatory = $true)]
	[string]
	$MigrationName
)

Push-Location "$PSScriptRoot/../src/CertManager"

dotnet ef migrations add $MigrationName --project ../CertManager.Migrations.SqlServer -- --DatabaseType SqlServer
dotnet ef migrations add $MigrationName --project ../CertManager.Migrations.Postgresql -- --DatabaseType Postgresql

Pop-Location