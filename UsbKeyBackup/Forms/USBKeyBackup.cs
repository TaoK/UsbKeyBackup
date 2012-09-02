using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using KlerksSoft.EasyProgressDialog;

namespace KlerksSoft.UsbKeyBackup
{
    //TODO: Less frequent cleanup, identifying physical files that do not belong.

    public partial class frm_FolderBackup : Form
    {
        private BackupSource _backupSource = null;
        private BackupManager _backupMgr = null;
        private SingleAssemblyResourceManager generalResourceManager = null;
        private ProgressDialog progressDialog = null;

        public frm_FolderBackup()
        {
            Thread.CurrentThread.CurrentUICulture = Properties.Settings.Default.Language;
            generalResourceManager = new SingleAssemblyResourceManager("General", Assembly.GetExecutingAssembly());

            progressDialog = new ProgressDialog();
            progressDialog.DisplayCounts = false;
            progressDialog.DisplayIntervalSeconds = 1;
            progressDialog.DisplayTimeEstimates = false;

            InitializeComponent();
        }

        private void frm_FolderBackup_Load(object sender, EventArgs e)
        {
            btn_Previous.Enabled = false;
            grp_BackupLocation.Visible = false;

            string ownerText = Properties.Settings.Default.OwnerName;
            if (!String.IsNullOrEmpty(ownerText)) ownerText = " " + ownerText;
            lbl_Intro.Text = lbl_Intro.Text.Replace("{OwnerText}", ownerText);

            BackupCleaningOptions cleaningOptions = new BackupCleaningOptions();
            cleaningOptions.FinalCutOffDays = Properties.Settings.Default.FinalCutOffDays;
            cleaningOptions.Period1Interval = Properties.Settings.Default.Period1Interval;
            cleaningOptions.Period1Start = Properties.Settings.Default.Period1Start;
            cleaningOptions.Period2Interval = Properties.Settings.Default.Period2Interval;
            cleaningOptions.Period2Start = Properties.Settings.Default.Period2Start;
            cleaningOptions.Period3Interval = Properties.Settings.Default.Period3Interval;
            cleaningOptions.Period3Start = Properties.Settings.Default.Period3Start;

            _backupSource = new BackupSource(Properties.Settings.Default.LocationToBackup, 
                generalResourceManager, 
                progressDialog);

            _backupMgr = new BackupManager(_backupSource, 
                Properties.Settings.Default.BackupLocation,
                Properties.Settings.Default.EncryptionPassword,
                Properties.Settings.Default.EncryptionCheckPhrase,
                generalResourceManager, 
                cleaningOptions, 
                Properties.Settings.Default.UseDatabase, 
                progressDialog);

            lbl_BackupLocation.Text = _backupSource.SourcePath;
            lbl_LocToBackUp.Text = _backupSource.SourcePath;
            txt_BackupDestination.Text = _backupMgr.BackupLocation;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            grp_BackupLocation.Visible = false;
            grp_Step1.Visible = true;
            btn_Next.Enabled = true;
            btn_Previous.Enabled = false;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (DisplayStatsTab())
            {
                if (_backupSource.SourceSize >= _backupMgr.FreeSpace)
                    MessageBox.Show(generalResourceManager.GetString("NoSpaceDisplayWarningText"), generalResourceManager.GetString("NoSpaceDisplayWarningTitle"), MessageBoxButtons.OK);
            }
        }

        private void btn_Finish_Click(object sender, EventArgs e)
        {
            if (DisplayStatsTab())
            {
                bool abortBackup = false;

                if (_backupSource.SourceSize >= _backupMgr.FreeSpace)
                {
                    DialogResult warningResponse = MessageBox.Show(generalResourceManager.GetString("NoSpaceDisplayContinueQuestionText"),
                        generalResourceManager.GetString("NoSpaceDisplayContinueQuestionTitle"),
                        MessageBoxButtons.YesNo);

                    if (warningResponse != DialogResult.Yes)
                        abortBackup = true;
                }

                if (!abortBackup)
                {
                    if (DoBackupWithConfirmation())
                    {
                        DoCleanUp();
                        DialogResult warningRersponse = MessageBox.Show(generalResourceManager.GetString("BackupCompleteConfirmationText"),
                            generalResourceManager.GetString("BackupCompleteConfirmationTitle"),
                            MessageBoxButtons.OK);
                        this.Close();
                    }
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool DisplayStatsTab()
        {
            //check whether both these are actual values, indicating lazy-load space calculations were successful
            if (_backupSource.SourceSize != null && _backupMgr.FreeSpace != null)
            {
                lbl_Size.Text = Utils.FormatSize(_backupSource.SourceSize.Value);
                lbl_FreeSpace.Text = Utils.FormatSize(_backupMgr.FreeSpace.Value);

                if (_backupMgr.LatestBackupDate != null)
                    lbl_RecentBackup.Text = _backupMgr.LatestBackupDate.Value.ToShortDateString();
                else
                    lbl_RecentBackup.Text = generalResourceManager.GetString("None_Parens");

                grp_Step1.Visible = false;
                grp_BackupLocation.Visible = true;
                btn_Next.Enabled = false;
                btn_Previous.Enabled = true;

                return true;
            }
            else
            {
                MessageBox.Show(generalResourceManager.GetString("OperationCancelledText"), generalResourceManager.GetString("OperationCancelledTitle"), MessageBoxButtons.OK);
                return false;
            }
        }

        private bool DoBackupWithConfirmation()
        {
            if (!_backupMgr.FullBackupLocationExists)
            {
                string warningMessage = generalResourceManager.GetString("NoBackupsWarning");
                if (string.IsNullOrEmpty(Properties.Settings.Default.EncryptionPassword))
                    warningMessage += generalResourceManager.GetString("NoBackupsWarningDetail_NoPassword");
                else
                    warningMessage += generalResourceManager.GetString("NoBackupsWarningDetail_WithPassword");

                DialogResult responseToNewBackupLocationQuestion = MessageBox.Show(warningMessage, generalResourceManager.GetString("NoBackupsTitle"), MessageBoxButtons.YesNo);

                //if the user did not respond that they are OK with it, then stop.
                if (responseToNewBackupLocationQuestion != DialogResult.Yes)
                    return false;
            }

            return _backupMgr.BackUp();
        }

        private void DoCleanUp()
        {
            if (_backupMgr.ExpiredBackupCount > 0)
            {
                DialogResult responseToBackupDeletionQuestion = MessageBox.Show(generalResourceManager.GetString("BackupsToDeleteWarning").Replace("{BackCount}", _backupMgr.ExpiredBackupCount.ToString()), generalResourceManager.GetString("BackupsToDeleteTitle"), MessageBoxButtons.YesNo);

                if (responseToBackupDeletionQuestion == DialogResult.Yes)
                {
                    _backupMgr.DeleteExpiredBackups();
                }
            }
        }

        private void btn_Restore_Click(object sender, EventArgs e)
        {
            BackupSourceSelectionDialogForm quizForm = new BackupSourceSelectionDialogForm();
            quizForm.ShowDialog();
            BackupSourceSelectionDialogForm.BackupSourceDialogResultType quizResult = quizForm.Result;
            quizForm.Dispose();

            if (quizResult == BackupSourceSelectionDialogForm.BackupSourceDialogResultType.AllAvailable)
            {
                MatchingBackupLocationListForm locationForm = new MatchingBackupLocationListForm(_backupMgr);
                DialogResult locationDialogResult = locationForm.ShowDialog();
                int? locationResultID = locationForm.SelectedLocationID;
                locationForm.Dispose();
                if (locationDialogResult == DialogResult.OK)
                {
                    if (locationResultID != null)
                    {
                        this.Visible = false;
                        RestoreForm restoreForm = new RestoreForm(_backupMgr.GetRestoreManager(locationResultID.Value), generalResourceManager);
                        restoreForm.ShowDialog();
                        restoreForm.Dispose();
                        this.Close();
                    }
                }
            }
            else if (quizForm.Result == BackupSourceSelectionDialogForm.BackupSourceDialogResultType.ThisKeyOnly)
            {
                this.Visible = false;
                RestoreForm restoreForm = new RestoreForm(_backupMgr.GetRestoreManager(), generalResourceManager);
                restoreForm.ShowDialog();
                restoreForm.Dispose();
                this.Close();
            }
            else
            {
                //user cancelled, do nothing
            }
        }
    }
}
