using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

namespace ParallelTasksExample
{
    class Operat
    {
        public static BigInteger Fibbonaci(int maxIter)
        {
            BigInteger F2 = 0;
            BigInteger F1 = 1;
            BigInteger CurrNum = 0;

            for(int i = 0; i<maxIter-1; i++)
            {
                CurrNum = F1 + F2;
                F2 = F1;
                F1 = CurrNum;

            }

            return CurrNum;
        }


    }
}
