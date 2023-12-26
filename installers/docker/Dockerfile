FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR .
COPY *.props ./
COPY src/ ./src/
RUN dotnet publish ./src/grate/grate.csproj -r linux-musl-x64 -c release --self-contained -p:SelfContained=true -o ./publish/app

FROM alpine:3 AS base
WORKDIR /app

COPY --chmod=0755 --from=build /publish/app .

# Add globalization support to the OS so .Net can use cultures
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Add the grate executable
ENTRYPOINT ./grate \
            --sqlfilesdirectory=/db \
            --version=$VERSION \
            --connstring="$APP_CONNSTRING" \
            --silent \
            --outputPath=/output
