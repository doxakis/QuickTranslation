@echo off
ECHO Clean up bin folder.
rd /S /Q QuickTranslation\bin

ECHO Compile QuickTranslation.
cd QuickTranslation
dotnet pack --configuration Release

ECHO Press enter to upload the nuget package.
PAUSE

cd bin\Release

ECHO Upload nuget.
dotnet nuget push *.nupkg --source https://www.nuget.org/api/v2/package

ECHO Done
PAUSE