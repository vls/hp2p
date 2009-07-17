using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Amib.Threading;

namespace HPPClientLibrary
{
    class DownLoadState
    {

        private string _FileName;//文件名
        private string _AttachmentName;//附件名
        private int _Offset;//开始位置
        private string _RequestURL;//请求URL
        private WebResponse response;
       
        private int _Length;//文件总长度
        private byte[] _Data;//转换出来的字节数组
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                
            }
        }
        public int Offset
        {
            get
            {
                return _Offset;
            }
        }
        public int Length
        {
            get
            {
                return _Length;
            }
        }
        //public string AttachmentName
        //{
        //    get
        //    {
        //        return _AttachmentName;
        //    }
        //}
        public string RequestURL
        {
            get
            {
                return _RequestURL;
            }
        }
        public WebResponse Response
        {
            get
            {
                return this.response;
            }
            set
            {
                this.response = value;
            }
        }
        public byte[] Data
        {
            get
            {
                return _Data;
            }
        }

        internal DownLoadState(string fileName, int offset, int len, byte[] buffer)
        {
            this._FileName = fileName;
            this._Offset = offset;
            this._Length = len;
            this._Data = buffer;
           
        }
        internal DownLoadState(string RequestURL, string FileName, string AttachmentName, int Position, int Length, byte[] Data)
        {
            this._FileName = FileName;
            this._RequestURL = RequestURL;
            
            this._AttachmentName = AttachmentName;
            this._Offset = Position;
            this._Data = Data;
            this._Length = Length;
        }
        internal DownLoadState(string RequestURL, WebResponse response, string FileName, int Position, int Length, ThreadCallbackHandler tch)
        {
            this._RequestURL = RequestURL;
            this.response = response;
            this._FileName = FileName;
            this._Offset = Position;
            this._Length = Length;
            this._ThreadCallback = tch;
        }
       
        //委托代理线程的所执行的方法签名一致
        public delegate byte[] ThreadCallbackHandler(string _RequestURL, WebResponse response, int len, int offset,string filename);
        private ThreadCallbackHandler _ThreadCallback;//声明一个回调变量
        private HttpWebClient _hwc;
        public SmartThreadPool _smartThreadPool;
        private Action<string, string, int, int> _item;

       
       // private Thread _thread;
        
        public HttpWebClient httpWebClient
        {
            get
            {
                return this._hwc;
            }
            set
            {
                this._hwc = value;
            }
        }

        public Action<string,string ,int,int> Item
        {
            get
            {
                return _item;
            }
                
            set
            {
                this._item = value;
            }
        }

        internal void StartDownloadFileChunk()
        {

            if (this._ThreadCallback != null)
            {

                this._ThreadCallback(this._RequestURL, this.response, this._Offset, this._Length,this.FileName);
                //this._hwc.OnThreadProcess(this.Item);//开始处理
            }
        }
    }
}
