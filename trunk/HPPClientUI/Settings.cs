using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HPPClientUI.FileSystemTreeView;

namespace HPPClientUI
{
    

    public partial class Settings : Form
    {
        private MainForm _frmRoot;

        public ListView HashFileList
        {
            get
            {
                return this.lvHashFileList;
            }
        }

        public ListView ShareDirList
        {
            get
            {
                return this.lvShareDir;
            }
        }

        public Settings(MainForm root)
        {
            InitializeComponent();
            _frmRoot = root;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.fileSystemTreeView1.AddToSharedir += new EventHandler<StringEventArgs>(OnAddToSharedir);
            this.fileSystemTreeView1.Load();
            this._frmRoot.Client.DictChanged += new EventHandler(OnDictChanged);
            
            InitListView();
            ListViewDataBind();
        }

        private void OnDictChanged(object sender, EventArgs e)
        {
            if(this.InvokeRequired)
            {
                Action action = new Action(ListViewDataBind);
                
                this.Invoke(action);
            }
            else
            {
                ListViewDataBind();
            }
        }

        private void InitListView()
        {
            ShareDirList.Columns.Add("路径", 200);
            ShareDirList.GridLines = true;
            ShareDirList.View = View.Details;

            HashFileList.Columns.Add("文件名", 80);
            HashFileList.Columns.Add("Hash", 80);
            HashFileList.GridLines = true;
            HashFileList.View = View.Details;
            
        }

        private void ListViewDataBind()
        {
            ShareDirList.Items.Clear();
            
            foreach (string sharedir in _frmRoot.Client.ReadOnlySharedir)
            {
                ListViewItem item = new ListViewItem(sharedir);
                ShareDirList.Items.Add(item);
            }

            HashFileList.Items.Clear();
            foreach (KeyValuePair<string, string> pair in _frmRoot.Client.HashFullName)
            {
                ListViewItem item = new ListViewItem(new string[]
                                                         {
                                                             pair.Value,
                                                             pair.Key,
                                        });
                HashFileList.Items.Add(item);
            }
        }

        private void OnAddToSharedir(object sender, StringEventArgs e)
        {
            AddToSharedir(e.Path);
            ListViewDataBind();
        }

        private void AddToSharedir(string path)
        {
            _frmRoot.Client.AddToShareDir(path, true);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DirectoryNode node = this.fileSystemTreeView1.SelectedNode as DirectoryNode;
            if (node == null)
            {
                return;
            }
            AddToSharedir(node.DirectoryInfo.FullName);
            ListViewDataBind();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _frmRoot.Client.CheckNewShareDir();
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
