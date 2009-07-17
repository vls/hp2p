using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HPPClientLibrary;

namespace HPPClientUI
{
    public partial class MainForm : Form
    {
        private HPPClient _client;

        public HPPClient Client
        {
            get
            {
                return _client;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void Test()
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _client = new HPPClient();
            FakeUI();
        }

        private void btnDirectory_Click(object sender, EventArgs e)
        {
            Settings frmSettings = new Settings(this);
            frmSettings.Show();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _client.Test();
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            _client.Test2();
        }

        private void FakeUI()
        {
            ColumnHeader[] headers;
            headers= new ColumnHeader[]
                                         {
                                             new ColumnHeader(){Text = "状态"}, 
                                             new ColumnHeader(){Text = "名称" ,Width = 150}, 
                                             new ColumnHeader(){Text = "大小"}, 
                                             new ColumnHeader(){Text = "进度"}, 
                                             new ColumnHeader(){Text = "速度"}, 
                                             
        };
            this.lvDownload.Columns.AddRange(headers);

            ListViewItem item = new ListViewItem(new string[]{"停止", "梁静茹-情歌.AVI", "104 MB", "70%", ""});
            this.lvDownload.Items.Add(item);


            headers = new ColumnHeader[]
                          {
                              new ColumnHeader(){Text = "时间", Width = 80}, 
                              new ColumnHeader(){Text = "内容", Width = 150}, 
            };
            this.lvStatus.Columns.AddRange(headers);

            ListViewItem[] items;
                items = new ListViewItem[]
                                       {
                                           new ListViewItem(new string[]
                                               {
                                                   "2009-06-01 12:07:11", "开始连接"
                                               }), 
                                               new ListViewItem(new string[]
                                               {
                                                   "2009-06-01 12:07:11", "GET http://192.168.0.101/梁静茹-情歌.AVI|7e32b0df47269cf420866216a7076b58 HTTP 1.1"
                                               }), 
                                               new ListViewItem(new string[]
                                               {
                                                   "2009-06-01 12:07:11", "Content-Length: 0"
                                               }), 
                                               new ListViewItem(new string[]
                                               {
                                                   "2009-06-01 12:07:11", "Connection: Keep-Alive"
                                               }), 
                                               new ListViewItem(new string[]
                                               {
                                                   "2009-06-01 12:07:11", "用户取消下载"
                                               }),
        };
            this.lvStatus.Items.AddRange(items);

            headers = new ColumnHeader[]
                          {
                              new ColumnHeader(){Text = "文件名", Width = 150}, 
                              new ColumnHeader(){Text = "大小", }, 
                              new ColumnHeader(){Text = "Hash", Width = 200}, 
            };
            this.lvSearchResult.Columns.AddRange(headers);

            this.tbSearchTxt.Text = "梁静茹";
            items = new ListViewItem[]
                        {
                            new ListViewItem(new string[]
                                                 {
                                                     "梁静茹-情歌.AVI", "104 MB", "7e32b0df47269cf420866216a7076b58"
                                                 }),
                            new ListViewItem(new string[]
                                                 {
                                                     "梁静茹.-.[爱的大游行Live全记录CD1]专辑.(ape).ape", "410 MB",
                                                     "c9740242b2eb089e56cb1b24d3e76c6a"
                                                 }),
                            new ListViewItem(new string[]
                                                 {
                                                     "梁静茹.-.[爱的大游行Live全记录CD2]专辑.(ape).ape", "364 MB",
                                                     "0323dda1f59499574870f697314893a2"
                                                 }),
                            
        };
            this.lvSearchResult.Items.AddRange(items);
        }
    }
}
