using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitOne
{
   class MyClass
   {
      public void Operation()
      {
         Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
         Console.WriteLine("Метод Operation запущен");
         Thread.Sleep(2000);
         Console.WriteLine("Метод Operation завершен");
      }

      public async void OperationAsync()
      {
         Task task = new Task(Operation);
         task.Start();
         await task;
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