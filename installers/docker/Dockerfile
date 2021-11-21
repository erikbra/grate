FROM alpine:3 AS base
WORKDIR /app

COPY --chmod=0755 grate .

# Add globalization support to the OS so .Net can use cultures
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["./grate"]
