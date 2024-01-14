using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitTwo
{
    internal class Program
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
            // Id потока совпадает с Id первичного потока.
            // Это значит, что данный метод начинает выполняться в контексте первичного потока.
            Console.WriteLine("Метод OperationAsync (Часть 1). Идентификатор потока {0}", Thread.CurrentThread.ManagedThreadId);
            Task task = new Task(Operation);
            task.Start();
            await task;
            // Id потока совпадает с Id вторичного потока.
            // Это значит, что данный метод заканчивает выполняться в контексте вторичного потока.
            Console.WriteLine("Метод OperationAsync (Часть 2). Идентификатор потока {0}", Thread.CurrentThread.ManagedThreadId);
        }


        static void Main()
        {
            Console.WriteLine("Идентификатор потока метода Main: {0}", Thread.CurrentThread.ManagedThreadId);
            Program my = new Program();
            my.OperationAsync();

            // Задержка
            Console.ReadKey();
        }
    }
}