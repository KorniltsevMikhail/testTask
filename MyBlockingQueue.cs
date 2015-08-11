using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testTask
{
     class MyBlockingQueue<Task> : Queue<Task>
    {
        private readonly object pulse = new object();
        
        public MyBlockingQueue ()
        {

        }
        public void AddToQueue(Task t)
        {
            lock (pulse)
            {
                this.Enqueue(t);
                Monitor.Pulse(pulse);                
            }
        }
        public Task Take()
        {
            lock (pulse)
            {
                while (this.Count == 0) {
                    Monitor.Wait(pulse);
                }                
                return Dequeue();
                
            }
        }

    }
}
