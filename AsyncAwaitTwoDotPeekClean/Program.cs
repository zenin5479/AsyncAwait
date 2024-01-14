using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncAwaitTwoDotPeekClean
{
    internal class Program
    {
        public void Operation()
        {
            Console.WriteLine("Operation ThreadID {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Begin");
            Thread.Sleep(2000);
            Console.WriteLine("End");
        }

        public void OperationAsync()
        {
            AsyncStateMachine stateMachine;
            stateMachine.outer = this;
            stateMachine.builder = AsyncVoidMethodBuilder.Create();
            stateMachine.state = -1;
            stateMachine.builder.Start(ref stateMachine);
        }

        private struct AsyncStateMachine : IAsyncStateMachine
        {
            public Program outer;
            public AsyncVoidMethodBuilder builder;
            public int state;

            void IAsyncStateMachine.MoveNext()
            {
                if (state == -1)
                {
                    Console.WriteLine("OperationAsync (Part I) ThreadID {0}\n", Thread.CurrentThread.ManagedThreadId);

                    Task task = new Task(outer.Operation);
                    task.Start();

                    state = 0;
                    TaskAwaiter awaiter = task.GetAwaiter();
                    builder.AwaitOnCompleted(ref awaiter, ref this);
                    return;
                }

                Console.WriteLine("\nOperationAsync (Part II) ThreadID {0}", Thread.CurrentThread.ManagedThreadId);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                /* Данный метод не играет важной роли в этом примере. */
                builder.SetStateMachine(stateMachine);
            }
        }

        static void Main()
        {
            Console.WriteLine("Main ThreadID {0}\n", Thread.CurrentThread.ManagedThreadId);
            Program my = new Program();
            my.OperationAsync();

            // Delay
            Console.ReadKey();
        }
    }
}