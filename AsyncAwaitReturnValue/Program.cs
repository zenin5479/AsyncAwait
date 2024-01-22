using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitReturnValue
{
   class MyClass
   {
      long Operation()
      {
         Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
         Thread.Sleep(2000);
         return Math.BigMul(1212, 2121);
      }

      public async Task<long> OperationAsync()
      {
         //int result = await Task<int>.Factory.StartNew(Operation);
         //return result;
         return await Task<long>.Factory.StartNew(Operation);
      }
   }

   internal class Program
   {
      static void Main()
      {
         MyClass my = new MyClass();
         Task<long> task = my.OperationAsync();
         Console.WriteLine("Первичный поток завершил работу. Идентификатор потока метода {0}", Thread.CurrentThread.ManagedThreadId);
         task.ContinueWith(t => Console.WriteLine("Результат : {0}", t.Result));

         // Задержка
         Console.ReadKey();
      }
   }
}