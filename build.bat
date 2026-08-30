@echo off
setlocal

rem === Prefer .NET Framework v4.0.30319 ===
set "CSC64=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set "CSC32=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if exist "%CSC64%" (
  set "CSC=%CSC64%"
) else if exist "%CSC32%" (
  set "CSC=%CSC32%"
) else (
  echo [ERROR] csc.exe not found in default .NET Framework locations.
  echo Checked:
  echo   %CSC64%
  echo   %CSC32%
  exit /b 1
)

echo Using compiler: %CSC%

rem === Resolve current user directory ===
set "USERDIR=%USERPROFILE%"

rem === Ensure Projects folder exists ===
if not exist "%USERDIR%\projects" (
  echo Creating Projects folder...
  mkdir "%USERDIR%\projects"
)

rem === Ensure VB-Editor folder exists ===
if not exist "%USERDIR%\projects\VB-Editor" (
  echo Creating VB-Editor folder...
  mkdir "%USERDIR%\projects\VB-Editor"
)

echo Building VBEditor.exe ...

"%CSC%" /nologo ^
  /target:winexe ^
  /reference:System.dll ^
  /reference:System.Drawing.dll ^
  /reference:System.Windows.Forms.dll ^
  /out:%USERDIR%\projects\VB-Editor\VBEditor.exe ^
  %USERDIR%\projects\VB-Editor\*.cs

if errorlevel 1 (
  echo [ERROR] Build failed.
  exit /b 1
)

echo [OK] Build succeeded: %USERDIR%\projects\VB-Editor\VBEditor.exe
exit /b 0
