using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using Amib.Threading;


namespace HPPClientLibrary
{
    class HttpWebClient
    {
        private SmartThreadPool _SmartThreadPool;

        public HttpWebClient(SmartThreadPool pool)
        {
            this._SmartThreadPool = pool;
        }

        private static object _SyncLockObject = new object();
        public delegate void DataReceiveEventHandler(HttpWebClient Sender, DownLoadEventArgs e);
        public event DataReceiveEventHandler DataReceive; //接收字节数据事件
        public delegate void ExceptionEventHandler(HttpWebClient Sender, ExceptionEventArgs e);
        public event ExceptionEventHandler ExceptionOccurrs; //发生异常事件
        public delegate void ThreadProcessEventHandler(HttpWebClient Sender, ThreadProcessEventArgs e);
        public event ThreadProcessEventHandler ThreadProcessEnd; //发生多线程处理完毕事件
        private int _FileLength; //下载文件的总大小

        public int FileLength
        {
            get
            {
                return _FileLength;
            }
        }

        //将Contant-Range分割
        public List<int> GetRange(string range)
        {
            string[] sep = { "", "-", "/" };
            string[] str = range.Split(sep, StringSplitOptions.None);
            List<int> ran = new List<int>();
            for (int i = 1; i < 3; i++)
            {
                ran.Add(Convert.ToInt32(str[i]));
            }
            return ran;
        }


        public void DownloadFile(string url, string fileName)
        {
            HttpWebRequest request;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                string range = response.Headers["Content-Range"];
                List<int> ran = new List<int>();
                ran = GetRange(range);//求出要下的文件分块的起始点，终点，以及总长

                int len = ran[2];//分块长度
                int offset = ran[0];
                DownLoadState x = new DownLoadState(url, response, fileName, offset, len, new DownLoadState.ThreadCallbackHandler(ResponseAsBytes));
                //DownLoadState x = new DownLoadState(url,response,offset,len,new DownLoadState.ThreadCallbackHandler(ResponseAsBytes));
                //       单线程下载
                //       x.StartDownloadFileChunk();
                x.httpWebClient = this;
                //多线程下载

                _SmartThreadPool.QueueWorkItem(new Action(x.StartDownloadFileChunk));


                //byte[] buffer = this.ResponseAsBytes(url, hwrp, Length, FileName);



            }
            catch (Exception e)
            {
                //ExceptionEventArgs.ExceptionActions ea = ExceptionEventArgs.ExceptionActions.Throw;
                //if (this.ExceptionOccurrs != null)
                //{
                //    DownLoadState x = new DownLoadState(url, response.ResponseUri.AbsolutePath, fileName, realFileName, offset, blockSize);
                //    ExceptionEventArgs eea = new ExceptionEventArgs(e, x);
                //    ExceptionOccurrs(this, eea);
                //    ea = eea.ExceptionAction;
                //}
                //if (ea == ExceptionEventArgs.ExceptionActions.Throw)
                //{
                //    if (!(e is WebException) && !(e is SecurityException))
                //    {
                //        throw new WebException("net_webclient", e);
                //    }
                //    throw;
                //}
            }
        }

        internal byte[] ResponseAsBytes(string RequestURL, WebResponse Response, int Length, int ptr, string FileName)
        {
            // string a = null; //AttachmentName
            int filePtr = 0; //整个文件的位置指针
            int readBytes = 0;
            try
            {
                //a = Response.Headers["Content-Disposition"]; //attachment
                //if (a != null)
                //{
                //    a = a.Substring(a.LastIndexOf("filename=") + 9);
                //}
                long blockSize = Length; //Response.ContentLength;
                bool flagNoLen = false;
                if (blockSize == -1)//这里可能有问题
                {
                    flagNoLen = true;
                    blockSize = 0x10000; //64k
                }
                byte[] buffer1 = new byte[(int)blockSize];
                int blockPtr = 0; //本块的位置指针
                //string s = Response.Headers["Content-Range"];
                //if (s != null)
                //{
                //    s = s.Replace("bytes ", "");
                //    s = s.Substring(0, s.IndexOf("-"));
                //    filePtr = Convert.ToInt32(s);
                //}
                filePtr = ptr;
                int totalRead = 0;
                Stream responseStream = Response.GetResponseStream();
                do
                {
                    readBytes = responseStream.Read(buffer1, totalRead, ((int)blockSize) - totalRead);
                    totalRead += readBytes;
                    if (flagNoLen && (totalRead == blockSize))
                    {
                        blockSize += 0x10000;
                        byte[] buffer2 = new byte[(int)blockSize];
                        Buffer.BlockCopy(buffer1, 0, buffer2, 0, totalRead);
                        buffer1 = buffer2;
                    }
                    //    lock (_SyncLockObject)
                    //    {
                    //     this._bytes += num2;
                    //    }
                    if (readBytes > 0)
                    {
                        if (this.DataReceive != null)
                        {
                            byte[] buffer = new byte[readBytes];
                            Buffer.BlockCopy(buffer1, blockPtr, buffer, 0, buffer.Length);
                            //写硬盘
                            DownLoadState dls = new DownLoadState(FileName, filePtr, readBytes, buffer);
                            DownLoadEventArgs dlea = new DownLoadEventArgs(dls);
                            //触发事件
                            this.OnDataReceive(dlea);
                            //System.Threading.Thread.Sleep(100);
                        }
                        blockPtr += readBytes; //本块的位置指针
                        filePtr += readBytes; //整个文件的位置指针
                    }
                    else
                    {
                        break;
                    }
                }
                while (readBytes != 0);

                responseStream.Close();
                responseStream = null;
                if (flagNoLen)
                {
                    byte[] buffer3 = new byte[totalRead];
                    Buffer.BlockCopy(buffer1, 0, buffer3, 0, totalRead);
                    buffer1 = buffer3;
                }
                return buffer1;
            }
            catch (Exception e)
            {
                //ExceptionEventArgs.ExceptionActions ea = ExceptionEventArgs.ExceptionActions.Throw;
                //if (this.ExceptionOccurrs != null)
                //{
                //    DownLoadState x = new DownLoadState(RequestURL, Response.ResponseUri.AbsolutePath, FileName, a, filePtr, readBytes);
                //    ExceptionEventArgs eea = new ExceptionEventArgs(e, x);
                //    ExceptionOccurrs(this, eea);
                //    ea = eea.ExceptionAction;
                //}
                //if (ea == ExceptionEventArgs.ExceptionActions.Throw)
                //{
                //    if (!(e is WebException) && !(e is SecurityException))
                //    {
                //        throw new WebException("net_webclient", e);
                //    }
                //    throw;
                //}
                //return null;
                return null;
            }
        }

        private void OnDataReceive(DownLoadEventArgs e)
        {
            //触发数据到达事件
            if (DataReceive != null)
            {
                DataReceive(this, e);
            }

        }

        //private void x_DataReceive(Microshaoft.Utils.HttpWebClient Sender, Microshaoft.Utils.DownLoadEventArgs e)
        public void WriteToDisk(HttpWebClient Sender, DownLoadEventArgs e)
        {

            string f = e.DownloadState.FileName;
            //if (e.DownloadState.AttachmentName != null)
            // f = System.IO.Path.GetDirectoryName(f) + @"\" + e.DownloadState.AttachmentName;
            //this._f = f;
            using (System.IO.FileStream sw = new System.IO.FileStream(f, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
            {
                sw.Position = e.DownloadState.Offset;
                sw.Write(e.DownloadState.Data, 0, e.DownloadState.Data.Length);
                sw.Close();
            }
        }


    }
}
