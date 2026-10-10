#Requires -Version 7.0
[CmdletBinding()]
param(
    [string]$Owner = 'tdufresne99',
    [string]$Repository = 'tdufresne99/MealPrepApp',
    [string]$Title = 'MealPrepApp Prototype'
)

$ErrorActionPreference = 'Stop'
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw 'GitHub CLI is required. Install it from https://cli.github.com/ and authenticate with project access.'
}

function Invoke-GhJson {
    param([string[]]$Arguments)
    $result = & gh @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "GitHub command failed: gh $($Arguments -join ' '). Check account access and the project scope."
    }
    return ($result -join "`n" | ConvertFrom-Json)
}

# Check the expected story set before creating any project.
$issues = @(Invoke-GhJson -Arguments @('issue', 'list', '--repo', $Repository, '--state', 'all', '--limit', '1000', '--json', 'number,title,url'))
$stories = @($issues | Where-Object { $_.title -match '^US-\d{2}\s' })
foreach ($index in 1..25) {
    $storyId = 'US-{0:00}' -f $index
    $matching = @($stories | Where-Object { $_.title -match "^$storyId\s" })
    if ($matching.Count -ne 1) {
        throw "Expected exactly one issue for $storyId; found $($matching.Count). Resolve this before creating a project."
    }
}
$stories = @($stories | Where-Object { [int]($_.title.Substring(3, 2)) -le 25 } | Sort-Object title)

$listing = Invoke-GhJson -Arguments @('project', 'list', '--owner', $Owner, '--limit', '1000', '--format', 'json')
$matchingProjects = @($listing.projects | Where-Object { $_.title -eq $Title })
if ($matchingProjects.Count -gt 1) {
    throw "Several projects have the title '$Title'. Select a unique title before continuing."
}
if ($matchingProjects.Count -eq 1) {
    $project = $matchingProjects[0]
    Write-Host "Reusing project $($project.number)."
} else {
    $project = Invoke-GhJson -Arguments @('project', 'create', '--owner', $Owner, '--title', $Title, '--format', 'json')
    Write-Host "Created project $($project.number)."
}

$projectNumber = [string]$project.number
if ([string]::IsNullOrWhiteSpace($projectNumber)) {
    throw 'The GitHub response did not contain a project number.'
}
& gh project link $projectNumber --owner $Owner --repo $Repository
if ($LASTEXITCODE -ne 0) {
    throw 'Project exists, but linking it to the repository failed. Correct access and rerun the script.'
}

$items = Invoke-GhJson -Arguments @('project', 'item-list', $projectNumber, '--owner', $Owner, '--limit', '1000', '--format', 'json')
$existingUrls = @{}
foreach ($item in $items.items) {
    if ($item.content.url) { $existingUrls[$item.content.url] = $true }
}
foreach ($issue in $stories) {
    if ($existingUrls.ContainsKey($issue.url)) {
        Write-Host "Already present: $($issue.title)"
        continue
    }
    $null = Invoke-GhJson -Arguments @('project', 'item-add', $projectNumber, '--owner', $Owner, '--url', $issue.url, '--format', 'json')
    $existingUrls[$issue.url] = $true
    Write-Host "Added: $($issue.title)"
}

$verified = Invoke-GhJson -Arguments @('project', 'item-list', $projectNumber, '--owner', $Owner, '--limit', '1000', '--format', 'json')
$verifiedUrls = @($verified.items | ForEach-Object { $_.content.url })
$missing = @($stories | Where-Object { $_.url -notin $verifiedUrls })
if ($missing.Count -gt 0) {
    throw "$($missing.Count) expected issue(s) are still missing. Rerun to resume without duplicate items."
}
Write-Host 'All 25 stories are present in the project.'
Write-Host "https://github.com/users/$Owner/projects/$projectNumber"
