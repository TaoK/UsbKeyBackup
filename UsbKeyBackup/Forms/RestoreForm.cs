using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KlerksSoft.UsbKeyBackup
{
    internal partial class RestoreForm : Form
    {
        private RestoreManager _restoreManager;
        private SingleAssemblyResourceManager _generalResourceManager;
        private int? _selectedBackupID = null;
        private string _selectedBackupPath = "";

        internal RestoreForm(RestoreManager restoreManager, SingleAssemblyResourceManager generalResourceManager)
        {
            _restoreManager = restoreManager;
            _generalResourceManager = generalResourceManager;
            InitializeComponent();
        }

        private void RestoreForm_Load(object sender, EventArgs e)
        {
            RenderList();
        }

        internal bool RestoreFilesWithDestinationSelection(Dictionary<string, string> restoreList)
        {
            MessageBox.Show(_generalResourceManager.GetString("RestoreChooseDestinationText"), _generalResourceManager.GetString("RestoreChooseDestinationTitle"), MessageBoxButtons.OK);

            brws_RestoreLocation.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DialogResult saveLocationResult = brws_RestoreLocation.ShowDialog();

            if (saveLocationResult == DialogResult.OK)
            {
                string destinationPath = brws_RestoreLocation.SelectedPath;
                if (Directory.Exists(destinationPath))
                {
                    bool result = _restoreManager.RestoreFilesToDestination(restoreList, destinationPath);
                    if (result)
                    {
                        MessageBox.Show(_generalResourceManager.GetString("RestoreSuccessfulConfirmationText"), _generalResourceManager.GetString("RestoreSuccessfulConfirmationTitle"), MessageBoxButtons.OK);
                    }
                    return result;
                }
                else
                {
                    MessageBox.Show(_generalResourceManager.GetString("RestoreDestinationNotExistsText"), _generalResourceManager.GetString("RestoreDestinationNotExistsTitle"), MessageBoxButtons.OK);
                    return false;
                }

            }
            else
                return false;

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstV_BrowseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (lstV_BrowseList.SelectedItems.Count == 1)
            {
                if (_selectedBackupID == null)
                    btn_Open.Enabled = true;
                else
                {
                    BackupPathEntry pathEntry = (BackupPathEntry)lstV_BrowseList.SelectedItems[0].Tag;
                    btn_Open.Enabled = pathEntry.IsDirectory;
                }
                btn_Restore.Enabled = true;
            }
            else
            {
                btn_Open.Enabled = false;
                btn_Restore.Enabled = false;
            }
        }

        private void OpenSelectedItem()
        {
            if (lstV_BrowseList.SelectedItems.Count == 1)
            {
                if (_selectedBackupID == null)
                {
                    BackupInfo selectedBackup = (BackupInfo)lstV_BrowseList.SelectedItems[0].Tag;
                    _selectedBackupID = selectedBackup.EntryID;
                }
                else
                {
                    BackupPathEntry pathEntry = (BackupPathEntry)lstV_BrowseList.SelectedItems[0].Tag;
                    if (pathEntry.IsDirectory)
                        _selectedBackupPath = Path.Combine(_selectedBackupPath, pathEntry.Name);
                }
                RenderList();
                btn_Up.Enabled = true;
            }
        }

        private void RenderList()
        {
            lstV_BrowseList.Items.Clear();
            if (_selectedBackupID == null)
            {
                lbl_PleaseSelect.Text = _generalResourceManager.GetString("RestoreFormMainPromptChooseBackup");
                foreach (BackupInfo backupInfo in _restoreManager.GetBackups())
                {
                    ListViewItem newItem = lstV_BrowseList.Items.Add(backupInfo.BackupDate.ToString());
                    newItem.Tag = backupInfo;
                }
            }
            else
            {
                lbl_PleaseSelect.Text = _generalResourceManager.GetString("RestoreFormMainPromptChooseFileFolder");
                foreach (BackupPathEntry backupPathEntry in _restoreManager.GetBackupPathEntries(_selectedBackupID.Value, _selectedBackupPath))
                {
                    ListViewItem newItem = lstV_BrowseList.Items.Add(backupPathEntry.Name);
                    newItem.Tag = backupPathEntry;
                }
            }
            UpdateButtonStates();
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        private void lstV_BrowseList_DoubleClick(object sender, EventArgs e)
        {
            SelectOneItem();
        }

        private void SelectOneItem()
        {
            if (lstV_BrowseList.SelectedItems.Count == 1)
            {
                if (_selectedBackupID != null)
                {
                    BackupPathEntry pathEntry = (BackupPathEntry)lstV_BrowseList.SelectedItems[0].Tag;
                    if (!pathEntry.IsDirectory)
                    {
                        DialogResult restoreFileAnswer = MessageBox.Show(_generalResourceManager.GetString("RestoreActionConfirmationText").Replace("{SelectedItem}", pathEntry.Name), _generalResourceManager.GetString("RestoreActionConfirmationTitle"), MessageBoxButtons.YesNo);
                        if (restoreFileAnswer == DialogResult.Yes)
                            RestoreSelectedItem();
                    }
                    else
                        OpenSelectedItem();
                }
                OpenSelectedItem();
            }
        }

        private void btn_Up_Click(object sender, EventArgs e)
        {
            UpOneLevel();
        }

        private void UpOneLevel()
        {
            if (_selectedBackupID != null)
            {
                if (_selectedBackupPath != "")
                {
                    string[] pathParts = _selectedBackupPath.Split(Path.DirectorySeparatorChar);
                    if (pathParts.Length > 1)
                        _selectedBackupPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts, 0, pathParts.Length - 1);
                    else
                        _selectedBackupPath = "";
                }
                else
                {
                    _selectedBackupID = null;
                    btn_Up.Enabled = false;
                }

                RenderList();
            }
        }

        private void btn_Restore_Click(object sender, EventArgs e)
        {
            RestoreSelectedItem();
        }

        private void RestoreSelectedItem()
        {
            if (lstV_BrowseList.SelectedItems.Count == 1)
            {
                Dictionary<string, string> restoreList = null;
                if (_selectedBackupID == null)
                {
                    BackupInfo backup = (BackupInfo)lstV_BrowseList.SelectedItems[0].Tag;
                    restoreList = _restoreManager.GetRestoreFilesByBackupID(backup.EntryID);
                }
                else
                {
                    BackupPathEntry pathEntry = (BackupPathEntry)lstV_BrowseList.SelectedItems[0].Tag;
                    restoreList = _restoreManager.GetRestoreFilesByBackupIDAndPartialPath(_selectedBackupID.Value, Path.Combine(_selectedBackupPath, pathEntry.Name));
                }

                RestoreFilesWithDestinationSelection(restoreList);
            }
        }

        private void lstV_BrowseList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.KeyChar == (char)8) //backspace
                    UpOneLevel();

                if (e.KeyChar == (char)13) //enter
                    SelectOneItem();
            }
        }

    }
}
