using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amib.Threading;
using Amib.Threading.Internal;
using HPPNet;

namespace HPPClientLibrary
{
    public class HashCalculator
    {
        private IHashAlgorithm _method;
        private SmartThreadPool _pool;

        private static IWorkItemsGroup _hashGroup;


        public HashCalculator(IHashAlgorithm method, SmartThreadPool pool)
        {
            _method = method;
            _pool = pool;
            if(_hashGroup == null)
            {
                _hashGroup = _pool.CreateWorkItemsGroup(1);
            }
        }

        /// <summary>
        /// 计算Hash
        /// </summary>
        /// <param name="fileFullNameList">要计算Hash的文件全路径列表</param>
        /// <param name="postAction">每计算完一个Hash执行的委托</param>
        public void CalcAsync(List<string> fileFullNameList, Action<string, string> postAction)
        {
            foreach (string fileName in fileFullNameList)
            {
                _hashGroup.QueueWorkItem(new System.Action<string, Action<string,string>>(Calc) , fileName, postAction);
                
            }
           
        }

        public void Calc(string path, Action<string,string> postAction)
        {
            string md5 = _method.Calc(path);
            
            if(md5 != null && postAction != null)
            {
                postAction(path, md5);
            }
        }
    }
}
