using System;
using System.Threading;
using WorkingWithParameterizedThreadStartDelegate.Models;

namespace WorkingWithParameterizedThreadStartDelegate
{
    // When you want to programmatically create additional threads to carry on some unit of work,
    // follow this predictable process when using the types of the System.Threading namespace:
    // 1. Create a method to be the entry point for the new thread.
    // 2. Create a new ParameterizedThreadStart(or ThreadStart) delegate, passing the
    // address of the method defined in step 1 to the constructor.
    // 3. Create a Thread object, passing the ParameterizedThreadStart/ThreadStart delegate
    // as a constructor argument.
    // 4. Establish any initial thread characteristics (name, priority, etc.).
    // 5. Call the Thread.Start() method. This starts the thread at the method referenced
    // by the delegate created in step 2 as soon as possible.
    class Program
    {
        // One simple and thread-safe way to force a thread to wait until another is completed is to use the AutoResetEvent class.
        // In the thread that needs to wait, create an instance of this class and pass in false to the constructor to
        // signify you have not yet been notified.Then, at the point at which you are willing to wait, call the WaitOne() method.
        static AutoResetEvent _waitHandle = new AutoResetEvent(false);

        static void Main()
        {
            // Recall that the ThreadStart delegate can point only to methods that return void and take no arguments.
            // While this might fit the bill in some cases, if you want to pass data to the method executing on the secondary
            // thread, you will need to use the ParameterizedThreadStart delegate type.
            Console.WriteLine("***** Adding with Thread objects *****");
            Console.WriteLine("ID of thread in Main(): {0}", Thread.CurrentThread.ManagedThreadId);

            // Make an AddParams object to pass to the secondary thread.
            AddParams ap = new AddParams(10, 10);

            Thread t = new Thread(new ParameterizedThreadStart(Add));
            t.Start(ap);

            // Force a wait to let other thread finish.
            //Thread.Sleep(5);

            // Wait here until you are notified!
            _waitHandle.WaitOne();
            Console.WriteLine("Other thread is done!");

            Console.ReadLine();
        }

        static void Add(object data)
        {
            if (data is AddParams ap)
            {
                Console.WriteLine("ID of thread in Add(): {0}", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("{0} + {1} is {2}", ap.a, ap.b, ap.a + ap.b);

                // Tell other thread we are done.
                _waitHandle.Set();
            }
        }
    }
}