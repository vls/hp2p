using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HPPNet;

namespace HPPServer
{
    class TestSSServer : HTTPServer
    {
        public TestSSServer(int numConnections, int receiveBufferSize) : base(numConnections, receiveBufferSize)
        {
        }

        protected override Response ProcessHttpReq(EndPoint endpoint,Hashtable headers, string body)
        {
            FileResponse response = new FileResponse(@"c:\devtemp\allfeed.xml");
            /*
            string s;
            
            using (FileStream fs = new FileStream(@"c:\devtemp\allfeed.xml", FileMode.Open, FileAccess.Read))
            using(StreamReader sr = new StreamReader(fs))
            {
                
                s = sr.ReadToEnd();
            }
            
            
            //string s = "DFKSFSLJL";
            
            StringResponse response = new StringResponse(s);
            */
            /*
            object value = headers["Range"];
            if (value == null)
            {
                return null;
            }
            string range = value.ToString();
            if(!string.IsNullOrEmpty(range))
            {
                
                string[] lr = range.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);
                if(lr.Length == 2)
                {
                    string left = lr[0];
                    string right = lr[1];

                    string[] be = right.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries);

                    string begin = be[0];
                    string end = be[1];

                    long iBegin,iEnd;
                    if(long.TryParse(begin, out iBegin) && long.TryParse(end, out iEnd))
                    {
                        FileResponse response = new FileResponse(@"c:\devtemp\allfeed.xml");
                        response.HasRange = true;
                        response.RangeBegin = iBegin;
                        response.RangeEnd = iEnd;
                        return response;
                    }
                }
            }
            */



            return response;
             
        }
    }
}
