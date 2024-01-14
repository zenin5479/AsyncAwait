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
            Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Метод Operation запущен");
            Thread.Sleep(2000);
            Console.WriteLine("Метод Operation завершен");
        }

        public void OperationAsync()
        {
            AsyncStateMachine stateMachine;
            stateMachine.Outer = this;
            stateMachine.Builder = AsyncVoidMethodBuilder.Create();
            stateMachine.State = -1;
            stateMachine.Builder.Start(ref stateMachine);
        }

        private struct AsyncStateMachine : IAsyncStateMachine
        {
            public Program Outer;
            public AsyncVoidMethodBuilder Builder;
            public int State;

            void IAsyncStateMachine.MoveNext()
            {
                if (State == -1)
                {
                    Console.WriteLine("Метод OperationAsync (Часть 1). Идентификатор потока {0}", Thread.CurrentThread.ManagedThreadId);
                    Task task = new Task(Outer.Operation);
                    task.Start();
                    State = 0;
                    TaskAwaiter awaiter = task.GetAwaiter();
                    Builder.AwaitOnCompleted(ref awaiter, ref this);
                    return;
                }
                Console.WriteLine("Метод OperationAsync (Часть 2). Идентификатор потока {0}", Thread.CurrentThread.ManagedThreadId);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                /* Данный метод не играет важной роли в этом примере. */
                Builder.SetStateMachine(stateMachine);
            }
        }

        static void Main()
        {
            Console.WriteLine("Идентификатор потока метода Main: {0}", Thread.CurrentThread.ManagedThreadId);
            Program my = new Program();
            my.OperationAsync();

            // Задержка
            Console.ReadKey();
        }
    }
}