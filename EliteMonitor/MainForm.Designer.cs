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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.appStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.volatilesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTailingFailed = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.resetSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchEliteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hUDEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eDMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchEDMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dEBUGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testBodiesDatabaseReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDistanceFromSolToNetoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayCommanderCountDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTestRowsToDataViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOUNDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waterWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terraformableWaterWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.earthlikeWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ammoniaWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tHMCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemSearchTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTimestampWidthTestRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayDataGridViewColumnWidthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceOpenUpdateDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayActiveScreenResolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayTestNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testEXERenamingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testUpdateDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceVolatilesRedownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceVolatilesUpdateCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMusicEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.friendsNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockingLocationNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchJournalJSONAsWellAsDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commanderBox = new System.Windows.Forms.GroupBox();
            this.commanderLocationLabel = new System.Windows.Forms.Label();
            this.labelCreditsChange = new System.Windows.Forms.Label();
            this.empireRankProgress = new System.Windows.Forms.ProgressBar();
            this.empireRankName = new System.Windows.Forms.Label();
            this.empireRankImage = new System.Windows.Forms.PictureBox();
            this.fedRankProgress = new System.Windows.Forms.ProgressBar();
            this.fedRankName = new System.Windows.Forms.Label();
            this.federationRankImage = new System.Windows.Forms.PictureBox();
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
            this.journalContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pickCommanderLabel = new System.Windows.Forms.Label();
            this.comboCommanderList = new System.Windows.Forms.ComboBox();
            this.rankInfoTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.homeSystemTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonDiscoveredBodies = new System.Windows.Forms.Button();
            this.buttonExpeditions = new System.Windows.Forms.Button();
            this.eventList = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonTop = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonMaterials = new System.Windows.Forms.Button();
            this.materialCountNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.commanderBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.empireRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.federationRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cqcRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeRankImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.combatRankImage)).BeginInit();
            this.journalContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventList)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appStatus,
            this.volatilesLabel,
            this.toolStripTailingFailed,
            this.eliteRunningStatus,
            this.appVersionStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 641);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
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
            // volatilesLabel
            // 
            this.volatilesLabel.Name = "volatilesLabel";
            this.volatilesLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripTailingFailed
            // 
            this.toolStripTailingFailed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTailingFailed.ForeColor = System.Drawing.Color.Red;
            this.toolStripTailingFailed.Name = "toolStripTailingFailed";
            this.toolStripTailingFailed.Size = new System.Drawing.Size(113, 17);
            this.toolStripTailingFailed.Text = "TAILING DISABLED";
            this.toolStripTailingFailed.ToolTipText = "Automatic updating (tailing) is currently disabled because EliteMonitor came acro" +
    "ss an error while attempting to retrieve data from the Journal files. If this pe" +
    "rsists, please file a bug report.";
            this.toolStripTailingFailed.Visible = false;
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
            this.eDMCToolStripMenuItem,
            this.dEBUGToolStripMenuItem,
            this.optionsToolStripMenuItem});
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
            this.cachingToolStripMenuItem,
            this.resetSettingsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // applicationSettingsToolStripMenuItem
            // 
            this.applicationSettingsToolStripMenuItem.Enabled = false;
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
            // resetSettingsToolStripMenuItem
            // 
            this.resetSettingsToolStripMenuItem.Name = "resetSettingsToolStripMenuItem";
            this.resetSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetSettingsToolStripMenuItem.Text = "Reset settings";
            this.resetSettingsToolStripMenuItem.Click += new System.EventHandler(this.resetSettingsToolStripMenuItem_Click);
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
            this.launchEliteToolStripMenuItem,
            this.hUDEditorToolStripMenuItem});
            this.eliteToolStripMenuItem.Name = "eliteToolStripMenuItem";
            this.eliteToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.eliteToolStripMenuItem.Text = "Elite";
            // 
            // launchEliteToolStripMenuItem
            // 
            this.launchEliteToolStripMenuItem.Enabled = false;
            this.launchEliteToolStripMenuItem.Name = "launchEliteToolStripMenuItem";
            this.launchEliteToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.launchEliteToolStripMenuItem.Text = "Launch Elite";
            // 
            // hUDEditorToolStripMenuItem
            // 
            this.hUDEditorToolStripMenuItem.Name = "hUDEditorToolStripMenuItem";
            this.hUDEditorToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.hUDEditorToolStripMenuItem.Text = "HUD Editor";
            this.hUDEditorToolStripMenuItem.Click += new System.EventHandler(this.hUDEditorToolStripMenuItem_Click);
            // 
            // eDMCToolStripMenuItem
            // 
            this.eDMCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchEDMCToolStripMenuItem});
            this.eDMCToolStripMenuItem.Enabled = false;
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
            // dEBUGToolStripMenuItem
            // 
            this.dEBUGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testBodiesDatabaseReadToolStripMenuItem,
            this.showDistanceFromSolToNetoToolStripMenuItem,
            this.displayCommanderCountDataToolStripMenuItem,
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem,
            this.addTestRowsToDataViewToolStripMenuItem,
            this.sOUNDSToolStripMenuItem,
            this.systemSearchTestToolStripMenuItem,
            this.addTimestampWidthTestRowToolStripMenuItem,
            this.displayDataGridViewColumnWidthsToolStripMenuItem,
            this.forceOpenUpdateDialogToolStripMenuItem,
            this.displayActiveScreenResolutionToolStripMenuItem,
            this.displayTestNotificationsToolStripMenuItem,
            this.testEXERenamingToolStripMenuItem,
            this.testUpdateDownloadToolStripMenuItem,
            this.forceVolatilesRedownloadToolStripMenuItem,
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem,
            this.forceVolatilesUpdateCheckToolStripMenuItem});
            this.dEBUGToolStripMenuItem.Name = "dEBUGToolStripMenuItem";
            this.dEBUGToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.dEBUGToolStripMenuItem.Text = "DEBUG";
            // 
            // testBodiesDatabaseReadToolStripMenuItem
            // 
            this.testBodiesDatabaseReadToolStripMenuItem.Name = "testBodiesDatabaseReadToolStripMenuItem";
            this.testBodiesDatabaseReadToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.testBodiesDatabaseReadToolStripMenuItem.Text = "Test Bodies Database Read";
            this.testBodiesDatabaseReadToolStripMenuItem.Click += new System.EventHandler(this.testBodiesDatabaseReadToolStripMenuItem_Click);
            // 
            // showDistanceFromSolToNetoToolStripMenuItem
            // 
            this.showDistanceFromSolToNetoToolStripMenuItem.Name = "showDistanceFromSolToNetoToolStripMenuItem";
            this.showDistanceFromSolToNetoToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.showDistanceFromSolToNetoToolStripMenuItem.Text = "Show distance from Sol to Neto";
            this.showDistanceFromSolToNetoToolStripMenuItem.Click += new System.EventHandler(this.showDistanceFromSolToNetoToolStripMenuItem_Click);
            // 
            // displayCommanderCountDataToolStripMenuItem
            // 
            this.displayCommanderCountDataToolStripMenuItem.Name = "displayCommanderCountDataToolStripMenuItem";
            this.displayCommanderCountDataToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.displayCommanderCountDataToolStripMenuItem.Text = "Display commander count data";
            this.displayCommanderCountDataToolStripMenuItem.Click += new System.EventHandler(this.displayCommanderCountDataToolStripMenuItem_Click);
            // 
            // listViewedCommanderFirstDiscoveriesToolStripMenuItem
            // 
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem.Name = "listViewedCommanderFirstDiscoveriesToolStripMenuItem";
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem.Text = "List viewed commander first discoveries";
            this.listViewedCommanderFirstDiscoveriesToolStripMenuItem.Click += new System.EventHandler(this.listViewedCommanderFirstDiscoveriesToolStripMenuItem_Click);
            // 
            // addTestRowsToDataViewToolStripMenuItem
            // 
            this.addTestRowsToDataViewToolStripMenuItem.Name = "addTestRowsToDataViewToolStripMenuItem";
            this.addTestRowsToDataViewToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.addTestRowsToDataViewToolStripMenuItem.Text = "Add test rows to data view";
            this.addTestRowsToDataViewToolStripMenuItem.Click += new System.EventHandler(this.addTestRowsToDataViewToolStripMenuItem_Click);
            // 
            // sOUNDSToolStripMenuItem
            // 
            this.sOUNDSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.waterWorldToolStripMenuItem,
            this.terraformableWaterWorldToolStripMenuItem,
            this.earthlikeWorldToolStripMenuItem,
            this.ammoniaWorldToolStripMenuItem,
            this.hMCToolStripMenuItem,
            this.tHMCToolStripMenuItem});
            this.sOUNDSToolStripMenuItem.Name = "sOUNDSToolStripMenuItem";
            this.sOUNDSToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.sOUNDSToolStripMenuItem.Text = "SOUNDS";
            // 
            // waterWorldToolStripMenuItem
            // 
            this.waterWorldToolStripMenuItem.Name = "waterWorldToolStripMenuItem";
            this.waterWorldToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.waterWorldToolStripMenuItem.Text = "Water World";
            this.waterWorldToolStripMenuItem.Click += new System.EventHandler(this.waterWorldToolStripMenuItem_Click);
            // 
            // terraformableWaterWorldToolStripMenuItem
            // 
            this.terraformableWaterWorldToolStripMenuItem.Name = "terraformableWaterWorldToolStripMenuItem";
            this.terraformableWaterWorldToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.terraformableWaterWorldToolStripMenuItem.Text = "Terraformable Water World";
            this.terraformableWaterWorldToolStripMenuItem.Click += new System.EventHandler(this.terraformableWaterWorldToolStripMenuItem_Click);
            // 
            // earthlikeWorldToolStripMenuItem
            // 
            this.earthlikeWorldToolStripMenuItem.Name = "earthlikeWorldToolStripMenuItem";
            this.earthlikeWorldToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.earthlikeWorldToolStripMenuItem.Text = "Earthlike World";
            this.earthlikeWorldToolStripMenuItem.Click += new System.EventHandler(this.earthlikeWorldToolStripMenuItem_Click);
            // 
            // ammoniaWorldToolStripMenuItem
            // 
            this.ammoniaWorldToolStripMenuItem.Name = "ammoniaWorldToolStripMenuItem";
            this.ammoniaWorldToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.ammoniaWorldToolStripMenuItem.Text = "Ammonia World";
            this.ammoniaWorldToolStripMenuItem.Click += new System.EventHandler(this.ammoniaWorldToolStripMenuItem_Click);
            // 
            // hMCToolStripMenuItem
            // 
            this.hMCToolStripMenuItem.Name = "hMCToolStripMenuItem";
            this.hMCToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hMCToolStripMenuItem.Text = "HMC";
            this.hMCToolStripMenuItem.Click += new System.EventHandler(this.hMCToolStripMenuItem_Click);
            // 
            // tHMCToolStripMenuItem
            // 
            this.tHMCToolStripMenuItem.Name = "tHMCToolStripMenuItem";
            this.tHMCToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.tHMCToolStripMenuItem.Text = "THMC";
            this.tHMCToolStripMenuItem.Click += new System.EventHandler(this.tHMCToolStripMenuItem_Click);
            // 
            // systemSearchTestToolStripMenuItem
            // 
            this.systemSearchTestToolStripMenuItem.Name = "systemSearchTestToolStripMenuItem";
            this.systemSearchTestToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.systemSearchTestToolStripMenuItem.Text = "System Search Test";
            this.systemSearchTestToolStripMenuItem.Click += new System.EventHandler(this.systemSearchTestToolStripMenuItem_Click);
            // 
            // addTimestampWidthTestRowToolStripMenuItem
            // 
            this.addTimestampWidthTestRowToolStripMenuItem.Name = "addTimestampWidthTestRowToolStripMenuItem";
            this.addTimestampWidthTestRowToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.addTimestampWidthTestRowToolStripMenuItem.Text = "Add timestamp width test row";
            this.addTimestampWidthTestRowToolStripMenuItem.Click += new System.EventHandler(this.addTimestampWidthTestRowToolStripMenuItem_Click);
            // 
            // displayDataGridViewColumnWidthsToolStripMenuItem
            // 
            this.displayDataGridViewColumnWidthsToolStripMenuItem.Name = "displayDataGridViewColumnWidthsToolStripMenuItem";
            this.displayDataGridViewColumnWidthsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.displayDataGridViewColumnWidthsToolStripMenuItem.Text = "Display DataGridView column widths";
            this.displayDataGridViewColumnWidthsToolStripMenuItem.Click += new System.EventHandler(this.displayDataGridViewColumnWidthsToolStripMenuItem_Click);
            // 
            // forceOpenUpdateDialogToolStripMenuItem
            // 
            this.forceOpenUpdateDialogToolStripMenuItem.Name = "forceOpenUpdateDialogToolStripMenuItem";
            this.forceOpenUpdateDialogToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.forceOpenUpdateDialogToolStripMenuItem.Text = "Force open update dialog";
            this.forceOpenUpdateDialogToolStripMenuItem.Click += new System.EventHandler(this.forceOpenUpdateDialogToolStripMenuItem_Click);
            // 
            // displayActiveScreenResolutionToolStripMenuItem
            // 
            this.displayActiveScreenResolutionToolStripMenuItem.Name = "displayActiveScreenResolutionToolStripMenuItem";
            this.displayActiveScreenResolutionToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.displayActiveScreenResolutionToolStripMenuItem.Text = "Display active screen resolution";
            this.displayActiveScreenResolutionToolStripMenuItem.Click += new System.EventHandler(this.displayActiveScreenResolutionToolStripMenuItem_Click);
            // 
            // displayTestNotificationsToolStripMenuItem
            // 
            this.displayTestNotificationsToolStripMenuItem.Name = "displayTestNotificationsToolStripMenuItem";
            this.displayTestNotificationsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.displayTestNotificationsToolStripMenuItem.Text = "Display test notifications";
            this.displayTestNotificationsToolStripMenuItem.Click += new System.EventHandler(this.displayTestNotificationsToolStripMenuItem_Click);
            // 
            // testEXERenamingToolStripMenuItem
            // 
            this.testEXERenamingToolStripMenuItem.Name = "testEXERenamingToolStripMenuItem";
            this.testEXERenamingToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.testEXERenamingToolStripMenuItem.Text = "Test EXE renaming";
            this.testEXERenamingToolStripMenuItem.Click += new System.EventHandler(this.testEXERenamingToolStripMenuItem_Click);
            // 
            // testUpdateDownloadToolStripMenuItem
            // 
            this.testUpdateDownloadToolStripMenuItem.Name = "testUpdateDownloadToolStripMenuItem";
            this.testUpdateDownloadToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.testUpdateDownloadToolStripMenuItem.Text = "Test Update Download";
            this.testUpdateDownloadToolStripMenuItem.Click += new System.EventHandler(this.testUpdateDownloadToolStripMenuItem_Click);
            // 
            // forceVolatilesRedownloadToolStripMenuItem
            // 
            this.forceVolatilesRedownloadToolStripMenuItem.Name = "forceVolatilesRedownloadToolStripMenuItem";
            this.forceVolatilesRedownloadToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.forceVolatilesRedownloadToolStripMenuItem.Text = "Force reload of volatiles (REDOWNLOAD)";
            this.forceVolatilesRedownloadToolStripMenuItem.Click += new System.EventHandler(this.forceVolatilesRedownloadToolStripMenuItem_Click);
            // 
            // forceReloadOfVolatilesNODownloadToolStripMenuItem
            // 
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem.Name = "forceReloadOfVolatilesNODownloadToolStripMenuItem";
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem.Text = "Force reload of volatiles (NO download)";
            this.forceReloadOfVolatilesNODownloadToolStripMenuItem.Click += new System.EventHandler(this.forceReloadOfVolatilesNODownloadToolStripMenuItem_Click);
            // 
            // forceVolatilesUpdateCheckToolStripMenuItem
            // 
            this.forceVolatilesUpdateCheckToolStripMenuItem.Name = "forceVolatilesUpdateCheckToolStripMenuItem";
            this.forceVolatilesUpdateCheckToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.forceVolatilesUpdateCheckToolStripMenuItem.Text = "Force full-procedure volatiles update check";
            this.forceVolatilesUpdateCheckToolStripMenuItem.Click += new System.EventHandler(this.forceVolatilesUpdateCheckToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableSoundsToolStripMenuItem,
            this.hideMusicEventsToolStripMenuItem,
            this.notificationsToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // enableSoundsToolStripMenuItem
            // 
            this.enableSoundsToolStripMenuItem.Checked = true;
            this.enableSoundsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableSoundsToolStripMenuItem.Name = "enableSoundsToolStripMenuItem";
            this.enableSoundsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.enableSoundsToolStripMenuItem.Text = "Enable planetary scan sounds";
            this.enableSoundsToolStripMenuItem.Click += new System.EventHandler(this.enableSoundsToolStripMenuItem_Click);
            // 
            // hideMusicEventsToolStripMenuItem
            // 
            this.hideMusicEventsToolStripMenuItem.Checked = true;
            this.hideMusicEventsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideMusicEventsToolStripMenuItem.Name = "hideMusicEventsToolStripMenuItem";
            this.hideMusicEventsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.hideMusicEventsToolStripMenuItem.Text = "Hide \"Music\" events";
            this.hideMusicEventsToolStripMenuItem.Click += new System.EventHandler(this.hideMusicEventsToolStripMenuItem_Click);
            // 
            // notificationsToolStripMenuItem
            // 
            this.notificationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notificationsEnabledToolStripMenuItem,
            this.friendsNotificationsToolStripMenuItem,
            this.scanNotificationsToolStripMenuItem,
            this.dockingLocationNotificationsToolStripMenuItem,
            this.materialCountNotificationsToolStripMenuItem});
            this.notificationsToolStripMenuItem.Name = "notificationsToolStripMenuItem";
            this.notificationsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.notificationsToolStripMenuItem.Text = "Notifications";
            // 
            // notificationsEnabledToolStripMenuItem
            // 
            this.notificationsEnabledToolStripMenuItem.Name = "notificationsEnabledToolStripMenuItem";
            this.notificationsEnabledToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.notificationsEnabledToolStripMenuItem.Text = "Notifications Enabled";
            this.notificationsEnabledToolStripMenuItem.Click += new System.EventHandler(this.notificationsEnabledToolStripMenuItem_Click);
            // 
            // friendsNotificationsToolStripMenuItem
            // 
            this.friendsNotificationsToolStripMenuItem.Name = "friendsNotificationsToolStripMenuItem";
            this.friendsNotificationsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.friendsNotificationsToolStripMenuItem.Text = "Friends Notifications";
            this.friendsNotificationsToolStripMenuItem.Click += new System.EventHandler(this.friendsNotificationsToolStripMenuItem_Click);
            // 
            // scanNotificationsToolStripMenuItem
            // 
            this.scanNotificationsToolStripMenuItem.Name = "scanNotificationsToolStripMenuItem";
            this.scanNotificationsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.scanNotificationsToolStripMenuItem.Text = "Scan Notifications";
            this.scanNotificationsToolStripMenuItem.Click += new System.EventHandler(this.scanNotificationsToolStripMenuItem_Click);
            // 
            // dockingLocationNotificationsToolStripMenuItem
            // 
            this.dockingLocationNotificationsToolStripMenuItem.Name = "dockingLocationNotificationsToolStripMenuItem";
            this.dockingLocationNotificationsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.dockingLocationNotificationsToolStripMenuItem.Text = "Docking Location Notifications";
            this.dockingLocationNotificationsToolStripMenuItem.Click += new System.EventHandler(this.dockingLocationNotificationsToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchJournalJSONAsWellAsDataToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // searchJournalJSONAsWellAsDataToolStripMenuItem
            // 
            this.searchJournalJSONAsWellAsDataToolStripMenuItem.Name = "searchJournalJSONAsWellAsDataToolStripMenuItem";
            this.searchJournalJSONAsWellAsDataToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.searchJournalJSONAsWellAsDataToolStripMenuItem.Text = "Search Journal entry JSON as well as data";
            this.searchJournalJSONAsWellAsDataToolStripMenuItem.Click += new System.EventHandler(this.searchJournalJSONAsWellAsDataToolStripMenuItem_Click);
            // 
            // commanderBox
            // 
            this.commanderBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commanderBox.BackColor = System.Drawing.SystemColors.Control;
            this.commanderBox.Controls.Add(this.commanderLocationLabel);
            this.commanderBox.Controls.Add(this.labelCreditsChange);
            this.commanderBox.Controls.Add(this.empireRankProgress);
            this.commanderBox.Controls.Add(this.empireRankName);
            this.commanderBox.Controls.Add(this.empireRankImage);
            this.commanderBox.Controls.Add(this.fedRankProgress);
            this.commanderBox.Controls.Add(this.fedRankName);
            this.commanderBox.Controls.Add(this.federationRankImage);
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
            // commanderLocationLabel
            // 
            this.commanderLocationLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.commanderLocationLabel.AutoSize = true;
            this.commanderLocationLabel.Location = new System.Drawing.Point(46, 31);
            this.commanderLocationLabel.MaximumSize = new System.Drawing.Size(500, 13);
            this.commanderLocationLabel.MinimumSize = new System.Drawing.Size(500, 13);
            this.commanderLocationLabel.Name = "commanderLocationLabel";
            this.commanderLocationLabel.Size = new System.Drawing.Size(500, 13);
            this.commanderLocationLabel.TabIndex = 22;
            this.commanderLocationLabel.Click += new System.EventHandler(this.commanderLabel_Click);
            // 
            // labelCreditsChange
            // 
            this.labelCreditsChange.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelCreditsChange.AutoSize = true;
            this.labelCreditsChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreditsChange.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.labelCreditsChange.Location = new System.Drawing.Point(589, 29);
            this.labelCreditsChange.MaximumSize = new System.Drawing.Size(350, 13);
            this.labelCreditsChange.MinimumSize = new System.Drawing.Size(350, 13);
            this.labelCreditsChange.Name = "labelCreditsChange";
            this.labelCreditsChange.Size = new System.Drawing.Size(350, 13);
            this.labelCreditsChange.TabIndex = 21;
            this.labelCreditsChange.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            this.empireRankName.Click += new System.EventHandler(this.tradeRankName_Click);
            // 
            // empireRankImage
            // 
            this.empireRankImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.empireRankImage.Image = global::EliteMonitor.Properties.Resources.empire;
            this.empireRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.empireRankImage.Location = new System.Drawing.Point(656, 47);
            this.empireRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.empireRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.empireRankImage.Name = "empireRankImage";
            this.empireRankImage.Size = new System.Drawing.Size(137, 120);
            this.empireRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.empireRankImage.TabIndex = 18;
            this.empireRankImage.TabStop = false;
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
            this.fedRankName.Click += new System.EventHandler(this.tradeRankName_Click);
            // 
            // federationRankImage
            // 
            this.federationRankImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.federationRankImage.BackColor = System.Drawing.SystemColors.Control;
            this.federationRankImage.Image = global::EliteMonitor.Properties.Resources.federation;
            this.federationRankImage.InitialImage = global::EliteMonitor.Properties.Resources.rank_1_combat;
            this.federationRankImage.Location = new System.Drawing.Point(513, 47);
            this.federationRankImage.MaximumSize = new System.Drawing.Size(137, 120);
            this.federationRankImage.MinimumSize = new System.Drawing.Size(137, 120);
            this.federationRankImage.Name = "federationRankImage";
            this.federationRankImage.Size = new System.Drawing.Size(137, 120);
            this.federationRankImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.federationRankImage.TabIndex = 15;
            this.federationRankImage.TabStop = false;
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
            this.cqcRankName.Text = "Helpless";
            this.cqcRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cqcRankName.Click += new System.EventHandler(this.tradeRankName_Click);
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
            this.exploreRankName.Text = "Aimless";
            this.exploreRankName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exploreRankName.Click += new System.EventHandler(this.tradeRankName_Click);
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
            this.tradeRankName.Click += new System.EventHandler(this.tradeRankName_Click);
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
            this.combatRankName.Click += new System.EventHandler(this.tradeRankName_Click);
            // 
            // creditsLabel
            // 
            this.creditsLabel.AutoSize = true;
            this.creditsLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.creditsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.commanderLabel.Click += new System.EventHandler(this.commanderLabel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
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
            // journalContextMenu
            // 
            this.journalContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem5,
            this.toolStripMenuItem4});
            this.journalContextMenu.Name = "journalContextMenu";
            this.journalContextMenu.Size = new System.Drawing.Size(193, 114);
            this.journalContextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.journalContextMenu_Closed);
            this.journalContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.journalContextMenu_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem1.Text = "Copy entry timestamp";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem2.Text = "Copy entry event";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem3.Text = "Copy entry data";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem5.Text = "Copy entry JSON";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem4.Text = "Copy entry notes";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // pickCommanderLabel
            // 
            this.pickCommanderLabel.AutoSize = true;
            this.pickCommanderLabel.Location = new System.Drawing.Point(115, 253);
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
            this.comboCommanderList.Location = new System.Drawing.Point(222, 250);
            this.comboCommanderList.Name = "comboCommanderList";
            this.comboCommanderList.Size = new System.Drawing.Size(169, 21);
            this.comboCommanderList.Sorted = true;
            this.comboCommanderList.TabIndex = 7;
            this.comboCommanderList.ValueMember = "None";
            this.comboCommanderList.SelectionChangeCommitted += new System.EventHandler(this.comboCommanderList_SelectionChangeCommitted);
            // 
            // rankInfoTooltip
            // 
            this.rankInfoTooltip.AutoPopDelay = 15000;
            this.rankInfoTooltip.InitialDelay = 500;
            this.rankInfoTooltip.ReshowDelay = 100;
            // 
            // homeSystemTooltip
            // 
            this.homeSystemTooltip.AutoPopDelay = 15000;
            this.homeSystemTooltip.InitialDelay = 500;
            this.homeSystemTooltip.ReshowDelay = 100;
            // 
            // buttonDiscoveredBodies
            // 
            this.buttonDiscoveredBodies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDiscoveredBodies.Enabled = false;
            this.buttonDiscoveredBodies.Location = new System.Drawing.Point(811, 248);
            this.buttonDiscoveredBodies.Name = "buttonDiscoveredBodies";
            this.buttonDiscoveredBodies.Size = new System.Drawing.Size(137, 23);
            this.buttonDiscoveredBodies.TabIndex = 8;
            this.buttonDiscoveredBodies.Text = "Discovered Bodies";
            this.buttonDiscoveredBodies.UseVisualStyleBackColor = true;
            this.buttonDiscoveredBodies.Click += new System.EventHandler(this.buttonDiscoveredBodies_Click);
            // 
            // buttonExpeditions
            // 
            this.buttonExpeditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExpeditions.Enabled = false;
            this.buttonExpeditions.Location = new System.Drawing.Point(668, 248);
            this.buttonExpeditions.Name = "buttonExpeditions";
            this.buttonExpeditions.Size = new System.Drawing.Size(137, 23);
            this.buttonExpeditions.TabIndex = 9;
            this.buttonExpeditions.Text = "Expedition History";
            this.buttonExpeditions.UseVisualStyleBackColor = true;
            this.buttonExpeditions.Click += new System.EventHandler(this.buttonExpeditions_Click);
            // 
            // eventList
            // 
            this.eventList.AllowUserToAddRows = false;
            this.eventList.AllowUserToDeleteRows = false;
            this.eventList.AllowUserToResizeRows = false;
            this.eventList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.eventList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.eventList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.eventList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.eventList.Location = new System.Drawing.Point(12, 277);
            this.eventList.MultiSelect = false;
            this.eventList.Name = "eventList";
            this.eventList.RowHeadersVisible = false;
            this.eventList.RowTemplate.Height = 18;
            this.eventList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eventList.Size = new System.Drawing.Size(936, 361);
            this.eventList.TabIndex = 10;
            this.eventList.Scroll += new System.Windows.Forms.ScrollEventHandler(this.eventList_Scroll);
            this.eventList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.eventList_MouseClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column1.HeaderText = "Timestamp";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 83;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.HeaderText = "Event";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 129;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "Data";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // buttonTop
            // 
            this.buttonTop.Location = new System.Drawing.Point(947, 277);
            this.buttonTop.Name = "buttonTop";
            this.buttonTop.Size = new System.Drawing.Size(19, 16);
            this.buttonTop.TabIndex = 11;
            this.buttonTop.Text = "^";
            this.buttonTop.UseVisualStyleBackColor = true;
            this.buttonTop.Visible = false;
            this.buttonTop.Click += new System.EventHandler(this.buttonTop_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Enabled = false;
            this.buttonSearch.Location = new System.Drawing.Point(12, 248);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(97, 23);
            this.buttonSearch.TabIndex = 12;
            this.buttonSearch.Text = "Search Journal";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonMaterials
            // 
            this.buttonMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMaterials.Enabled = false;
            this.buttonMaterials.Location = new System.Drawing.Point(525, 248);
            this.buttonMaterials.Name = "buttonMaterials";
            this.buttonMaterials.Size = new System.Drawing.Size(137, 23);
            this.buttonMaterials.TabIndex = 13;
            this.buttonMaterials.Text = "Material Inventory";
            this.buttonMaterials.UseVisualStyleBackColor = true;
            this.buttonMaterials.Click += new System.EventHandler(this.buttonMaterials_Click);
            // 
            // materialCountNotificationsToolStripMenuItem
            // 
            this.materialCountNotificationsToolStripMenuItem.Name = "materialCountNotificationsToolStripMenuItem";
            this.materialCountNotificationsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.materialCountNotificationsToolStripMenuItem.Text = "Material Count Notifications";
            this.materialCountNotificationsToolStripMenuItem.Click += new System.EventHandler(this.materialCountNotificationsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(966, 663);
            this.Controls.Add(this.buttonMaterials);
            this.Controls.Add(this.buttonTop);
            this.Controls.Add(this.eventList);
            this.Controls.Add(this.buttonExpeditions);
            this.Controls.Add(this.buttonDiscoveredBodies);
            this.Controls.Add(this.comboCommanderList);
            this.Controls.Add(this.pickCommanderLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.commanderBox);
            this.Controls.Add(this.buttonSearch);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(982, 702);
            this.Name = "MainForm";
            this.Text = "Elite Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.MouseEnter += new System.EventHandler(this.MainForm_MouseEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.commanderBox.ResumeLayout(false);
            this.commanderBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.empireRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.federationRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cqcRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exploreRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeRankImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.combatRankImage)).EndInit();
            this.journalContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.eventList)).EndInit();
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
        public System.Windows.Forms.ProgressBar empireRankProgress;
        public System.Windows.Forms.Label empireRankName;
        public System.Windows.Forms.PictureBox empireRankImage;
        public System.Windows.Forms.ProgressBar fedRankProgress;
        public System.Windows.Forms.Label fedRankName;
        public System.Windows.Forms.PictureBox federationRankImage;
        public System.Windows.Forms.ToolStripStatusLabel appStatus;
        private System.Windows.Forms.ToolStripMenuItem cachingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCacheDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCachesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realoadJournalEntriesFromCacheToolStripMenuItem;
        private System.Windows.Forms.Label pickCommanderLabel;
        public System.Windows.Forms.ComboBox comboCommanderList;
        private System.Windows.Forms.ToolStripStatusLabel appVersionStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel eliteRunningStatus;
        private System.Windows.Forms.ToolStripMenuItem hUDEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dEBUGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testBodiesDatabaseReadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDistanceFromSolToNetoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSettingsToolStripMenuItem;
        public System.Windows.Forms.ToolTip toolTip;
        public System.Windows.Forms.ToolTip rankInfoTooltip;
        public System.Windows.Forms.ToolStripStatusLabel toolStripTailingFailed;
        public System.Windows.Forms.Label labelCreditsChange;
        private System.Windows.Forms.ToolStripMenuItem displayCommanderCountDataToolStripMenuItem;
        public System.Windows.Forms.ToolTip homeSystemTooltip;
        public System.Windows.Forms.Label commanderLocationLabel;
        private System.Windows.Forms.ToolStripMenuItem listViewedCommanderFirstDiscoveriesToolStripMenuItem;
        public System.Windows.Forms.Button buttonDiscoveredBodies;
        private System.Windows.Forms.ContextMenuStrip journalContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        public System.Windows.Forms.Button buttonExpeditions;
        public System.Windows.Forms.DataGridView eventList;
        private System.Windows.Forms.ToolStripMenuItem addTestRowsToDataViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOUNDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waterWorldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terraformableWaterWorldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem earthlikeWorldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ammoniaWorldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hMCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tHMCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableSoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMusicEventsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemSearchTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTimestampWidthTestRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayDataGridViewColumnWidthsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.ToolStripMenuItem forceOpenUpdateDialogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayActiveScreenResolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayTestNotificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationsEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem friendsNotificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanNotificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dockingLocationNotificationsToolStripMenuItem;
        private System.Windows.Forms.Button buttonTop;
        public System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchJournalJSONAsWellAsDataToolStripMenuItem;
        public System.Windows.Forms.Button buttonMaterials;
        private System.Windows.Forms.ToolStripMenuItem testEXERenamingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testUpdateDownloadToolStripMenuItem;
        public System.Windows.Forms.ToolStripStatusLabel volatilesLabel;
        private System.Windows.Forms.ToolStripMenuItem forceVolatilesRedownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceReloadOfVolatilesNODownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceVolatilesUpdateCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialCountNotificationsToolStripMenuItem;
    }
}

