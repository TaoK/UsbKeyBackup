namespace KlerksSoft.UsbKeyBackup
{
    partial class BackupSourceSelectionDialogForm
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
            SingleAssemblyComponentResourceManager resources = new SingleAssemblyComponentResourceManager(typeof(BackupSourceSelectionDialogForm));
            this.lbl_RestoreFromHereText = new System.Windows.Forms.Label();
            this.btn_AllAvailable = new System.Windows.Forms.Button();
            this.btn_ThisKeyOnly = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_RestoreFromHereText
            // 
            resources.ApplyResources(this.lbl_RestoreFromHereText, "lbl_RestoreFromHereText");
            this.lbl_RestoreFromHereText.Name = "lbl_RestoreFromHereText";
            // 
            // btn_AllAvailable
            // 
            resources.ApplyResources(this.btn_AllAvailable, "btn_AllAvailable");
            this.btn_AllAvailable.Name = "btn_AllAvailable";
            this.btn_AllAvailable.UseVisualStyleBackColor = true;
            this.btn_AllAvailable.Click += new System.EventHandler(this.btn_AllAvailable_Click);
            // 
            // btn_ThisKeyOnly
            // 
            resources.ApplyResources(this.btn_ThisKeyOnly, "btn_ThisKeyOnly");
            this.btn_ThisKeyOnly.Name = "btn_ThisKeyOnly";
            this.btn_ThisKeyOnly.UseVisualStyleBackColor = true;
            this.btn_ThisKeyOnly.Click += new System.EventHandler(this.btn_ThisKeyOnly_Click);
            // 
            // BackupSourceSelectionDialogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_ThisKeyOnly);
            this.Controls.Add(this.btn_AllAvailable);
            this.Controls.Add(this.lbl_RestoreFromHereText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackupSourceSelectionDialogForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_RestoreFromHereText;
        private System.Windows.Forms.Button btn_AllAvailable;
        private System.Windows.Forms.Button btn_ThisKeyOnly;
    }
}