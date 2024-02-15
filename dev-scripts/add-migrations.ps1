[CmdletBinding()]
param (
	[Parameter(mandatory = $true)]
	[string]
	$MigrationName
)

Push-Location "$PSScriptRoot/../src/CertManager"

dotnet ef migrations add $MigrationName --project ../CertManager.Migrations.SqlServer -- --provider SqlServer
dotnet ef migrations add $MigrationName --project ../CertManager.Migrations.Postgresql -- --provider Postgresql

Pop-Location