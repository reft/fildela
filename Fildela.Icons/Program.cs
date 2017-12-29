using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Fildela.Data.Storage.Services;

namespace Fildela.Icons
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Searching system for file icons..");

            //Get all icon extensions
            List<string> iconList = RegistryFileName.GetFileType();

            //Convert all icons to bitmap
            List<Bitmap> iconListBitmap = RegistryFileName.GetIconImageFromFilename(iconList);

            Console.WriteLine("Found " + iconList.Count() + " icons.");
            Console.WriteLine("Uploading..");

            //Upload all icons to Azure
            RegistryFileName.UploadIcons(iconList, iconListBitmap);

            Console.WriteLine("Upload done. Press any key to quit.");

            Console.ReadKey();
        }
    }

    public class RegistryFileName
    {
        public static Dictionary<string, Bitmap> FileIconAssociation = new Dictionary<string, Bitmap>();

        public static List<string> GetFileType()
        {

            List<string> allFiles = new List<string>();
            try
            {
                // Create a registry key object to represent the HKEY_CLASSES_ROOT registry section
                RegistryKey rkRoot = Registry.ClassesRoot;

                //Gets all sub keys' names.
                string[] keyNames = rkRoot.GetSubKeyNames();
                //Hashtable iconsInfo = new Hashtable();

                //Find the file icon.
                foreach (string keyName in keyNames)
                {
                    if (String.IsNullOrEmpty(keyName))
                        continue;
                    int indexOfPoint = keyName.IndexOf(".");

                    //If this key is not a file exttension(eg, .zip), skip it.
                    if (indexOfPoint != 0)
                        continue;

                    RegistryKey rkFileType = rkRoot.OpenSubKey(keyName);
                    if (rkFileType == null)
                        continue;
                    allFiles.Add(keyName);
                    rkFileType.Close();
                }
                rkRoot.Close();
                return allFiles;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        public static List<Bitmap> GetIconImageFromFilename(List<string> FileNames)
        {
            Bitmap bmpImage = null;
            string FileExtension = string.Empty;
            List<Bitmap> iconListSmall = new List<Bitmap>();
            List<Bitmap> iconListLarge = new List<Bitmap>();

            foreach (var file in FileNames)
            {
                int IndexOfLastDot = file.LastIndexOf(".");
                if (IndexOfLastDot >= 0)
                {
                    FileExtension = file.Substring(IndexOfLastDot + 1).ToLower();
                }
                if (!FileIconAssociation.TryGetValue(FileExtension, out bmpImage))
                {
                    //Small icon
                    IntPtr sImgSmall;
                    SHFILEINFO shinfoForSmallIcon = new SHFILEINFO();
                    sImgSmall = Win32.SHGetFileInfo(file, 0, ref shinfoForSmallIcon, (uint)Marshal.SizeOf(shinfoForSmallIcon), Win32.SHGFI_USEFILEATTRIBUTES | Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                    System.Drawing.Icon smallIcon = (System.Drawing.Icon)(System.Drawing.Icon.FromHandle(shinfoForSmallIcon.hIcon).Clone());
                    Win32.DestroyIcon(shinfoForSmallIcon.hIcon);
                    System.Drawing.Bitmap bmp1 = smallIcon.ToBitmap();
                    iconListSmall.Add(bmp1);

                    //Large icon
                    IntPtr hImgLarge;
                    SHFILEINFO shinfoForLargeIcon = new SHFILEINFO();
                    hImgLarge = Win32.SHGetFileInfo(file, 0, ref shinfoForLargeIcon, (uint)Marshal.SizeOf(shinfoForLargeIcon),
                        Win32.SHGFI_USEFILEATTRIBUTES | Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
                    System.Drawing.Icon largeIcon = (System.Drawing.Icon)(System.Drawing.Icon.FromHandle(shinfoForLargeIcon.hIcon).Clone());
                    Win32.DestroyIcon(shinfoForLargeIcon.hIcon);
                    System.Drawing.Bitmap bmp2 = largeIcon.ToBitmap();
                    iconListLarge.Add(bmp2);
                }
            }

            //return iconlist small or large
            return iconListSmall;
        }

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

        public static void UploadIcons(List<string> iconListName, List<Bitmap> iconListBitmap)
        {
            int listSize = iconListBitmap.Count();

            //Include blank
            int listSizeCount = listSize + 1;

            for (int i = 0; i < listSize; i++)
            {
                iconListName[i] = iconListName[i].Substring(1);

                CloudStorageServices CloudStorageServices = new CloudStorageServices();

                CloudBlobContainer blobIconContainer = CloudStorageServices.GetCloudBlobIconContainer();
                CloudBlockBlob blockBlob = blobIconContainer.GetBlockBlobReference(iconListName[i] + ".png");

                MemoryStream ms = new MemoryStream();
                iconListBitmap[i].Save(ms, ImageFormat.Png);
                ms.Position = 0;
                blockBlob.UploadFromStream(ms);

                Console.WriteLine(i + 1 + "/" + listSizeCount + " Fileicon " + iconListName[i] + " uploaded to Azure.");
            }

            UploadBlankIcon(listSizeCount);
        }

        public static void UploadBlankIcon(int listSize)
        {
            List<string> iconListName = new List<string>();
            //this icon gives us a blank png. lets upload it name: blank
            iconListName.Add(".3dmark-result");
            List<Bitmap> iconListBitmap = RegistryFileName.GetIconImageFromFilename(iconListName);

            CloudStorageServices CloudStorageServices = new CloudStorageServices();

            CloudBlobContainer blobIconContainer = CloudStorageServices.GetCloudBlobIconContainer();
            CloudBlockBlob blockBlob = blobIconContainer.GetBlockBlobReference("blank" + ".png");

            MemoryStream ms = new MemoryStream();
            iconListBitmap[0].Save(ms, ImageFormat.Png);
            ms.Position = 0;
            blockBlob.UploadFromStream(ms);

            listSize = listSize + 1;

            Console.WriteLine(listSize + "/" + listSize + " Fileicon " + iconListName[0] + " uploaded to Azure.");
        }
    }

    public class Win32
    {
        public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_DISPLAYNAME = 0x000000200;
        public const uint SHGFI_TYPENAME = 0x000000400;
        public const uint SHGFI_ATTRIBUTES = 0x000000800;
        public const uint SHGFI_ICONLOCATION = 0x000001000;
        public const uint SHGFI_EXETYPE = 0x000002000;
        public const uint SHGFI_SYSICONINDEX = 0x000004000;
        public const uint SHGFI_LINKOVERLAY = 0x000000000;// 0x000008000;
        public const uint SHGFI_SELECTED = 0x000010000;
        public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;
        public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint SHGFI_SMALLICON = 0x000000001;
        public const uint SHGFI_OPENICON = 0x000000002;
        public const uint SHGFI_SHELLICONSIZE = 0x000000004;
        public const uint SHGFI_PIDL = 0x000000008;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        public const uint SHGFI_ADDOVERLAYS = 0x000000020;
        public const uint SHGFI_OVERLAYINDEX = 0x000000040;
        //  public const uint SHGFI_ICON = 0x100;
        //  public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        //  public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon
        public const uint ILD_TRANSPARENT = 0x1;

        public const int GWL_STYLE = (-16);

        public const UInt32 SWP_FRAMECHANGED = 0x0020;
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const int WS_SYSMENU = 0x00080000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);

            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
         uint dwFileAttributes,
         ref RegistryFileName.SHFILEINFO psfi,
         uint cbSizeFileInfo,
         uint uFlags);

        [DllImport("user32")]
        public static extern int DestroyIcon(IntPtr hIcon);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("shell32.dll")]
        public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}
