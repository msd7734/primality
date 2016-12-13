// C# Implementation of the Miller-Rabin primality test
// By: Matthew Dennis (msd7734)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primality
{
    class Program
    {
        // Size of memo table; accounts for 64-bit entries (32-bit key and 32-bit value)
        static readonly int MEMOSIZE = 0x010000;

        // memoize powers
        static Dictionary<uint, uint> _powers = new Dictionary<uint, uint>(MEMOSIZE);

        static uint SqModPower(uint a, uint b, uint n)
        {
            // modular square for an exponent b that's a power of 2
            // a^b mod n

            if (b == 0)
                return 1;

            // cantor pairing function
            // fast key function where inputs a and b are non-commutative
            uint key = (((a + b) * (a + b + 1)) >> 1) + b; 
            key = key ^ n;


            if (_powers.ContainsKey(key))
                return _powers[key];
                

            if (b == 1)
            {
                if (_powers.Count == MEMOSIZE)
                    _powers.Clear();
                    

                uint res = a % n;
                _powers.Add(key, res);
                return res;
            }    
            else
            {
                uint halfpower = SqModPower(a, b >> 1, n);
                uint res = (halfpower * halfpower) % n;

                if (_powers.Count == MEMOSIZE)
                    _powers.Clear();
                    
                _powers.Add(key, res);
                return res;
            }
        }

        static uint SqAndMult(uint a, uint b, uint n)
        {
            // a^b mod n
            // Calculate via the square and multiply method

            uint res = 1;
            uint power = 0;

            for (int i = 0; b != 0 && i < 32; ++i)
            {
                if ((b & 0x1) == 1)
                {
                    // make i uint to avoid this cast, dummy
                    power = (uint)(1 << i);
                    res *= SqModPower(a, power, n);
                    //_powers.Clear();
                }

                b >>= 1;
            }

            return res % n;
        }

        static bool DetIsPrime(uint n, params uint[] avals)
        {
            // A "slightly more deterministic" Miller-Rabin calculation
            //  by using given a-values
            // Note: does not check a-values for correctness

            if (n < 2)
            {
                return false;
            }
            else if ((n & 0x1) == 0 && n != 2)
            {
                return false;
            }

            uint s = n - 1;
            while (s % 2 == 0)
            {
                s >>= 1;
            }

            for (int i = 0; i < avals.Length; i++)
            {
                uint a = avals[i];
                uint d = s;
                uint mod = SqAndMult(a, d, n);
                while (d != n - 1 && mod != 1 && mod != n - 1)
                {
                    mod = (mod * mod) % n;
                    d *= 2;
                }
                if (mod != n - 1 && (d & 0x1) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        static decimal ErrorOfMR(uint n)
        {
            decimal count = 0;

            // for small integers, testing a = 2 and a = 3 makes MR deterministic (allegedly...)
            bool trueAnswer = DetIsPrime(n, 2, 3);
            // the probabilistic answer
            bool probAnswer;

            for (uint a = 1; a < n; ++a)
            {
                probAnswer = DetIsPrime(n, a);
                if (probAnswer != trueAnswer)
                    ++count;
            }

            return count / (n - 1);
        }

        static void Main(string[] args)
        {
            uint lowBound = 105000;
            uint hiBound = 115000;

            
            // decimal type gives 128 bits of precision
            decimal greatestError = 0;
            decimal error = 0;

            Dictionary<uint, decimal> maxErrVals = new Dictionary<uint, decimal>(10);

            for (uint i = lowBound + 1; i < hiBound; i += 2)
            {
                error = ErrorOfMR(i);

                // maintain bucket of 10 highest values at any time
                if (maxErrVals.Keys.Count < 10)
                        maxErrVals.Add(i, error);
                else if (error >= maxErrVals.Values.Min())
                {
                    maxErrVals.Remove(
                        maxErrVals.First(
                            kv => kv.Value == maxErrVals.Values.Min()
                        )
                        .Key
                    );
                    maxErrVals.Add(i, error);
                }

                if (error > greatestError)
                {
                    greatestError = error;
                    Console.WriteLine("New greatest error found: {0}", greatestError);
                }
            }

            Console.WriteLine("Max error: {0}", greatestError);
            Console.WriteLine("Maximized error values: \n{{\n{0}\n}}", String.Join(",\n", maxErrVals));
            Console.Write("Press any key to exit.");
            
            Console.ReadKey(true);
        }
    }
}
