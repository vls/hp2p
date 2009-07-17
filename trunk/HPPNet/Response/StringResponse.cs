using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPPNet
{
    public class StringResponse : Response
    {
        private string _responseStr;

        public string ResponseStr
        {
            get { return _responseStr; }
            private set { _responseStr = value; }
        }

        public StringResponse(string responseStr)
        {
            ResponseStr = responseStr;
        }

    }
}