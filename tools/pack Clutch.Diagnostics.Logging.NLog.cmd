pushd %~dp0

nuget pack ..\src\Clutch.Diagnostics.Logging.Nlog\Clutch.Diagnostics.Logging.NLog.csproj -Build -Symbols -Properties Configuration=Release

pause

popd