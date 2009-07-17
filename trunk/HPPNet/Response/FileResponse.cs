using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPNet
{
    public class FileResponse : Response
    {
        private string _fileName;
        private bool _hasRange = false;
        private long _rangeBegin;
        private long _rangeEnd;

        public string FileName
        {
            get { return _fileName; }
            private set { _fileName = value; }
        }

        public bool HasRange
        {
            get { return _hasRange; }
            set { _hasRange = value; }
        }

        public long RangeBegin
        {
            get { return _rangeBegin; }
            set { _rangeBegin = value; }
        }

        public long RangeEnd
        {
            get { return _rangeEnd; }
            set { _rangeEnd = value; }
        }

        public bool RangeValid
        {
            get { return RangeEnd > RangeBegin; }
        }

        public long Length
        {
            get { return RangeEnd - RangeBegin + 1; }
        }


        public FileResponse(string fileName)
        {
            FileName = fileName;
        }
    }
}
