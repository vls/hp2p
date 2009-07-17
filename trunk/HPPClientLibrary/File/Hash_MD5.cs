using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HPPClientLibrary
{
    class Hash_MD5 : IHashAlgorithm
    {

        public string Calc(string fileFullName)
        {
            try
            {
                FileStream get_file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] byte_hash = md5.ComputeHash(get_file);
                string string_hash = BitConverter.ToString(byte_hash);
                string_hash = string_hash.Replace("-", "");
                return string_hash;
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
