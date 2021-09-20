# Migrating to grate from RoundhousE

[RoundhousE](https://github.com/chucknorris/roundhouse) is an amazing tool, and grate is trying to be backwards compatible with RH to make upgrading easy.

# Changes compared to RoundhousE

There's a number of general changes in grate as a result of moving to the latest .Net bits that will apply to all users.

grate is built using the new [`System.CommandLine`](https://github.com/dotnet/command-line-api) API's, so:
- Command line arguments are now **case-sensitive** on all operating systems (including windows)
- Support for `/argument` on windows has been removed, and you'll have to use `-a` or `--argument` instead.  See `grate --help` for the full set of allowed options.

- By default grate stores version information in the `grate` database schema.  To continue using your existing version information pass `--schema=RoundhousE`
- grate has a single mandatory `-cs`/`--connstring` argument for simplicity.  RH's `--database`, `--server`, `--accesstoken` etc arguments are now longer allowed.
- Not all previously supported tokens are available yet.  For the current set of supported tokens please see [the `TokenProvider` class](../grate/infrastructure/TokenProvider.cs).
- The `DropCreate` mode and the `--drop` option have been merged.  grate uses the `--drop` option only, but **it operates like `DropCreate`**! If you have a scenario for dropping a database but _not_ then running a migration, please open an issue!


## RH Features that aren't yet in grate

Rebuilding a decade old product from scratch takes time, and features have to be prioritised. The list of items below shows the current set of outstanding RH features that haven't been built yet.  If there's something on this list you can't live without, raise an issue to help us out.  If people take ownership of a feature and contribute PR's then we'll all get there faster 👍

Expect this list to shrink over time.

- `--baseline`
- `--defaultencoding`
- `--donotstorescriptsruntext`
- `--dryrun`
- `--isuptodate`
- Modes other than 'Normal' (`DropCreate` and `RestoreRun`)
- MSBuild Task.
- Multiple Environments per run. Scripts can target multiple environments (`blah.env.test.uat.sql`), but each migration can only target a single environment.  If this is a limiting factor for please raise an issue to discuss.
- Oracle Database support.  We know this is a big one for some people, but none of the current maintainers are Oracle devs.  If you'd like this support please raise an issue!
- `--runallanytimescripts`
- Recovery Modes (`--simple`, `--recoverymode` etc)
- Restore Options (`--restorefrompath`, `--restoreoptions`, `--restoretimeout` etc)
- User Token Replacement (`--usertokens`)
- Version info sourced from a file.  `--version=<VALUE>` support is available on the command line
- `--warnononetimescriptchanges`, `--warnandignoreonetimescriptchanges`




