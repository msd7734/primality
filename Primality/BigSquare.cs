using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primality
{
    public static class BigSquare
    {
        // Size of memo table; accounts for 64-bit entries (32-bit key and 32-bit value)
        static readonly int MEMOSIZE = 0x010000;

        // memoize powers
        static Dictionary<uint, uint> _powers = new Dictionary<uint, uint>(MEMOSIZE);

        private static uint SqModPower(uint a, uint b, uint n)
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

        public static uint SqAndMult(uint a, uint b, uint n)
        {
            // a^b mod n
            // Calculate via the square and multiply method

            uint res = 1;
            uint power = 0;

            for (int i = 0; b != 0 && i < 32; ++i)
            {
                if ((b & 0x1) == 1)
                {
                    power = (uint)(1 << i);
                    res *= SqModPower(a, power, n);
                    //_powers.Clear();
                }

                b >>= 1;
            }

            return res % n;
        }
    }
}
