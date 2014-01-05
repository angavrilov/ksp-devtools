#!/bin/bash

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

rm -f devtools-linux32.zip devtools-win32.zip pdb-win32.zip

zip -r devtools-linux32.zip KSP_linux Tools LICENSE -x \*.gitignore
zip -r devtools-win32.zip KSP_win Tools LICENSE -x \*.gitignore Tools/emveepee.sh
zip pdb-win32.zip PDB/*.pdb
