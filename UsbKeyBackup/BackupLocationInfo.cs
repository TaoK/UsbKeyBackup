using System;
using System.Collections.Generic;
using System.Text;

namespace KlerksSoft.UsbKeyBackup
{
    internal class BackupLocationInfo
    {
        public int LocationEntryID { get; set; }
        public string SourceLabel { get; set; }
        public string EncryptionCheckValue { get; set; }
        public string BackupPath { get; set; }
    }
}
