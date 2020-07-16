using System;
using System.Collections.Generic;
using System.Threading;


public sealed class Pool_V1
{
    /// <summary>
    /// Queue of worker threads, ready to process tasks
    /// </summary>
    private readonly LinkedList<Thread> FreeWorkers;

    /// <summary>
    /// Queue of tasks, waiting to process
    /// </summary>
    private readonly LinkedList<Action> TaskQueue = new LinkedList<Action>();

    /// <summary>
    /// object for locking work with TaskQueue
    /// </summary>
    private object TaskQueueLock = new object();

    public Pool_V1(int size)
    {
        this.FreeWorkers = new LinkedList<Thread>();
        
        for (var i = 0; i < size; ++i)
        {
            var worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
            worker.Start();
            this.FreeWorkers.AddLast(worker);
        }
    }

    public void QueueTask(Action task)
    {
        lock (this.TaskQueueLock)
        {
            this.TaskQueue.AddLast(task);
            //pulse because tasks count changed
            Monitor.PulseAll(this.TaskQueueLock);
        }
    }

    private void Worker()
    {
        while (true) 
        {
            Action task = null;
            lock (this.TaskQueueLock)
            {
                // wait for our turn in FreeWorkers and an available task
                while (true) 
                {
                    if (this.FreeWorkers.First !=  null //no free workers
                        && object.ReferenceEquals(Thread.CurrentThread, this.FreeWorkers.First.Value) //it our turn, because this thread is first in FreeWorkers list
                        && this.TaskQueue.Count > 0) // there is a task to process
                    {
                        task = this.TaskQueue.First.Value;
                        this.TaskQueue.RemoveFirst();
                        this.FreeWorkers.RemoveFirst();

                        // pulse because First worker changed so that next available sleeping worker now can pick its task
                        Monitor.PulseAll(this.TaskQueueLock);
                        break; 
                    }
                    // go to sleep, if no task to process or not our turn
                    Monitor.Wait(this.TaskQueueLock);
                }
            }

            task(); 
            lock (this.TaskQueueLock)
            {
                this.FreeWorkers.AddLast(Thread.CurrentThread);
            }
        }
    }
}


//public static class Program
//{
//    static void Main()
//    {
//        var pool = new Pool_V1(5);
//        var random = new Random();
//        Action<int> longAction = (index =>
//        {
//            Console.WriteLine("{0}: Start work #{1}", Thread.CurrentThread.Name, index);
//            Thread.Sleep(random.Next(100, 1000));
//            Console.WriteLine("{0}: End work #{1}", Thread.CurrentThread.Name, index);
//        });

//        for (var i = 0; i < 100; ++i)
//        {
//            //we need to save it in new local variable for every task
//            var queueArgument = i;
//            pool.QueueTask(() => longAction(queueArgument));
//        }

//        Console.ReadKey();
//    }
//}