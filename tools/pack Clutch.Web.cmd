pushd %~dp0

nuget pack ..\src\Clutch.Web\Clutch.Web.csproj -Build -Symbols -Properties Configuration=Release

pause

popd