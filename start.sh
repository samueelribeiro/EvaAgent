#!/bin/bash

echo "🚀 Iniciando EvaAgent..."
echo ""

# Cores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}[1/4]${NC} Subindo infraestrutura Docker..."
cd deploy
docker-compose up -d
cd ..

echo ""
echo -e "${BLUE}[2/4]${NC} Aguardando PostgreSQL iniciar..."
sleep 5

echo ""
echo -e "${BLUE}[3/4]${NC} Aplicando migrations..."
dotnet ef database update --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api --context AppDbContext

echo ""
echo -e "${BLUE}[4/4]${NC} Iniciando API..."
echo ""
echo -e "${GREEN}✅ Infraestrutura pronta!${NC}"
echo ""
echo "📊 Serviços disponíveis:"
echo "  - API:      http://localhost:5000"
echo "  - pgAdmin:  http://localhost:5050 (admin@admin.com / admin)"
echo "  - Seq:      http://localhost:5341"
echo "  - Jaeger:   http://localhost:16686"
echo ""
echo -e "${YELLOW}Iniciando servidor...${NC}"
echo ""

dotnet run --project src/EvaAgent.Api/EvaAgent.Api.csproj
