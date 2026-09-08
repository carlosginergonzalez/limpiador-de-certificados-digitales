@echo off
setlocal
set EXE=%~dp0LimpiarCertificados.exe
if not exist "%EXE%" set EXE=%~dp0bin\LimpiarCertificados.exe
if not exist "%EXE%" (
  echo No se encuentra LimpiarCertificados.exe
  echo Compila antes con build.cmd
  pause
  exit /b 1
)
start "" "%EXE%"
endlocal
