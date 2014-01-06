@echo off

if defined ProgramFiles(x86) (
    set UnityDir="%ProgramFiles(x86)%\Unity"
) else (
    set UnityDir="%ProgramFiles%\Unity"
)

set MONODEVELOP_SDB_TEST=1

%UnityDir%\MonoDevelop\bin\MonoDevelop.exe
