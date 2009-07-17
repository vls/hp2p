using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPClientLibrary
{
    class DownLoadEventArgs : System.EventArgs
    {
        private DownLoadState _DownloadState;
        public DownLoadState DownloadState
        {
            get
            {
                return _DownloadState;
            }
        }
        //构造函数
        public DownLoadEventArgs(DownLoadState DownloadState)
        {
            this._DownloadState = DownloadState;
        }
    }
}
