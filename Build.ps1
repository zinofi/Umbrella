# bootstrap DotNet CLI tools into this session.
$script = {$Branch='rel/1.0.0';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1'))}

# load up the global.json so we can find the version
$globalJson = Get-Content -Path $PSScriptRoot\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore

if($globalJson)
{
    $sdkVersion = $globalJson.sdk.version
}
else
{
    Write-Warning "Unable to locate global.json to determine using 'latest'"
    $sdkVersion = "latest"
}

&$script -version $sdkVersion

 # run dotnet restore which will restore all packages by examing the global.json file and finding the project.json files automatically
& $env:USERPROFILE\Microsoft\DotNet\dotnet restore

# run dotnet build on all project.json files in the folder structure
& $env:USERPROFILE\Microsoft\DotNet\dotnet build $PSScriptRoot/**/project.json