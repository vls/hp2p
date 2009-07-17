using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HPPClientUI.FileSystemTreeView
{
    /// <summary>
    /// Summary description for ShellIcon.
    /// </summary>
    /// <summary>
    /// Summary description for ShellIcon.  Get a small or large Icon with an easy C# function call
    /// that returns a 32x32 or 16x16 System.Drawing.Icon depending on which function you call
    /// either GetSmallIcon(string fileName) or GetLargeIcon(string fileName)
    /// </summary>
    public class ShellIcon
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };


        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon


            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }


        public ShellIcon()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// 通过路径获取小图标
        /// </summary>
        /// <param name="fileName">文件或文件夹路径</param>
        /// <returns>获取的图标</returns>
        public static Icon GetSmallIcon(string fileName)
        {
            try
            {
                IntPtr hImgSmall; //the handle to the system image list
                SHFILEINFO shinfo = new SHFILEINFO();

                //Use this to get the small Icon
                hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

                //The icon is returned in the hIcon member of the shinfo struct
                return System.Drawing.Icon.FromHandle(shinfo.hIcon);
            }
            catch
            {
                return null;
            }

            
            
        }

        /// <summary>
        /// 通过路径获取大图标
        /// </summary>
        /// <param name="fileName">文件或文件夹路径</param>
        /// <returns>获取的图标</returns>
        public static Icon GetLargeIcon(string fileName)
        {

            try
            {
                IntPtr hImgLarge; //the handle to the system image list
                SHFILEINFO shinfo = new SHFILEINFO();


                //Use this to get the large Icon
                hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);


                //The icon is returned in the hIcon member of the shinfo struct
                return System.Drawing.Icon.FromHandle(shinfo.hIcon);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
