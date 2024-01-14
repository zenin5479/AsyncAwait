using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitTwoDotPeekTesting
{
    internal class MyClass
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
            AsyncStateMachine stateMachine = default;
            stateMachine.Outer = this;
            stateMachine.Builder = AsyncVoidMethodBuilder.Create();
            stateMachine.State = -1;
            stateMachine.Builder.Start(ref stateMachine);
        }

        private struct AsyncStateMachine : IAsyncStateMachine
        {
            public MyClass Outer;
            public AsyncVoidMethodBuilder Builder;
            public int State;
            int _counterCallMoveNext;

            // builder.Start первый раз вызывает метод MoveNext - Синхронно, 
            // а второй раз builder.AwaitOnCompleted вызывает его - Асинхронно, только после того как отработает задача.
            void IAsyncStateMachine.MoveNext()
            {
                Console.WriteLine("Mетод MoveNext вызван {0}-й раз в потоке: {1}", ++_counterCallMoveNext, Thread.CurrentThread.ManagedThreadId);
                if (State == -1)
                {
                    Console.WriteLine("Метод OperationAsync (Часть 1). Идентификатор потока: {0}", Thread.CurrentThread.ManagedThreadId);
                    Task task = new Task(Outer.Operation);
                    task.Start();
                    State = 0;
                    TaskAwaiter awaiter = task.GetAwaiter();
                    // Закомментировать.
                    Builder.AwaitOnCompleted(ref awaiter, ref this);
                    // Не позволяет продолжиться методу (при первом вызове).
                    return;
                }

                // Срабатывает только при втором вызове метода MoveNext.
                Console.WriteLine("Метод OperationAsync (Часть 2). Идентификатор потока: {0}", Thread.CurrentThread.ManagedThreadId);
            }

            // builder.AwaitOnCompleted вызывает данный метод синхронно, во время выполнения задачи.
            // В .NET 4.7.2 выводится в консоль, в версиях .NET Core не выводится.
            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                Console.WriteLine("Метод SetStateMachine Идентификатор потока: {0}", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("stateMachine.GetHashCode: {0}", stateMachine.GetHashCode());
                Console.WriteLine("this.GetHashCode: {0}", GetHashCode());
                Builder.SetStateMachine(stateMachine);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Идентификатор потока метода Main: {0}", Thread.CurrentThread.ManagedThreadId);
            MyClass my = new MyClass();
            my.OperationAsync();

            // Задержка
            Console.ReadKey();
        }
    }
}