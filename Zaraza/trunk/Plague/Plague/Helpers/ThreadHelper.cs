using System.Collections.Generic;
using System.Threading;

namespace PlagueEngine.Helpers
{
    static class ThreadHelper
    {
       static private readonly List<Thread> Threads = new List<Thread>();

       public static Thread GetNewThread(ThreadStart threadStart, ThreadPriority threadPriority = ThreadPriority.Normal)
       {
           var thread = new Thread(threadStart) { Priority = threadPriority };
           thread.SetApartmentState(Program.ApartmentState);
           Threads.Add(thread);
           return thread;
       }
       public static Thread GetNewThread(ParameterizedThreadStart threadStart, ThreadPriority threadPriority = ThreadPriority.Normal)
       {
           var thread = new Thread(threadStart) { Priority = threadPriority };
           thread.SetApartmentState(Program.ApartmentState);
           Threads.Add(thread);
           return thread;
       }
       public static void Kill(Thread thread)
       {
           if (thread == null) return;
           thread.Abort();
           Threads.Remove(thread);
       }

       public static void KillAll()
       {
           foreach (var thread in Threads)
           {
               if (thread == null) break;
               thread.Abort();
           }
           Threads.Clear();
       }
    }
}
