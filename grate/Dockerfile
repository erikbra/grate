# TODO: Work out what other dependencies we need to apt-install in order to use the aline:3 base image instead (smaller image!)
# Current image size is about 110Mb
FROM mcr.microsoft.com/dotnet/runtime:6.0.0-rc.1-alpine3.14 AS base
WORKDIR /app

ARG GRATE_VERSION
ENV VERSION=$GRATE_VERSION


FROM mcr.microsoft.com/dotnet/sdk:6.0.100-rc.1-alpine3.14 AS build
WORKDIR /src
COPY ["../nuget.config", "."]
COPY ["grate/grate.csproj", "grate/"]
RUN dotnet restore "grate/grate.csproj"
COPY . .
WORKDIR "/src/grate"
RUN dotnet publish "grate.csproj" -c Release -o /app/publish -p:NugetVersion=$VERSION -r alpine-x64 --self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Add globalization support to the OS sp .Net can use cultures
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["./grate"]