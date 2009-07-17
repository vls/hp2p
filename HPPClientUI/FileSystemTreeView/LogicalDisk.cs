using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;

namespace HPPClientUI.FileSystemTreeView
{
    /// <summary>
    /// LogicalDisk ��ժҪ˵����
    /// </summary>
    public class LogicalDisk
    {
        private LogicalDisk(){}

        /// <summary>
        /// ��ȡ���е��߼�������Ϣ
        /// </summary>
        /// <returns>�߼�������Ϣ</returns>
        public static LogicalDiskInfo[] GetLogicalDisks()
        {
            ManagementClass _diskClass = new ManagementClass("Win32_LogicalDisk");


            SelectQuery query = new SelectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType != 2");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            //ManagementObjectCollection _disks = _diskClass.GetInstances();
            

            List<LogicalDiskInfo> _logicalDisks = new List<LogicalDiskInfo>();

            ManagementObjectCollection _disks = searcher.Get();

            foreach(ManagementBaseObject _disk in searcher.Get())
            {
                _logicalDisks.Add(new LogicalDiskInfo(_disk));
            }

            LogicalDiskInfo[] _lDisks = new LogicalDiskInfo[_logicalDisks.Count];

            for(int i=0; i<_logicalDisks.Count; i++)
            {
                _lDisks[i] = (LogicalDiskInfo)_logicalDisks[i];
            }

            return _lDisks;
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    public enum DiskType : short
    {
        Unknown = 1,
        Removable = 2,
        Fixed = 3,
        Network = 4,
        CD_ROM = 5,
        RAM_Disk = 6
    }

    /// <summary>
    /// �߼�������Ϣ
    /// </summary>
    public struct LogicalDiskInfo
    {
        private string _deviceID;
        private string _name;
        private string _volumeName;
        private string _fileSystem;
        private string _description;
        private long _size;
        private DiskType _driveType;

        public LogicalDiskInfo(ManagementBaseObject disk)
        {
            _deviceID		= disk["DeviceID"]		!= null ? disk["DeviceID"].ToString()		: "";
            _name			= disk["Name"]			!= null ? disk["Name"].ToString()			: "";
            _volumeName		= disk["VolumeName"]	!= null ? disk["VolumeName"].ToString()		: "";
            _fileSystem		= disk["FileSystem"]	!= null ? disk["FileSystem"].ToString()		: "";
            _description	= disk["Description"]	!= null ? disk["Description"].ToString()	: "";
            _size			= Convert.ToInt64(disk["Size"]);
            _driveType		= (DiskType)Convert.ToInt16(disk["DriveType"]);

            switch(_driveType)
            {
                case DiskType.CD_ROM:
                    _volumeName = "����������";
                    break;
                case DiskType.Fixed:
                    if(_volumeName == "")
                        _volumeName = "���ش���";
                    break;
            }
        }

        public LogicalDiskInfo(
            string deviceID, 
            string name, 
            string volumeName, 
            string fileSystem,
            string description,
            long size,
            short driveType)
        {
            _deviceID = deviceID;
            _name = name;
            _volumeName = volumeName;
            _fileSystem = fileSystem;
            _description = description;
            _size = size;
            _driveType = (DiskType)driveType;
        }

        /// <summary>
        /// �豸ID
        /// </summary>
        public string DeviceID
        {
            get{ return this._deviceID;}
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string Name
        {
            get{ return this._name;}
        }

        /// <summary>
        /// ���̾��
        /// </summary>
        public string VolumeName
        {
            get{ return this._volumeName;}
        }

        /// <summary>
        /// �ļ�ϵͳ
        /// </summary>
        public string FileSystem
        {
            get{ return this._fileSystem;}
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string Decription
        {
            get{ return this._description;}
        }

        /// <summary>
        /// ���̴�С
        /// </summary>
        public long Size
        {
            get{ return this._size;}
        }

        /// <summary>
        /// ��������
        /// </summary>
        public DiskType DriveType
        {
            get{ return this._driveType;}
        }
    }
}