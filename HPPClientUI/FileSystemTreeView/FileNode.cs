using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HPPClientUI.FileSystemTreeView
{
    public class FileNode : TreeNode
    {
        private FileInfo _fileInfo;
        private DirectoryNode _directoryNode;

        public FileNode(DirectoryNode directoryNode, FileInfo fileInfo)
            : base(fileInfo.Name)
        {
            this._directoryNode = directoryNode;
            this._fileInfo = fileInfo;

            this.ImageIndex = ((FileSystemTreeView)_directoryNode.TreeView).GetIconImageIndex(_fileInfo.FullName);
            this.SelectedImageIndex = this.ImageIndex;

            _directoryNode.Nodes.Add(this);
        }
    }
}
