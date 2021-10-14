# grate
SQL scripts migration runner 

[![CI](https://github.com/erikbra/grate/actions/workflows/ci.yml/badge.svg)](https://github.com/erikbra/grate/actions/workflows/ci.yml)
[![Build](https://github.com/erikbra/grate/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/erikbra/grate/actions/workflows/build.yml)

## Goal

The goal of grate is to be largely backwards compatible with RoundhousE, which is an amazing tool. It is initiated by the main
maintainer of RoundhousE for the last three years, please see this issue in the RoundhousE repo for details: https://github.com/chucknorris/roundhouse/issues/438.

While early versions of grate may not support every last RoundhousE feature, those features that are implemented should work identically, or with only very small changes.  For detailed information see the [migration](docs/MigratingFromRoundhousE.md) docco.

## Docs

Full documentation is available [on the grate site](https://erikbra.github.io/grate/).

## Why the name grate?

grate is short for migrate. And it's also pronounced the same way as _great_, so, there you go. 

## Status

grate is catching up on RoundhousE features, there are a couple of features missing, they are documented in unit tests. But I've successfully replaced 
RoundhousE with grate in a 5-year-in-development folder of SQL scripts, without any issues. 

## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md)
