namespace EliteMonitor
{
    partial class HomeSystemChooser
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
            this.textBoxSystemInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSetHomeSystem = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBoxSystemInput
            // 
            this.textBoxSystemInput.Location = new System.Drawing.Point(15, 25);
            this.textBoxSystemInput.Name = "textBoxSystemInput";
            this.textBoxSystemInput.Size = new System.Drawing.Size(208, 20);
            this.textBoxSystemInput.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter system name";
            // 
            // buttonSetHomeSystem
            // 
            this.buttonSetHomeSystem.Location = new System.Drawing.Point(229, 23);
            this.buttonSetHomeSystem.Name = "buttonSetHomeSystem";
            this.buttonSetHomeSystem.Size = new System.Drawing.Size(99, 23);
            this.buttonSetHomeSystem.TabIndex = 2;
            this.buttonSetHomeSystem.Text = "Set home system";
            this.buttonSetHomeSystem.UseVisualStyleBackColor = true;
            this.buttonSetHomeSystem.Click += new System.EventHandler(this.buttonSetHomeSystem_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 56);
            this.progressBar1.MarqueeAnimationSpeed = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(313, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // HomeSystemChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 91);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonSetHomeSystem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSystemInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HomeSystemChooser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Choose Home System";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxSystemInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSetHomeSystem;
        public System.Windows.Forms.ProgressBar progressBar1;
    }
}