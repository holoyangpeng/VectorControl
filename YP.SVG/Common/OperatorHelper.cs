using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Common
{
    public class OperatorHelper<T>
    {
        public static void Swap(ref T t1, ref T t2)
        {
            T temp = t2;
            t2 = t1;
            t1 = temp;
        }
    }
}
