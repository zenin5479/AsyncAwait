using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

// Код, сгенерированный компилятором. Декомпилирован с помощью JetBrains DotPeek в обработке.

namespace AsyncAwaitOneDotPeekClean
{
   internal class Program
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

   class MyClass
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
         stateMachine.Builder.Start(ref stateMachine);
      }

      struct AsyncStateMachine : IAsyncStateMachine
      {
         public MyClass Outer;
         public AsyncVoidMethodBuilder Builder;

         void IAsyncStateMachine.MoveNext()
         {
            Task task = new Task(Outer.Operation);
            task.Start();
         }

         void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
         {
            /* Данный метод не играет важной роли в этом примере. */
            throw new NotImplementedException();
         }
      }
   }
}