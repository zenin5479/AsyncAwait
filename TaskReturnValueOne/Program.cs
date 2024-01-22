using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TaskReturnValueOne
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
         Task<long> task = Task<long>.Factory.StartNew(Operation);
         TaskAwaiter<long> awaiter = task.GetAwaiter();
         Action continuation = () => Console.WriteLine("Результат: {0}", awaiter.GetResult());
         awaiter.OnCompleted(continuation);
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