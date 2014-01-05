#!/bin/bash

# NVidia driver options
export __GL_FSAA_MODE=1
#export __GL_LOG_MAX_ANISO=1
export __GL_THREADED_OPTIMIZATIONS=1

CUR_DIR=`pwd`

export LD_LIBRARY_PATH=$CUR_DIR:$CUR_DIR/KSP_Data/Mono/x86
export LD_PRELOAD="load-fork.so libpthread.so.0 libGL.so.1"

# Built-in profiler
#export MONO_PROFILE=default:stat,file=monoprof.out

# The legacy 'logging' profiler (provided by an independent .so)
export MONO_PROFILE=logging:stat=16,out=monoprof.mprof

# Same, but starts disabled and waits on HTTP port for commands ('enable'/'disable')
#export MONO_PROFILE=logging:stat=16,out=monoprof.mprof,command-port=12345,start-disabled

./KSP.x86
