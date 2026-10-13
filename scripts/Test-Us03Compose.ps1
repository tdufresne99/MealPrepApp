#Requires -Version 7.0
[CmdletBinding()]
param([ValidateRange(1024, 65532)][int]$BasePort = 18080)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path $PSScriptRoot
$runId = [Guid]::NewGuid().ToString('N').Substring(0, 8)
$projects = @()
$previousApiPort = $env:API_PORT

function Invoke-Compose {
    param($Target, [string[]]$Arguments)
    & docker compose --project-directory $repositoryRoot -f (Join-Path $repositoryRoot 'compose.yaml') --env-file $Target.File -p $Target.Project @Arguments
    if ($LASTEXITCODE -ne 0) { throw "Compose operation failed for $($Target.Environment)." }
}

function Assert-Http {
    param([int]$Port, [string]$Path, [int]$Status, [string]$Body)
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        try {
            $response = Invoke-WebRequest "http://127.0.0.1:$Port$Path" -SkipHttpErrorCheck -TimeoutSec 7
            if ($response.StatusCode -eq $Status -and $response.Content -eq $Body) { return }
        } catch { }
        Start-Sleep -Milliseconds 500
    }
    throw "Expected HTTP $Status with $Body on $Path (port $Port)."
}

try {
    $index = 0
    foreach ($environment in @('Development', 'Staging', 'Production')) {
        & (Join-Path $PSScriptRoot 'Initialize-LocalEnvironment.ps1') -Environment $environment
        $key = $environment.ToLowerInvariant()
        $target = [pscustomobject]@{
            Environment = $environment
            File = Join-Path $repositoryRoot ".env.$key"
            Project = "mealprep-us03-$runId-$key"
            Port = $BasePort + $index
            Database = "mealprep_$key"
        }
        $projects += $target
        $env:API_PORT = [string]$target.Port
        Invoke-Compose $target @('config', '--quiet')
        Invoke-Compose $target @('up', '-d', '--wait', 'db')
        Invoke-Compose $target @('run', '--rm', '--build', 'migrate')
        # Bootstrap must be safe to repeat.
        Invoke-Compose $target @('run', '--rm', 'migrate')
        Invoke-Compose $target @('up', '-d', 'api')
        Assert-Http $target.Port '/health' 200 'Healthy'
        Assert-Http $target.Port '/health/ready' 200 'Healthy'
        $schemas = @(Invoke-Compose $target @('exec', '-T', 'db', 'psql', '-U', 'mealprep', '-d', $target.Database, '-At', '-v', 'ON_ERROR_STOP=1', '-c', "SELECT schema_name FROM information_schema.schemata WHERE schema_name IN ('identity','households','recipes','planning','groceries') ORDER BY schema_name;"))
        if (($schemas -join ',') -ne 'groceries,households,identity,planning,recipes') { throw 'Expected module schemas were not initialized.' }
        $sql = "CREATE TABLE public.us03_probe (marker text NOT NULL); INSERT INTO public.us03_probe VALUES ('$key');"
        Invoke-Compose $target @('exec', '-T', 'db', 'psql', '-U', 'mealprep', '-d', $target.Database, '-v', 'ON_ERROR_STOP=1', '-c', $sql)
        Invoke-Compose $target @('up', '-d', '--no-deps', '--force-recreate', 'api')
        Assert-Http $target.Port '/health/ready' 200 'Healthy'
        $index++
    }

    foreach ($target in $projects) {
        $env:API_PORT = [string]$target.Port
        $marker = @(Invoke-Compose $target @('exec', '-T', 'db', 'psql', '-U', 'mealprep', '-d', $target.Database, '-At', '-v', 'ON_ERROR_STOP=1', '-c', 'SELECT marker FROM public.us03_probe;'))
        if (($marker -join '').Trim() -ne $target.Environment.ToLowerInvariant()) { throw 'Persistence or environment isolation check failed.' }
    }

    $target = $projects[0]
    $env:API_PORT = [string]$target.Port
    Invoke-Compose $target @('stop', 'db')
    Assert-Http $target.Port '/health/ready' 503 'Unhealthy'
    Assert-Http $target.Port '/health' 200 'Healthy'
    Invoke-Compose $target @('up', '-d', '--force-recreate', '--wait', 'db')
    Assert-Http $target.Port '/health/ready' 200 'Healthy'
    $marker = @(Invoke-Compose $target @('exec', '-T', 'db', 'psql', '-U', 'mealprep', '-d', $target.Database, '-At', '-c', 'SELECT marker FROM public.us03_probe;'))
    if (($marker -join '').Trim() -ne 'development') { throw 'Database container recreation lost data.' }
    Write-Host 'US-03 checks passed: three environments, repeatable schema bootstrap, durable data, database outage and recovery.'
} finally {
    foreach ($target in $projects) {
        # Only remove projects created by this invocation; never target normal environment volumes.
        if ($target.Project -notmatch "^mealprep-us03-$runId-(development|staging|production)$") { throw 'Unsafe cleanup target.' }
        $env:API_PORT = [string]$target.Port
        & docker compose --project-directory $repositoryRoot -f (Join-Path $repositoryRoot 'compose.yaml') --env-file $target.File -p $target.Project down --volumes --remove-orphans
    }
    $env:API_PORT = $previousApiPort
}
