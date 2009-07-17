using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HPPClientUI.FileSystemTreeView
{
    public class DirectoryNode : TreeNode
    {
        private DirectoryInfo _directoryInfo;

        public DirectoryNode(DirectoryNode parent, DirectoryInfo directoryInfo)
            : base(directoryInfo.Name)
        {
            this._directoryInfo = directoryInfo;

            parent.Nodes.Add(this);
            Icon icon = ShellIcon.GetSmallIcon(directoryInfo.FullName);
            this.ImageIndex = TreeView.GetIconImageIndex(icon);
            this.SelectedImageIndex = this.ImageIndex;
            Virtualize();
        }

        public DirectoryNode(FileSystemTreeView treeView, DirectoryInfo directoryInfo)
            : base(directoryInfo.Name)
        {
            this._directoryInfo = directoryInfo;

            
            this.SelectedImageIndex = this.ImageIndex;

            treeView.Nodes.Add(this);
            Icon icon = ShellIcon.GetSmallIcon(directoryInfo.FullName);
            this.ImageIndex = TreeView.GetIconImageIndex(icon);
            this.SelectedImageIndex = this.ImageIndex;
            Virtualize();

        }

        void Virtualize()
        {
            int fileCount = 0;

            try
            {
                if (this.TreeView.ShowFiles == true)
                {
                    fileCount = this.DirectoryInfo.GetFiles().Length;
                }
                    

                if ((fileCount + this.DirectoryInfo.GetDirectories().Length) > 0)
                {
                    new FakeChildNode(this);
                }
                    
            }
            catch
            {
            }
        }

        public void LoadDirectory()
        {
            foreach (DirectoryInfo directoryInfo in DirectoryInfo.GetDirectories())
            {
                DirectoryNode dn = new DirectoryNode(this, directoryInfo);
                dn.ContextMenuStrip =  this.TreeView.DirectoryContextMenuStrip;
            }
        }

        public void LoadFiles()
        {
            foreach (FileInfo file in DirectoryInfo.GetFiles())
            {
                new FileNode(this, file);
            }
        }

        public bool Loaded
        {
            get
            {
                if (this.Nodes.Count != 0)
                {
                    if (this.Nodes[0] is FakeChildNode)
                        return false;
                }

                return true;
            }
        }

        public new FileSystemTreeView TreeView
        {
            get { return (FileSystemTreeView)base.TreeView; }
        }

        public DirectoryInfo DirectoryInfo
        {
            get { return _directoryInfo; }
        }
    }
}
