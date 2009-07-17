using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Amib.Threading;
using HPPNet;
using HPPUtil;

namespace HPPClientLibrary
{
    class WebHelper
    {
        /// <summary>
        /// 异步上传共享文件夹
        /// </summary>
        /// <param name="targetIPEndPoint">服务器IPEndPoint</param>
        /// <param name="csIPEndPoint">Client-Side Server 监听的IPEndPoint</param>
        /*
         *<?XML version = "1.0"? />
            <Info>
            <ShareFiles>
            <File FileName="c#" Hash="888" Size="888">
            </File>
            <File FileName="C++" Hash="999" Size="999">
            </File>
            </ShareFiles>
            <Client Port="">
            </Client>
            </Info>
         * 
         */
        public void UploadShareDirAsync(IPEndPoint targetIPEndPoint, IPEndPoint csIPEndPoint)
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement info = new XElement("Info");


            XElement shareFiles = new XElement("ShareFiles");
            
                                    
            foreach (KeyValuePair<string, string> dict in HPPClient.HashFullNameDict)
            {
                try
                {
                    XElement file = new XElement("File");
                    FileInfo fi = new FileInfo(dict.Value);
                    file.SetAttributeValue("FileName", fi.Name);
                    file.SetAttributeValue("Hash", dict.Key);
                    file.SetAttributeValue("Size", fi.Length);

                    shareFiles.Add(file);
                }
                catch (Exception)
                {
                    
                }
               
            }
            XElement client = new XElement("Client");
            client.SetAttributeValue("Port", csIPEndPoint.Port);
            info.Add(client);
            info.Add(shareFiles);
            doc.Add(info);
            string xmlStr = doc.Declaration.ToString() + doc.ToString();

            HTTPClient httpClient = new HTTPClient();
            httpClient.Encoding = Encoding.UTF8;
            string url = string.Format("http://{0}:{1}/", targetIPEndPoint.Address, targetIPEndPoint.Port);

            Uri uri = new Uri(url);
            httpClient.UploadStringAsync(uri, "POST", xmlStr);
            
        }

        public ServerFileInfoResponse GetServerResponse(IPEndPoint ipEndPoint, string hash)
        {
            ServerFileInfoResponse response = new ServerFileInfoResponse();

            List<IPEndPoint> ipList = new List<IPEndPoint>();

            Uri requestUri = new Uri(string.Format("http://{0}:{1}/filename|{2}", ipEndPoint.Address, ipEndPoint.Port, hash));

            HTTPClient client = new HTTPClient();
            string xml = client.DownloadString(requestUri);
            XDocument doc = XDocument.Parse(xml);


            var userlist = from user in doc.Descendants("User") select user;
            foreach (XElement element in userlist)
            {
                
                if(element.Attribute("IP") != null && element.Attribute("Port") != null)
                {
                    string ipStr = element.Attribute("IP").Value;
                    string portStr = element.Attribute("Port").Value;
                    int port;
                    IPAddress ip;
                    if(int.TryParse(portStr, out port) && IPAddress.TryParse(ipStr, out ip))
                    {
                        IPEndPoint endPoint = new IPEndPoint(ip, port);
                        ipList.Add(endPoint);
                    }
                    
                }
            }

            response.IpList = ipList;

            var fileSize = doc.Descendants("FileSize");
            if(fileSize.Count() != 0 && fileSize.First().Attribute("Size") != null)
            {
                long size;
                string sizeStr = fileSize.First().Attribute("Size").Value;
                if (long.TryParse(sizeStr, out size))
                {
                    response.FileSize = size;
                }
            }

            var blocks = doc.Descendants("Blocks");
            if(blocks.Count() != 0 && blocks.First().Attribute("Number") != null)
            {
                int blockNum;
                string blockNumStr = blocks.First().Attribute("Number").Value;
                if(int.TryParse(blockNumStr, out blockNum))
                {
                    response.BlockNum = blockNum;
                }
            }

            return response;

        }
    }
}
