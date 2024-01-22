using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// Код, сгенерированный компилятором. Декомпилирован с помощью JetBrains DotPeek

namespace AsyncAwaitOneDotPeek
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
         MyClass.AsyncStateMachine stateMachine;
         stateMachine.outer = this;
         stateMachine.builder = AsyncVoidMethodBuilder.Create();
         stateMachine.state = -1;
         stateMachine.builder.Start<MyClass.AsyncStateMachine>(ref stateMachine);
      }

      [CompilerGenerated]
      [StructLayout(LayoutKind.Auto)]
      private struct AsyncStateMachine : IAsyncStateMachine
      {
         public int state;
         public AsyncVoidMethodBuilder builder;
         public MyClass outer;
         private TaskAwaiter awaiter;

         void IAsyncStateMachine.MoveNext()
         {
            int num1 = this.state;
            try
            {
               TaskAwaiter awaiter;
               int num2;
               if (num1 == 0 || num1 != 1)
               {
                  // ISSUE: method pointer
                  Task task = new Task(new Action(outer.Operation));
                  task.Start();
                  awaiter = task.GetAwaiter();
                  if (!awaiter.IsCompleted)
                  {
                     this.state = num2 = 1;
                     this.awaiter = awaiter;
                     this.builder.AwaitUnsafeOnCompleted<TaskAwaiter, MyClass.AsyncStateMachine>(ref awaiter, ref this);
                     return;
                  }
               }
               else
               {
                  awaiter = this.awaiter;
                  this.awaiter = new TaskAwaiter();
                  this.state = num2 = -1;
               }
               awaiter.GetResult();
               awaiter = new TaskAwaiter();
            }
            catch (Exception ex)
            {
               this.state = -2;
               this.builder.SetException(ex);
               return;
            }
            this.state = -2;
            this.builder.SetResult();
         }

         [DebuggerHidden]
         void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
         {
            this.builder.SetStateMachine(stateMachine);
         }
      }
   }
}