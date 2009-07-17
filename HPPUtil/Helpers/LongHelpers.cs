using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPUtil.Helpers
{
    public static class LongHelpers
    {
        public static long GetBitArrayLength(this long num)
        {
            long len = num/8;
            if(num % 8 != 0)
            {
                len++;
            }

            return len;
        }
    }
}
