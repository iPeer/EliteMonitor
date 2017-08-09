namespace EliteMonitor
{
    partial class DiscoveredBodyList
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
            this.listViewDiscoveries = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonRescan = new System.Windows.Forms.Button();
            this.labelBodiesDiscovered = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewDiscoveries
            // 
            this.listViewDiscoveries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewDiscoveries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewDiscoveries.GridLines = true;
            this.listViewDiscoveries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewDiscoveries.Location = new System.Drawing.Point(0, 16);
            this.listViewDiscoveries.Name = "listViewDiscoveries";
            this.listViewDiscoveries.Size = new System.Drawing.Size(454, 381);
            this.listViewDiscoveries.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listViewDiscoveries.TabIndex = 0;
            this.listViewDiscoveries.UseCompatibleStateImageBehavior = false;
            this.listViewDiscoveries.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 1;
            this.columnHeader1.Text = "Time Turned in (Game Time)";
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 0;
            this.columnHeader2.Text = "Body Name";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(0, 426);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(454, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonRescan
            // 
            this.buttonRescan.Enabled = false;
            this.buttonRescan.Location = new System.Drawing.Point(0, 403);
            this.buttonRescan.Name = "buttonRescan";
            this.buttonRescan.Size = new System.Drawing.Size(454, 23);
            this.buttonRescan.TabIndex = 2;
            this.buttonRescan.Text = "Rescan Discoveries";
            this.buttonRescan.UseVisualStyleBackColor = true;
            // 
            // labelBodiesDiscovered
            // 
            this.labelBodiesDiscovered.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelBodiesDiscovered.Location = new System.Drawing.Point(0, 0);
            this.labelBodiesDiscovered.Name = "labelBodiesDiscovered";
            this.labelBodiesDiscovered.Size = new System.Drawing.Size(454, 13);
            this.labelBodiesDiscovered.TabIndex = 3;
            this.labelBodiesDiscovered.Text = "Bodies discovered: 0";
            // 
            // DiscoveredBodyList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 449);
            this.Controls.Add(this.labelBodiesDiscovered);
            this.Controls.Add(this.buttonRescan);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewDiscoveries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiscoveredBodyList";
            this.Text = "Discovered Body List";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewDiscoveries;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonRescan;
        private System.Windows.Forms.Label labelBodiesDiscovered;
    }
}