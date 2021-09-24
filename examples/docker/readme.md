# Docker Example

This directory shows one fairly simple way of building a docker container to apply your database migrations.  This is predicated on the idea that a 'release' is one set of artifacts with only different configuration separating each environment. 

## Usage

To build the image ready for testing:
 `docker build -f .\dockerfile --build-arg BUILD_BUILDNUMBER="1.0.0.0" --build-arg  -t mydockersample:1.0.0.0 .`

The hardcoded 1.0.0.0 version in this example is normally replaced with your actual build number (eg `$(Build.BuildNumber)`)

and to run a container against a database for testing:

``` bash

docker run --rm '
--env RH_ENVIRONMENT="Dev" `
--env RH_CONNSTRING="your connection string here" `
mydockersample:1.0.0.0
```

## Notes

You may obviously need to change he above commands depending on your circumstances (eg configure the `--network` option when run)