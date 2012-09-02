using System;
using System.Collections.Generic;
using System.Text;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupFileInfo
    {
        public int FileEntryID {get; set;}
        public string FileFullPath {get; set;}
        public DateTime FileDate {get; set;}
        public long FileSizeBytes {get; set;}
        public string FileSHA1 {get; set;}
        public string BackupFilePath {get; set;}
    }
}
