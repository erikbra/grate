#!/usr/bin/env pwsh


dotnet tool uninstall grate -g
#dotnet tool install grate -g --version 1.3.2
dotnet tool install grate -g --version 1.4.0


grate `
--files=./db `
--env=Local `
--connstring="Server=tcp:localhost,32769;User Id=sa;Password=*****;Encrypt=false;Database=grate_test3;Pooling=false" `
--version=1.0 `
--silent `
--drop