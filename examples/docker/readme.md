# Docker Example

This directory shows a very simple way of building a docker container to apply your database migrations.  This is not intended to be prod ready (passing environments etc) but just to get you started.

## Usage

Simply `docker-compose up` to:
- start a sql database server
- run the `grate` migration against the server with script locate in `db` folder and store the backup script in `output`

## Notes

- You wouldn't normally do db migrations using compose, this is just an example.

