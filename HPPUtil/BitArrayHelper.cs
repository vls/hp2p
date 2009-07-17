using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HPPUtil
{
    public class BitArrayHelper
    {
        /// <summary>
        /// _table是一个保存0-255中每个数字和它对应的二进制数中哪些位是1
        /// </summary>
        private static Dictionary<int, List<int>> _table = new Dictionary<int, List<int>>()
            {
#region
                {0, new List<int>(){}},
                {1, new List<int>(){1}},
                {2, new List<int>(){2}},
                {3, new List<int>(){1,2}},
                {4, new List<int>(){3}},
                {5, new List<int>(){1,3}},
                {6, new List<int>(){2,3}},
                {7, new List<int>(){1,2,3}},
                {8, new List<int>(){4}},
                {9, new List<int>(){1,4}},
                {10, new List<int>(){2,4}},
                {11, new List<int>(){1,2,4}},
                {12, new List<int>(){3,4}},
                {13, new List<int>(){1,3,4}},
                {14, new List<int>(){2,3,4}},
                {15, new List<int>(){1,2,3,4}},
                {16, new List<int>(){5}},
                {17, new List<int>(){1,5}},
                {18, new List<int>(){2,5}},
                {19, new List<int>(){1,2,5}},
                {20, new List<int>(){3,5}},
                {21, new List<int>(){1,3,5}},
                {22, new List<int>(){2,3,5}},
                {23, new List<int>(){1,2,3,5}},
                {24, new List<int>(){4,5}},
                {25, new List<int>(){1,4,5}},
                {26, new List<int>(){2,4,5}},
                {27, new List<int>(){1,2,4,5}},
                {28, new List<int>(){3,4,5}},
                {29, new List<int>(){1,3,4,5}},
                {30, new List<int>(){2,3,4,5}},
                {31, new List<int>(){1,2,3,4,5}},
                {32, new List<int>(){6}},
                {33, new List<int>(){1,6}},
                {34, new List<int>(){2,6}},
                {35, new List<int>(){1,2,6}},
                {36, new List<int>(){3,6}},
                {37, new List<int>(){1,3,6}},
                {38, new List<int>(){2,3,6}},
                {39, new List<int>(){1,2,3,6}},
                {40, new List<int>(){4,6}},
                {41, new List<int>(){1,4,6}},
                {42, new List<int>(){2,4,6}},
                {43, new List<int>(){1,2,4,6}},
                {44, new List<int>(){3,4,6}},
                {45, new List<int>(){1,3,4,6}},
                {46, new List<int>(){2,3,4,6}},
                {47, new List<int>(){1,2,3,4,6}},
                {48, new List<int>(){5,6}},
                {49, new List<int>(){1,5,6}},
                {50, new List<int>(){2,5,6}},
                {51, new List<int>(){1,2,5,6}},
                {52, new List<int>(){3,5,6}},
                {53, new List<int>(){1,3,5,6}},
                {54, new List<int>(){2,3,5,6}},
                {55, new List<int>(){1,2,3,5,6}},
                {56, new List<int>(){4,5,6}},
                {57, new List<int>(){1,4,5,6}},
                {58, new List<int>(){2,4,5,6}},
                {59, new List<int>(){1,2,4,5,6}},
                {60, new List<int>(){3,4,5,6}},
                {61, new List<int>(){1,3,4,5,6}},
                {62, new List<int>(){2,3,4,5,6}},
                {63, new List<int>(){1,2,3,4,5,6}},
                {64, new List<int>(){7}},
                {65, new List<int>(){1,7}},
                {66, new List<int>(){2,7}},
                {67, new List<int>(){1,2,7}},
                {68, new List<int>(){3,7}},
                {69, new List<int>(){1,3,7}},
                {70, new List<int>(){2,3,7}},
                {71, new List<int>(){1,2,3,7}},
                {72, new List<int>(){4,7}},
                {73, new List<int>(){1,4,7}},
                {74, new List<int>(){2,4,7}},
                {75, new List<int>(){1,2,4,7}},
                {76, new List<int>(){3,4,7}},
                {77, new List<int>(){1,3,4,7}},
                {78, new List<int>(){2,3,4,7}},
                {79, new List<int>(){1,2,3,4,7}},
                {80, new List<int>(){5,7}},
                {81, new List<int>(){1,5,7}},
                {82, new List<int>(){2,5,7}},
                {83, new List<int>(){1,2,5,7}},
                {84, new List<int>(){3,5,7}},
                {85, new List<int>(){1,3,5,7}},
                {86, new List<int>(){2,3,5,7}},
                {87, new List<int>(){1,2,3,5,7}},
                {88, new List<int>(){4,5,7}},
                {89, new List<int>(){1,4,5,7}},
                {90, new List<int>(){2,4,5,7}},
                {91, new List<int>(){1,2,4,5,7}},
                {92, new List<int>(){3,4,5,7}},
                {93, new List<int>(){1,3,4,5,7}},
                {94, new List<int>(){2,3,4,5,7}},
                {95, new List<int>(){1,2,3,4,5,7}},
                {96, new List<int>(){6,7}},
                {97, new List<int>(){1,6,7}},
                {98, new List<int>(){2,6,7}},
                {99, new List<int>(){1,2,6,7}},
                {100, new List<int>(){3,6,7}},
                {101, new List<int>(){1,3,6,7}},
                {102, new List<int>(){2,3,6,7}},
                {103, new List<int>(){1,2,3,6,7}},
                {104, new List<int>(){4,6,7}},
                {105, new List<int>(){1,4,6,7}},
                {106, new List<int>(){2,4,6,7}},
                {107, new List<int>(){1,2,4,6,7}},
                {108, new List<int>(){3,4,6,7}},
                {109, new List<int>(){1,3,4,6,7}},
                {110, new List<int>(){2,3,4,6,7}},
                {111, new List<int>(){1,2,3,4,6,7}},
                {112, new List<int>(){5,6,7}},
                {113, new List<int>(){1,5,6,7}},
                {114, new List<int>(){2,5,6,7}},
                {115, new List<int>(){1,2,5,6,7}},
                {116, new List<int>(){3,5,6,7}},
                {117, new List<int>(){1,3,5,6,7}},
                {118, new List<int>(){2,3,5,6,7}},
                {119, new List<int>(){1,2,3,5,6,7}},
                {120, new List<int>(){4,5,6,7}},
                {121, new List<int>(){1,4,5,6,7}},
                {122, new List<int>(){2,4,5,6,7}},
                {123, new List<int>(){1,2,4,5,6,7}},
                {124, new List<int>(){3,4,5,6,7}},
                {125, new List<int>(){1,3,4,5,6,7}},
                {126, new List<int>(){2,3,4,5,6,7}},
                {127, new List<int>(){1,2,3,4,5,6,7}},
                {128, new List<int>(){8}},
                {129, new List<int>(){1,8}},
                {130, new List<int>(){2,8}},
                {131, new List<int>(){1,2,8}},
                {132, new List<int>(){3,8}},
                {133, new List<int>(){1,3,8}},
                {134, new List<int>(){2,3,8}},
                {135, new List<int>(){1,2,3,8}},
                {136, new List<int>(){4,8}},
                {137, new List<int>(){1,4,8}},
                {138, new List<int>(){2,4,8}},
                {139, new List<int>(){1,2,4,8}},
                {140, new List<int>(){3,4,8}},
                {141, new List<int>(){1,3,4,8}},
                {142, new List<int>(){2,3,4,8}},
                {143, new List<int>(){1,2,3,4,8}},
                {144, new List<int>(){5,8}},
                {145, new List<int>(){1,5,8}},
                {146, new List<int>(){2,5,8}},
                {147, new List<int>(){1,2,5,8}},
                {148, new List<int>(){3,5,8}},
                {149, new List<int>(){1,3,5,8}},
                {150, new List<int>(){2,3,5,8}},
                {151, new List<int>(){1,2,3,5,8}},
                {152, new List<int>(){4,5,8}},
                {153, new List<int>(){1,4,5,8}},
                {154, new List<int>(){2,4,5,8}},
                {155, new List<int>(){1,2,4,5,8}},
                {156, new List<int>(){3,4,5,8}},
                {157, new List<int>(){1,3,4,5,8}},
                {158, new List<int>(){2,3,4,5,8}},
                {159, new List<int>(){1,2,3,4,5,8}},
                {160, new List<int>(){6,8}},
                {161, new List<int>(){1,6,8}},
                {162, new List<int>(){2,6,8}},
                {163, new List<int>(){1,2,6,8}},
                {164, new List<int>(){3,6,8}},
                {165, new List<int>(){1,3,6,8}},
                {166, new List<int>(){2,3,6,8}},
                {167, new List<int>(){1,2,3,6,8}},
                {168, new List<int>(){4,6,8}},
                {169, new List<int>(){1,4,6,8}},
                {170, new List<int>(){2,4,6,8}},
                {171, new List<int>(){1,2,4,6,8}},
                {172, new List<int>(){3,4,6,8}},
                {173, new List<int>(){1,3,4,6,8}},
                {174, new List<int>(){2,3,4,6,8}},
                {175, new List<int>(){1,2,3,4,6,8}},
                {176, new List<int>(){5,6,8}},
                {177, new List<int>(){1,5,6,8}},
                {178, new List<int>(){2,5,6,8}},
                {179, new List<int>(){1,2,5,6,8}},
                {180, new List<int>(){3,5,6,8}},
                {181, new List<int>(){1,3,5,6,8}},
                {182, new List<int>(){2,3,5,6,8}},
                {183, new List<int>(){1,2,3,5,6,8}},
                {184, new List<int>(){4,5,6,8}},
                {185, new List<int>(){1,4,5,6,8}},
                {186, new List<int>(){2,4,5,6,8}},
                {187, new List<int>(){1,2,4,5,6,8}},
                {188, new List<int>(){3,4,5,6,8}},
                {189, new List<int>(){1,3,4,5,6,8}},
                {190, new List<int>(){2,3,4,5,6,8}},
                {191, new List<int>(){1,2,3,4,5,6,8}},
                {192, new List<int>(){7,8}},
                {193, new List<int>(){1,7,8}},
                {194, new List<int>(){2,7,8}},
                {195, new List<int>(){1,2,7,8}},
                {196, new List<int>(){3,7,8}},
                {197, new List<int>(){1,3,7,8}},
                {198, new List<int>(){2,3,7,8}},
                {199, new List<int>(){1,2,3,7,8}},
                {200, new List<int>(){4,7,8}},
                {201, new List<int>(){1,4,7,8}},
                {202, new List<int>(){2,4,7,8}},
                {203, new List<int>(){1,2,4,7,8}},
                {204, new List<int>(){3,4,7,8}},
                {205, new List<int>(){1,3,4,7,8}},
                {206, new List<int>(){2,3,4,7,8}},
                {207, new List<int>(){1,2,3,4,7,8}},
                {208, new List<int>(){5,7,8}},
                {209, new List<int>(){1,5,7,8}},
                {210, new List<int>(){2,5,7,8}},
                {211, new List<int>(){1,2,5,7,8}},
                {212, new List<int>(){3,5,7,8}},
                {213, new List<int>(){1,3,5,7,8}},
                {214, new List<int>(){2,3,5,7,8}},
                {215, new List<int>(){1,2,3,5,7,8}},
                {216, new List<int>(){4,5,7,8}},
                {217, new List<int>(){1,4,5,7,8}},
                {218, new List<int>(){2,4,5,7,8}},
                {219, new List<int>(){1,2,4,5,7,8}},
                {220, new List<int>(){3,4,5,7,8}},
                {221, new List<int>(){1,3,4,5,7,8}},
                {222, new List<int>(){2,3,4,5,7,8}},
                {223, new List<int>(){1,2,3,4,5,7,8}},
                {224, new List<int>(){6,7,8}},
                {225, new List<int>(){1,6,7,8}},
                {226, new List<int>(){2,6,7,8}},
                {227, new List<int>(){1,2,6,7,8}},
                {228, new List<int>(){3,6,7,8}},
                {229, new List<int>(){1,3,6,7,8}},
                {230, new List<int>(){2,3,6,7,8}},
                {231, new List<int>(){1,2,3,6,7,8}},
                {232, new List<int>(){4,6,7,8}},
                {233, new List<int>(){1,4,6,7,8}},
                {234, new List<int>(){2,4,6,7,8}},
                {235, new List<int>(){1,2,4,6,7,8}},
                {236, new List<int>(){3,4,6,7,8}},
                {237, new List<int>(){1,3,4,6,7,8}},
                {238, new List<int>(){2,3,4,6,7,8}},
                {239, new List<int>(){1,2,3,4,6,7,8}},
                {240, new List<int>(){5,6,7,8}},
                {241, new List<int>(){1,5,6,7,8}},
                {242, new List<int>(){2,5,6,7,8}},
                {243, new List<int>(){1,2,5,6,7,8}},
                {244, new List<int>(){3,5,6,7,8}},
                {245, new List<int>(){1,3,5,6,7,8}},
                {246, new List<int>(){2,3,5,6,7,8}},
                {247, new List<int>(){1,2,3,5,6,7,8}},
                {248, new List<int>(){4,5,6,7,8}},
                {249, new List<int>(){1,4,5,6,7,8}},
                {250, new List<int>(){2,4,5,6,7,8}},
                {251, new List<int>(){1,2,4,5,6,7,8}},
                {252, new List<int>(){3,4,5,6,7,8}},
                {253, new List<int>(){1,3,4,5,6,7,8}},
                {254, new List<int>(){2,3,4,5,6,7,8}},
                {255, new List<int>(){1,2,3,4,5,6,7,8}}
#endregion        
            };
        /// <summary>
        /// 实现将byte数组中的内容转换为十六进制的字符串
        /// </summary>
        /// <param name="bitArray">要进行十六进制转换的byte数组</param>
        /// <returns>返回对应的十六进制数的字符串</returns>
        public static string GetHexString(byte[] bitArray)
        {
            
            string str = BitConverter.ToString(bitArray);   
            str = str.Replace("-", "");
            return str;

        }
        /// <summary>
        /// 判断一个文件是否有指定的某一块
        /// </summary>
        /// <param name="bitArray">文件的byte数组</param>
        /// <param name="blockNum"></param>
        /// <returns></returns>
        public static bool IsHasBlock(byte [] bitArray,int blockNum)
        {
            List<int> blocks = GetHasBlocks(bitArray);
            foreach (var i in blocks)
            {
                if (blockNum == i)
                {
                    return true;
                }
            }
            return false;
        }

        public static void TestIsHasBlock()
        {
            byte[] bitArr = new byte[]{80};
            if (IsHasBlock(bitArr,5))
            {
                Console.WriteLine("有这块！");
            }
            else
            {
                Console.WriteLine("没有");
            }
        }

        public static List<int> GetHasBlocks(byte[] bitArray)
        {
            List<int>blocks = new List<int>();
            int offset;
            for (int i = 0; i < bitArray.Length; i++)
            {
                offset = bitArray.Length - 1 - i;
                foreach (var i1 in _table[bitArray[i]] )
                {
                    blocks.Add(i1 + offset * 8);
                }                
            }
            return blocks;
        }

        public static void TestGetHasBlocks()
        {
            byte[] bitArr = new byte[]{80};
            List<int> blocks = GetHasBlocks(bitArr);
            foreach (var i in blocks)
            {
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 实现查找哪些块是别人有的，而自己没有的
        /// </summary>
        /// <param name="myBitArray">自己的文件对应的byte数组</param>
        /// <param name="otherBitArray">别人的文件对应的byte数组</param>
        /// <returns>返回哪些块是别人有的而自己没有的</returns>
        public static List<int> GetDownLoadBlockNum(byte[] myBitArray,byte[] otherBitArray)
        {
            List<int> blockNumber = new List<int>();
            if (myBitArray.Length != otherBitArray.Length)
            {
                Console.WriteLine("自己的文件对应的byte数组与其他人的byte长度不同！");
                return blockNumber;
            }

            int offset;
            for (int i = 0; i <= myBitArray.Length - 1; i++)
            {
                offset = myBitArray.Length - 1 - i;
                int a = Convert.ToByte(myBitArray[i] ^ otherBitArray[i]) & otherBitArray[i];
                foreach (var i1 in _table[a] )
                {
                    blockNumber.Add(i1 + offset * 8);
                }
            }

            return blockNumber;
        }
        public static void TestGetDownNum()
        {
            byte[] my = new byte[] { Convert.ToByte(80), Convert.ToByte(0), Convert.ToByte(80) };
            byte[] other = new byte[] { Convert.ToByte(52), Convert.ToByte(0) };
            List<int> ret = new List<int>();
            ret = GetDownLoadBlockNum(my, other);
            foreach (var i in ret)
            {
                Console.WriteLine(i);
            }
        }
        /// <summary>
        /// 将byte数组中指定的二进制中的位置为1
        /// </summary>
        /// <param name="pos">要置为1的位</param>
        /// <param name="bitArray">要进行置位的byte数组</param>
        public static void SetPosition(int pos,byte[]bitArray)
        {
            if (pos <= 0 || pos > 8 * bitArray.Length)
            {
                Console.WriteLine("要置1的位置越界！");
                return;
            }
            int index;
            int len = bitArray.Length;
            int offset;
            if (pos % 8 == 0)
            {
                index = len - 1 - pos/8 + 1;
            }
            else
            {
                index = len - 1 - pos/8;
            }
            int temp = pos%8;
            if (temp == 0)
            {
                offset = 8;
            }
            else
            {
                offset = temp;
            }
            bitArray[index] = Convert.ToByte(Convert.ToByte(Math.Pow(2, offset - 1)) | bitArray[index]);
        }
        public static void TestSetPosition()
        {
            byte[] bitArr = new byte[]{80,80};
            SetPosition(0,bitArr);
            foreach (var b in bitArr)
            {
                Console.WriteLine(b);
            }
        }

        /// <summary>
        /// 打表函数
        /// </summary>
        public void PrintTable()
        {
            //             {2, new List<int>(){2,3}}     

            FileStream fst = new FileStream(@"e:/table.txt",FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);//追加模式
            StreamWriter stw = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));//指定编码.否则将出错!
           

            byte a;
            string ret = "";
            string head = "{";
            string sec = ", new List<int>(){";
            string tail = "}}";

            for (int i = 0; i <= 255; i++)
            {
                string body = "";
                int count = 0;
                a = Convert.ToByte(i);
                for (int j = 1; j <= 8; j++)
                {
                    if (j == 1)
                    {
                        if ((a & 1) == 1)
                        {
                            body += "1";
                            count++;
                        }
                    }
                    else
                    {
                        /*if (j == 8)
                        {
                            if (((a >> j - 1) & 1) == 1)
                            {
                                body += j;
                            }                           
                        }*/
                        if(((a >> j - 1) & 1) == 1)
                        {
                            if (count == 0)
                            {
                                body += j;
                                count++;
                            }
                            else
                            {
                                body += ",";
                                body += j;
                                count++;
                            }

                        }
                    }
                }
                if (i == 255)
                {
                    stw.WriteLine(head + i + sec + body + tail);
                    ret = ret + head + i + sec + body + tail;
                }
                else
                {
                    stw.WriteLine(head + i + sec + body + tail + ",");
                    ret = ret + head + i + sec + body + tail + "," + "\n";                    
                }

            }
            stw.Close();
            fst.Close();
            Console.WriteLine(ret);
        }
        /// <summary>
        /// 将十六进制的字符串转换为byte数组
        /// </summary>
        /// <param name="value">要进行转换的十六进制的字符串</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] ConvertHexToBytes(string value)
        {
            int len = value.Length / 2;
            byte[] ret = new byte[len];
           // Convert.toi
            for (int i = 0; i < len; i++)
            ret[i]=(byte)(Convert.ToInt32(value.Substring(i * 2, 2), 16));
            return ret;
        }
        public void TestGexString()
        {
            byte[] bitArr = new byte[] { Convert.ToByte(3), Convert.ToByte(5), Convert.ToByte(5), Convert.ToByte(5), Convert.ToByte(5) };
           
            //Console.WriteLine(bitArr.Length);
            Console.WriteLine(GetHexString(bitArr));
            Console.WriteLine();
            Console.WriteLine();
            byte[] ret = ConvertHexToBytes(GetHexString(bitArr));
            foreach (var b in ret)
            {
                Console.WriteLine(b.ToString());
            }
        }
    }
}
