using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPClientLibrary
{
    class ExceptionEventArgs:System.EventArgs
    {
        //异常处理动作
        public enum ExceptionActions
        {
            Throw,
            CancelAll,
            Ignore,
            Retry
        }
        /// <summary>
        /// 包含 Exception 事件数据的类
        /// </summary>
        
        private System.Exception _Exception;
        private ExceptionActions _ExceptionAction;
        private DownLoadState _DownloadState;
        public DownLoadState DownloadState
        {
            get
            {
                return _DownloadState;
            }
        }
        public Exception Exception
        {
            get
            {
                return _Exception;
            }
        }
        public ExceptionActions ExceptionAction
        {
            get
            {
                return _ExceptionAction;
            }
            set
            {
                _ExceptionAction = value;
            }
        }
        internal ExceptionEventArgs(System.Exception e, DownLoadState DownloadState)
        {
            this._Exception = e;
            this._DownloadState = DownloadState;
        }
        
    }
}
