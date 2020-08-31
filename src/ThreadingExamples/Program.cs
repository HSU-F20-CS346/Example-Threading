using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskExample().Wait();
            ThreadExample();
        }

        static async Task TaskExample()
        {
            //tasks are great for one-off calculations that can be done in parallel

            //find some primes
            Console.WriteLine("Finding primes between 1 and 1000000");
            var primes = Task.Run(() => { return FindPrimes(1, 1000000); });

            //and also find pi
            Console.WriteLine("Calculating pi to the 10000th digit");
            var pi = Task.Run(() => { return CalculatePi(10000); });

            //you can do as many of these as you want.  When you're ready to get the results
            //use await
            var primesResult = await primes;
            var piResult = await pi;

            //afer await, you can use the return values from the function
            Console.WriteLine("Found {0} primes", primesResult.Count);
            Console.WriteLine("Pi: {0}", piResult);
        }

        static void ThreadExample()
        {
            //threads are better for something that always has to run in the background
            ThreadStart start = TimeTeller;
            Thread timeThread = new Thread(start);
            timeThread.Start();

            //join says to wait until the thread finishes (which it won't in our case)
            timeThread.Join();

            //In practice, threading ends up being much more difficult than tasks due
            //to the fact that you usually have to synchronize behaviors between multiple
            //threads.  Failing to do this properly can cause bad program behavior 
            //(e.g. crashing, memory leaks, "deadlock", data being overwritten, etc.).  
        }

        static void TimeTeller()
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                DateTime end = DateTime.Now;
                var difference = end - start;
                if(difference.TotalSeconds > 1)
                {
                    //a second has elapsed
                    start = end;

                    //output new time
                    Console.WriteLine(end.TimeOfDay);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        //from https://www.dotnetperls.com/prime
        public static bool IsPrime(int candidate)
        {
            // Test whether the parameter is a prime number.
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Note:
            // ... This version was changed to test the square.
            // ... Original version tested against the square root.
            // ... Also we exclude 1 at the end.
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }

        private static List<int> FindPrimes(int min, int max)
        {
            List<int> primes = new List<int>();
            for(int i = min; i < max; i++)
            {
                if(IsPrime(i))
                {
                    primes.Add(i);
                }
            }
            return primes;
        }

        //from https://stackoverflow.com/questions/11677369/how-to-calculate-pi-to-n-number-of-places-in-c-sharp-using-loops/11679007
        public static string CalculatePi(int digits)
        {
            digits++;

            uint[] x = new uint[digits * 10 / 3 + 2];
            uint[] r = new uint[digits * 10 / 3 + 2];

            uint[] pi = new uint[digits];

            for (int j = 0; j < x.Length; j++)
                x[j] = 20;

            for (int i = 0; i < digits; i++)
            {
                uint carry = 0;
                for (int j = 0; j < x.Length; j++)
                {
                    uint num = (uint)(x.Length - j - 1);
                    uint dem = num * 2 + 1;

                    x[j] += carry;

                    uint q = x[j] / dem;
                    r[j] = x[j] % dem;

                    carry = q * num;
                }


                pi[i] = (x[x.Length - 1] / 10);


                r[x.Length - 1] = x[x.Length - 1] % 10; ;

                for (int j = 0; j < x.Length; j++)
                    x[j] = r[j] * 10;
            }

            var result = "";

            uint c = 0;

            for (int i = pi.Length - 1; i >= 0; i--)
            {
                pi[i] += c;
                c = pi[i] / 10;

                result = (pi[i] % 10).ToString() + result;
            }

            return result;
        }
    }
}
