@echo off
echo Parando EvaAgent...
cd deploy
docker-compose down
cd ..
echo.
echo Todos os servicos foram parados.
