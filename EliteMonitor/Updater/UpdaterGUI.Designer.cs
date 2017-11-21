namespace EliteMonitor.Updater
{
    partial class UpdaterGUI
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelDownloadStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(12, 9);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(487, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "{STATUS}";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 25);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(487, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // labelDownloadStatus
            // 
            this.labelDownloadStatus.Location = new System.Drawing.Point(12, 51);
            this.labelDownloadStatus.Name = "labelDownloadStatus";
            this.labelDownloadStatus.Size = new System.Drawing.Size(487, 13);
            this.labelDownloadStatus.TabIndex = 2;
            this.labelDownloadStatus.Text = "{DOWNLOADSTATUS}";
            this.labelDownloadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Updater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 77);
            this.ControlBox = false;
            this.Controls.Add(this.labelDownloadStatus);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Updater";
            this.Text = "Updater";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelDownloadStatus;
    }
}