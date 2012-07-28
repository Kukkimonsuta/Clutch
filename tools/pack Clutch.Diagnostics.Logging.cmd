pushd %~dp0

nuget pack ..\src\Clutch.Diagnostics.Logging\Clutch.Diagnostics.Logging.csproj -Build -Symbols -Properties Configuration=Release

pause

popd