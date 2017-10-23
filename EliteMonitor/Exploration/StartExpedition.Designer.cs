namespace EliteMonitor
{
    partial class StartExpedition
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxExpeditionName = new System.Windows.Forms.TextBox();
            this.textBoxStartPoint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxAutoComplete = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonEndOnStart = new System.Windows.Forms.RadioButton();
            this.radioButtonEndOnSpecific = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabelEndingSystem = new System.Windows.Forms.LinkLabel();
            this.buttonPickEnd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Expedition name:";
            // 
            // textBoxExpeditionName
            // 
            this.textBoxExpeditionName.Location = new System.Drawing.Point(106, 6);
            this.textBoxExpeditionName.Name = "textBoxExpeditionName";
            this.textBoxExpeditionName.Size = new System.Drawing.Size(342, 20);
            this.textBoxExpeditionName.TabIndex = 1;
            // 
            // textBoxStartPoint
            // 
            this.textBoxStartPoint.Location = new System.Drawing.Point(126, 32);
            this.textBoxStartPoint.Name = "textBoxStartPoint";
            this.textBoxStartPoint.ReadOnly = true;
            this.textBoxStartPoint.Size = new System.Drawing.Size(322, 20);
            this.textBoxStartPoint.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Expedition start point:";
            // 
            // checkBoxAutoComplete
            // 
            this.checkBoxAutoComplete.AutoSize = true;
            this.checkBoxAutoComplete.Checked = true;
            this.checkBoxAutoComplete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoComplete.Location = new System.Drawing.Point(15, 58);
            this.checkBoxAutoComplete.Name = "checkBoxAutoComplete";
            this.checkBoxAutoComplete.Size = new System.Drawing.Size(97, 17);
            this.checkBoxAutoComplete.TabIndex = 4;
            this.checkBoxAutoComplete.Text = "Automatically...";
            this.checkBoxAutoComplete.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(322, 153);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(126, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonStart.Location = new System.Drawing.Point(15, 153);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(301, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "Start expedition";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(438, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Elite Monitor will automatically include existing journal entries after this poin" +
    "t in the expedition";
            // 
            // radioButtonEndOnStart
            // 
            this.radioButtonEndOnStart.AutoSize = true;
            this.radioButtonEndOnStart.Checked = true;
            this.radioButtonEndOnStart.Location = new System.Drawing.Point(35, 81);
            this.radioButtonEndOnStart.Name = "radioButtonEndOnStart";
            this.radioButtonEndOnStart.Size = new System.Drawing.Size(296, 17);
            this.radioButtonEndOnStart.TabIndex = 8;
            this.radioButtonEndOnStart.TabStop = true;
            this.radioButtonEndOnStart.Text = "... complete the expedition when I return to the start point.";
            this.radioButtonEndOnStart.UseVisualStyleBackColor = true;
            this.radioButtonEndOnStart.CheckedChanged += new System.EventHandler(this.radioButtonEndOnStart_CheckedChanged);
            // 
            // radioButtonEndOnSpecific
            // 
            this.radioButtonEndOnSpecific.AutoSize = true;
            this.radioButtonEndOnSpecific.Location = new System.Drawing.Point(35, 104);
            this.radioButtonEndOnSpecific.Name = "radioButtonEndOnSpecific";
            this.radioButtonEndOnSpecific.Size = new System.Drawing.Size(300, 17);
            this.radioButtonEndOnSpecific.TabIndex = 9;
            this.radioButtonEndOnSpecific.Text = "... complete the expedition when I reach a specific system.\r\n";
            this.radioButtonEndOnSpecific.UseVisualStyleBackColor = true;
            this.radioButtonEndOnSpecific.CheckedChanged += new System.EventHandler(this.radioButtonEndOnSpecific_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Ending system:";
            // 
            // linkLabelEndingSystem
            // 
            this.linkLabelEndingSystem.Location = new System.Drawing.Point(149, 124);
            this.linkLabelEndingSystem.Name = "linkLabelEndingSystem";
            this.linkLabelEndingSystem.Size = new System.Drawing.Size(218, 13);
            this.linkLabelEndingSystem.TabIndex = 11;
            this.linkLabelEndingSystem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEndingSystem_LinkClicked);
            // 
            // buttonPickEnd
            // 
            this.buttonPickEnd.Enabled = false;
            this.buttonPickEnd.Location = new System.Drawing.Point(373, 119);
            this.buttonPickEnd.Name = "buttonPickEnd";
            this.buttonPickEnd.Size = new System.Drawing.Size(75, 23);
            this.buttonPickEnd.TabIndex = 12;
            this.buttonPickEnd.Text = "Pick System";
            this.buttonPickEnd.UseVisualStyleBackColor = true;
            this.buttonPickEnd.Click += new System.EventHandler(this.buttonPickEnd_Click);
            // 
            // StartExpedition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 201);
            this.ControlBox = false;
            this.Controls.Add(this.buttonPickEnd);
            this.Controls.Add(this.linkLabelEndingSystem);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.radioButtonEndOnSpecific);
            this.Controls.Add(this.radioButtonEndOnStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxAutoComplete);
            this.Controls.Add(this.textBoxStartPoint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxExpeditionName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StartExpedition";
            this.ShowIcon = false;
            this.Text = "StartExpedition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxExpeditionName;
        private System.Windows.Forms.TextBox textBoxStartPoint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxAutoComplete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonEndOnStart;
        private System.Windows.Forms.RadioButton radioButtonEndOnSpecific;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabelEndingSystem;
        private System.Windows.Forms.Button buttonPickEnd;
    }
}