---
title: "Getting Grate"
permalink: /getting-grate/
---

# Getting Grate

There's a variety of ways to access grate depending on your needs:

## Source Code/raw binaries

The [github site]({{ site.github.repository.html_url }}) has both the raw source code for local compilation, and also has binaries for a variety of OS's published at [each release]({{ site.github.repository.html_url }}/releases).

## Docker

There's a `{{ site.github.repository_nwo }}` docker image published to [dockerhub](https://hub.docker.com/r/{{ site.github.repository_nwo }}) on every release.  See the Examples folder for a demo using this to a migration.

## Dotnet Tool

grate is available as a [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools).  Simply `dotnet tool install -g grate` to get the [package](https://www.nuget.org/packages/grate/).

## Winget

grate is available on [winget](https://docs.microsoft.com/en-us/windows/package-manager/winget/).  Simply `winget install erikbra.grate` for awesome!

## Notes

Plans are afoot for more OS specific package managers, watch this space.