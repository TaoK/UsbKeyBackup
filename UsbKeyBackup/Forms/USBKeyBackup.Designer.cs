namespace KlerksSoft.UsbKeyBackup
{
    partial class frm_FolderBackup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SingleAssemblyComponentResourceManager resources = new SingleAssemblyComponentResourceManager(typeof(frm_FolderBackup));
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Finish = new System.Windows.Forms.Button();
            this.btn_Next = new System.Windows.Forms.Button();
            this.btn_Previous = new System.Windows.Forms.Button();
            this.grp_Step1 = new System.Windows.Forms.GroupBox();
            this.lbl_BackupLocation = new System.Windows.Forms.Label();
            this.lbl_LocToBackLabel = new System.Windows.Forms.Label();
            this.lbl_Intro = new System.Windows.Forms.Label();
            this.grp_BackupLocation = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_dataSize = new System.Windows.Forms.Label();
            this.lbl_LocToBackUp = new System.Windows.Forms.Label();
            this.lbl_LocToBack = new System.Windows.Forms.Label();
            this.lbl_BackupDestinationLabel = new System.Windows.Forms.Label();
            this.lbl_Size = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_BackupDestination = new System.Windows.Forms.TextBox();
            this.btn_BackupDestChange = new System.Windows.Forms.Button();
            this.lbl_MostRecentBackupLabel = new System.Windows.Forms.Label();
            this.lbl_FreeSpaceLabel = new System.Windows.Forms.Label();
            this.lbl_FreeSpace = new System.Windows.Forms.Label();
            this.lbl_RecentBackup = new System.Windows.Forms.Label();
            this.lbl_BackupDestination = new System.Windows.Forms.Label();
            this.btn_Restore = new System.Windows.Forms.Button();
            this.grp_Step1.SuspendLayout();
            this.grp_BackupLocation.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Finish
            // 
            resources.ApplyResources(this.btn_Finish, "btn_Finish");
            this.btn_Finish.Name = "btn_Finish";
            this.btn_Finish.UseVisualStyleBackColor = true;
            this.btn_Finish.Click += new System.EventHandler(this.btn_Finish_Click);
            // 
            // btn_Next
            // 
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Previous
            // 
            resources.ApplyResources(this.btn_Previous, "btn_Previous");
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // grp_Step1
            // 
            resources.ApplyResources(this.grp_Step1, "grp_Step1");
            this.grp_Step1.Controls.Add(this.lbl_BackupLocation);
            this.grp_Step1.Controls.Add(this.lbl_LocToBackLabel);
            this.grp_Step1.Controls.Add(this.lbl_Intro);
            this.grp_Step1.Name = "grp_Step1";
            this.grp_Step1.TabStop = false;
            // 
            // lbl_BackupLocation
            // 
            resources.ApplyResources(this.lbl_BackupLocation, "lbl_BackupLocation");
            this.lbl_BackupLocation.Name = "lbl_BackupLocation";
            // 
            // lbl_LocToBackLabel
            // 
            resources.ApplyResources(this.lbl_LocToBackLabel, "lbl_LocToBackLabel");
            this.lbl_LocToBackLabel.Name = "lbl_LocToBackLabel";
            // 
            // lbl_Intro
            // 
            resources.ApplyResources(this.lbl_Intro, "lbl_Intro");
            this.lbl_Intro.Name = "lbl_Intro";
            // 
            // grp_BackupLocation
            // 
            resources.ApplyResources(this.grp_BackupLocation, "grp_BackupLocation");
            this.grp_BackupLocation.Controls.Add(this.tableLayoutPanel1);
            this.grp_BackupLocation.Controls.Add(this.lbl_BackupDestination);
            this.grp_BackupLocation.Name = "grp_BackupLocation";
            this.grp_BackupLocation.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lbl_dataSize, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbl_LocToBackUp, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_LocToBack, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_BackupDestinationLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_Size, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl_MostRecentBackupLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbl_FreeSpaceLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbl_FreeSpace, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbl_RecentBackup, 1, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lbl_dataSize
            // 
            resources.ApplyResources(this.lbl_dataSize, "lbl_dataSize");
            this.lbl_dataSize.Name = "lbl_dataSize";
            // 
            // lbl_LocToBackUp
            // 
            resources.ApplyResources(this.lbl_LocToBackUp, "lbl_LocToBackUp");
            this.lbl_LocToBackUp.AutoEllipsis = true;
            this.lbl_LocToBackUp.Name = "lbl_LocToBackUp";
            // 
            // lbl_LocToBack
            // 
            resources.ApplyResources(this.lbl_LocToBack, "lbl_LocToBack");
            this.lbl_LocToBack.Name = "lbl_LocToBack";
            // 
            // lbl_BackupDestinationLabel
            // 
            resources.ApplyResources(this.lbl_BackupDestinationLabel, "lbl_BackupDestinationLabel");
            this.lbl_BackupDestinationLabel.Name = "lbl_BackupDestinationLabel";
            // 
            // lbl_Size
            // 
            resources.ApplyResources(this.lbl_Size, "lbl_Size");
            this.lbl_Size.Name = "lbl_Size";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.txt_BackupDestination, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btn_BackupDestChange, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 2);
            // 
            // txt_BackupDestination
            // 
            resources.ApplyResources(this.txt_BackupDestination, "txt_BackupDestination");
            this.txt_BackupDestination.Name = "txt_BackupDestination";
            // 
            // btn_BackupDestChange
            // 
            resources.ApplyResources(this.btn_BackupDestChange, "btn_BackupDestChange");
            this.btn_BackupDestChange.Name = "btn_BackupDestChange";
            this.btn_BackupDestChange.UseVisualStyleBackColor = true;
            // 
            // lbl_MostRecentBackupLabel
            // 
            resources.ApplyResources(this.lbl_MostRecentBackupLabel, "lbl_MostRecentBackupLabel");
            this.lbl_MostRecentBackupLabel.Name = "lbl_MostRecentBackupLabel";
            // 
            // lbl_FreeSpaceLabel
            // 
            resources.ApplyResources(this.lbl_FreeSpaceLabel, "lbl_FreeSpaceLabel");
            this.lbl_FreeSpaceLabel.Name = "lbl_FreeSpaceLabel";
            // 
            // lbl_FreeSpace
            // 
            resources.ApplyResources(this.lbl_FreeSpace, "lbl_FreeSpace");
            this.lbl_FreeSpace.Name = "lbl_FreeSpace";
            // 
            // lbl_RecentBackup
            // 
            resources.ApplyResources(this.lbl_RecentBackup, "lbl_RecentBackup");
            this.lbl_RecentBackup.Name = "lbl_RecentBackup";
            // 
            // lbl_BackupDestination
            // 
            resources.ApplyResources(this.lbl_BackupDestination, "lbl_BackupDestination");
            this.lbl_BackupDestination.Name = "lbl_BackupDestination";
            // 
            // btn_Restore
            // 
            resources.ApplyResources(this.btn_Restore, "btn_Restore");
            this.btn_Restore.Name = "btn_Restore";
            this.btn_Restore.UseVisualStyleBackColor = true;
            this.btn_Restore.Click += new System.EventHandler(this.btn_Restore_Click);
            // 
            // frm_FolderBackup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_Restore);
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Finish);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.grp_BackupLocation);
            this.Controls.Add(this.grp_Step1);
            this.Name = "frm_FolderBackup";
            this.Load += new System.EventHandler(this.frm_FolderBackup_Load);
            this.grp_Step1.ResumeLayout(false);
            this.grp_Step1.PerformLayout();
            this.grp_BackupLocation.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Finish;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.Button btn_Previous;
        private System.Windows.Forms.GroupBox grp_Step1;
        private System.Windows.Forms.Label lbl_Intro;
        private System.Windows.Forms.Label lbl_BackupLocation;
        private System.Windows.Forms.Label lbl_LocToBackLabel;
        private System.Windows.Forms.GroupBox grp_BackupLocation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbl_BackupDestination;
        private System.Windows.Forms.Label lbl_dataSize;
        private System.Windows.Forms.Label lbl_LocToBack;
        private System.Windows.Forms.Label lbl_BackupDestinationLabel;
        private System.Windows.Forms.Label lbl_FreeSpaceLabel;
        private System.Windows.Forms.Label lbl_MostRecentBackupLabel;
        private System.Windows.Forms.Label lbl_LocToBackUp;
        private System.Windows.Forms.Label lbl_Size;
        private System.Windows.Forms.TextBox txt_BackupDestination;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btn_BackupDestChange;
        private System.Windows.Forms.Label lbl_FreeSpace;
        private System.Windows.Forms.Label lbl_RecentBackup;
        private System.Windows.Forms.Button btn_Restore;
    }
}

