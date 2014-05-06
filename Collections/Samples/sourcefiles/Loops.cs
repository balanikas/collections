using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samples
{
    public class Loops
    {
        public void StandardForEach()
        {
            int loopCount = 10;
            var items = new List<int>();
            for (int i = 0; i < loopCount; i++)
            {
                items.Add(i);
            }

            foreach (int item in items)
            {
                long nthPrime = FindPrimeNumber(100);
            }
        }

        public void ParallelForEach()
        {
            int loopCount = 10;
            var items = new List<int>();
            for (int i = 0; i < loopCount; i++)
            {
                items.Add(i);
            }
            Parallel.ForEach(items, x => { long nthPrime = FindPrimeNumber(100); });
        }

        public void StandardLoop()
        {
            int loopCount = 10;

            for (int i = 0; i < loopCount; i++)
            {
                long nthPrime = FindPrimeNumber(100);
            }
        }

        public void ParallelLoop()
        {
            int loopCount = 10;


            Parallel.For(0, loopCount, x => { long nthPrime = FindPrimeNumber(100); });
        }

        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1; // to check if found a prime
                while (b*b <= a)
                {
                    if (a%b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                    count++;
                a++;
            }
            return (--a);
        }
    }
}