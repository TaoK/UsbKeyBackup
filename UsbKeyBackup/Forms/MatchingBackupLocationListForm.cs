using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KlerksSoft.UsbKeyBackup
{
    public partial class MatchingBackupLocationListForm : Form
    {
        public int? SelectedLocationID;

        private BackupManager _backupManager;

        internal MatchingBackupLocationListForm(BackupManager backupManager)
        {
            InitializeComponent();
            _backupManager = backupManager;
            RenderList();
        }

        private void RenderList()
        {
            lstV_MatchingLocations.Items.Clear();
            foreach (BackupLocationInfo thisLocation in _backupManager.GetMatchingBackupLocations())
            {
                ListViewItem newItem = lstV_MatchingLocations.Items.Add(thisLocation.SourceLabel);
                newItem.Tag = thisLocation.LocationEntryID;
            }
        }

        private void lstV_MatchingLocations_DoubleClick(object sender, EventArgs e)
        {
            btn_Select_Click(null, null);
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            if (lstV_MatchingLocations.SelectedItems.Count == 1)
            {
                SelectedLocationID = (int)lstV_MatchingLocations.SelectedItems[0].Tag;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /*
                private void btn_BrowseData_Click(object sender, EventArgs e)
                {
                    opn_BrwseData.InitialDirectory = _restoreManager.FullBackupLocationDBDataPath;
                    DialogResult browseDataResult = opn_BrwseData.ShowDialog();
                    if (browseDataResult == DialogResult.OK)
                    {
                        string sourcePath = opn_BrwseData.FileName;

                        if (File.Exists(sourcePath))
                        {
                            Dictionary<string, string> restoreList = new Dictionary<string, string>();
                            restoreList.Add(sourcePath, BackupManager.GetFilenameFromDBDataFileName(sourcePath));
                            RestoreFilesWithDestinationSelection(restoreList);
                        }
                        else
                        {
                            MessageBox.Show(_generalResourceManager.GetString("RestoreSelectedFileDoesNotExistText"), _generalResourceManager.GetString("RestoreSelectedFileDoesNotExistTitle"), MessageBoxButtons.OK);
                        }

                    } //otherwise do nothing, we cancelled.
                }
        */

    }
}
