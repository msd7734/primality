using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Primality;

namespace NaiveRSA
{
    class Program
    {

        static void Main(string[] args)
        {
            uint lowBound = (uint)Encoding.ASCII.GetBytes("A")[0];
            uint hiBound = (uint)Encoding.ASCII.GetBytes("Z")[0];

            uint n = 3763;
            uint e = 11;

            Console.WriteLine("x | e(x)");
            for (uint i = lowBound; i <= hiBound; ++i)
            {
                Console.WriteLine(
                    "{0} | {1}", 
                    Convert.ToChar(i),
                    BigSquare.SqAndMult(i, e, n)
                );
            }

            Console.ReadKey(true);
        }
    }
}
