 # run dotnet restore which will restore all packages by examing the global.json file and finding the project.json files automatically
& $env:LOCALAPPDATA\Microsoft\DotNet\dotnet restore

# run dotnet build on all project.json files in the folder structure
& $env:LOCALAPPDATA\Microsoft\DotNet\dotnet build **/project.json