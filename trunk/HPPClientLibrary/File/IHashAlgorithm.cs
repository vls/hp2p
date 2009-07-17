using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HPPClientLibrary
{
    public interface IHashAlgorithm
    {
        string Calc(string fileFullName);
    }

   
    
}
