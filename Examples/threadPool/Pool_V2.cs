using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class Pool_V2
{

    /// <summary>
    /// Queue of worker threads, ready to process tasks
    /// </summary>
    private readonly LinkedList<Thread> FreeWorkers;

    /// <summary>
    /// Queue of tasks, waiting to process
    /// </summary>
    private readonly LinkedList<Task > TaskQueue = new LinkedList<Task >();

    /// <summary>
    /// object for locking work with TaskQueue
    /// </summary>
    private object TaskQueueLock = new object();

    /// <summary>
    /// Pool queueing is disable, but there are still tasks pending
    /// </summary>
    private bool QueueingDisabled;

    /// <summary>
    /// All tasks are done, pool doesnt accept any new tasks
    /// </summary>
    private bool IsStopped;

    /// <summary>
    /// how many times we will choose high priority tasks over normal priority
    /// </summary>
    private int HighCountLeftBeforeWeCanChooseNormalPriority { get; set; } = 3;

    public Pool_V2(int size)
    {
        this.FreeWorkers = new LinkedList<Thread>();
        
        for (var i = 0; i < size; ++i)
        {
            var worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
            worker.Start();
            this.FreeWorkers.AddLast(worker);
        }
    }

    public void Stop()
    {
        var waitForThreads = false;
        lock (this.TaskQueueLock)
        {
            if (!this.IsStopped)
            {
                //forbid add new tasks
                this.QueueingDisabled = true; 
                // wait for all tasks to finish
                while (this.TaskQueue.Count > 0)
                {
                    Monitor.Wait(this.TaskQueueLock);
                }

                this.IsStopped = true;
                // wake all workers, at this point all of them are not active, so because of flag IsStopped they will "return" and stop their existence
                Monitor.PulseAll(this.TaskQueueLock); 
                waitForThreads = true;
            }
        }
        if (waitForThreads)
        {
            foreach (var worker in this.FreeWorkers)
            {
                worker.Join();
            }
        }
    }

    public bool QueueTask(Action task, TaskPriority priority)
    {
        lock (this.TaskQueueLock)
        {
            if (this.IsStopped || this.QueueingDisabled) { return false; }
            this.TaskQueue.AddLast(new Task(task, priority));
            //pulse because tasks count changed
            Monitor.PulseAll(this.TaskQueueLock);
            return true;
        }
    }

    private void Worker()
    {
        while (true) 
        {
            Task task = null;
            lock (this.TaskQueueLock)
            {
                // wait for our turn in FreeWorkers and an available task
                while (true) 
                {
                    if (this.IsStopped)
                    {
                        return;
                    }
                    if (this.FreeWorkers.First !=  null //no free workers
                        && object.ReferenceEquals(Thread.CurrentThread, this.FreeWorkers.First.Value) //it our turn, because this thread is first in FreeWorkers list
                        && this.TaskQueue.Count > 0) // there is a task to process
                    {
                        task = GetTaskToProcess();
                        this.TaskQueue.Remove(task);
                        this.FreeWorkers.RemoveFirst();

                        // pulse because First worker changed so that next available sleeping worker now can pick its task
                        Monitor.PulseAll(this.TaskQueueLock); 
                        break; 
                    }
                    // go to sleep, if no task to process or not our turn
                    Monitor.Wait(this.TaskQueueLock); 
                }
            }

            task.Execute();

            lock (this.TaskQueueLock)
            {
                this.FreeWorkers.AddLast(Thread.CurrentThread);
            }
        }
    }

    private Task GetTaskToProcess()
    {
        lock (this.TaskQueueLock)
        {
            TaskPriority nextTaskPriority = TaskPriority.High;
            if (this.TaskQueue.All(x => x.TaskPriority == TaskPriority.Low))
            {
                nextTaskPriority = TaskPriority.Low;
            }
            else if(HighCountLeftBeforeWeCanChooseNormalPriority <= 0 && this.TaskQueue.Any(x=>x.TaskPriority == TaskPriority.High))
            {
                nextTaskPriority = TaskPriority.High;
                HighCountLeftBeforeWeCanChooseNormalPriority = 3;
            }
            else if (this.TaskQueue.Any(x => x.TaskPriority == TaskPriority.Normal))
            {
                nextTaskPriority = TaskPriority.Normal;
                HighCountLeftBeforeWeCanChooseNormalPriority--;
            }

            return this.TaskQueue.First(x => x.TaskPriority == nextTaskPriority);
        }
    }
}

public enum TaskPriority
{
    High = 0,
    Normal,
    Low
}

public class Task 
{
    public Action Action { get; set; }

    public TaskPriority TaskPriority { get; set; }

    public Task (Action action, TaskPriority taskPriority)
    {
        TaskPriority = taskPriority;
        Action = action;
    }
    public void Execute()
    {
        Action();
    }
}

//public static class Program
//{
//    static void Main()
//    {
//        var pool = new Pool_V2(5);
//        var random = new Random();
//        Action<int> longAction = (index =>
//        {
//            Console.WriteLine("{0}: Start work #{1}", Thread.CurrentThread.Name, index);
//            Thread.Sleep(random.Next(100, 1000));
//            Console.WriteLine("{0}: End work #{1}", Thread.CurrentThread.Name, index);
//        });
//        Array values = Enum.GetValues(typeof(TaskPriority));

//        for (var i = 0; i < 100; ++i)
//        {
//            //we need to save it in new local variable for every task
//            var queueArgument = i;
//            var priority = (TaskPriority)values.GetValue(random.Next(values.Length));
//            pool.QueueTask(() => longAction(queueArgument), priority);
//        }

//        Console.ReadKey();
//    }
//}