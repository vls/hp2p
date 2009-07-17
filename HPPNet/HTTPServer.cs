using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using Amib.Threading;
using Amib.Threading.Internal;
using HPPUtil;

namespace HPPNet
{
    public abstract class HTTPServer : Server
    {

        public HTTPServer(int numConnections, int receiveBufferSize) : base(numConnections, receiveBufferSize)
        {
        }

        /// <summary>
        /// 实现基类OnReceived函数
        /// </summary>
        /// <param name="e">SocketAsyncEventArgs对象</param>
        protected override void OnReceived(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;


            int totalRead = e.BytesTransferred;
            int contentLength = -1;
            while (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {

                string tempdata = Encoding.UTF8.GetString(e.Buffer, e.Offset, totalRead);
                
                if (contentLength == -1)
                {
                    int index;
                    if ((index = tempdata.IndexOf("Content-Length")) != -1)
                    {
                        int indexCRLF;
                        if ((indexCRLF = tempdata.IndexOf("\r\n", index)) != -1)
                        {
                            int len = indexCRLF - index;
                            string sub = tempdata.Substring(index, len);
                            string[] sarr = sub.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                            if (sarr.Length == 2)
                            {
                                string lenStr = sarr[1].Trim();
                                int bodyLen;
                                if (int.TryParse(lenStr, out bodyLen))
                                {
                                    contentLength = bodyLen;
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (contentLength != -1)
                {
                    int headEnd = tempdata.IndexOf("\r\n\r\n");
                    if (headEnd != -1)
                    {
                        if (totalRead >= headEnd + 4 + contentLength)
                        {
                            break;
                        }
                    }
                }


                int read;
                read = token.Socket.Receive(e.Buffer, e.Offset + totalRead, e.Count, SocketFlags.None);
                if (read <= 0)
                {
                    break;
                }
                totalRead += read;
                
            }

            string data = Encoding.UTF8.GetString(e.Buffer, e.Offset, totalRead);
            
            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now, data));

            if (token == null)
            {
                return;
            }
            //解释HTTP请求
            HttpProcessor processor = new HttpProcessor(token.Socket.RemoteEndPoint, new System.Func<EndPoint, Hashtable, string, Response>(ProcessHttpReq), token.Socket, data);

            processor.Process();

        }

        

        
        
        protected abstract Response ProcessHttpReq(EndPoint remoteEndPoint, Hashtable headers, string body);

    }

    /// <summary>
    /// HTTP解释器
    /// </summary>
    public class HttpProcessor
    {
        private bool keepAlive = false;
        private NetworkStream ns;
        private StreamWriter sw;
        private Socket _socket;
        private Hashtable _headers;
        private string _body;
        private System.Func<EndPoint, Hashtable, string, Response> _processFunc;
        private string _requestString;
        private EndPoint _endPoint;

        private static Dictionary<string, object> supportMethodDict;

        public HttpProcessor(EndPoint endPoint, System.Func<EndPoint, Hashtable, string, Response> processFunc, Socket s, string request)
        {
            _socket = s;
            _endPoint = endPoint;
            _processFunc = processFunc;
            _requestString = request;
            supportMethodDict = new Dictionary<string, object>()
                                    {
                                        {"GET", null},
                                        {"POST", null}
                                    };
        }
        /*
        /// <summary>
        /// 获取错误的Response头（推荐仅在ParseRequest()== false时使用）
        /// </summary>
        /// <returns>Response字符串</returns>
        public string GetErrorResponse()
        {
            return sb.ToString();
        }
        */
        public void Process()
        {
           

            try
            {
                ns = new NetworkStream(_socket, FileAccess.ReadWrite);

                sw = new StreamWriter(ns);

                if (ParseRequest(_requestString, out _headers, out _body))
                {
                    keepAlive = "Keep-Alive".Equals(_headers["Connection"]);

                    //调用处理函数，获得Response
                    Response res = _processFunc(_endPoint, _headers, _body);

                    //Response为空，返回404
                    if(res == null)
                    {
                        WriteFail();
                        return;
                    }

                    if(res is VoidResponse)
                    {
                        WriteOK(0, null);
                        return;
                    }

                    //根据Response类型，决定调用的方法
                    if (res is FileResponse)
                    {
                        try
                        {
                            FileResponse fileResponse = res as FileResponse;
                            FileInfo fileInfo = new FileInfo(fileResponse.FileName);

                            //如果文件有Range
                            if (fileResponse.HasRange && fileResponse.RangeValid)
                            {
                                FileStream fs = new FileStream(fileResponse.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                                bool flag = fs.CanSeek;

                                fs.Seek(fileResponse.RangeBegin, SeekOrigin.Begin);

                                int buffLength = 256;

                                byte[] buff = new byte[buffLength];

                                Hashtable headers = new Hashtable();
                                headers["Content-Range"] = string.Format("bytes {0}-{1}/{2}", fileResponse.RangeBegin, fileResponse.RangeEnd, fileInfo.Length);
                                headers["Content-Type"] = "application/octet-stream";
                                headers["Content-Disposition"] = string.Format("attachment;filename={0}", fileInfo.Name);
                                WritePartial(fileResponse.Length, headers);

                                int totalRead = 0;
                                Console.WriteLine(string.Format("BeginSendFile {0} Part {1}-{2}", fileResponse.FileName, fileResponse.RangeBegin, fileResponse.RangeEnd));
                                while(true)
                                {
                                    long remain = fileResponse.Length - totalRead;

                                    if(remain <= 0)
                                    {
                                        break;
                                    }

                                    int len = (int) (remain > buffLength ? buffLength : remain);

                                    int reads = fs.Read(buff, 0, len);

                                    string readed = Encoding.Default.GetString(buff);

                                    totalRead += reads;
                                    if (reads == 0)
                                    {
                                        break;
                                    }

                                    ns.Write(buff, 0, len);
                                    ns.Flush();
                                }

                                Console.WriteLine("EndSendFile Part");

                                
                                    
                                int zz = 0;
                            }
                            else
                            {
                                WriteOK(fileInfo.Length, null);
                                sw.Close();
                                ns.Close();
                                Console.WriteLine("BeginSendFile {0}", fileResponse.FileName);
                                _socket.SendFile(fileResponse.FileName);
                                Console.WriteLine("EndSendFile {0}", fileResponse.FileName);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            WriteFail();
                        }

                    }
                    else if (res is StringResponse)
                    {

                        StringResponse strRes = res as StringResponse;
                        string result = strRes.ResponseStr;

                        byte[] bytes = Encoding.UTF8.GetBytes(result);

                        //计算字符串的UTF-8模式下所占字节数
                        long length = result.GetUTF8Count();
                        WriteOK(length, null);
                        
                        //这里使用UTF-8 Encoding获取字符串的byte[], 直接对NetworkStream写入，避免StreamWriter可能有的BOM问题
                        ns.Write(bytes, 0, bytes.Length);
                        ns.Flush();
                    }
                }
            }
            catch(IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    SocketException se = e.InnerException as SocketException;
                    Console.WriteLine(se.ErrorCode);
                    return;
                }
                throw;
                
            }
            finally
            {
                try
                {
                    sw.Close();
                    ns.Close();
                }
                catch (Exception)
                {
                    
                }
            }

            

            
           
        }

        private void WritePartial(long length, Hashtable headers)
        {
            headers["Accept-Ranges"] = "bytes";
            headers["Date"] = DateTime.Now.ToUniversalTime().ToString("r");
            WriteResult(206, "Partial Content", length, headers);
        }

        private void WriteContine()
        {
            WriteResult(100, "Continue", 0, null);
            sw.Flush();
        }

        private void WriteHeader(string key,string value)
        {
            sw.Write(string.Format("{0}: {1}\r\n", key, value));
        }

        private void WriteOK(long length, Hashtable headers)
        {
            WriteResult(200, "OK", length, headers);
        }

        private void WriteFail()
        {
            WriteError(404, "File not found");
        }

        
        /// <summary>
        /// 构造发生错误时的HTTP Response
        /// </summary>
        /// <param name="status">HTTP Status Code</param>
        /// <param name="message">错误信息</param>
        private void WriteError(int status, string message)
        {
            string output = "<h1>HTTP/1.0 " + status + " " + message + "</h1>";
            WriteResult(status, message, (long)output.Length, null);
            sw.Write(output);
            sw.Flush();
        }

        /// <summary>
        /// 构造HTTP头
        /// </summary>
        /// <param name="status">HTTP Status Code</param>
        /// <param name="message">回发信息</param>
        /// <param name="length">回发信息长度</param>
        /// <param name="pairCollection">回发Headers</param>
        private void WriteResult(int status, string message, long length, Hashtable headers)
        {
            
            sw.Write("HTTP/1.0 " + status + " " + message + "\r\n");
            sw.Write("Content-Length: " + length + "\r\n");
            if (keepAlive)
            {
                sw.Write("Connection: Keep-Alive\r\n");
            }
            else
            {
                sw.Write("Connection: close\r\n");
            }

            if (headers != null)
            {
                foreach (DictionaryEntry entry in headers)
                {
                    WriteHeader(entry.Key.ToString(), entry.Value.ToString());
                }
            }

            

            sw.Write("\r\n");
            sw.Flush();
        }

        /// <summary>
        /// 解析Request
        /// </summary>
        /// <param name="reqString">完整的Request</param>
        /// <param name="method">Request的Method</param>
        /// <param name="url">Request的URL</param>
        /// <param name="protocol">Request的协议</param>
        /// <param name="headers">Request的Headers</param>
        /// <param name="body">Request的Body</param>
        /// <returns>是否解析成功</returns>
        private bool ParseRequest(string reqString,out Hashtable headers, out string body)
        {
            headers = new Hashtable();
            string method = String.Empty;
            string url = String.Empty;
            string protocol = String.Empty;
            
            //以\r\n\r\n分割Header与BOdy
            string[] strings = reqString.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            
            string headerStr = strings[0];

            if(strings.Length > 1)
            {
                body = strings[1];
            }
            else
            {
                body = "";
            }

            //以\r\n分割HTTP Header的每一行
            string[] headerArr = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            //获取Header第一行,并以空格分割，取出Method,URL,Protocol
            string[] firstLine = headerArr[0].Split(new string[] { " " }, StringSplitOptions.None);
            if(firstLine.Length != 3)
            {
                WriteError(400, "Bad Request");
                return false;
            }

            method = firstLine[0];
            if(!supportMethodDict.ContainsKey(method))
            {
                WriteError(501, method + " not implemented");
                return false;
            }
            headers["Method"] = method;

            url = firstLine[1];

            if(!url.StartsWith("/"))
            {
                WriteError(400, "Bad URL");
                return false;
            }

            url = HttpUtility.UrlDecode(url);
            headers["Url"] = url;

            protocol = firstLine[2];

            if(!protocol.StartsWith("HTTP/"))
            {
                WriteError(400, "Bad Protocol: " + protocol);
            }

            headers["Protocol"] = protocol;

            //把剩下的Header放进HashTable
            string name = null;
            if(headerArr.Length > 1)
            {
                for (int i = 1; i < headerArr.Length; i++)
                {
                    string line = headerArr[i];
                    if(name != null && Char.IsWhiteSpace(line[0]))
                    {
                        headers[name] += line;
                        continue;
                        
                    }

                    int firstCol = line.IndexOf(":");
                    if(firstCol != -1)
                    {
                        name = line.Substring(0, firstCol);
                        string value = line.Substring(firstCol + 1).Trim();
                        headers[name] = value;
                    }
                    else
                    {
                        WriteError(400, "Bad Header:" + line);
                        return false;
                    }
                }
            }

            if (headers.ContainsKey("Expect"))
            {
                //WriteContine();
            }

            return true;
        }

    }
}
