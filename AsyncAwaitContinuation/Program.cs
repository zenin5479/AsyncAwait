using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitContinuation
{
   class MyClass
   {
      public void Operation()
      {
         Thread.Sleep(2000);
         Console.WriteLine("Основная задача");
      }

      public async Task OperationAsync()
      {
         // return указывать не нужно, т.к. Await сформирует return (неявно) самостоятельно.
         await Task.Factory.StartNew(Operation);
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