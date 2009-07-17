using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HPPNet;
using HPPUtil;
namespace HPPClientLibrary
{
    public class CSServer : HTTPServer
    {
        /// <summary>
        /// 每一块的大小，为固定值
        /// </summary>
        private const int BLOCKSIZE = 1048576;  

        /// <summary>
        /// 解释url后得到的信息
        /// </summary>
        private Hashtable urlInfo;
        private Response response;

        public CSServer(int numConnections, int receiveBufferSize) : base(numConnections, receiveBufferSize)
        {
        }
        /// <summary>
        /// 解释请求的url
        /// </summary>
        /// <param name="urlInfo">存储文件名、文件的hash值及要请求的块号的一个hash表</param>
        /// <param name="url">请求的url</param>
        protected void ParseUrl(out Hashtable urlInfo, string url)
        {
            
            
            string fileName = "",hash = "",blockNum = "";
            urlInfo = new Hashtable();
            if (url.EndsWith("/") || url.LastIndexOf("/") == 0)
            {
                int pos = url.IndexOf("|");
              
                blockNum = "none";
                if (url.EndsWith("/")) //url格式为：  /abc.txt|hash/
                {
                    int lastPos = url.IndexOf("/", url.IndexOf("/") + 1);
                    fileName = url.Substring(1, pos - 1);
                    hash = url.Substring(pos + 1, lastPos - 1 - pos);                    
                }
                else// url格式为： /abc.txt|hash
                {
                    fileName = url.Substring(1, pos - 1);
                    hash = url.Substring(pos + 1, url.Length - 1 - pos);
                }

            }
            else if (url.LastIndexOf("/") != url.Length - 1)// url格式为：  /abc.txt|hash/22
            {
                int pos = url.IndexOf("|");
                int lastPos = url.IndexOf("/",url.IndexOf("/") + 1);
                fileName = url.Substring(1, pos - 1);
                hash = url.Substring(pos + 1, lastPos - 1 - pos);
                blockNum = url.Substring(lastPos + 1, url.Length - lastPos - 1);

            }
                urlInfo.Add("FileName",fileName);
                urlInfo.Add("Hash",hash);
                urlInfo.Add("BlockNum",blockNum);
        }
        public void TestParseUrl(string url)
        {
            Hashtable urls;
            ParseUrl(out urls,url);
            if (urls.ContainsKey("FileName"))
            {
                Console.WriteLine("FileName: {0}",urls["FileName"]);
            }
            if (urls.ContainsKey("Hash"))
            {
                Console.WriteLine("Hash: {0}",urls["Hash"]);
            }
            if (urls.ContainsKey("BlockNum"))
            {
                Console.WriteLine("BlockNum: {0}",urls["BlockNum"]);
            }
        }

        protected override Response ProcessHttpReq(EndPoint remoteEndPoint, Hashtable headers, string body)
        {
            string url = String.Empty;
            if (headers.ContainsKey("Url"))
            {
                url = (string) headers["Url"];
            }
            ParseUrl(out urlInfo,url);
            //TestParseUrl(url);
            //TestGetHasFilBlocks();
            //TestGetDownLoadFile();
            //return null;
            if (urlInfo.ContainsKey("BlockNum"))
            {
                if ((string)urlInfo["BlockNum"] == "none")
                {
                    if (urlInfo.ContainsKey("Hash"))
                    {
                        if (((string)urlInfo["Hash"]).Trim().ToString() == "")
                        {
                            response = null;
                        }
                        else
                        {
                            response = new StringResponse(GetHasFileBlocks((string)urlInfo["Hash"]));
                        }
                    }
                }
                else
                {
                    if (urlInfo.ContainsKey("Hash") && ((string)urlInfo["Hash"]).Trim().ToString() != "" && ((string)urlInfo["BlockNum"]).Trim().ToString() != "")
                    {
                        response = GetDownLoadFile((string) urlInfo["Hash"],Convert.ToInt32((string)urlInfo["BlockNum"]));
                    }
                }
            }
            return response;
        }
        /// <summary>
        /// 根据文件的hash值查找拥有该文件哪些块
        /// </summary>
        /// <param name="fileHash">文件的hash值</param>
        /// <returns>拥有该文件哪些块的xml格式的字符串</returns>
        protected string GetHasFileBlocks(string fileHash)
        {
            XDocument xdoc = new XDocument();
            xdoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement file = new XElement("File");
            if (HPPClient.DownloadJobDict.ContainsKey(fileHash))
            {
                file.Add(new XElement("FileHash",new XAttribute("Hash",fileHash)));
                file.Add(new XElement("HasBlocks", new XAttribute("Blocks", BitArrayHelper.GetHexString(HPPClient.DownloadJobDict[fileHash].Mine))));
            }
            xdoc.Add(file);
            string ret = xdoc.Declaration.ToString();
            ret += xdoc.ToString();
            return ret;
        }
        //public void TestGetHasFilBlocks()
        //{
        //    HPPClient.HashFullNameDict.Add("88", @"D:\test\abc.txt");
        //    HPPClient.HashDownloadDict.Add("88", new byte[] { 80, 80 });
        //    Console.WriteLine(GetHasFileBlocks("8"));

        //}

        /// <summary>
        /// 根据文件的hash值及要找的块的编号，得到一个FileResponse的对象
        /// </summary>
        /// <param name="fileHash">文件的hash值</param>
        /// <param name="blockNum">要找的哪一块</param>
        /// <returns></returns>
        protected FileResponse GetDownLoadFile(string fileHash,int blockNum)
        {
            if (!BitArrayHelper.IsHasBlock(HPPClient.DownloadJobDict[fileHash].Mine, blockNum)) 
            {
                Console.WriteLine("没有找到所需的块！");
                return null;
            }
            int lastBlockSize;
            int blockAmount;
            long fileLen = HPPClient.DownloadJobDict[fileHash].FileLen;
            if (fileLen % BLOCKSIZE == 0)
            {
                blockAmount = (int)fileLen / BLOCKSIZE;
                lastBlockSize = BLOCKSIZE;
            }
            else
            {
                blockAmount = (int)fileLen / BLOCKSIZE + 1;
                lastBlockSize = (int)fileLen % BLOCKSIZE;
            }


            string fileName = HPPClient.HashFullNameDict[fileHash];
            long beginPos;
            long endPos;
            if (blockNum != blockAmount)
            {
                beginPos = (blockNum - 1)*BLOCKSIZE;
                endPos = beginPos + BLOCKSIZE - 1;                
            }
            else
            {
                if (lastBlockSize == BLOCKSIZE)
                {
                     beginPos = (blockNum - 1)*BLOCKSIZE;
                     endPos = beginPos + BLOCKSIZE - 1;     
                }
                else
                {
                    beginPos = (blockNum - 1) * BLOCKSIZE;
                    endPos = beginPos + lastBlockSize - 1;
                }
            }

            FileResponse fileResponse = new FileResponse(fileName);
            fileResponse.HasRange = true;
            fileResponse.RangeBegin = beginPos;
            fileResponse.RangeEnd = endPos;
            return fileResponse;
        }
        //public void TestGetDownLoadFile()
        //{
        //    HPPClient.HashFullNameDict.Add("88", @"D:\test\abc.txt");
        //    HPPClient.HashDownloadDict.Add("88", new byte[] { 7 });
        //    HPPClient.HashFileLenDict.Add("88", 3058564);
        //    FileResponse fp = GetDownLoadFile((string) urlInfo["Hash"], Convert.ToInt32(urlInfo["BlockNum"]));
        //    if (fp == null)
        //    {
        //        Console.WriteLine("没找到！！！！");
        //    }
        //    else
        //    {
        //        Console.WriteLine("FileName:{0},RangeBegin:{1},RangeEnd:{2}",fp.FileName,fp.RangeBegin,fp.RangeEnd);
        //    }
        //}
    }
}
