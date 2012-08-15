pushd %~dp0

nuget pack ..\src\Clutch\Clutch.csproj -Build -Symbols -Properties Configuration=Release

pause

popd