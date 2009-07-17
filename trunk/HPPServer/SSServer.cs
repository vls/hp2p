using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;
using System.Collections;
using System.Xml.Linq;
using HPPNet;
namespace HPPServer
{
    public class FileInfomations
    {
        private List<IPEndPoint> ipepList;
        private long size;
        private int blockNumber;
        private int lastBlockSize;
        public long Size
        {
            get { return size; }
            set { size = value; }
        }
        public int BlockNumber
        {
            get { return blockNumber; }
            set { blockNumber = value; }
        }
        public int LastBlockSize
        {
            get { return lastBlockSize; }
            set { lastBlockSize = value; }
        }
        public List<IPEndPoint> IPEndPointList
        {
            get
            {
                return ipepList;
            }
            set
            {
                ipepList = value;
            }
        }
    }

    class SSServer:HTTPServer
    {   
        private const int BLOCKSIZE = 1048576;        
        private Hashtable urlInfo = null;
        private Response strResponse;
        private Dictionary<string, FileInfomations> hashUser = new Dictionary<string,FileInfomations>();
        private Dictionary<string ,string >fileNameHash = new Dictionary<string, string>();
        private XmlDocument userFilesInfo = new XmlDocument();

        public SSServer(int numConnections, int receiveBufferSize):base(numConnections, receiveBufferSize)
        {
            
        }



        protected override Response ProcessHttpReq(EndPoint remoteEndPoint, Hashtable headers, string body)
        {

            string url = String.Empty;
            string method = String.Empty;
            string protocol = String.Empty;
            if (headers.ContainsKey("Url"))
            {
                url = (string) headers["Url"];
            }
            if (headers.ContainsKey("Method"))
            {
                method = (string) headers["Method"];
            }
            if (method == "GET")
            {
                ParseUrl(out urlInfo,url);                
            }
 
            //TestGetFileInfo();
            
            //return null;

            switch (method)
            {
                case "POST":
                    SaveUserFilesInfo(body,remoteEndPoint);
                    strResponse = new VoidResponse();
                    break;
                case "GET":
                    if (urlInfo != null && urlInfo.ContainsKey("Hash") && urlInfo.ContainsKey("FileName"))
                    {
                        if ((string)urlInfo["Hash"] == "none")
                        {
                            strResponse = new StringResponse(SearchFile((string) urlInfo["FileName"]));
                        }
                        else
                        {
                            if ((string)urlInfo["Hash"] != "")
                            {
                                strResponse = new StringResponse(GetFileInfomation((string) urlInfo["Hash"]));                                 
                            }
                    
                        }
                    }
                    break;
                default:
                    break;
            }

            return strResponse;
        }
        protected void TestSaveUserFileInfo(EndPoint remoteEndPoint)
        {
            string tstr2 =
                @"<?xml version='1.0' encoding='utf-8' ?>
<ShareFiles>
</ShareFiles>";
            SaveUserFilesInfo(tstr2, remoteEndPoint);
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> s in fileNameHash) //遍历dic中的所有元素，
                System.Console.WriteLine("key{0},value:{1}", s.Key, s.Value);
            Console.WriteLine();

            foreach (KeyValuePair<string, FileInfomations> s in hashUser) //遍历dic中的所有元素，
            {
                           
                System.Console.WriteLine("key:{0},  values:size: {1}, blockNumber:{2}, lastBlockSize:{3}", s.Key,
                                         s.Value.Size, s.Value.BlockNumber, s.Value.LastBlockSize);
                foreach (var c in s.Value.IPEndPointList)
                {
                    Console.WriteLine("IPADDR:{0}, Port:{1}",c.Address,c.Port);
                }
            }
        }

        

        public void TestGetFileInfo()
        {
            FileInfomations fileinfo = new FileInfomations();
            List<IPEndPoint> ipepList = new List<IPEndPoint>();
            fileinfo.IPEndPointList = ipepList;
            fileinfo.BlockNumber = 88;
            fileinfo.LastBlockSize = 12365;
            fileinfo.Size = 886459;
            fileinfo.IPEndPointList.Add(new IPEndPoint(IPAddress.Parse("192.168.0.88"),5555));
            fileinfo.IPEndPointList.Add(new IPEndPoint(IPAddress.Parse("192.168.0.99"), 6666));
            fileinfo.IPEndPointList.Add(new IPEndPoint(IPAddress.Parse("192.168.1.99"), 3333));
            hashUser.Add("123",fileinfo);
            string ret = GetFileInfomation((string) urlInfo["Hash"]);
            Console.WriteLine(ret);
            Console.WriteLine();
            XDocument doc = new XDocument();
            try
            {
                doc = XDocument.Parse(ret);
                //TextReader tr = new StringReader(ret); ;
                //doc = XDocument.Load(tr);
            }
            catch (Exception)
            {

                Console.WriteLine("body数据不符合xml格式！fuckfukcukfjkc");
            }
            Console.WriteLine(doc.Declaration.ToString());
            Console.WriteLine(doc.ToString());
           // Console.WriteLine(GetFileInfomation((string)urlInfo["Hash"]));
        }

        protected void TestSearchFile()
        {
            fileNameHash.Add("helloC#","123");
            fileNameHash.Add("welcome C# haha","555");
            fileNameHash.Add("C#demo","888");
            fileNameHash.Add("C#","999");
            fileNameHash.Add("hahaC#haha","789");
            fileNameHash.Add("hdfieCldfie#ldjf","88");
            fileNameHash.Add("ifeoife","99");
            Console.WriteLine(SearchFile("C#"));
        }

        /// <summary>
        /// 解释请求的url中文件名和该文件的hash值(url的格式规定为: /abc.txt|hash/ )
        /// </summary>
        /// <param name="urlInfo">存储文件名及hash值的Hashable</param>
        /// <param name="url">请求的url</param>
        protected void ParseUrl(out Hashtable urlInfo,string url)
        {
            urlInfo = new Hashtable();
            string fileAndHadh =  url.Replace("/", "");
            //Console.WriteLine(fileAndHadh);
            int pos = fileAndHadh.IndexOf('|');
            string fileName = fileAndHadh.Substring(0, pos);
            urlInfo.Add("FileName",fileName);
            //Console.WriteLine("length:{0},pos:{1},resu:{2}",fileAndHadh.Length,pos,fileAndHadh.Length - pos - 1);
            string hash = fileAndHadh.Substring(pos + 1,fileAndHadh.Length - pos - 1);
            urlInfo.Add("Hash",hash);
            //Console.WriteLine("filename:{0},hash:{1}",result[0],result[1]);
            //Console.WriteLine("Filename:{0}, Hash:{1}",fileName,hash);
        }

        /// <summary>
        /// 根据文件名进行模糊搜索
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>返回一个xml格式的字符串</returns>
        protected string SearchFile(string fileName)
        {
            Regex r = new Regex(".*" + fileName + ".*"); 
            Match m;
            XDocument xdoc = new XDocument();
            xdoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement files = new XElement("Files");
            foreach (KeyValuePair<string ,string> s in fileNameHash)
            {
                m = r.Match(s.Key);
                if (m.Success)
                {
                    files.Add(new XElement("File",new XAttribute("FileName",s.Key),new XAttribute("Hash",fileNameHash[s.Key])));
                }
            }
            xdoc.Add(files);
            string ret = xdoc.Declaration.ToString();
            ret += xdoc.ToString();
            return ret;
        }

        /// <summary>
        /// 根据文件的hash值返回一个xml格式的字符串
        /// </summary>
        /// <param name="fileHash">文件的hash值</param>
        /// <returns></returns>
        protected string GetFileInfomation(string fileHash)
        {
            XDocument xdoc = new XDocument();
            xdoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement fileInfomation = new XElement("FileInfomation");
            XElement userList = new XElement("UserList");
            if (!hashUser.ContainsKey(fileHash))
            {
                fileInfomation.Add(userList);
                xdoc.Add(fileInfomation);
            }
            else
            {
                FileInfomations fileinfos = hashUser[fileHash];
                foreach (var c in fileinfos.IPEndPointList )
                {
                    userList.Add(new XElement("User",new XAttribute("IP",c.Address.ToString()),new XAttribute("Port",c.Port.ToString())));
                }
                fileInfomation.Add(userList);
                fileInfomation.Add(new XElement("FileSize",new XAttribute("Size",fileinfos.Size)));
                fileInfomation.Add(new XElement("Blocks",new XAttribute("Number",fileinfos.BlockNumber)));
                fileInfomation.Add(new XElement("LastBlock",new XAttribute("Size",fileinfos.LastBlockSize)));
                xdoc.Add(fileInfomation);                
            }
            string str = xdoc.Declaration.ToString();
            str += xdoc.ToString();
            return str;
        }

        /// <summary>
        /// 保存用户的共享文件等信息
        /// </summary>
        /// <param name="data">一个xml格式的string</param>
        /// <param name="remoteEndPoint">用户的IPEndPoint</param>
        protected void SaveUserFilesInfo(string data,EndPoint remoteEndPoint)
        {
            XDocument doc = new XDocument();
            IPEndPoint ipEndPoint = null;
            try
            {
                ipEndPoint = (IPEndPoint) remoteEndPoint;
     
            }
            catch (Exception)
            {
                
                Console.WriteLine("remoteEndPoint有错");
            }

            try
            {
                TextReader tr = new StringReader(data);;
                doc = XDocument.Load(tr);
            }
            catch (Exception e)
            {
                
                Console.WriteLine("body数据不符合xml格式！");
            }
            try
            {
                //没有共享文件时返回
                if (doc.Descendants("File").Count() == 0)
                {
                   // Console.WriteLine("没有共享文件");
                    return;
                    
                }
                var files = from f in doc.Descendants("File")
                     select new { fileName = f.Attribute("FileName").Value, hash = f.Attribute("Hash").Value,fileSize =f.Attribute("Size") };
                foreach (var file in files)
                {
                    if (!fileNameHash.ContainsKey(file.fileName))
                    {
                        fileNameHash.Add(file.fileName,file.hash);
                    }
                    if (!hashUser.ContainsKey(file.hash))
                    {
                        List<IPEndPoint>userList = new List<IPEndPoint>();
                        FileInfomations fileinfos = new FileInfomations();
                        userList.Add(ipEndPoint);
                        fileinfos.IPEndPointList = userList;
                        long size = (long)file.fileSize;
                        fileinfos.Size = (long)file.fileSize;
                        if (size % BLOCKSIZE == 0)
                        {
                            fileinfos.BlockNumber = (int)size/BLOCKSIZE;
                            fileinfos.LastBlockSize = BLOCKSIZE;
                        }
                        else
                        {
                            fileinfos.BlockNumber = (int) size/BLOCKSIZE + 1;
                            fileinfos.LastBlockSize = (int) size%BLOCKSIZE;
                        }
                        hashUser.Add(file.hash,fileinfos);
                    }
                    else
                    {
                        hashUser[file.hash].IPEndPointList.Add(ipEndPoint);
                    }
                }
            }
            catch (Exception)
            {
                
                Console.WriteLine("保共享文件信息时出错！");
            }

        }
    }
    
}
