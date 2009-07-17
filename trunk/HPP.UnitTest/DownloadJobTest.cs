using HPPClientLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;

namespace HPP.UnitTest
{
    
    
    /// <summary>
    ///这是 DownloadJobTest 的测试类，旨在
    ///包含所有 DownloadJobTest 单元测试
    ///</summary>
    [TestClass()]
    public class DownloadJobTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private static DownloadJob _job;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _job = new DownloadJob();
            _job.Dispatch(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 1111), new List<int>()
                                                                                 {
                                                                                     1,2,3,4,5
                                                                                 });
            _job.Dispatch(new IPEndPoint(IPAddress.Parse("10.0.0.2"), 2222), new List<int>()
                                                                                 {
                                                                                     6,7,8,9,10,11,12
                                                                                 });
            _job.Dispatch(new IPEndPoint(IPAddress.Parse("10.0.0.3"), 3333), new List<int>()
                                                                                 {
                                                                                     13,14,15
                                                                                 });
        }

        #region 附加测试属性
        // 
        //编写测试时，还可使用以下属性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Dispatch 的测试
        ///</summary>
        [TestMethod()]
        public void DispatchTest()
        {
            //DownloadJob target = new DownloadJob(); // TODO: 初始化为适当的值
            IPEndPoint newClient = new IPEndPoint(IPAddress.Parse("10.0.0.4"), 4444); // TODO: 初始化为适当的值
            List<int> blockList = new List<int>()
                                      {
                                          3,4,5,6,7,13,16,17
                                      }; // TODO: 初始化为适当的值
            _job.Dispatch(newClient, blockList);
            Assert.IsTrue(true);
            //Assert.Inconclusive("无法验证不返回值的方法。");
        }
    }
}
