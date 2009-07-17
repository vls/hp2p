using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HPPClientLibrary
{
    class ServerFileInfoResponse
    {
        private List<IPEndPoint> _ipList;
        private long _fileSize;
        private int _blockNum;

        public int BlockNum
        {
            get { return _blockNum; }
            set { _blockNum = value; }
        }

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        public List<IPEndPoint> IpList
        {
            get { return _ipList; }
            set { _ipList = value; }
        }
    }
}
