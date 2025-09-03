# Détecter automatiquement le chemin du dossier PostgreSQL le plus récent
$pgBase = "C:\Program Files\PostgreSQL"
$pgDirs = Get-ChildItem -Path $pgBase -Directory | Sort-Object Name -Descending
$pgBin = Join-Path $pgDirs[0].FullName "bin\psql.exe"

# Vérifier si psql existe
if (-Not (Test-Path $pgBin)) {
    Write-Error "❌ Impossible de trouver psql.exe dans $pgBin"
    exit 1
}

Write-Host "✅ Utilisation de psql : $pgBin"

# Exécuter ton script SQL
& $pgBin -U postgres -d MusicxDb -f ".\CreateScript.sql"