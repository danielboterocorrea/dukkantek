# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

WORKDIR /src
COPY . .

RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine
WORKDIR /src
COPY --from=build /out ./

ENTRYPOINT ["./dukkantek.Api"]