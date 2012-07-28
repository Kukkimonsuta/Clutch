pushd %~dp0

nuget pack ..\src\Clutch.Diagnostics.EntityFramework\Clutch.Diagnostics.EntityFramework.csproj -Build -Symbols -Properties Configuration=Release

pause

popd