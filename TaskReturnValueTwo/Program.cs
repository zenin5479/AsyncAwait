using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskReturnValueTwo
{
    class MyClass
    {
        long Operation()
        {
            Console.WriteLine("Идентификатор потока метода Operation: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            return Math.BigMul(1212, 2121);
        }

        // Async указывает, что метод является асинхронным.
        public async void OperationAsync()
        {
            Task<long> task = Task<long>.Factory.StartNew(Operation);
            // Await - ожидание завершения работы асинхронной задачи.
            Console.WriteLine("Результат: {0}", await task);
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