using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KlerksSoft.UsbKeyBackup
{
    public partial class BackupSourceSelectionDialogForm : Form
    {
        public BackupSourceDialogResultType Result = BackupSourceDialogResultType.Cancel;

        public BackupSourceSelectionDialogForm()
        {
            InitializeComponent();
        }

        private void btn_AllAvailable_Click(object sender, EventArgs e)
        {
            Result = BackupSourceDialogResultType.AllAvailable;
            this.Close();
        }

        private void btn_ThisKeyOnly_Click(object sender, EventArgs e)
        {
            Result = BackupSourceDialogResultType.ThisKeyOnly;
            this.Close();
        }

        public enum BackupSourceDialogResultType {
            AllAvailable,
            ThisKeyOnly,
            Cancel
        }
    }
}
