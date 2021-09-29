# Build and run the grate container locally for testing
docker build -f .\grate\Dockerfile -t grate:dev --build-arg GRATE_VERSION=0.0.0.1 .
docker run --rm grate:dev