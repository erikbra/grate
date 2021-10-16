#!/usr/bin/env pwsh

# Feel free to rem this out once grate is installed to speed up the script
winget install erikbra.grate

grate `
--files=.\db `
--env=Local `
--connstring="Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=grate_test" `
--version=1.0 `
--silent
# add --drop if you want grate to drop the whole db and start again.