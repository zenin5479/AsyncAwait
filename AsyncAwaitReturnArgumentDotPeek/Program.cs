using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitReturnArgumentDotPeek
{
    class MyClass
    {
        long Operation(object argument)
        {
            Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1000);
            return Math.BigMul((int)argument, (int)argument);
        }

        public Task<long> OperationAsync(int argument)
        {
            AsyncStateMachine stateMachine;
            stateMachine.Outer = this;
            stateMachine.Builder = AsyncTaskMethodBuilder<long>.Create();
            stateMachine.State = -1;
            stateMachine.Argument = argument;
            stateMachine.Builder.Start(ref stateMachine);
            return stateMachine.Builder.Task;
        }

        struct AsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<long> Builder;
            public MyClass Outer;
            public int State;
            public int Argument;
            TaskAwaiter<long> _awaiter;

            void IAsyncStateMachine.MoveNext()
            {
                if (State == -1)
                {
                    Func<object, long> function = Outer.Operation;
                    Task<long> task = Task<long>.Factory.StartNew(function, Argument);
                    _awaiter = task.GetAwaiter();
                    State = 0;
                    Builder.AwaitOnCompleted(ref _awaiter, ref this);
                    return;
                }

                long result = _awaiter.GetResult();
                Builder.SetResult(result);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                Builder.SetStateMachine(stateMachine);
            }
        }
    }

    internal class Program
    {
        static void Main()
        {
            MyClass my = new MyClass();
            Task<long> task = my.OperationAsync(2121212121);
            Console.WriteLine("Первичный поток завершил работу. Идентификатор потока метода {0}", Thread.CurrentThread.ManagedThreadId);
            task.ContinueWith(t => Console.WriteLine("Результат : {0}", t.Result));

            // Задержка
            Console.ReadKey();
        }
    }
}