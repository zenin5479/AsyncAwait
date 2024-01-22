using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TaskReturnValueTwoDotPeekClean
{
   class MyClass
   {
      long Operation()
      {
         Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
         Thread.Sleep(2000);
         return Math.BigMul(1212, 2121);
      }

      public void OperationAsync()
      {
         AsyncStateMachine stateMachine;
         stateMachine.Outer = this;
         stateMachine.Builder = AsyncVoidMethodBuilder.Create();
         stateMachine.State = -1;
         stateMachine.Builder.Start(ref stateMachine);
      }

      struct AsyncStateMachine : IAsyncStateMachine
      {
         public MyClass Outer;
         public AsyncVoidMethodBuilder Builder;
         public int State;
         TaskAwaiter<long> _awaiter;

         void IAsyncStateMachine.MoveNext()
         {
            // Первая половина метода выполнится в первичном потоке.
            if (State == -1)
            {
               Func<long> function = Outer.Operation;
               Task<long> task = Task<long>.Factory.StartNew(function);
               State = 0;
               _awaiter = task.GetAwaiter();
               Builder.AwaitOnCompleted(ref _awaiter, ref this);
               return;
            }

            // Вторая половина метода выполнится во вторичном потоке.
            long result = _awaiter.GetResult();
            Console.WriteLine("Результат: {0}", result);
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
         my.OperationAsync();
         Console.WriteLine("Первичный поток завершил работу. Идентификатор потока метода {0}", Thread.CurrentThread.ManagedThreadId);

         // Задержка
         Console.ReadKey();
      }
   }
}