pushd %~dp0

nuget pack ..\src\Clutch.Web.Mvc\Clutch.Web.Mvc.csproj -Build -Symbols -Properties Configuration=Release

pause

popd