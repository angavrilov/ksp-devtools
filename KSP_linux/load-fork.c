// gcc -shared -m32 -o load-fork.so load-fork.c

#define _GNU_SOURCE
#include <stdlib.h>
#include <unistd.h>
#include <signal.h>
#include <semaphore.h>
#include <dlfcn.h>
#include <errno.h>

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

// Work around some Unity code not handling EINTR
static int (*do_sem_wait)(sem_t*) = NULL;

int sem_wait(sem_t *sem)
{
  if (do_sem_wait == NULL)
  {
    do_sem_wait = dlvsym(RTLD_NEXT, "sem_wait", "GLIBC_2.1");
    if (do_sem_wait == NULL || do_sem_wait == sem_wait)
      abort();
  }

  for(;;)
  {
    int rv = do_sem_wait(sem);
    if (rv != -1 || errno != EINTR)
      return rv;
  }
}
