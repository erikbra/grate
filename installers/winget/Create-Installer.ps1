#/bin/env pwsh

param (
    [string]$grateExe, # Not necessary, but try to keep a common interface for all Create-Installer.ps1 scripts
    [Parameter(Mandatory=$true)] [string]$version 
)

$Root = Resolve-Path "$($PSScriptRoot)";

$packageUrl="https://github.com/grate-devs/grate/releases/download/$version/grate-$version.msi"

# Download and extract winget
$wingetExe="https://aka.ms/wingetcreate/latest/self-contained"
$localExe="wingetcreate.exe"

function Recreate {
	param ($dirName)

	$dir = Join-Path $Root $dirName;

	If (Test-Path($dir)) {
	   $null = Remove-Item -Recurse $dir
	}
	mkdir $dir;
}

$wingetDir = Recreate "winget"

$client = New-Object System.Net.WebClient;
$localFile = (Join-Path $wingetDir $localExe);

try {
    $client.DownloadFile($wingetExe, $localFile);
} catch {
   Write-Error "Error: " + $_
   $error[0].Exception.ToString()
}

$winget = $localFile

# Update the winget release

& $winget update erikbra.grate -u $packageUrl -v $version -o $(Join-Path $Root grate)


