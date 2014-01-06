@echo off

if defined ProgramFiles(x86) (
    set MonoDir=%ProgramFiles(x86)%\Mono-3.2.3
) else (
    set MonoDir=%ProgramFiles%\Mono-3.2.3
)

rem set GTK2_RC_FILES=%ProgramFiles%\Unity\MonoDevelop\bin\gtkrc.win32

set LD_LIBRARY_PATH=%MonoDir%\lib;
set MONO_PATH=
set PATH=%MonoDir%\lib;%MonoDir%\bin;%PATH%
set PKG_CONFIG_LIBDIR=
set PKG_CONFIG_PATH=%MonoDir%\share\pkgconfig;%MonoDir%\lib\pkgconfig

set MonoDir=

mono.exe --debug emveepee.exe
