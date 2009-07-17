using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPClientLibrary
{
    public class ThreadProcessEventArgs : System.EventArgs
    {
        private Action<string, string, int, int> _actionItem;
        public Action<string, string, int, int> Action
        {
            get
            {
                return this._actionItem;
            }
            set
            {
                this._actionItem = value;
            }
        }
        public ThreadProcessEventArgs(Action<string, string, int, int> thread)
        {
            this._actionItem = thread;
        }
    }
}
