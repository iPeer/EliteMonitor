namespace EliteMonitor
{
    partial class MaterialList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialList));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.listViewData = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageElements = new System.Windows.Forms.TabPage();
            this.listViewElements = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageManufactured = new System.Windows.Forms.TabPage();
            this.listViewManufactured = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageUnknown = new System.Windows.Forms.TabPage();
            this.listViewUnknown = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageData.SuspendLayout();
            this.tabPageElements.SuspendLayout();
            this.tabPageManufactured.SuspendLayout();
            this.tabPageUnknown.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageData);
            this.tabControl.Controls.Add(this.tabPageElements);
            this.tabControl.Controls.Add(this.tabPageManufactured);
            this.tabControl.Controls.Add(this.tabPageUnknown);
            this.tabControl.Location = new System.Drawing.Point(12, 66);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(538, 239);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageData
            // 
            this.tabPageData.Controls.Add(this.listViewData);
            this.tabPageData.Location = new System.Drawing.Point(4, 22);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPageData.Size = new System.Drawing.Size(530, 213);
            this.tabPageData.TabIndex = 0;
            this.tabPageData.Text = "Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // listViewData
            // 
            this.listViewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewData.FullRowSelect = true;
            this.listViewData.GridLines = true;
            this.listViewData.Location = new System.Drawing.Point(0, 0);
            this.listViewData.MultiSelect = false;
            this.listViewData.Name = "listViewData";
            this.listViewData.Size = new System.Drawing.Size(530, 213);
            this.listViewData.TabIndex = 0;
            this.listViewData.UseCompatibleStateImageBehavior = false;
            this.listViewData.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Count";
            // 
            // tabPageElements
            // 
            this.tabPageElements.Controls.Add(this.listViewElements);
            this.tabPageElements.Location = new System.Drawing.Point(4, 22);
            this.tabPageElements.Name = "tabPageElements";
            this.tabPageElements.Size = new System.Drawing.Size(530, 213);
            this.tabPageElements.TabIndex = 1;
            this.tabPageElements.Text = "Elements";
            this.tabPageElements.UseVisualStyleBackColor = true;
            // 
            // listViewElements
            // 
            this.listViewElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listViewElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewElements.FullRowSelect = true;
            this.listViewElements.GridLines = true;
            this.listViewElements.Location = new System.Drawing.Point(0, 0);
            this.listViewElements.MultiSelect = false;
            this.listViewElements.Name = "listViewElements";
            this.listViewElements.Size = new System.Drawing.Size(530, 213);
            this.listViewElements.TabIndex = 2;
            this.listViewElements.UseCompatibleStateImageBehavior = false;
            this.listViewElements.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Item";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Count";
            // 
            // tabPageManufactured
            // 
            this.tabPageManufactured.Controls.Add(this.listViewManufactured);
            this.tabPageManufactured.Location = new System.Drawing.Point(4, 22);
            this.tabPageManufactured.Name = "tabPageManufactured";
            this.tabPageManufactured.Size = new System.Drawing.Size(530, 213);
            this.tabPageManufactured.TabIndex = 2;
            this.tabPageManufactured.Text = "Manufactured";
            this.tabPageManufactured.UseVisualStyleBackColor = true;
            // 
            // listViewManufactured
            // 
            this.listViewManufactured.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.listViewManufactured.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewManufactured.FullRowSelect = true;
            this.listViewManufactured.GridLines = true;
            this.listViewManufactured.Location = new System.Drawing.Point(0, 0);
            this.listViewManufactured.MultiSelect = false;
            this.listViewManufactured.Name = "listViewManufactured";
            this.listViewManufactured.Size = new System.Drawing.Size(530, 213);
            this.listViewManufactured.TabIndex = 2;
            this.listViewManufactured.UseCompatibleStateImageBehavior = false;
            this.listViewManufactured.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Item";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Count";
            // 
            // tabPageUnknown
            // 
            this.tabPageUnknown.Controls.Add(this.listViewUnknown);
            this.tabPageUnknown.Location = new System.Drawing.Point(4, 22);
            this.tabPageUnknown.Name = "tabPageUnknown";
            this.tabPageUnknown.Size = new System.Drawing.Size(530, 213);
            this.tabPageUnknown.TabIndex = 3;
            this.tabPageUnknown.Text = "Unknown";
            this.tabPageUnknown.UseVisualStyleBackColor = true;
            // 
            // listViewUnknown
            // 
            this.listViewUnknown.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
            this.listViewUnknown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewUnknown.FullRowSelect = true;
            this.listViewUnknown.GridLines = true;
            this.listViewUnknown.Location = new System.Drawing.Point(0, 0);
            this.listViewUnknown.MultiSelect = false;
            this.listViewUnknown.Name = "listViewUnknown";
            this.listViewUnknown.Size = new System.Drawing.Size(530, 213);
            this.listViewUnknown.TabIndex = 1;
            this.listViewUnknown.UseCompatibleStateImageBehavior = false;
            this.listViewUnknown.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Item";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Count";
            // 
            // buttonClose
            // 
            this.buttonClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonClose.Location = new System.Drawing.Point(0, 311);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(562, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(562, 54);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // MaterialList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 334);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(578, 373);
            this.Name = "MaterialList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Material Inventory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MaterialList_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageData.ResumeLayout(false);
            this.tabPageElements.ResumeLayout(false);
            this.tabPageManufactured.ResumeLayout(false);
            this.tabPageUnknown.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageData;
        private System.Windows.Forms.ListView listViewData;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabPageElements;
        private System.Windows.Forms.TabPage tabPageManufactured;
        private System.Windows.Forms.TabPage tabPageUnknown;
        private System.Windows.Forms.ListView listViewUnknown;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewManufactured;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListView listViewElements;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}