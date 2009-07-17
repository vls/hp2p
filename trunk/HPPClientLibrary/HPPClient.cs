using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Amib.Threading;
using HPPNet;

namespace HPPClientLibrary
{
    public class HPPClient
    {
        private const int NUM_CONNECTION = 10;
        internal static SmartThreadPool ClientThreadPool;
        private static CSServer _server;
        private static HashCalculator _hashCalc;
        private static WebHelper _webHelper;

        private const string DAT_HASH_FULLNAME = "hash_file.dat";
        private const string DAT_HASH_KNOWN = "known.dat";
        private const string DAT_SHAREDIR = "sharedir.dat";
        private const string DAT_HASH_JOB = "downloadJob.dat";

        public event EventHandler DictChanged;

        /// <summary>
        /// 是否已经Hash字典,KEY=文件全路径
        /// </summary>
        internal static HashSet<string> KnownDict;

        /// <summary>
        /// Hash与全路径对应，KEY=HASH, VALUE=全路径
        /// </summary>
        internal static Dictionary<string, string> HashFullNameDict;

        /// <summary>
        /// 共享文件夹列表
        /// </summary>
        internal static HashSet<string> Sharedir;

        internal static DownloadManager DownloadManager;

        internal static IPEndPoint _serverIpEndPoint;

        internal static IPEndPoint _csServerIPEndPoint;

        internal static Dictionary<string, DownloadJob> DownloadJobDict;

        static HPPClient()
        {
            ClientThreadPool = new SmartThreadPool();
            HashFullNameDict = GetSerializeObject(DAT_HASH_FULLNAME) as Dictionary<string, string> ?? new Dictionary<string, string>();

            KnownDict = GetSerializeObject(DAT_HASH_KNOWN) as HashSet<string> ?? new HashSet<string>();

            DownloadJobDict = GetSerializeObject(DAT_HASH_JOB) as Dictionary<string, DownloadJob> ??
                              new Dictionary<string, DownloadJob>();

            _server = new CSServer(NUM_CONNECTION, Int16.MaxValue);
            _hashCalc = new HashCalculator(new Hash_MD5(), ClientThreadPool);

            _webHelper = new WebHelper();
            Sharedir = GetShareDir();
            DownloadManager = new DownloadManager();

            _serverIpEndPoint = new IPEndPoint(IPAddress.Any, 9998);

            _csServerIPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 9999);
        }

        public HPPClient()
        {
            
        }

        public void Test()
        {
            _webHelper.UploadShareDirAsync(_serverIpEndPoint, _csServerIPEndPoint);
        }

        public void Test2()
        {
            //ServerFileInfoResponse response = _webHelper.GetServerResponse(new IPEndPoint(IPAddress.Parse("192.168.0.101"), 9999),"424BFAAF17F147A94759888AFFA2D929");

            DownloadManager.Download("424BFAAF17F147A94759888AFFA2D929", "c:\\1.rar");

            int zz = 0;
        }

        #region 事件触发函数
        public virtual void OnDictChanged(object sender, EventArgs e)
        {
            if (DictChanged != null)
            {
                DictChanged(sender, e);
            }
        }

        #endregion

        #region 只读字典

        /// <summary>
        /// 只读共享文件夹列表
        /// </summary>
        public IEnumerable<string> ReadOnlySharedir
        {
            get { return Sharedir.AsEnumerable(); }
        }

        /// <summary>
        /// 只读Hash-全路径 对应列表
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>>  HashFullName
        {
            get { return HashFullNameDict.AsEnumerable();  }
        }

        #endregion


        #region 字典操作

        /// <summary>
        /// 添加文件路径到共享文件夹列表
        /// </summary>
        /// <param name="path">文件夹全路径</param>
        public void AddToShareDir(string path, bool flagInvokeEvent)
        {
            if(!Sharedir.Contains(path))
            {
                Sharedir.Add(path);
                if(flagInvokeEvent)
                {
                    OnDictChanged(this, new EventArgs());
                }
                
            }
        }

        /// <summary>
        /// 添加Hash-全路径 对应
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="fullname">全路径</param>
        public void AddToHashFullName(string hash, string fullname, bool flagInvokeEvent)
        {
            if(!HashFullNameDict.ContainsKey(hash))
            {
                HashFullNameDict.Add(hash, fullname);
                if (flagInvokeEvent)
                {
                    OnDictChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 添加到已Hash字典
        /// </summary>
        /// <param name="fullname">全路径</param>
        public void AddToKnown(string fullname, bool flagInvokeEvent)
        {
            if(!KnownDict.Contains(fullname))
            {
                KnownDict.Add(fullname);
                if (flagInvokeEvent)
                {
                    OnDictChanged(this, new EventArgs());
                }
            }
        }



        #endregion

        #region 公共方法

        public void Download(string hash)
        {
            
        }

        public void CheckNewShareDir()
        {
            List<string> toHashList = new List<string>();
            foreach (string s in Sharedir)
            {
                DirectoryInfo di = new DirectoryInfo(s);
                FileInfo[] fileInfos = di.GetFiles();

                foreach (FileInfo info in fileInfos)
                {
                    if (!KnownDict.Contains(info.FullName))
                    {
                        toHashList.Add(info.FullName);
                    }
                }

                
            }

            _hashCalc.CalcAsync(toHashList, new Action<string, string>(AfterHashFile) );

        }

        public void StartListen()
        {
            _server.Init();
            //_server.Start(new IPEndPoint(IPAddress.Any, ));
        }

        #endregion 公共方法

        #region 私有方法

        private void AfterHashFile(string path, string hash)
        {
            AddToHashFullName(hash, path, false);
            AddToKnown(path, false);
            OnDictChanged(this, new EventArgs());
        }
        
        

        #endregion 私有方法

        #region 持久化保存方法

        /// <summary>
        /// 从持久化创建共享文件列表
        /// </summary>
        /// <returns>共享文件列表</returns>
        private static HashSet<string> GetShareDir()
        {
            HashSet<string> set = new HashSet<string>();

            try
            {
                using (FileStream fs = new FileStream(DAT_SHAREDIR, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string file = sr.ReadToEnd();
                    string[] sarr = file.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in sarr)
                    {
                        if (!set.Contains(s))
                        {
                            set.Add(s);
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
            return set;
        }

        /// <summary>
        /// 异步保存Hash-全路径字典
        /// </summary>
        public void SaveHashFullNameDictAsync()
        {
            ClientThreadPool.QueueWorkItem(new Func<bool>(SaveHashFullNameDict));

        }

        public void SaveKnownDictAsync()
        {
            ClientThreadPool.QueueWorkItem(new Func<bool>(SaveKnownDict));
        }

        /// <summary>
        /// 保存Hash-全路径字典
        /// </summary>
        /// <returns>是否保存成功</returns>
        private bool SaveHashFullNameDict()
        {
            return SerializeSaveObject(DAT_HASH_FULLNAME, HashFullNameDict);
        }

        private bool SaveKnownDict()
        {
            return SerializeSaveObject(DAT_HASH_KNOWN, KnownDict);
        }

        /// <summary>
        /// 序列化保存对象
        /// </summary>
        /// <param name="saveFileName">保存到的文件名称</param>
        /// <param name="saveObject">要保存的对象</param>
        /// <returns>是否保存成功</returns>
        public bool SerializeSaveObject(string saveFileName, object saveObject)
        {
            try
            {
                using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter b = new BinaryFormatter();
                    b.Serialize(fs, saveObject);
                }
                
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static object GetSerializeObject(string saveFileName)
        {
            object obj = null;
            try
            {
                using (FileStream fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter b = new BinaryFormatter();
                    obj = b.Deserialize(fs);
                }
            }
            catch (Exception)
            {
            }

            return obj;
        }

        #endregion
    }
}
