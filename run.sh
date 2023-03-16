#!/usr/bin/env bash

cd "$PWD"


export KESTREL_CERTIFICATE_PASSWORD="12345"
export KESTREL_CERTIFICATE_PATH=/https/aspnetapp.pfx
export KESTREL_CERTIFICATE_FOLDER=$HOME/.aspnet/http

make setup-services --ignore-errors
make create-certificate --ignore-errors

docker-compose --file "docker-compose.yml" build --force-rm --quiet
docker-compose --file "docker-compose.yml" up --force-recreate --detach

make execute-migrations