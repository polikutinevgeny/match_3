using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match_3
{
    public static class Utility
    {
        public static void Swap<T>(ref T l, ref T r)
        {
            T temp = l;
            l = r;
            r = temp;
        }
    }
}
