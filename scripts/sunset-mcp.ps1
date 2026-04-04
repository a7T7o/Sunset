[CmdletBinding()]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$RemainingArgs
)

$ErrorActionPreference = "Stop"

$pythonScript = Join-Path $PSScriptRoot "sunset_mcp.py"
if (-not (Test-Path -LiteralPath $pythonScript)) {
    throw "Missing Python entrypoint: $pythonScript"
}

$py = Get-Command py -ErrorAction SilentlyContinue
if ($null -ne $py) {
    & $py.Source -3 $pythonScript @RemainingArgs
    exit $LASTEXITCODE
}

$python = Get-Command python -ErrorAction SilentlyContinue
if ($null -ne $python) {
    & $python.Source $pythonScript @RemainingArgs
    exit $LASTEXITCODE
}

throw "Python was not found. Ensure 'py' or 'python' is available on PATH."
