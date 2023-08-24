using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
class Program
{
    static int NOD(int x, int y)
    {
        while (x != y)
        {
            if (x > y)
                x = x - y;
            else
                y = y - x;
        }
        return x;
    }
    static void Main(string[] args)
    {
        //$"{Environment.CurrentDirectory}\\list.txt";
        string path = $"{Environment.CurrentDirectory}\\test.txt";
        string path2 = $"{Environment.CurrentDirectory}\\answer.txt";
        var semaphore = new SemaphoreSlim(0);
        var semaphore2 = new SemaphoreSlim(0);
        var concurrent = new ConcurrentQueue<string>();
        var concurrent2 = new ConcurrentQueue<string>();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Thread myThread0 = new Thread(threadaction0);
        Thread myThread1 = new Thread(threadaction1);
        Thread myThread2 = new Thread(threadaction2);
        void threadaction0()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    concurrent.Enqueue(line);
                    semaphore.Release();
                }
            }
        }

        void threadaction1()
        {
            semaphore.Wait();
            while (concurrent.IsEmpty == false)
            {
                string value;
                concurrent.TryDequeue(out value);
                string[] xxx = value.Split(char.Parse(" "));
                int a = Convert.ToInt32(xxx[0]);
                int b = Convert.ToInt32(xxx[1]);
                concurrent2.Enqueue(Convert.ToString(NOD(a, b)));
                semaphore2.Release();
            }
        }


        void threadaction2()
        {
            semaphore2.Wait();
            while (concurrent2.IsEmpty == false)
            {
                using (StreamWriter writer = new StreamWriter(path2, true))
                {

                    string value2;
                    concurrent2.TryDequeue(out value2);
                    writer.WriteLineAsync(value2);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Время затрачено: {stopwatch.ElapsedMilliseconds}");
            Console.ReadKey();
        }
        myThread0.Start();
        myThread1.Start();
        myThread2.Start();
        myThread0.Join();
        myThread1.Join();
        myThread2.Join();
    }
}