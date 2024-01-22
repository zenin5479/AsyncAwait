using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitContinuationDotPeek
{
   class MyClass
   {
      public void Operation()
      {
         Thread.Sleep(2000);
         Console.WriteLine("Основная задача");
      }

      public Task OperationAsync()
      {
         AsyncStateMachine stateMachine;
         stateMachine.Outer = this;
         stateMachine.Builder = AsyncTaskMethodBuilder.Create();
         stateMachine.State = -1;
         stateMachine.Builder.Start(ref stateMachine);
         return stateMachine.Builder.Task;
      }

      private struct AsyncStateMachine : IAsyncStateMachine
      {
         // для void OperationAsync() {...}
         //public AsyncVoidMethodBuilder builder;
         // для Task OperationAsync() {...}
         public AsyncTaskMethodBuilder Builder;
         public MyClass Outer;
         public int State;
         TaskAwaiter _awaiter;

         void IAsyncStateMachine.MoveNext()
         {
            if (State == -1)
            {
               _awaiter = Task.Factory.StartNew(Outer.Operation).GetAwaiter();
               State = 0;
               Builder.AwaitOnCompleted(ref _awaiter, ref this);
               return;
            }

            // Задача помечается как успешно выполненная, тогда срабатывает продолжение.
            Builder.SetResult();
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
         Task task = my.OperationAsync();
         task.ContinueWith(t => Console.WriteLine("Продолжение задачи"));

         // Задержка
         Console.ReadKey();
      }
   }
}