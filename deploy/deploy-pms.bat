@ECHO OFF

ECHO Preparing NuGet...
CALL ..\..\set-nuget-key.bat
del *.nupkg
pause

ECHO Publishing to NuGet...
nuget pack ..\Ministry.SimpleTwitterStream\Ministry.SimpleTwitterStream.csproj -Prop Configuration=Release
nuget push *.nupkg

pause