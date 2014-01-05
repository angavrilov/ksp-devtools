@echo off

set MONO_DEBUGGER_AGENT=transport=dt_socket,address=127.0.0.1:10000

KSP.exe
