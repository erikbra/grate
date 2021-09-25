# Docker Example

This directory shows a very simple way of building a docker container to apply your database migrations.  This is not intended to be prod ready (passing version numbers etc) but just to get you started.

## Usage

Simply `docker-compose up` to:
- build a local `mygrateimage` image that contains both grate and the migration scipts
- start a postgres database server
- run the `mygrateimage` migration against the server

## Notes

