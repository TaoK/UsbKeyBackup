namespace KlerksSoft.UsbKeyBackup
{
    partial class RestoreForm
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
            SingleAssemblyComponentResourceManager resources = new SingleAssemblyComponentResourceManager(typeof(RestoreForm));
            this.lstV_BrowseList = new System.Windows.Forms.ListView();
            this.btn_Open = new System.Windows.Forms.Button();
            this.btn_Restore = new System.Windows.Forms.Button();
            this.brws_RestoreLocation = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.btn_Up = new System.Windows.Forms.Button();
            this.lbl_PleaseSelect = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstV_BrowseList
            // 
            resources.ApplyResources(this.lstV_BrowseList, "lstV_BrowseList");
            this.lstV_BrowseList.MultiSelect = false;
            this.lstV_BrowseList.Name = "lstV_BrowseList";
            this.lstV_BrowseList.UseCompatibleStateImageBehavior = false;
            this.lstV_BrowseList.View = System.Windows.Forms.View.List;
            this.lstV_BrowseList.SelectedIndexChanged += new System.EventHandler(this.lstV_BrowseList_SelectedIndexChanged);
            this.lstV_BrowseList.DoubleClick += new System.EventHandler(this.lstV_BrowseList_DoubleClick);
            this.lstV_BrowseList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstV_BrowseList_KeyPress);
            // 
            // btn_Open
            // 
            resources.ApplyResources(this.btn_Open, "btn_Open");
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // btn_Restore
            // 
            resources.ApplyResources(this.btn_Restore, "btn_Restore");
            this.btn_Restore.Name = "btn_Restore";
            this.btn_Restore.UseVisualStyleBackColor = true;
            this.btn_Restore.Click += new System.EventHandler(this.btn_Restore_Click);
            // 
            // btn_Exit
            // 
            resources.ApplyResources(this.btn_Exit, "btn_Exit");
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // btn_Up
            // 
            resources.ApplyResources(this.btn_Up, "btn_Up");
            this.btn_Up.Name = "btn_Up";
            this.btn_Up.UseVisualStyleBackColor = true;
            this.btn_Up.Click += new System.EventHandler(this.btn_Up_Click);
            // 
            // lbl_PleaseSelect
            // 
            resources.ApplyResources(this.lbl_PleaseSelect, "lbl_PleaseSelect");
            this.lbl_PleaseSelect.Name = "lbl_PleaseSelect";
            // 
            // RestoreForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_PleaseSelect);
            this.Controls.Add(this.btn_Up);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.btn_Restore);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.lstV_BrowseList);
            this.Name = "RestoreForm";
            this.Load += new System.EventHandler(this.RestoreForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstV_BrowseList;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.Button btn_Restore;
        private System.Windows.Forms.FolderBrowserDialog brws_RestoreLocation;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.Button btn_Up;
        private System.Windows.Forms.Label lbl_PleaseSelect;
    }
}