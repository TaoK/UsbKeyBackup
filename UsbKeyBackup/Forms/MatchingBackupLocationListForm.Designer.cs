namespace KlerksSoft.UsbKeyBackup
{
    partial class MatchingBackupLocationListForm
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
            SingleAssemblyComponentResourceManager resources = new SingleAssemblyComponentResourceManager(typeof(MatchingBackupLocationListForm));
            this.lstV_MatchingLocations = new System.Windows.Forms.ListView();
            this.lbl_ChooseLocation = new System.Windows.Forms.Label();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Select = new System.Windows.Forms.Button();
            this.btn_BrowseData = new System.Windows.Forms.Button();
            this.opn_BrwseData = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lstV_MatchingLocations
            // 
            resources.ApplyResources(this.lstV_MatchingLocations, "lstV_MatchingLocations");
            this.lstV_MatchingLocations.Name = "lstV_MatchingLocations";
            this.lstV_MatchingLocations.UseCompatibleStateImageBehavior = false;
            this.lstV_MatchingLocations.View = System.Windows.Forms.View.List;
            this.lstV_MatchingLocations.DoubleClick += new System.EventHandler(this.lstV_MatchingLocations_DoubleClick);
            // 
            // lbl_ChooseLocation
            // 
            resources.ApplyResources(this.lbl_ChooseLocation, "lbl_ChooseLocation");
            this.lbl_ChooseLocation.Name = "lbl_ChooseLocation";
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_Select
            // 
            resources.ApplyResources(this.btn_Select, "btn_Select");
            this.btn_Select.Name = "btn_Select";
            this.btn_Select.UseVisualStyleBackColor = true;
            this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
            // 
            // btn_BrowseData
            // 
            resources.ApplyResources(this.btn_BrowseData, "btn_BrowseData");
            this.btn_BrowseData.Name = "btn_BrowseData";
            this.btn_BrowseData.UseVisualStyleBackColor = true;
            // 
            // opn_BrwseData
            // 
            this.opn_BrwseData.AddExtension = false;
            resources.ApplyResources(this.opn_BrwseData, "opn_BrwseData");
            // 
            // MatchingBackupLocationListForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_BrowseData);
            this.Controls.Add(this.btn_Select);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_ChooseLocation);
            this.Controls.Add(this.lstV_MatchingLocations);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MatchingBackupLocationListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstV_MatchingLocations;
        private System.Windows.Forms.Label lbl_ChooseLocation;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Select;
        private System.Windows.Forms.Button btn_BrowseData;
        private System.Windows.Forms.OpenFileDialog opn_BrwseData;
    }
}