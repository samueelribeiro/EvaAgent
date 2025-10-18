@echo off
echo ========================================
echo    Iniciando EvaAgent
echo ========================================
echo.

echo [1/4] Subindo infraestrutura Docker...
cd deploy
docker-compose up -d
cd ..

echo.
echo [2/4] Aguardando PostgreSQL iniciar...
timeout /t 5 /nobreak > nul

echo.
echo [3/4] Aplicando migrations...
dotnet ef database update --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api --context AppDbContext

echo.
echo [4/4] Iniciando API...
echo.
echo ========================================
echo    Infraestrutura Pronta!
echo ========================================
echo.
echo Servicos disponiveis:
echo   - API:      http://localhost:5000
echo   - pgAdmin:  http://localhost:5050 (admin@admin.com / admin)
echo   - Seq:      http://localhost:5341
echo   - Jaeger:   http://localhost:16686
echo.
echo Iniciando servidor...
echo.

dotnet run --project src/EvaAgent.Api/EvaAgent.Api.csproj
