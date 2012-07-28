pushd %~dp0

nuget pack ..\src\Clutch.Web.Diagnostics.Logging\Clutch.Web.Diagnostics.Logging.csproj -Build -Symbols -Properties Configuration=Release

pause

popd