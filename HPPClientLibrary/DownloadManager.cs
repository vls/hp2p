using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Amib.Threading;
using HPPNet;
using HPPUtil;
using HPPUtil.Helpers;

namespace HPPClientLibrary
{
    class DownloadManager
    {
        private WebHelper _webHelper;

        internal static SmartThreadPool DownLoadThreadPool;

        private Dictionary<string, DownloadJob> _downloadJobDict;

        public DownloadManager()
        {
            _webHelper = new WebHelper();
            
            DownLoadThreadPool = new SmartThreadPool();

            _downloadJobDict = HPPClient.DownloadJobDict;
        }
        /// <summary>
        /// DownloadFacade
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="saveFileName"></param>
        public void Download(string hash, string saveFileName)
        {
            ServerFileInfoResponse response = _webHelper.GetServerResponse(HPPClient._serverIpEndPoint, hash);

            if (_downloadJobDict.ContainsKey(hash))
            {
                return;
            }



            DownloadJob downloadJob = new DownloadJob();
            downloadJob.Hash = hash;
            downloadJob.SaveFileName = saveFileName;
            downloadJob.Mine = new byte[response.FileSize.GetBitArrayLength()];
            downloadJob.FileLen = response.FileSize;

            _downloadJobDict.Add(hash, downloadJob);

            PreAlloc(saveFileName, response.FileSize);

            downloadJob.Process(response.IpList);
        }

        /// <summary>
        /// 预分配磁盘空间
        /// </summary>
        /// <param name="saveFileName"></param>
        /// <param name="fileLen"></param>
        private void PreAlloc(string saveFileName, long fileLen)
        {
            using (FileStream fs =new FileStream(saveFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                fs.SetLength(fileLen);
            }
        }


        


 

        
    }
}
