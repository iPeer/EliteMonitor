namespace EliteMonitor
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.appStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.eliteRunningStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.appVersionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cachingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCacheDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realoadJournalEntriesFromCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchEliteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eDMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchEDMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commanderBox = new System.Windows.Forms.GroupBox();
            this.empireRankProgress = new System.Windows.Forms.ProgressBar();
            this.empireRankName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fedRankProgress = new System.Windows.Forms.ProgressBar();
            this.fedRankName = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.cqcRankProgress = new System.Windows.Forms.ProgressBar();
            this.cqcRankName = new System.Windows.Forms.Label();
            this.cqcRankImage = new System.Windows.Forms.PictureBox();
            this.exploreRankProgress = new System.Windows.Forms.ProgressBar();
            this.exploreRankName = new System.Windows.Forms.Label();
            this.exploreRankImage = new System.Windows.Forms.PictureBox();
            this.tradeRankProgress = new System.Windows.Forms.ProgressBar();
            this.tradeRankName = new System.Windows.Forms.Label();
            this.tradeRankImage = new System.Windows.Forms.PictureBox();
            this.combatProgress = new System.Windows.Forms.ProgressBar();
            this.combatRankName = new System.Windows.Forms.Label();
            this.creditsLabel = new System.Windows.Forms.Label();
            this.commanderLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.combatRankImage = new System.Windows.Forms.PictureBox();
            this.eventList = new System.Windows.Forms.ListView();
            this.TimeStamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EventName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EventData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AdditionalData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.eventFilterDropdown = new System.Windows.Forms.ComboBox();
            this.pickCommanderLabel = new System.Windows.Forms.Label();
            this.comboCommanderList = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.commanderBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cqcRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.combatRankImage)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appStatus,
            this.eliteRunningStatus,
            this.appVersionStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 641);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(966, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // appStatus
            // 
            this.appStatus.Name = "appStatus";
            this.appStatus.Size = new System.Drawing.Size(698, 17);
            this.appStatus.Spring = true;
            this.appStatus.Text = "Starting up...";
            this.appStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eliteRunningStatus
            // 
            this.eliteRunningStatus.Name = "eliteRunningStatus";
            this.eliteRunningStatus.Size = new System.Drawing.Size(169, 17);
            this.eliteRunningStatus.Text = "Elite: Dangerous is not running";
            // 
            // appVersionStatusLabel
            // 
            this.appVersionStatusLabel.Enabled = false;
            this.appVersionStatusLabel.Name = "appVersionStatusLabel";
            this.appVersionStatusLabel.Size = new System.Drawing.Size(84, 17);
            this.appVersionStatusLabel.Text = "{APPVERSION}";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.eliteToolStripMenuItem,
            this.eDMCToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(966, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationSettingsToolStripMenuItem,
            this.cachingToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // applicationSettingsToolStripMenuItem
            // 
            this.applicationSettingsToolStripMenuItem.Name = "applicationSettingsToolStripMenuItem";
            this.applicationSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.applicationSettingsToolStripMenuItem.Text = "&Application Settings";
            // 
            // cachingToolStripMenuItem
            // 
            this.cachingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCacheDirectoryToolStripMenuItem,
            this.clearCachesToolStripMenuItem,
            this.realoadJournalEntriesFromCacheToolStripMenuItem});
            this.cachingToolStripMenuItem.Name = "cachingToolStripMenuItem";
            this.cachingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cachingToolStripMenuItem.Text = "Caching";
            // 
            // openCacheDirectoryToolStripMenuItem
            // 
            this.openCacheDirectoryToolStripMenuItem.Name = "openCacheDirectoryToolStripMenuItem";
            this.openCacheDirectoryToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openCacheDirectoryToolStripMenuItem.Text = "Open cache directory";
            this.openCacheDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openCacheDirectoryToolStripMenuItem_Click);
            // 
            // clearCachesToolStripMenuItem
            // 
            this.clearCachesToolStripMenuItem.Name = "clearCachesToolStripMenuItem";
            this.clearCachesToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.clearCachesToolStripMenuItem.Text = "&Clear caches";
            this.clearCachesToolStripMenuItem.Click += new System.EventHandler(this.clearCachesToolStripMenuItem_Click);
            // 
            // realoadJournalEntriesFromCacheToolStripMenuItem
            // 
            this.realoadJournalEntriesFromCacheToolStripMenuItem.Name = "realoadJournalEntriesFromCacheToolStripMenuItem";
            this.realoadJournalEntriesFromCacheToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.realoadJournalEntriesFromCacheToolStripMenuItem.Text = "Reload current commander\'s events";
            this.realoadJournalEntriesFromCacheToolStripMenuItem.Click += new System.EventHandler(this.debugRefreshJournal_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // eliteToolStripMenuItem
            // 
            this.eliteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchEliteToolStripMenuItem});
            this.eliteToolStripMenuItem.Name = "eliteToolStripMenuItem";
            this.eliteToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.eliteToolStripMenuItem.Text = "Elite";
            // 
            // launchEliteToolStripMenuItem
            // 
            this.launchEliteToolStripMenuItem.Name = "launchEliteToolStripMenuItem";
            this.launchEliteToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.launchEliteToolStripMenuItem.Text = "Launch Elite";
            // 
            // eDMCToolStripMenuItem
            // 
            this.eDMCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchEDMCToolStripMenuItem});
            this.eDMCToolStripMenuItem.Name = "eDMCToolStripMenuItem";
            this.eDMCToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.eDMCToolStripMenuItem.Text = "EDMC";
            // 
            // launchEDMCToolStripMenuItem
            // 
            this.launchEDMCToolStripMenuItem.Name = "launchEDMCToolStripMenuItem";
            this.launchEDMCToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.launchEDMCToolStripMenuItem.Text = "Launch EDMC";
            // 
            // commanderBox
            // 
            this.commanderBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commanderBox.BackColor = System.Drawing.SystemColors.Control;
            this.commanderBox.Controls.Add(this.empireRankProgress);
            this.commanderBox.Controls.Add(this.empireRankName);
            this.commanderBox.Controls.Add(this.pictureBox1);
            this.commanderBox.Controls.Add(this.fedRankProgress);
            this.commanderBox.Controls.Add(this.fedRankName);
            this.commanderBox.Controls.Add(this.pictureBox2);
            this.commanderBox.Controls.Add(this.cqcRankProgress);
            this.commanderBox.Controls.Add(this.cqcRankName);
            this.commanderBox.Controls.Add(this.cqcRankImage);
            this.commanderBox.Controls.Add(this.exploreRankProgress);
            this.commanderBox.Controls.Add(this.exploreRankName);
            this.commanderBox.Controls.Add(this.exploreRankImage);
            this.commanderBox.Controls.Add(this.tradeRankProgress);
            this.commanderBox.Controls.Add(this.tradeRankName);
            this.commanderBox.Controls.Add(this.tradeRankImage);
            this.commanderBox.Controls.Add(this.combatProgress);
            this.commanderBox.Controls.Add(this.combatRankName);
            this.commanderBox.Controls.Add(this.creditsLabel);
            this.commanderBox.Controls.Add(this.commanderLabel);
            this.commanderBox.Controls.Add(this.label1);
            this.commanderBox.Controls.Add(this.combatRankImage);
            this.commanderBox.Location = new System.Drawing.Point(12, 27);
            this.commanderBox.Name = "commanderBox";
            this.commanderBox.Size = new System.Drawing.Size(942, 217);
            this.commanderBox.TabIndex = 2;
            this.commanderBox.TabStop = false;
            this.commanderBox.Text = "Commander";
            // 
            // empireRankProgress
            // 
            this.empireRankProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.empireRankProgress.Location = new System.Drawing.Point(656, 186);
            this.empireRankProgress.Name = "empireRankProgress";
            this.empireRankProgress.Size = new System.Drawing.Size(137, 23);
            this.empireRankProgress.TabIndex = 20;
            // 
            // empireRankName
            // 
            this.empireRankName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.empireRankName.AutoSize = true;
            this.empireRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.empireRankName.Location = new System.Drawing.Point(656, 170);
            this.empireRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.empireRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.empireRankName.Name = "empireRankName";
            this.empireRankName.Size = new System.Drawing.Size(137, 13);
            this.empireRankName.TabIndex = 19;
            this.empireRankName.Text = "None";
            this.empireRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::EliteMonitor.Properties.Resources.empire;
            this.pictureBox1.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.pictureBox1.Location = new System.Drawing.Point(656, 47);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(137, 120);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(137, 120);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // fedRankProgress
            // 
            this.fedRankProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fedRankProgress.Location = new System.Drawing.Point(513, 186);
            this.fedRankProgress.MarqueeAnimationSpeed = 0;
            this.fedRankProgress.Name = "fedRankProgress";
            this.fedRankProgress.Size = new System.Drawing.Size(137, 23);
            this.fedRankProgress.TabIndex = 17;
            // 
            // fedRankName
            // 
            this.fedRankName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fedRankName.AutoSize = true;
            this.fedRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fedRankName.Location = new System.Drawing.Point(513, 170);
            this.fedRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.fedRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.fedRankName.Name = "fedRankName";
            this.fedRankName.Size = new System.Drawing.Size(137, 13);
            this.fedRankName.TabIndex = 16;
            this.fedRankName.Text = "None";
            this.fedRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox2.Image = global::EliteMonitor.Properties.Resources.federation;
            this.pictureBox2.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.pictureBox2.Location = new System.Drawing.Point(513, 47);
            this.pictureBox2.MaximumSize = new System.Drawing.Size(137, 120);
            this.pictureBox2.MinimumSize = new System.Drawing.Size(137, 120);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(137, 120);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            // 
            // cqcRankProgress
            // 
            this.cqcRankProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cqcRankProgress.Location = new System.Drawing.Point(799, 186);
            this.cqcRankProgress.Name = "cqcRankProgress";
            this.cqcRankProgress.Size = new System.Drawing.Size(137, 23);
            this.cqcRankProgress.TabIndex = 14;
            // 
            // cqcRankName
            // 
            this.cqcRankName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cqcRankName.AutoSize = true;
            this.cqcRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cqcRankName.Location = new System.Drawing.Point(799, 170);
            this.cqcRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.cqcRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.cqcRankName.Name = "cqcRankName";
            this.cqcRankName.Size = new System.Drawing.Size(137, 13);
            this.cqcRankName.TabIndex = 13;
            this.cqcRankName.Text = "cqc";
            this.cqcRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cqcRankImage
            // 
            this.cqcRankImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cqcRankImage.Image = global::EliteMonitor.Properties.Resources.rank_1_cqc;
            this.cqcRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.cqcRankImage.Location = new System.Drawing.Point(799, 47);
            this.cqcRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.cqcRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.cqcRankImage.Name = "cqcRankImage";
            this.cqcRankImage.Size = new System.Drawing.Size(137, 120);
            this.cqcRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.cqcRankImage.TabIndex = 12;
            this.cqcRankImage.TabStop = false;
            // 
            // exploreRankProgress
            // 
            this.exploreRankProgress.Location = new System.Drawing.Point(292, 186);
            this.exploreRankProgress.Name = "exploreRankProgress";
            this.exploreRankProgress.Size = new System.Drawing.Size(137, 23);
            this.exploreRankProgress.TabIndex = 11;
            // 
            // exploreRankName
            // 
            this.exploreRankName.AutoSize = true;
            this.exploreRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exploreRankName.Location = new System.Drawing.Point(292, 170);
            this.exploreRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.exploreRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.exploreRankName.Name = "exploreRankName";
            this.exploreRankName.Size = new System.Drawing.Size(137, 13);
            this.exploreRankName.TabIndex = 10;
            this.exploreRankName.Text = "????";
            this.exploreRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // exploreRankImage
            // 
            this.exploreRankImage.Image = global::EliteMonitor.Properties.Resources.rank_1;
            this.exploreRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.exploreRankImage.Location = new System.Drawing.Point(292, 47);
            this.exploreRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.exploreRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.exploreRankImage.Name = "exploreRankImage";
            this.exploreRankImage.Size = new System.Drawing.Size(137, 120);
            this.exploreRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.exploreRankImage.TabIndex = 9;
            this.exploreRankImage.TabStop = false;
            // 
            // tradeRankProgress
            // 
            this.tradeRankProgress.Location = new System.Drawing.Point(149, 186);
            this.tradeRankProgress.MarqueeAnimationSpeed = 0;
            this.tradeRankProgress.Name = "tradeRankProgress";
            this.tradeRankProgress.Size = new System.Drawing.Size(137, 23);
            this.tradeRankProgress.TabIndex = 8;
            // 
            // tradeRankName
            // 
            this.tradeRankName.AutoSize = true;
            this.tradeRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tradeRankName.Location = new System.Drawing.Point(149, 170);
            this.tradeRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.tradeRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.tradeRankName.Name = "tradeRankName";
            this.tradeRankName.Size = new System.Drawing.Size(137, 13);
            this.tradeRankName.TabIndex = 7;
            this.tradeRankName.Text = "Penniless";
            this.tradeRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tradeRankImage
            // 
            this.tradeRankImage.BackColor = System.Drawing.SystemColors.Control;
            this.tradeRankImage.Image = global::EliteMonitor.Properties.Resources.rank_1_trading;
            this.tradeRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.tradeRankImage.Location = new System.Drawing.Point(149, 47);
            this.tradeRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.tradeRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.tradeRankImage.Name = "tradeRankImage";
            this.tradeRankImage.Size = new System.Drawing.Size(137, 120);
            this.tradeRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.tradeRankImage.TabIndex = 6;
            this.tradeRankImage.TabStop = false;
            // 
            // combatProgress
            // 
            this.combatProgress.Location = new System.Drawing.Point(6, 186);
            this.combatProgress.Name = "combatProgress";
            this.combatProgress.Size = new System.Drawing.Size(137, 23);
            this.combatProgress.TabIndex = 5;
            // 
            // combatRankName
            // 
            this.combatRankName.AutoSize = true;
            this.combatRankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.combatRankName.Location = new System.Drawing.Point(6, 170);
            this.combatRankName.MaximumSize = new System.Drawing.Size(137, 13);
            this.combatRankName.MinimumSize = new System.Drawing.Size(137, 13);
            this.combatRankName.Name = "combatRankName";
            this.combatRankName.Size = new System.Drawing.Size(137, 13);
            this.combatRankName.TabIndex = 4;
            this.combatRankName.Text = "Harmless";
            this.combatRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // creditsLabel
            // 
            this.creditsLabel.AutoSize = true;
            this.creditsLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.creditsLabel.Location = new System.Drawing.Point(589, 16);
            this.creditsLabel.MaximumSize = new System.Drawing.Size(350, 13);
            this.creditsLabel.MinimumSize = new System.Drawing.Size(350, 13);
            this.creditsLabel.Name = "creditsLabel";
            this.creditsLabel.Size = new System.Drawing.Size(350, 13);
            this.creditsLabel.TabIndex = 2;
            this.creditsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // commanderLabel
            // 
            this.commanderLabel.AutoSize = true;
            this.commanderLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.commanderLabel.Location = new System.Drawing.Point(46, 16);
            this.commanderLabel.MaximumSize = new System.Drawing.Size(500, 13);
            this.commanderLabel.MinimumSize = new System.Drawing.Size(500, 13);
            this.commanderLabel.Name = "commanderLabel";
            this.commanderLabel.Size = new System.Drawing.Size(500, 13);
            this.commanderLabel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CMDR";
            // 
            // combatRankImage
            // 
            this.combatRankImage.Image = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.combatRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.combatRankImage.Location = new System.Drawing.Point(6, 47);
            this.combatRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.combatRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.combatRankImage.Name = "combatRankImage";
            this.combatRankImage.Size = new System.Drawing.Size(137, 120);
            this.combatRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.combatRankImage.TabIndex = 3;
            this.combatRankImage.TabStop = false;
            // 
            // eventList
            // 
            this.eventList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TimeStamp,
            this.EventName,
            this.EventData,
            this.AdditionalData});
            this.eventList.FullRowSelect = true;
            this.eventList.GridLines = true;
            this.eventList.LabelWrap = false;
            this.eventList.Location = new System.Drawing.Point(12, 277);
            this.eventList.MultiSelect = false;
            this.eventList.Name = "eventList";
            this.eventList.Size = new System.Drawing.Size(942, 361);
            this.eventList.TabIndex = 3;
            this.eventList.UseCompatibleStateImageBehavior = false;
            this.eventList.View = System.Windows.Forms.View.Details;
            this.eventList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.eventList_MouseMove);
            // 
            // TimeStamp
            // 
            this.TimeStamp.Text = "Timestamp";
            // 
            // EventName
            // 
            this.EventName.Text = "Event";
            // 
            // EventData
            // 
            this.EventData.Text = "Event Data";
            // 
            // AdditionalData
            // 
            this.AdditionalData.Text = "Additional Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 253);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Highlight event:";
            // 
            // eventFilterDropdown
            // 
            this.eventFilterDropdown.DisplayMember = "None";
            this.eventFilterDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eventFilterDropdown.FormattingEnabled = true;
            this.eventFilterDropdown.Items.AddRange(new object[] {
            "NONE"});
            this.eventFilterDropdown.Location = new System.Drawing.Point(100, 250);
            this.eventFilterDropdown.Name = "eventFilterDropdown";
            this.eventFilterDropdown.Size = new System.Drawing.Size(169, 21);
            this.eventFilterDropdown.TabIndex = 5;
            this.eventFilterDropdown.ValueMember = "None";
            this.eventFilterDropdown.SelectionChangeCommitted += new System.EventHandler(this.eventFilterDropdown_SelectionChangeCommitted);
            // 
            // pickCommanderLabel
            // 
            this.pickCommanderLabel.AutoSize = true;
            this.pickCommanderLabel.Location = new System.Drawing.Point(275, 253);
            this.pickCommanderLabel.Name = "pickCommanderLabel";
            this.pickCommanderLabel.Size = new System.Drawing.Size(101, 13);
            this.pickCommanderLabel.TabIndex = 6;
            this.pickCommanderLabel.Text = "Choose commander";
            // 
            // comboCommanderList
            // 
            this.comboCommanderList.DisplayMember = "None";
            this.comboCommanderList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCommanderList.FormattingEnabled = true;
            this.comboCommanderList.Location = new System.Drawing.Point(382, 250);
            this.comboCommanderList.Name = "comboCommanderList";
            this.comboCommanderList.Size = new System.Drawing.Size(169, 21);
            this.comboCommanderList.Sorted = true;
            this.comboCommanderList.TabIndex = 7;
            this.comboCommanderList.ValueMember = "None";
            this.comboCommanderList.SelectionChangeCommitted += new System.EventHandler(this.comboCommanderList_SelectionChangeCommitted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(966, 663);
            this.Controls.Add(this.comboCommanderList);
            this.Controls.Add(this.pickCommanderLabel);
            this.Controls.Add(this.eventFilterDropdown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.eventList);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.commanderBox);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(982, 702);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Elite Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.MouseEnter += new System.EventHandler(this.MainForm_MouseEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.commanderBox.ResumeLayout(false);
            this.commanderBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cqcRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.combatRankImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applicationSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eliteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchEliteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eDMCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchEDMCToolStripMenuItem;
        private System.Windows.Forms.GroupBox commanderBox;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label creditsLabel;
        public System.Windows.Forms.Label commanderLabel;
        public System.Windows.Forms.PictureBox combatRankImage;
        public System.Windows.Forms.Label combatRankName;
        public System.Windows.Forms.ProgressBar combatProgress;
        public System.Windows.Forms.ProgressBar cqcRankProgress;
        public System.Windows.Forms.Label cqcRankName;
        public System.Windows.Forms.PictureBox cqcRankImage;
        public System.Windows.Forms.ProgressBar exploreRankProgress;
        public System.Windows.Forms.Label exploreRankName;
        public System.Windows.Forms.PictureBox exploreRankImage;
        public System.Windows.Forms.ProgressBar tradeRankProgress;
        public System.Windows.Forms.Label tradeRankName;
        public System.Windows.Forms.PictureBox tradeRankImage;
        private System.Windows.Forms.ColumnHeader TimeStamp;
        private System.Windows.Forms.ColumnHeader EventName;
        private System.Windows.Forms.ColumnHeader EventData;
        private System.Windows.Forms.ColumnHeader AdditionalData;
        public System.Windows.Forms.ProgressBar empireRankProgress;
        public System.Windows.Forms.Label empireRankName;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ProgressBar fedRankProgress;
        public System.Windows.Forms.Label fedRankName;
        public System.Windows.Forms.PictureBox pictureBox2;
        public System.Windows.Forms.ToolStripStatusLabel appStatus;
        private System.Windows.Forms.ToolTip toolTip;
        public System.Windows.Forms.ListView eventList;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox eventFilterDropdown;
        private System.Windows.Forms.ToolStripMenuItem cachingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCacheDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCachesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realoadJournalEntriesFromCacheToolStripMenuItem;
        private System.Windows.Forms.Label pickCommanderLabel;
        public System.Windows.Forms.ComboBox comboCommanderList;
        private System.Windows.Forms.ToolStripStatusLabel appVersionStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel eliteRunningStatus;
    }
}

