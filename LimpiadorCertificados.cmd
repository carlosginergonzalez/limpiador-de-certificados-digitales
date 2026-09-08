@echo off
setlocal
set EXE=%~dp0LimpiadorCertificados.exe
if not exist "%EXE%" set EXE=%~dp0bin\LimpiadorCertificados.exe
if not exist "%EXE%" (
  echo No se encuentra LimpiadorCertificados.exe
  echo Compila antes con build.cmd
  pause
  exit /b 1
)
start "" "%EXE%"
endlocal
