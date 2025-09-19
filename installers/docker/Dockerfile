FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

ARG TARGETARCH
ARG TARGETFRAMEWORK=net9.0
WORKDIR /app
COPY *.props ./
COPY *.ruleset ./
COPY src/ ./src/
RUN dotnet publish --framework "$TARGETFRAMEWORK" --arch "$TARGETARCH" ./src/grate/grate.csproj \
    -c release --self-contained -p:SelfContained=true -o /publish

FROM --platform=$BUILDPLATFORM alpine:3 AS installer
RUN apk add icu-libs

FROM installer AS runtime
WORKDIR /app

COPY --chmod=0755 --from=build /publish .
RUN mkdir /output
# Add globalization support to the OS so .Net can use cultures
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Add the grate executable
ENTRYPOINT ./grate \
--sqlfilesdirectory=/db \
--version=${VERSION:-1.0.0} \
--connstring="$APP_CONNSTRING" \
--databasetype=${DATABASE_TYPE:-sqlserver} \
--silent \
--outputPath=/output \
--createdatabase=${CREATE_DATABASE:-true} \
--environment=${ENVIRONMENT:-LOCAL} \
--transaction=${TRANSACTION:-false}
