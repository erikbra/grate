# Docker Example

This directory shows a very simple way of building a docker container to apply your database migrations.  This is not intended to be prod ready (passing environments etc) but just to get you started.

## Usage

Simply `docker-compose up` to:
- build a local `myapp-dbmigration` image that contains both grate and the migration scripts based on the published `grate` image.
- start a sql database server
- run the `myapp-dbmigration` migration against the server

## Notes

- You wouldn't normally do db migrations using compose, this is just an example.

