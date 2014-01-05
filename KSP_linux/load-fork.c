// gcc -shared -m32 -o load-fork.so load-fork.c

#include <unistd.h>
#include <signal.h>

// Work around the SIGPROF vs fork() interaction issue on linux by wrapping it
pid_t __libc_fork();

pid_t fork() {
  sigset_t blkmask, oldmask;
  pid_t rv;

  sigemptyset(&blkmask);
  sigaddset(&blkmask, SIGPROF);
  sigprocmask(SIG_BLOCK, &blkmask, &oldmask);

  rv = __libc_fork();

  if (rv != 0)
    sigprocmask(SIG_SETMASK, &oldmask, NULL);

  return rv;
}
