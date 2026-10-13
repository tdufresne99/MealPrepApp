#Requires -Version 7.0
[CmdletBinding()]
param([ValidateSet('Development', 'Staging', 'Production')][string]$Environment = 'Development')

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path $PSScriptRoot
$key = $Environment.ToLowerInvariant()
$environmentFile = Join-Path $repositoryRoot ".env.$key"
$secretDirectory = Join-Path $repositoryRoot ".secrets/$key"
$secretFile = Join-Path $secretDirectory 'postgres-password'
if (-not (Test-Path -LiteralPath $environmentFile)) {
    Copy-Item -LiteralPath (Join-Path $repositoryRoot "deploy/$key.env.example") -Destination $environmentFile
}
if (-not (Test-Path -LiteralPath $secretFile)) {
    New-Item -ItemType Directory -Force $secretDirectory | Out-Null
    $password = [Convert]::ToHexString([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
    [System.IO.File]::WriteAllText($secretFile, $password)
}
Write-Host "Environment files ready for $Environment. Existing configuration and passwords were preserved."
