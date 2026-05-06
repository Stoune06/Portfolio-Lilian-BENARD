# =====================================================
# Script de setup du dépôt git pour le portfolio
# Lilian BENARD - ISART
# =====================================================
#
# Usage : ouvre PowerShell dans C:\Users\lilia\Documents\ISART\Projet
# puis lance : .\setup-git.ps1
#
# Si tu as un message de sécurité PowerShell, lance d'abord :
#   Set-ExecutionPolicy -Scope Process Bypass
# =====================================================

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Setup du portfolio git" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Forcer le bon dossier
Set-Location -Path "C:\Users\lilia\Documents\ISART\Projet"
Write-Host "Dossier de travail: $((Get-Location).Path)" -ForegroundColor Green

# Vérifier que git est installé
try {
    $gitVersion = git --version
    Write-Host "Git détecté: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "ERREUR: git n'est pas installé ou pas dans le PATH" -ForegroundColor Red
    Write-Host "Installe Git depuis https://git-scm.com/download/win" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[1/4] Suppression des .git internes des projets conservés..." -ForegroundColor Cyan

$gitFoldersToRemove = @(
    "6_PEACH\.git",
    "Croix Rouge\Croix-Rouge-Unity\.git",
    "OBG\2024_gdp1_obg_2025_gdp2029_obg-GDP1_BENARD_L\.git",
    "PROJET_F2P\.git",
    "PROJET_PLATFORMER\.git",
    "RUSH\2025_gdp2_rush_2026_gdp2rush_2025_2026-gdp2_benard_l\.git"
)

foreach ($gitFolder in $gitFoldersToRemove) {
    if (Test-Path $gitFolder) {
        try {
            # Retirer l'attribut read-only sur tous les fichiers
            attrib -R "$gitFolder\*" /S /D 2>$null
            Remove-Item -Path $gitFolder -Recurse -Force -ErrorAction Stop
            Write-Host "  OK   $gitFolder" -ForegroundColor Green
        } catch {
            Write-Host "  FAIL $gitFolder : $_" -ForegroundColor Red
        }
    } else {
        Write-Host "  --   $gitFolder (deja absent)" -ForegroundColor DarkGray
    }
}

Write-Host ""
Write-Host "[2/4] Initialisation du dépôt git principal..." -ForegroundColor Cyan

if (Test-Path ".git") {
    Write-Host "  .git existe deja - on continue avec le repo existant" -ForegroundColor Yellow
} else {
    git init -b main
    if ($LASTEXITCODE -ne 0) {
        # Anciennes versions de git
        git init
        git checkout -b main
    }
    Write-Host "  OK Dépôt initialisé sur la branche 'main'" -ForegroundColor Green
}

# Configurer git si pas fait
$gitName = git config user.name
$gitEmail = git config user.email
if ([string]::IsNullOrEmpty($gitName)) {
    git config user.name "Lilian BENARD"
    Write-Host "  Configuration: user.name = Lilian BENARD" -ForegroundColor Green
}
if ([string]::IsNullOrEmpty($gitEmail)) {
    git config user.email "mytsun3g@gmail.com"
    Write-Host "  Configuration: user.email = mytsun3g@gmail.com" -ForegroundColor Green
}

Write-Host ""
Write-Host "[3/4] Ajout des fichiers (peut prendre 1-2 min)..." -ForegroundColor Cyan

git add .
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR pendant git add" -ForegroundColor Red
    exit 1
}

# Afficher un résumé
$staged = git diff --cached --numstat | Measure-Object -Line
Write-Host "  $($staged.Lines) fichiers ajoutés au commit" -ForegroundColor Green

Write-Host ""
Write-Host "[4/4] Création du commit initial..." -ForegroundColor Cyan

git commit -m "Initial commit - Portfolio Lilian BENARD (ISART)"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR pendant git commit (rien à commit ?)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host " Dépôt local prêt !" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "ÉTAPES SUIVANTES :" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Va sur https://github.com/new" -ForegroundColor White
Write-Host "2. Nom du repo (ex: portfolio-isart ou Portfolio-Lilian-BENARD)" -ForegroundColor White
Write-Host "3. Choisis 'Public' (pour que les recruteurs voient)" -ForegroundColor White
Write-Host "4. NE COCHE PAS 'Add a README' / .gitignore / license" -ForegroundColor Yellow
Write-Host "5. Clique 'Create repository'" -ForegroundColor White
Write-Host ""
Write-Host "Ensuite, dans cette PowerShell, lance (en remplaçant TON_USER) :" -ForegroundColor Cyan
Write-Host ""
Write-Host "  git remote add origin https://github.com/TON_USER/NOM_DU_REPO.git" -ForegroundColor Yellow
Write-Host "  git push -u origin main" -ForegroundColor Yellow
Write-Host ""
