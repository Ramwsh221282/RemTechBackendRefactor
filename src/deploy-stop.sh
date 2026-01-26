#!/bin/bash
# Останавливает ваш compose-проект

set -e

echo "Останавливаем контейнеры из deploy-compose.yml..."
docker compose -f /home/farid/remtech/RemTechBackendRefactor/src/deploy-compose.yml down

echo "✅ Проект остановлен."
