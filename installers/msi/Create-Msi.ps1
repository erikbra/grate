#/bin/env pwsh

param (
    [Parameter(Mandatory=$true)] [string]$grateExe,
    [Parameter(Mandatory=$true)] [string]$version, 
    [string]$upgradeCode = [Guid]::NewGuid()
)

$Root = Resolve-Path "$($PSScriptRoot)";

# Download and extract WIX
$wixZip = "https://github.com/wixtoolset/wix3/releases/download/wix3112rtm/wix311-binaries.zip"
$localZip = "win311-binaries.zip"

function Recreate {
	param ($dirName)

	$dir = Join-Path $Root $dirName;

	If (Test-Path($dir)) {
	   $null = Remove-Item -Recurse $dir
	}
	mkdir $dir;
}

$wixDir = Recreate "wix"
$tmpDir = Recreate "tmp"

$client = New-Object System.Net.WebClient;
$localFile = (Join-Path $tmpDir $localZip);

try {
   $client.DownloadFile($wixZip, $localFile);
} catch {
   Write-Error "Error: " + $_
   $error[0].Exception.ToString()
}

Expand-Archive -Path  $localFile -DestinationPath $wixDir


$candle = Join-Path $wixDir "candle.exe"
$light = Join-Path $wixDir "light.exe"


# Create an MSI

$wxs = Join-Path $tmpDir "grate-$version.wxs"
$wixobj = Join-Path $tmpDir "grate-$version.wixobj"
$msi = Join-Path $tmpDir "grate-$version.msi"

$originalWxs = Join-Path $Root "grate.wxs"

((Get-Content -path $originalWxs -Raw) `
	-replace '%%%VERSION%%%',$version `
	-replace '%%%UPGRADECODE%%%',$upgradeCode `
	-replace '%%%GRATEEXE%%%',$grateExe `
	) | Set-Content -Path $wxs

#$null = & $candle -nologo -arch x64 $wxs -o $wixobj
& $candle -nologo -arch x64 $wxs -o $wixobj
& $light -nologo -wx0204 -wx1076 $wixobj -o $msi


