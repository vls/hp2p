using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPClientUI.FileSystemTreeView
{
    public class StringEventArgs : EventArgs
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
