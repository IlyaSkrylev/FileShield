using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileShield
{
    public class Constants
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AccountData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string login;

            public int password;
        }

        public const string accountDataFilePath = "accountsData.bin";
        public const string imgFolderIcon = "folder_icon.png";
    }
}
