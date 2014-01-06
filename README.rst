#####################
KSP Development Tools
#####################

.. contents::

============
Introduction
============

KSP is distributed as a release build of the Unity Player, and does
not officially provide any way to debug or profile mod plugins.

However, in order to run the code the player uses an LGPL open-source
mono library, with the source code for changes made by Unity available
on github at http://github.com/Unity-Technologies/mono. This allows
building a substitute library which activates the debugging and profiling
APIs if certain environment variables are set.


============
Installation
============

In order to install the libraries, copy the contents of the ``KSP_linux``
or ``KSP_win`` subfolders to the KSP installation folder, in the process
overwriting the original ``mono.dll`` or ``mono.so`` file.

The ``Tools`` subfolder contains programs for viewing the collected profiling
data, and can be placed anywhere.

In order to run the tools on windows, install Mono from http://www.go-mono.com/mono-downloads/download.html,
for example: http://download.mono-project.com/archive/3.2.3/windows-installer/mono-3.2.3-gtksharp-2.12.11-win32-0.exe.
On linux the mono package from your distribution should work.

Likewise, for debugging you need MonoDevelop, for example the version that
comes with Unity tools.


=========
Profiling
=========

In order to profile KSP, start it via the ``run32-prof`` script. The main
function of the script is to initialize the **MONO_PROFILE** environment
variable with a string that would normally be passed via the ``--profile``
command line option of ``mono``.

**Note:** Since Unity is using mono 2.6, instead of the modern *log*
profiler plugin, it has the legacy *logging* one. Search for old
documentation, or check the source code for a full list of options.

When profiling, it is best to quit the game nicely through the menus,
or the profiling data may be incompletely written out and/or corrupted.

**Warning:** The profiler has been known to crash or deadlock sometimes,
so don't actually play the game with it enabled.


Profiling modes
===============

The script contains three sample sets of options:

1.  ``default:stat,file=monoprof.out``

    This activates the primitive built-in profiling code, using the statistical
    profiling, and writing a report to a text file when the program ends. This
    is not actually very useful, since it doesn't use stack traces.

2.  ``logging:stat=32,out=monoprof.mprof``

    This enables the statistical logging profiler, recording stack traces up
    to 32 steps deep (the limit is 128). The data is written in binary to the
    specified file, and has to be analyzed with a separate tool.

3.  ``logging:stat=32,out=monoprof.mprof,command-port=12345,start-disabled``

    In addition to the above, the profiler is instructed to initialize in
    a disabled state, and listen on a TCP port for commands. To control it,
    connect to port 12345 with a program like telnet, and type:

    * ``enable`` to activate profiling.
    * ``disable`` to suspend it again.

    This mode is useful for profiling a specific situation, instead of
    everything starting with initial loading of the game. Also, it seems
    that after using the ``disable`` command, and waiting a bit, it is
    mostly safe to quit the game with Alt-F4.

    **Note:** Shutting down the game and then quickly restarting
    it may cause a failure to listen on the port due to a reuse timeout.
    The only way you will know is that telnet won't be able to connect.
    Also, you cannot reconnect if you close telnet by mistake.


OS specifics
============

The sampling profiler operates differently on linux and windows:

* On Linux, it uses the **SIGPROF** signal to track actual CPU time usage,
  and tracks all threads.

* On Windows, it uses a multimedia timer based on real time, and only monitors
  the main thread.

In both cases one sample is roughly 1 msec of the relevant time domain.


Profiling results
=================

The best way to view the logging profiling results is the ``emveepee`` GUI
application from the ``Tools`` folder. When you open ``monoprof.mprof`` with
it, it displays a list of functions on top, and additional data below.

The list of functions shows the percentage of time samples recorded for that
location, both on its own, and including callees. The list is sorted by total
time.

The panel below shows sample counts and percentages (in the same scale as the
function list) for calles and callers. The numbers reflect how many stack traces
were recorded where the current function was calling, or was called by the other
one. Note that in case of recursion, one actual stack trace may be counted multiple
times as the same function is repeatedly encountered in it.


=========
Debugging
=========

Similar to profiling, debugging involves starting KSP via the ``run32-debug`` script,
which sets the **MONO_DEBUGGER_AGENT** environment variable to the normal value of the
``--debugger-agent`` parameter. This instructs mono to connect to a debugger waiting
on a certain port. The difficult part is ensuring that something does wait there.

Setting up the project
======================

1. Add a dummy EXE project to your solution. Right click it and choose *Set As Startup Project*.

   MonoDevelop doesn't seem to want to start the debugger if there is no executable.
   Since you won't actually be running this dummy project, it just needs to be buildable.

2. Right click your DLL project, choose *Options*, then *Build*, *Output*, and set the
   output path to the appropriate directory in Game Data.

   Breakpoints don't seem to be recognized if you manually copy the DLL.

3. Right click on everything in *References* and uncheck the *Local copy* option.

   This is to stop the build process from copying ``Assembly-CSharp.dll`` and
   other unneeded files into GameData because of step 3.

Running the debugger
====================

1. Start MonoDevelop with the **MONODEVELOP_SDB_TEST** environment variable set to 1,
   for instance by using the provided ``monodevelop`` script.

2. Use *Run* -> *Run With* -> *Custom Command Mono Soft Debugger* to start debugging.

   Alternatively, go to *Tools* -> *Options* -> *Projects* -> *Debugger* -> *Preferred Debuggers*,
   and move *Custom Command Mono Soft Debugger* to the top of the list to make this mode
   the default activated by F5.

3. A *Launch Soft Debugger* window should appear, with a blank command and
   *127.0.0.1* and *10000* set as the address and port. Click the Listen button,
   and MonoDevelop should enter a waiting mode.

   Don't try to enter a command, because it crashes due to a bug.

4. Start KSP with ``run32-debug``. MonoDevelop should react by entering debugging mode.


=========
Compiling
=========

The libraries and tools in this package are compiled using this repository and its submodules:

  http://github.com/angavrilov/ksp-devtools

Specific component instructions:

1. To build the Mono library on linux, enter the ``mono/`` submodule and execute:

   ``perl build_runtime_linux.pl``

2. To build Mono on windows, open ``mono/msvc/mono.sln`` with Visual Studio Express 2010
   (ignoring messages about disabled features), select target *Release_eglib (Win32)*
   and build the solution.

3. To build tools on linux, enter ``mono-tools/``, configure and then ``make all`` in the
   ``Mono.Profiler`` subdirectory.

4. Finally, ``package.sh`` on linux copies all necessary files into the right places, and
   packs them into zip archives.
