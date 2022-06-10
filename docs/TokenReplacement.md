---
title: "Token replacement"
permalink: /token-replacement/
---
# Token Replacement
Any value that is in the configuration can be tokenised in the scripts.

`ALTER DATABASE {{DatabaseName}}` will be replaced as `ALTER DATABASE Bob` when the database name is `Bob`.  

This is not case sensitive.

## Notes
⚠ Using token replacement in your scripts locks those scripts into grate. I would use something like this VERY sparingly.

## Supported Tokens
The curently supported tokens are defined in the [`TokenProvider`]({{ site.github.repository_url }}/blob/main/grate/Infrastructure/TokenProvider.cs) class.  While not all the RoundhousE tokens are supported yet, this set of allowed tokens should improve over time (PR's welcome!).
 
## User defined tokens
User defined tokens are supported for token replacement. You can add user defined tokens using the `--ut` or `--usertoken` switch. The user defined token is defined as a `key=value` pair. You can define multiple user defined tokens by adding the switch multiple times.

Example:
`--ut=MyTablePrefix=local --ut=MyOtherToken=OtherValue`

The tokens can then be used for token replacement in scripts:
`ALTER TABLE {{MyTablePrefix}}_TableName` will become `ALTER TABLE local_TableName`

### Notes
⚠ If token replacement in general should be used VERY sparingly, then user tokens are an even larger foot gun.  They are built in a very simplistic manner, and are likely to fail just as simply.

- If you pass clearly invalid info on the commandline (eg missing the equal sign) then grate will terminate with an error.
- If you try and get tricksy and pass valid but broken information (eg including '{' chars, multiple '=' chars etc) then all bets are off.  Raise an issue if you have a genuine scenario you'd like help with.
