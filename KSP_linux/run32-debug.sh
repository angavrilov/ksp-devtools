#!/bin/bash

# NVidia driver options
export __GL_FSAA_MODE=1
#export __GL_LOG_MAX_ANISO=1
export __GL_THREADED_OPTIMIZATIONS=1

CUR_DIR=`pwd`

export LD_LIBRARY_PATH=$CUR_DIR:$CUR_DIR/KSP_Data/Mono/x86
export LD_PRELOAD="load-fork.so libpthread.so.0 libGL.so.1"

export MONO_DEBUGGER_AGENT=transport=dt_socket,address=127.0.0.1:10000

./KSP.x86
