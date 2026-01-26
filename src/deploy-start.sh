#!/bin/bash
# Запускает ваш compose-проект с пересборкой

set -e

echo "Собираем и запускаем контейнеры из deploy-compose.yml..."
docker compose -f /home/farid/remtech/RemTechBackendRefactor/src/deploy-compose.yml up -d --build

echo "✅ Проект запущен."
