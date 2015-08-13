using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace testTask
{


    class Worker
    {
        private static readonly int THREAD_EXIT_FLAG = -1;

        private readonly MyBlockingQueue<Task> queue = new MyBlockingQueue<Task>();
        private readonly MyBlockingQueue<Task> outputQueue = new MyBlockingQueue<Task>();
        private readonly MyBlockingQueue<Buffer> bufferQueue = new MyBlockingQueue<Buffer>();
        private readonly List<Thread> threads;
        private readonly IHash hasher;
        private readonly FileStream stream;
        private readonly int threadCount;
        private readonly long blockCount;
        private readonly int blockLength;
        private readonly Dictionary<int, String> result = new Dictionary<int, string>();
        private readonly Thread mainThread;
        private  Thread printThread;


        private volatile bool stopRead = false;
        

        public Worker(int threadCount, int blockLength, IHash hasher, FileStream stream)
        {
            mainThread = Thread.CurrentThread;
            this.threadCount = threadCount;
            this.stream = stream;
            this.hasher = hasher;
            

            this.blockLength = blockLength;
            threads = new List<Thread>(threadCount);
            

            for (int i = 0; i < threadCount; ++i)
            {
                var b = new Buffer();
                b.raw = new byte[blockLength];
                bufferQueue.AddToQueue(b);
            }

            blockCount = stream.Length / blockLength;
            if (stream.Length % blockLength != 0)
            {
                blockCount++;
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Cancel);
        }

        public void Work()
        {
            StartThreads();
            ReadFile();
            StopThreads();

        }

        private void ReadFile()
        {
            int num = 0;

            while (true)
            {
                if (stopRead)
                {
                    break;
                }
                var buffer = bufferQueue.Take();

                buffer.bytesCount = stream.Read(buffer.raw, 0, blockLength);
                if (buffer.bytesCount > 0)
                {
                    queue.AddToQueue(new Task(buffer, num++));
                }
                else
                {
                    break;
                }
            }
        }

        private void StopThreads()
        {   
            for (int i = 0; i < threadCount; i++)
            {
                queue.AddToQueue(new Task(null, THREAD_EXIT_FLAG));
            }
            foreach (var t in threads)
            {
                t.Join();
            }
            outputQueue.AddToQueue(new Task(null, THREAD_EXIT_FLAG));
            printThread.Join();
        }

        private void StartThreads()
        {
            for (int i = 0; i < threadCount; ++i)
            {
                var t = new Thread(() => HashBytes());
                threads.Add(t);
                t.Start();
            }
            printThread = new Thread(() => Print(blockCount));
            printThread.Start();
           // threads.Add(printThread);
        }

        private void Print(long blockCount)
        {
            for (int i = 0; i < blockCount; i++)
            {
               
                if (result.ContainsKey(i))
                {
                    Console.WriteLine("{0} : {1}", i + 1, result[i]);
                    continue;
                }
                while (true)
                {
                    
                    Task t = outputQueue.Take();
                    if (t.number == THREAD_EXIT_FLAG)
                    {
                        return;
                    }
                    if (t.number == i)
                    {
                        Console.WriteLine("{0} : {1}", i + 1, t.hashStr);
                        break;
                    }
                    else
                    {
                        result.Add(t.number, t.hashStr);
                    }
                }
            }
        }

        private void HashBytes()
        {
            for (Task t = queue.Take(); t.number != THREAD_EXIT_FLAG; t = queue.Take())
            {
                string hash = hasher.Hash(t.buffer.raw, t.buffer.bytesCount);
                outputQueue.AddToQueue(new Task(hash, t.number));
                bufferQueue.AddToQueue(t.buffer);
            }

        }

        private void Cancel(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("(Ctrl + C)");
            e.Cancel = true;
            stopRead = true;
            mainThread.Join();
            Environment.Exit(0);
        }

    }
}
