#!/bin/bash

if [ -z "$KSP_PATH" ]; then
  KSP_PATH=~/Games/KSP_linux
fi

if [ ! -d "$KSP_PATH" ]; then
  echo "KSP path $KSP_PATH not found."
  exit 1
fi

LPATH="$KSP_PATH/KSP_Data/Managed"

compile_ksp() {
  LIBNAME=$1
  shift
  gmcs -r:"$LPATH/Assembly-CSharp.dll,$LPATH/UnityEngine.dll" -t:library "$@" -out:"$LIBNAME"
}

compile_ksp Misc/FPS.dll Misc/FPS.cs
compile_ksp Misc/UnityObjectTree.dll Misc/UnityObjectTree.cs

MONODIR=mono/builds/embedruntimes/

cpstrip() {
  TGTFILE=$2/`basename $1`
  cp -f $1 $TGTFILE
  strip $TGTFILE
}

cpstrip $MONODIR/linux32/libmono.so KSP_linux/KSP_Data/Mono/x86/
cpstrip $MONODIR/linux32/libmono-profiler-logging.so KSP_linux/

cp -f $MONODIR/win32/mono.dll KSP_win/KSP_Data/Mono/
cp -f $MONODIR/win32/mono-profiler-logging.dll KSP_win/
cp -f $MONODIR/win32/*.pdb PDB/

MONOTOOLS=mono-tools/Mono.Profiler/lib/

cp -f $MONOTOOLS/*.dll $MONOTOOLS/*.exe Tools/

rst2html README.rst > README.html

rm -f devtools-linux32.zip devtools-win32.zip pdb-win32.zip

zip -r devtools-linux32.zip KSP_linux Tools Misc LICENSE README.html -x \*.gitignore Tools/emveepee.bat
zip -r devtools-win32.zip KSP_win Tools Misc LICENSE README.html -x \*.gitignore Tools/emveepee.sh
zip pdb-win32.zip PDB/*.pdb
