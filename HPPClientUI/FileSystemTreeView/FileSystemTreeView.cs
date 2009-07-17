using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HPPClientUI.FileSystemTreeView
{
    public class FileSystemTreeView : TreeView
    {
        private bool _showFiles = false;
        private ImageList _imageList = new ImageList();
        //private Hashtable _systemIcons = new Hashtable();
        private Dictionary<string, int> _extIndexDict;
        private Dictionary<int, int> _hashIndexDict;

        private ContextMenuStrip _contextMenuStrip;

        public event EventHandler<StringEventArgs> AddToSharedir;

        public static readonly int Folder = 0;

        public FileSystemTreeView()
        {
            this.ImageList = _imageList;
            _extIndexDict = new Dictionary<string, int>();
            _hashIndexDict = new Dictionary<int, int>();
            this.MouseDown += new MouseEventHandler(FileSystemTreeView_MouseDown);
            this.BeforeExpand += new TreeViewCancelEventHandler(FileSystemTreeView_BeforeExpand);

            
            //_systemIcons.Add(FileSystemTreeView.Folder, 0);
            //_extIndexDict.Add("", 0);

            IContainer container = new Container();
            ToolStripMenuItem item = new ToolStripMenuItem("添加到共享列表", null, new EventHandler(OnAddToSharedir));
            _contextMenuStrip = new ContextMenuStrip();
            DirectoryContextMenuStrip.Items.Add(item);
            
            
            int zz = 0;
        }

        public virtual void OnAddToSharedir(object sender, EventArgs e)
        {
            if(AddToSharedir != null)
            {
                AddToSharedir(this, new StringEventArgs(){ Path = ((DirectoryNode) this.SelectedNode).DirectoryInfo.FullName});
            }
        }

        public void Load()
        {
            GetLogicalDisk();
        }

        private void GetLogicalDisk()
        {
            LogicalDiskInfo[] _disks = LogicalDisk.GetLogicalDisks();

            foreach (LogicalDiskInfo _disk in _disks)
            {
                int len = this.Nodes.Count;
                DirectoryInfo di = GetRoot(new DirectoryInfo(_disk.Name));
                DirectoryNode _newItem = new DirectoryNode(this, di);
                _newItem.ContextMenuStrip = DirectoryContextMenuStrip;

                
            }
        }

        private DirectoryInfo GetRoot(DirectoryInfo di)
        {
            DirectoryInfo tempdi = di;
            while(tempdi.Parent != null)
            {
                tempdi = tempdi.Parent;
            }

            return tempdi;
        }

        void FileSystemTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = this.GetNodeAt(e.X, e.Y);

            if (node == null)
                return;

            this.SelectedNode = node; //select the node under the mouse        
            
        }

        void FileSystemTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is FileNode) return;

            DirectoryNode node = (DirectoryNode)e.Node;

            if (!node.Loaded)
            {
                node.Nodes[0].Remove(); //remove the fake child node used for virtualization
                node.LoadDirectory();
                if (this._showFiles == true)
                    node.LoadFiles();
            }
        }

        public int GetIconImageIndex(Icon icon)
        {
            int key = icon.Handle.GetHashCode();

            if(!_hashIndexDict.ContainsKey(key))
            {
                lock (_imageList)
                {
                    _imageList.Images.Add(icon);
                    _hashIndexDict.Add(key, _imageList.Images.Count - 1);
                }

                
            }

            return _hashIndexDict[key];
        }

        public int GetIconImageIndex(string path)
        {
            string extension = Path.GetExtension(path);

            if (!_extIndexDict.ContainsKey(extension))
            {
                Icon icon = ShellIcon.GetSmallIcon(path);

                lock (_imageList)
                {
                    _imageList.Images.Add(icon);
                    _extIndexDict.Add(extension, _imageList.Images.Count - 1);
                }
                
            }

            return (int)_extIndexDict[extension];
        }

        public bool ShowFiles
        {
            get { return this._showFiles; }
            set { this._showFiles = value; }
        }

        public ContextMenuStrip DirectoryContextMenuStrip
        {
            get { return _contextMenuStrip; }
        }
    }
}
