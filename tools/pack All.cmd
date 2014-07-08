pushd %~dp0

call "pack Clutch.cmd"
call "pack Clutch.Diagnostics.EntityFramework.cmd"
call "pack Clutch.Diagnostics.Logging.cmd"
call "pack Clutch.Diagnostics.Logging.NLog.cmd"
call "pack Clutch.Web.cmd"
call "pack Clutch.Web.Diagnostics.Logging.cmd"
call "pack Clutch.Web.Mvc.cmd"

pause

popd