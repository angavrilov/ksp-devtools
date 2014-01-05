@echo off

rem Built-in profiler
rem set MONO_PROFILE=default:stat,file=monoprof.out

rem The legacy 'logging' profiler (provided by an independent .so)
set MONO_PROFILE=logging:stat=16,out=monoprof.mprof

rem Same, but starts disabled and waits on HTTP port for commands ('enable'/'disable')
rem set MONO_PROFILE=logging:stat=16,out=monoprof.mprof,command-port=12345,start-disabled

KSP.exe
