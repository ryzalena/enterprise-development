Write-Host "=== Setting up Aspire ===" -ForegroundColor Cyan

# 1. Проверим структуру
Write-Host "`n1. Checking project structure..." -ForegroundColor Yellow
if (Test-Path "src/AppHost") {
    Write-Host "   ✓ AppHost exists in src/AppHost/" -ForegroundColor Green
} else {
    Write-Host "   ✗ AppHost not found in src/AppHost/" -ForegroundColor Red
    exit 1
}

# 2. Добавим пакеты в AppHost
Write-Host "`n2. Adding packages to AppHost..." -ForegroundColor Yellow
dotnet add src/AppHost package Aspire.Hosting --version 8.2.0
dotnet add src/AppHost package Aspire.Hosting.SqlServer --version 8.2.0

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Packages added to AppHost" -ForegroundColor Green
} else {
    Write-Host "   ✗ Failed to add packages to AppHost" -ForegroundColor Red
}

# 3. Добавим пакет в WebApi
Write-Host "`n3. Adding package to WebApi..." -ForegroundColor Yellow
dotnet add src/WebApi package Aspire.Microsoft.EntityFrameworkCore.SqlServer --version 8.2.0

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Package added to WebApi" -ForegroundColor Green
} else {
    Write-Host "   ✗ Failed to add package to WebApi" -ForegroundColor Red
}

# 4. Проверим зависимость AppHost на WebApi
Write-Host "`n4. Checking AppHost references..." -ForegroundColor Yellow
$appHostProj = Get-Content "src/AppHost/AppHost.csproj" -Raw
if ($appHostProj -match "WebApi") {
    Write-Host "   ✓ AppHost references WebApi" -ForegroundColor Green
} else {
    Write-Host "   ✗ AppHost doesn't reference WebApi" -ForegroundColor Red
    Write-Host "   Adding reference..." -ForegroundColor Gray
    dotnet add src/AppHost reference src/WebApi/WebApi.csproj
}

# 5. Соберём всё
Write-Host "`n5. Building solution..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Solution built successfully" -ForegroundColor Green
} else {
    Write-Host "   ✗ Build failed" -ForegroundColor Red
}

Write-Host "`n=== Setup complete! ===" -ForegroundColor Green
Write-Host "To run with Aspire:" -ForegroundColor Cyan
Write-Host "1. Ensure Docker Desktop is running" -ForegroundColor Gray
Write-Host "2. Run: dotnet run --project src/AppHost" -ForegroundColor Gray
Write-Host "3. Check: http://localhost:5000/api/health" -ForegroundColor Gray