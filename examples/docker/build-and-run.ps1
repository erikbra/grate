#!/bin/env pwsh

# App versioning is normally provided by your CI/CD pipelines...
docker-compose build --build-arg APP_VERSION=0.0.1
docker-compose up