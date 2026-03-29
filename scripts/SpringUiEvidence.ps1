[CmdletBinding()]
param(
    [ValidateSet('latest', 'promote-latest', 'prune')]
    [string]$Action = 'latest',
    [int]$RetentionDays = 14,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ArtifactRoot = Join-Path $RepoRoot '.codex\artifacts\ui-captures\spring-ui'
$PendingDir = Join-Path $ArtifactRoot 'pending'
$AcceptedDir = Join-Path $ArtifactRoot 'accepted'
$LatestPath = Join-Path $ArtifactRoot 'latest.json'
$ManifestPath = Join-Path $ArtifactRoot 'manifest.jsonl'

function Ensure-ArtifactLayout {
    New-Item -ItemType Directory -Force -Path $ArtifactRoot | Out-Null
    New-Item -ItemType Directory -Force -Path $PendingDir | Out-Null
    New-Item -ItemType Directory -Force -Path $AcceptedDir | Out-Null
}

function Resolve-RepoRelativePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$AbsolutePath
    )

    return [System.IO.Path]::GetRelativePath($RepoRoot, $AbsolutePath).Replace('\', '/')
}

function Resolve-AbsolutePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RelativePath
    )

    $normalized = $RelativePath.Replace('/', '\')
    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $normalized))
}

function Write-JsonFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        $Data
    )

    $json = $Data | ConvertTo-Json -Depth 10
    [System.IO.File]::WriteAllText($Path, $json, [System.Text.UTF8Encoding]::new($false))
}

function Read-JsonFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        return $null
    }

    return (Get-Content $Path -Raw | ConvertFrom-Json)
}

function Append-Manifest {
    param(
        [Parameter(Mandatory = $true)]
        $Data
    )

    $line = $Data | ConvertTo-Json -Depth 10 -Compress
    [System.IO.File]::AppendAllText($ManifestPath, $line + [Environment]::NewLine, [System.Text.UTF8Encoding]::new($false))
}

function Set-JsonNoteProperty {
    param(
        [Parameter(Mandatory = $true)]
        $Target,
        [Parameter(Mandatory = $true)]
        [string]$Name,
        $Value
    )

    $existing = $Target.PSObject.Properties[$Name]
    if ($null -eq $existing) {
        $Target | Add-Member -NotePropertyName $Name -NotePropertyValue $Value
    }
    else {
        $Target.$Name = $Value
    }
}

function Get-LatestPointer {
    return Read-JsonFile -Path $LatestPath
}

function Promote-LatestCapture {
    $latest = Get-LatestPointer
    if ($null -eq $latest) {
        throw '未找到 latest.json，暂无可提升证据。'
    }

    if ($latest.lifecycle -eq 'accepted') {
        return [pscustomobject]@{
            action = 'promote-latest'
            result = 'noop'
            message = "最新证据已经在 accepted：$($latest.imagePath)"
        }
    }

    $sourcePng = Resolve-AbsolutePath -RelativePath $latest.imagePath
    $sourceJson = Resolve-AbsolutePath -RelativePath $latest.jsonPath
    if (-not (Test-Path $sourcePng) -or -not (Test-Path $sourceJson)) {
        throw 'latest.json 指向的证据文件不存在，无法提升。'
    }

    $targetPng = Join-Path $AcceptedDir ([System.IO.Path]::GetFileName($sourcePng))
    $targetJson = Join-Path $AcceptedDir ([System.IO.Path]::GetFileName($sourceJson))

    if (Test-Path $targetPng) { [System.IO.File]::Delete($targetPng) }
    if (Test-Path $targetJson) { [System.IO.File]::Delete($targetJson) }

    [System.IO.File]::Move($sourcePng, $targetPng)
    [System.IO.File]::Move($sourceJson, $targetJson)

    $sidecar = Read-JsonFile -Path $targetJson
    if ($null -ne $sidecar) {
        Set-JsonNoteProperty -Target $sidecar -Name 'lifecycle' -Value 'accepted'
        Set-JsonNoteProperty -Target $sidecar -Name 'promotedAtUtc' -Value ([DateTime]::UtcNow.ToString('O'))
        Set-JsonNoteProperty -Target $sidecar -Name 'imagePath' -Value (Resolve-RepoRelativePath -AbsolutePath $targetPng)
        Set-JsonNoteProperty -Target $sidecar -Name 'jsonPath' -Value (Resolve-RepoRelativePath -AbsolutePath $targetJson)
        Write-JsonFile -Path $targetJson -Data $sidecar
    }

    $latest.lifecycle = 'accepted'
    $latest.promotedAtUtc = [DateTime]::UtcNow.ToString('O')
    $latest.imagePath = Resolve-RepoRelativePath -AbsolutePath $targetPng
    $latest.jsonPath = Resolve-RepoRelativePath -AbsolutePath $targetJson
    Write-JsonFile -Path $LatestPath -Data $latest

    Append-Manifest @{
        timestampUtc = [DateTime]::UtcNow.ToString('O')
        action = 'promote'
        actor = 'powershell'
        captureId = $latest.captureId
        lifecycle = 'accepted'
        imagePath = $latest.imagePath
        jsonPath = $latest.jsonPath
    }

    return [pscustomobject]@{
        action = 'promote-latest'
        result = 'ok'
        imagePath = $latest.imagePath
        jsonPath = $latest.jsonPath
    }
}

function Prune-PendingCapture {
    $cutoffUtc = [DateTime]::UtcNow.AddDays(-[Math]::Max(1, $RetentionDays))
    $files = Get-ChildItem -Path $PendingDir -File -ErrorAction SilentlyContinue
    $expired = @($files | Where-Object { $_.LastWriteTimeUtc -lt $cutoffUtc })

    if (-not $DryRun) {
        foreach ($file in $expired) {
            [System.IO.File]::Delete($file.FullName)
        }
    }

    $deletedPaths = @($expired | ForEach-Object { Resolve-RepoRelativePath -AbsolutePath $_.FullName })

    $latest = Get-LatestPointer
    if (($null -ne $latest) -and ($latest.lifecycle -eq 'pending')) {
        if ($deletedPaths -contains $latest.imagePath -or $deletedPaths -contains $latest.jsonPath) {
            if (-not $DryRun) {
                [System.IO.File]::Delete($LatestPath)
            }
        }
    }

    Append-Manifest @{
        timestampUtc = [DateTime]::UtcNow.ToString('O')
        action = 'prune'
        actor = 'powershell'
        lifecycle = 'pending'
        deletedCount = $expired.Count
        dryRun = [bool]$DryRun
        retentionDays = $RetentionDays
        deletedFiles = $deletedPaths
    }

    return [pscustomobject]@{
        action = 'prune'
        dryRun = [bool]$DryRun
        deletedCount = $expired.Count
        deletedFiles = $deletedPaths
    }
}

Ensure-ArtifactLayout

switch ($Action) {
    'latest' {
        $latest = Get-LatestPointer
        if ($null -eq $latest) {
            Write-Output '{"message":"no-latest-capture"}'
        }
        else {
            Get-Content $LatestPath -Raw
        }
    }
    'promote-latest' {
        Promote-LatestCapture | ConvertTo-Json -Depth 8
    }
    'prune' {
        Prune-PendingCapture | ConvertTo-Json -Depth 8
    }
}
