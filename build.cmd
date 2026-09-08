@echo off
setlocal
set FW=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319
if not exist "%FW%\csc.exe" set FW=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set OUT=%~dp0bin\LimpiarCertificados.exe
set ICO=%~dp0assets\app.ico
if not exist "%~dp0bin" mkdir "%~dp0bin"
if not exist "%ICO%" (
  echo Falta el icono: %ICO%
  exit /b 1
)
"%FW%\csc.exe" /nologo /codepage:65001 /utf8output /target:winexe /platform:anycpu /optimize+ /win32icon:"%ICO%" /out:"%OUT%" /r:"%FW%\System.Windows.Forms.dll" /r:"%FW%\System.Drawing.dll" /r:"%FW%\System.Security.dll" /r:"%FW%\System.Core.dll" "%~dp0src\*.cs"
if errorlevel 1 exit /b 1
copy /Y "%OUT%" "%~dp0LimpiarCertificados.exe" >nul
echo OK %OUT%
echo OK %~dp0LimpiarCertificados.exe
endlocal
