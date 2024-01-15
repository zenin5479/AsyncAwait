using System;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncAwaitReturnArgument
{
    class MyClass
    {
        long Operation(object argument)
        {
            Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1000);
            return Math.BigMul((int)argument, (int)argument);
        }

        public async Task<long> OperationAsync(int argument)
        {
            return await Task<long>.Factory.StartNew(Operation, argument);
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