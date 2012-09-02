using System;
using System.Collections.Generic;
using System.Text;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupInfo
    {
        public int EntryID { get; set; }
        public DateTime BackupDate { get; set; }
        public bool BackupComplete { get; set; }
    }
}
