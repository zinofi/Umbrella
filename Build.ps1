 # run dotnet restore which will restore all packages by examing the global.json file and finding the project.json files automatically
& $env:USERPROFILE\Microsoft\DotNet\dotnet restore

# run dotnet build on all project.json files in the folder structure
& $env:USERPROFILE\Microsoft\DotNet\dotnet build $PSScriptRoot/**/project.json