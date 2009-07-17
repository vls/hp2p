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
    class DownloadJob
    {
        private string saveFileName;
        private byte[] mine;
        private int totalBlockNum;
        private string hash;
        private long fileLen;

        /// <summary>
        /// 块-下载该块的Client对应
        /// </summary>
        private Dictionary<int, IPEndPoint> _queue;

        /// <summary>
        /// 每个Client拥有的块列表
        /// </summary>
        private Dictionary<IPEndPoint, HashSet<int>> _clientHasBlockDict;

        /// <summary>
        /// 每个Client入选下载的数量
        /// </summary>
        private Dictionary<IPEndPoint, int> _clientCountDict;

        /// <summary>
        /// 块的状态
        /// </summary>
        private Dictionary<int, DownloadStatus> _blockStatusDict;

        public DownloadJob()
        {
            _queue = new Dictionary<int, IPEndPoint>();
            _clientHasBlockDict = new Dictionary<IPEndPoint, HashSet<int>>();
            _clientCountDict = new Dictionary<IPEndPoint, int>();
            _blockStatusDict = new Dictionary<int, DownloadStatus>();
        }
        public string SaveFileName
        {
            get { return saveFileName; }
            set { saveFileName = value; }
        }

        public byte[] Mine
        {
            get { return mine; }
            set { mine = value; }
        }

        public int BlockNum
        {
            get { return totalBlockNum; }
            set { totalBlockNum = value; }
        }

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public long FileLen
        {
            get { return fileLen; }
            set { fileLen = value; }
        }

        public void Process(List<IPEndPoint> ipList)
        {
            AccessClientAsync(ipList);
        }

        public void Dispatch(IPEndPoint newClient, List<int> blockList)
        {
            IEnumerable<int> i1 = new List<int> {1, 2, 2, 3, 4};
            IEnumerable<int> i2 = new List<int> {  2, 2 };
            List<int> list = (i1.Intersect(i2)).ToList();

            HashSet<int> newSet = new HashSet<int>(blockList);
            HashSet<int> blockInQueue = GetBlockInQueue();
            HashSet<int> diff = Diff(newSet, blockInQueue);
            HashSet<int> join = Join(newSet, blockInQueue);

            //修改Client拥有块列表
            if(!_clientHasBlockDict.ContainsKey(newClient))
            {
                _clientHasBlockDict.Add(newClient, newSet);
            }
            else
            {
                _clientHasBlockDict[newClient] = newSet;
            }


            //把这个Client独有的添加到下载列表
            foreach (int block in diff)
            {
                _queue.Add(block, newClient);

                //修改block状态
                if (!_blockStatusDict.ContainsKey(block))
                {
                    
                    _blockStatusDict.Add(block, DownloadStatus.InQueue);
                }
                else
                {
                    _blockStatusDict[block] = DownloadStatus.InQueue;
                }

                //修改Client入选数量
                if (!_clientCountDict.ContainsKey(newClient))
                {
                    _clientCountDict.Add(newClient, 1);
                }
                else
                {
                    _clientCountDict[newClient]++;
                }
            }

            List<ClientCount> clientCounts = new List<ClientCount>();
            Dictionary<IPEndPoint, List<int>> _repeatCount = new Dictionary<IPEndPoint, List<int>>();

            foreach (int block in join)
            {
                IPEndPoint endPoint = _queue[block];
                if (!_repeatCount.ContainsKey(endPoint))
                {
                    _repeatCount.Add(endPoint, new List<int>(){ block });
                    ClientCount cc = new ClientCount();
                    cc.IP = endPoint;
                    cc.Count = _clientCountDict[endPoint];

                    clientCounts.Add(cc);
                }
                else
                {
                    _repeatCount[endPoint].Add(block);
                }

                
            }

            clientCounts.Sort();

            //newClient总负担
            int newClientCount = _clientCountDict[newClient];

            for (int i = clientCounts.Count-1; i >= 0; i++)
            {
                if (newClientCount < clientCounts[i].Count)
                {
                    List<int> blocklist = _repeatCount[clientCounts[i].IP];
                    int joinCount = blocklist.Count;

                    int changeCount = 0;

                    foreach (int block in blocklist)
                    {
                        lock (_blockStatusDict)
                        {
                            if (_blockStatusDict[block] != DownloadStatus.Downloading && _blockStatusDict[block] != DownloadStatus.Finish)
                            {
                                _clientCountDict[_queue[block]]--;
                                _queue[block] = newClient;
                                _clientCountDict[newClient]++;
                                changeCount++;
                            }
                        }
                    }

                    newClientCount += changeCount;
                }
                else
                {
                    break;
                }
            }


        }

        /// <summary>
        /// 与各Client交流
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="ipList"></param>
        /// <param name="saveFileName"></param>
        private void AccessClientAsync(List<IPEndPoint> ipList)
        {
            foreach (IPEndPoint ipEndPoint in ipList)
            {
                HPPClient.ClientThreadPool.QueueWorkItem(new Action<IPEndPoint>(CompareAndPreDownload), ipEndPoint);
            }

        }

        /// <summary>
        /// 看看对方有个啥块
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private byte[] GetTheirsBitArray(IPEndPoint ipEndPoint, string hash)
        {
            HTTPClient httpClient = new HTTPClient();
            Uri uri = new Uri(string.Format("http://{0}:{1}/filename|{2}", ipEndPoint.Address, ipEndPoint.Port, hash));
            string res = httpClient.DownloadString(uri);
            XDocument doc = XDocument.Parse(res);
            if (doc.Element("HasBlocks") == null)
            {
                return null;
            }

            string hexString = doc.Element("HasBlocks").Attribute("Blocks").Value;
            byte[] theirs = BitArrayHelper.ConvertHexToBytes(hexString);
            return theirs;
        }

        /// <summary>
        /// 比较对方和自己的下载情况，准备Download
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="ipEndPoint"></param>
        /// <param name="mine"></param>
        /// <param name="saveFileName"></param>
        private void CompareAndPreDownload(IPEndPoint ipEndPoint)
        {
            byte[] theirs = GetTheirsBitArray(ipEndPoint, hash);
            if (theirs == null)
            {
                return;
            }

            List<int> blockList = BitArrayHelper.GetDownLoadBlockNum(mine, theirs);
            foreach (int blockNum in blockList)
            {
                DownloadManager.DownLoadThreadPool.QueueWorkItem(new Action<int>(PreDownload),blockNum);
            }

        }

        /// <summary>
        /// 准备Download,如果原本预计的已存在，则不进行下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="saveFileName"></param>
        /// <param name="mine"></param>
        /// <param name="blockNum"></param>
        private void PreDownload(int blockNum)
        {
            if (BitArrayHelper.IsHasBlock(mine, blockNum))
            {
                return;
            }

            string url;
            lock (_blockStatusDict)
            {
                _blockStatusDict[blockNum] = DownloadStatus.Downloading;
                IPEndPoint ipEndPoint = _queue[blockNum];

                url = string.Format("http://{0}:{1}/filename|{2}/{3}", ipEndPoint.Address, ipEndPoint.Port, hash, blockNum);
            }

            ProcessDownload(url);
        }

        private void ProcessDownload(string url)
        {
            HttpWebClient client = new HttpWebClient(DownloadManager.DownLoadThreadPool);
            client.DownloadFile(url, this.saveFileName);
        }

        private class ClientCount : IComparable<ClientCount>
        {
            private IPEndPoint _ip;
            private int _count;

            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }

            public IPEndPoint IP
            {
                get { return _ip; }
                set { _ip = value; }
            }

            public int CompareTo(ClientCount other)
            {
                return this.Count - other.Count;
            }
        }

        

        

        private HashSet<int> GetBlockInQueue()
        {
            HashSet<int> set = new HashSet<int>();
            foreach (KeyValuePair<int, IPEndPoint> pair in _queue)
            {
                if (_blockStatusDict.ContainsKey(pair.Key) && (_blockStatusDict[pair.Key] == DownloadStatus.InQueue))
                {
                    set.Add(pair.Key);
                }
            }

            return set;
        }

        /// <summary>
        /// 返回两个集合的差(第一个参数-第二个参数)
        /// </summary>
        /// <param name="ha"></param>
        /// <param name="hb"></param>
        /// <returns></returns>
        private HashSet<int> Diff(HashSet<int> ha, HashSet<int> hb)
        {
            return new HashSet<int>(ha.Except(hb));
        }

        /// <summary>
        /// 返回两个集合的交
        /// </summary>
        /// <param name="ha"></param>
        /// <param name="hb"></param>
        /// <returns></returns>
        private HashSet<int> Join(HashSet<int> ha, HashSet<int> hb)
        {
            return new HashSet<int>(ha.Intersect(hb));
        }
    }

    internal enum DownloadStatus
    {
        InQueue,
        Downloading,
        Finish
    }
}
