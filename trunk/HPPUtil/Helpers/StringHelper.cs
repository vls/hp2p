using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPUtil
{
    public static class StringHelper
    {
        /// <summary>
        /// 计算一个字符串在UTF-8模式下所占字节数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字节数</returns>
        public static long GetUTF8Count(this string str)
        {
            int strLength = 0;
            foreach (char c in str)
            {
                if(((int)c) <= 0x7F)
                {
                    strLength++;
                }
                else if(c <= 0x7FF)
                {
                    strLength += 2;
                }
                else if(c <= 0xFFFF)
                {
                    strLength += 3;
                }
            }

            return strLength;

        }

    }
}
