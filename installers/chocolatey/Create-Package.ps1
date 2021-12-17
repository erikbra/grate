#!/usr/bin/env pwsh

param ( 
     [Parameter(Mandatory=$true)] [string] $grateExe,
     [Parameter(Mandatory=$true)] [string] $version
)

$Root = Resolve-Path "$($PSScriptRoot)";
$toolsDir=Get-Item -Path $(Join-Path $Root "grate/tools")

Copy-Item $grateExe $toolsDir

choco pack --version=$version ./grate/grate.nuspec
