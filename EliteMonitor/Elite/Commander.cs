﻿using EliteMonitor.Extensions;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Windows.Forms;

namespace EliteMonitor.Elite
{

    public class CommanderSession { } // TODO

    public class CommanderShipLoadout
    {
        public CommanderShip ShipData { get; set; }
        public string LoadoutJson { get; set; } = "";
    }

    public class CommanderShip
    {
        public string ShipNonVanityName { get; set; }
        [JsonIgnore]
        private string _shipName = "";
        public string ShipName
        {
            get
            {
                return this._shipName == string.Empty ? this.ShipNonVanityName : this._shipName;
            }
            set
            {
                this._shipName = value;
            }
        }
        public string ShipID { get; set; } = "";

        public CommanderShip(string nonVanity, string shipID = "", string shipName = "")
        {
            this.ShipNonVanityName = nonVanity;
            this.ShipID = shipID;
            this.ShipName = shipName;
        }

        public string getFormattedShipString()
        {
            if ((this.ShipID == null || this.ShipName == null) || this.ShipID.Equals("") && this.ShipName.Equals(""))
                return this.ShipNonVanityName;
            return string.Format("{0}{1}{2}", this.ShipID, this.ShipName.Equals(this.ShipNonVanityName) ? "" : string.Format(": {0} (", this.ShipName), string.Format(this.ShipName.Equals(this.ShipNonVanityName) ? "{0}" : "{0})", this.ShipNonVanityName));
        }
    }

    public class Commander
    {

        //[JsonIgnore]
        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        public List<string> EventsList { get; set; } = new List<string>();
        [JsonIgnore]
        [Obsolete("No longer needed", true)]
        public List<string> Journal { get; set; } = new List<string>();
        [JsonIgnore]
        public long lastCreditsChange = 0L;
        [JsonIgnore]
        public Dictionary<string, int> Materials { get; set; } = new Dictionary<string, int>();
        [JsonIgnore]
        public Dictionary<int, CommanderShipLoadout> Fleet = new Dictionary<int, CommanderShipLoadout>();
        public int cacheVersion = Utils.getBuildNumber();
        public bool newSaveMethod = false;
        [JsonIgnore]
        public string saveDirectory = string.Empty;
        [JsonIgnore]
        public List<CommanderSession> Sessions = new List<CommanderSession>();
        public long nextId = 0;
        [JsonIgnore]
        public bool isActive
        {
            get
            {
                return MainForm.Instance.journalParser.activeCommander == this;
            }
        }
        public bool isViewed { get; set; } = false;
        public string Name { get; set; }
        public long Credits { get; set; }
        public CommanderShip ShipData { get; set; }
        public string Ship { get; set; }
        [JsonIgnore] // We don't need to save this because it's kind of irrelevant
        public string PrivateGroup { get; set; } = string.Empty;
        public bool HasHomeSystem { get; set; } = false;
        public BasicSystem HomeSystem { get; set; } = null;

        public int combatRank { get; set; }
        public int tradeRank { get; set; }
        public int explorationRank { get; set; }
        public int cqcRank { get; set; }
        public int federationRank { get; set; }
        public int imperialRank { get; set; }
        public bool isDocked { get; set; }
        public bool isLanded { get; set; }
        public string CurrentSystem { get; set; }
        public SystemCoordinate CurrentSystemCoordinates { get; set; }
        public string CurrentLocation { get; set; }
        public bool isInMulticrew { get; set; } = false;
        [JsonIgnore]
        public string MultiCrewCommanderName { get; set; } = string.Empty;

        [JsonIgnore]
        public string combatRankName
        {
            get
            {
                return Ranks.rankNames["combat"][this.combatRank];
            }
        }

        [JsonIgnore]
        public string tradeRankName
        {
            get
            {
                return Ranks.rankNames["trade"][this.tradeRank];
            }
        }

        [JsonIgnore]
        public string explorationRankName
        {
            get
            {
                return Ranks.rankNames["explore"][this.explorationRank];
            }
        }

        [JsonIgnore]
        public string cqcRankName
        {
            get
            {
                return Ranks.rankNames["cqc"][this.cqcRank];
            }
        }

        [JsonIgnore]
        public string federationRankName
        {
            get
            {
                return Ranks.federation[this.federationRank];
            }
        }

        [JsonIgnore]
        public string imperialRankName
        {
            get
            {
                return Ranks.empire[this.imperialRank];
            }
        }

        public int combatProgress { get; set; }
        public int tradeProgress { get; set; }
        public int explorationProgress { get; set; }
        public int cqcProgress { get; set; }
        public int federationProgress { get; set; }
        public int imperialProgress { get; set; }

        public Commander(string name)
        {
            this.Name = name;
            this.setSaveDirectory();
        }

        public Commander setSaveDirectory()
        {

            this.saveDirectory = Path.Combine(MainForm.Instance.cacheController.cachePath, this.Name);

            return this;
        }

        public Commander SetShip (string nV, string sI, string sN)
        {
            this.ShipData = new CommanderShip(nV, sI, sN);
            return this;
        }

        public Commander SetShip(CommanderShip ship)
        {
            this.ShipData = ship;
            return this;
        }

        public Commander setBasicInfo(string ship, long credits, string privateGroup)
        {
            this.ShipData = new CommanderShip(ship);
            this.Credits = credits;
            this.PrivateGroup = privateGroup;
            return this;
        }

        public Commander setRanks(int combat, int trade, int explore, int cqc, int fed, int emp)
        {
            this.combatRank = combat;
            this.tradeRank = trade;
            this.explorationRank = explore;
            this.cqcRank = cqc;
            this.federationRank = fed;
            this.imperialRank = emp;
            return this;
        }

        public Commander setRankProgress(int combat, int trade, int explore, int cqc, int fed, int emp)
        {
            this.combatProgress = combat;
            this.tradeProgress = trade;
            this.explorationProgress = explore;
            this.cqcProgress = cqc;
            this.federationProgress = fed;
            this.imperialProgress = emp;
            return this;
        }

        public long saveData(bool uncompressed = false)
        {
            this.OnSave();
            return Utils.saveGZip("journal", this.saveDirectory, JsonConvert.SerializeObject(this));
            /*string commanderSavePath = Path.Combine(MainForm.Instance.cacheController.cachePath, MainForm.Instance.cacheController.commanderCaches.ContainsKey(this.Name) ? MainForm.Instance.cacheController.commanderCaches[this.Name].Item1 : $"./{this.Name}/journal.emj");
            if (uncompressed)
            {
                string theString = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(string.Format("{0}.decompressed", commanderSavePath), false, Encoding.UTF8))
                {
                    sw.WriteLine(theString);
                }
                return Encoding.UTF8.GetBytes(theString).LongLength;
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
                using (FileStream fs = new FileStream(commanderSavePath, FileMode.Create))
                {
                    using (GZipStream gz = new GZipStream(fs, CompressionLevel.Optimal))
                    {
                        gz.Write(bytes, 0, bytes.Length);
                    }
                }
                return bytes.LongLength;
            }*/
        }

        public Commander adjustCredits(long amount)
        {
            if (amount < 0)
                return deductCredits(Math.Abs(amount));
            else return addCredits(amount);
        }

        public Commander deductCredits(long credits)
        {
            this.Credits -= credits;
            this.lastCreditsChange = -credits;
            return this;
        }

        public Commander addCredits(long credits)
        {
            this.Credits += credits;
            this.lastCreditsChange = credits;
            return this;
        }

        public DirectoryInfo CreateSaveDirectory()
        {
            string mainCacheDir = MainForm.Instance.cacheController.cachePath;
            string fullCommanderPath = Path.Combine(mainCacheDir, $"./{this.Name}/");
            return Directory.CreateDirectory(fullCommanderPath);
        }

        public Commander removeMaterial(string material, int count)
        {
            if (this.Materials.ContainsKey(material))
                this.Materials[material] -= count;
            else
                this.Materials.Add(material, 0);
            if (this.Materials[material] < 0)
                this.Materials[material] = 0;
            return this;
        }

        public Commander addMaterial(string material, int count)
        {
            if (this.Materials.ContainsKey(material))
                this.Materials[material] += count;
            else
                this.Materials.Add(material, count);
            return this;
        }

        [Obsolete]
        public Commander addEvent(string @event)
        {
            if (!this.Journal.Contains(@event))
                this.Journal.Add(@event);
            return this;
        }

        public void updateDialogDisplays()
        {
            this.updateDialogDisplays(MainForm.Instance);
        }

        public Commander applyPromotion(string type, int rank)
        {
            type = type.ToLower();
            if (type.Equals("combat")) {
                if (rank > this.combatRank)
                {
                    this.combatRank = rank;
                    this.combatProgress = 0;
                }
            }
            else if (type.Equals("trade"))
            {
                if (rank > this.tradeRank)
                {
                    this.tradeRank = rank;
                    this.tradeProgress = 0;
                }
            }
            else if (type.Equals("explore"))
            {
                if (rank > this.explorationRank)
                {
                    this.explorationRank = rank;
                    this.explorationProgress = 0;
                }
            }
            else if (type.Equals("cqc"))
            {
                if (rank > this.cqcRank)
                {
                    this.cqcRank = rank;
                    this.cqcProgress = 0;
                }
            }
            else if (type.Equals("federation"))
            {
                if (rank > this.federationRank)
                {
                    this.federationRank = rank;
                    this.federationProgress = 0;
                }
            }
            else if (type.Equals("empire"))
            {
                if (rank > this.imperialRank)
                {
                    this.imperialRank = rank;
                    this.imperialProgress = 0;
                }
            }
            return this;
        }

        public void updateDialogDisplays(MainForm m)
        {

            // Home system tooltip

            m.InvokeIfRequired(() =>
            {
                m.homeSystemTooltip.SetToolTip(m.commanderLabel, "Click to set or change home system.");
                m.homeSystemTooltip.SetToolTip(m.commanderLocationLabel, "Click to set or change home system.");
            });


            // Commander data

            m.commanderLabel.InvokeIfRequired(() => {
                /*string format = "{0} {1} | {2}";
                if (isInMulticrew)
                    format = "{0} {1} | {2}";
                if (string.IsNullOrEmpty(this.CurrentLocation) && string.IsNullOrEmpty(this.CurrentSystem))
                    format = "{0} {1} | {2}";*/

                m.commanderLabel.Text = string.Format("{0}{1} | {2}", this.Name, (this.isInMulticrew ? string.Format(" ({0})", this.MultiCrewCommanderName) : (string.IsNullOrEmpty(this.PrivateGroup) ? "" : string.Format(" ({0})", this.PrivateGroup))), (this.ShipData != null ? this.ShipData.getFormattedShipString() : this.Ship));
                if (!(string.IsNullOrEmpty(this.CurrentLocation) && string.IsNullOrEmpty(this.CurrentSystem)))
                {
                    m.commanderLocationLabel.Text = string.Format("{0}{1}{2}", this.isDocked || this.isLanded ? this.isDocked ? "Docked at " : "Landed on " : "", String.Format("{0} | ", this.CurrentLocation), this.CurrentSystem);
                    if (this.HasHomeSystem && this.CurrentSystemCoordinates != null)
                    {
                        m.commanderLocationLabel.Text += string.Format(" ({0} ly from {1})", Utils.CalculateLyDistance(to: this.CurrentSystemCoordinates, from: this.HomeSystem.Coordinates).ToString("0,0.00"), this.HomeSystem.Name);
                    }
                }
                else
                {
                    m.commanderLocationLabel.Text = string.Empty;
                }
            });
            //m.commanderLabel.InvokeIfRequired(() => m.commanderLabel.Text = String.Format("{0} | {1} {2}", this.Name, this.Ship, !this.PrivateGroup.Equals(string.Empty) && this.PrivateGroup != null ? $"/ {this.PrivateGroup}" : ""));

            // Credits

            m.creditsLabel.InvokeIfRequired(() => m.creditsLabel.Text = string.Format("{0:n0} Cr", this.Credits));
            if (this.lastCreditsChange != 0L)
            {
                m.labelCreditsChange.InvokeIfRequired(() =>
                {
                    m.labelCreditsChange.ForeColor = this.lastCreditsChange < 0L ? Color.Red : Color.DarkGreen;
                    m.labelCreditsChange.Text = string.Format("{1}{0:n0} Cr", Math.Abs(this.lastCreditsChange), this.lastCreditsChange < 0L ? "-" : "+");
                });
            }

            // Ranks - Images

            Image combatImg = Elite.Ranks.combat[this.combatRank];
            Image tradeImg = Elite.Ranks.trade[this.tradeRank];
            Image exploreImg = Elite.Ranks.explore[this.explorationRank];
            Image cqcImg = Elite.Ranks.cqc[this.cqcRank];

            m.combatRankImage.InvokeIfRequired(() => m.combatRankImage.Image = combatImg);
            m.tradeRankImage.InvokeIfRequired(() => m.tradeRankImage.Image = tradeImg);
            m.exploreRankImage.InvokeIfRequired(() => m.exploreRankImage.Image = exploreImg);
            m.cqcRankImage.InvokeIfRequired(() => m.cqcRankImage.Image = cqcImg);

            // Ranks - Titles and percentages

            string combatRank = string.Format("{0} | {1}%", this.combatRankName, this.combatProgress);
            string tradeRank = string.Format("{0} | {1}%", this.tradeRankName, this.tradeProgress);
            string explorationRank = string.Format("{0} | {1}%", this.explorationRankName, this.explorationProgress);
            string cqcRank = string.Format("{0} | {1}%", this.cqcRankName, this.cqcProgress);
            string federationRank = string.Format("{0} | {1}%", this.federationRankName, this.federationProgress);
            string imperialRank = string.Format("{0} | {1}%", this.imperialRankName, this.imperialProgress);

            m.combatRankName.InvokeIfRequired(() => { m.combatRankName.Text = combatRank; m.rankInfoTooltip.SetToolTip(m.combatRankName, "Left or right click to manually update rank progression."); });
            m.tradeRankName.InvokeIfRequired(() => 
            {
                m.tradeRankName.Text = tradeRank;
                long[] rankData = Ranks.calculateRankCredits(Ranks.RankType.TRADE, this.tradeRank, this.tradeProgress);
                StringBuilder sb = new StringBuilder();
                if (this.tradeRank < 8)
                {
                    sb.AppendLine("Rank progression based on approximate values:");
                    sb.AppendLineFormatted("Credits past {0}: {1:n0}", this.tradeRankName, rankData[0]);
                    sb.AppendLineFormatted("Credits to {0}: {1:n0}", Ranks.rankNames["trade"][this.tradeRank + 1], rankData[1]);
                    sb.AppendLineFormatted("Credits past {0}: {1:n0}", Ranks.rankNames["trade"][0], rankData[2]);

                    sb.AppendLine();
                }
                sb.AppendLine("Left or right click to manually update rank progression.");
#if DEBUG
                Console.WriteLine("DEBUG TRADE RANK DATA:");
                Console.WriteLine(sb.ToString());
#endif
                m.rankInfoTooltip.SetToolTip(m.tradeRankName, sb.ToString());
            });
            m.exploreRankName.InvokeIfRequired(() => 
            {
                m.exploreRankName.Text = explorationRank;
                long[] rankData = Ranks.calculateRankCredits(Ranks.RankType.EXPLORATION, this.explorationRank, this.explorationProgress);
                StringBuilder sb = new StringBuilder();
                if (this.explorationRank < 8)
                {
                    sb.AppendLine("Rank progression based on approximate values:");
                    sb.AppendLineFormatted("Credits past {0}: {1:n0}", this.explorationRankName, rankData[0]);
                    sb.AppendLineFormatted("Credits to {0}: {1:n0}", Ranks.rankNames["explore"][this.explorationRank + 1], rankData[1]);
                    sb.AppendLineFormatted("Credits past {0}: {1:n0}", Ranks.rankNames["explore"][0], rankData[2]);

                    sb.AppendLine();
                }
                sb.AppendLine("Left or right click to manually update rank progression.");
#if DEBUG
                Console.WriteLine("DEBUG EXPLORATION RANK DATA:");
                Console.WriteLine(sb.ToString());
#endif
                m.rankInfoTooltip.SetToolTip(m.exploreRankName, sb.ToString());
            });
            m.cqcRankName.InvokeIfRequired(() => { m.cqcRankName.Text = cqcRank; m.rankInfoTooltip.SetToolTip(m.cqcRankName, "Left or right click to manually update rank progression."); });
            m.fedRankName.InvokeIfRequired(() => { m.fedRankName.Text = federationRank; m.rankInfoTooltip.SetToolTip(m.fedRankName, "Left or right click to manually update rank progression."); });
            m.empireRankName.InvokeIfRequired(() => { m.empireRankName.Text = imperialRank; m.rankInfoTooltip.SetToolTip(m.empireRankName, "Left or right click to manually update rank progression."); });

            m.combatProgress.InvokeIfRequired(() => m.combatProgress.Value = this.combatProgress);
            m.tradeRankProgress.InvokeIfRequired(() => m.tradeRankProgress.Value = this.tradeProgress);
            m.exploreRankProgress.InvokeIfRequired(() => m.exploreRankProgress.Value = this.explorationProgress);
            m.cqcRankProgress.InvokeIfRequired(() => m.cqcRankProgress.Value = this.cqcProgress);
            m.fedRankProgress.InvokeIfRequired(() => m.fedRankProgress.Value = this.federationProgress);
            m.empireRankProgress.InvokeIfRequired(() => m.empireRankProgress.Value = this.imperialProgress);

            // Event Type list

            updateEventListDropdown();

        }

        public void updateEventListDropdown()
        {
            MainForm m = MainForm.Instance;
            List<string> tmp = new List<string>(this.EventsList);
            tmp.Sort();
            tmp.Insert(0, "NONE");
            m.eventFilterDropdown.InvokeIfRequired(() =>
            {
                m.eventFilterDropdown.Items.Clear();
                m.eventFilterDropdown.Items.AddRange(tmp.ToArray());
                m.eventFilterDropdown.SelectedIndex = 0;
            });
        }

        public Commander addJournalEntry(JournalEntry je, bool checkDuplicates = false, bool dontUpdateDialogDisplays = false)
        {
            if (!checkDuplicates || (checkDuplicates && !this.JournalEntries.Contains(je)))
            {
                if (!this.EventsList.Contains(je.Event))
                {
                    this.EventsList.Add(je.Event);
                    if (this.isViewed)
                        this.updateEventListDropdown();
                }
                je.ID = this.nextId++;
                this.JournalEntries.Add(je);
                if (this.isViewed)
                {
                    if (!dontUpdateDialogDisplays)
                        this.updateDialogDisplays();
                    MainForm m = MainForm.Instance;
                    m.eventList.InvokeIfRequired(() => m.eventList.Items.Insert(0, m.journalParser.getListViewEntryForEntry(je)));
                }
            }

            return this;
        }

        public void UpdateCurrentShip(int shipID, string shipIdent, string shipName)
        {
            if (this.Fleet.ContainsKey(shipID))
            {
                this.Fleet[shipID].ShipData.ShipID = this.ShipData.ShipID = shipIdent;
                this.Fleet[shipID].ShipData.ShipName = this.ShipData.ShipName = shipName;
                //this.updateDialogDisplays();
            }
        }

        public void UpdateShipLoadout(int shipID, string shipNoneVanityName, string shipIdent, string shipName, string modules)
        {
            CommanderShipLoadout sl = new CommanderShipLoadout();
            sl.ShipData = new CommanderShip(shipNoneVanityName, shipIdent, shipName);
            sl.LoadoutJson = modules;
            if (this.Fleet.ContainsKey(shipID))
                this.Fleet[shipID] = sl;
            else
                this.Fleet.Add(shipID, sl);
        }

        /// <summary>
        /// Do things before saving commander data here
        /// </summary>
        public void OnSave()
        {

            MainForm m = MainForm.Instance;

            this.cacheVersion = Utils.getBuildNumber();

            string messageText = "[" + this.Name + "] Saving fleet data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("fleet", this.saveDirectory, JsonConvert.SerializeObject(this.Fleet));

            messageText = "[" + this.Name + "] Saving materials data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("materials", this.saveDirectory, JsonConvert.SerializeObject(this.Materials));

            messageText = "[" + this.Name + "] Saving session history data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("sessions", this.saveDirectory, JsonConvert.SerializeObject(this.Sessions));

        }

        /// <summary>
        /// Do things after loading commander data here
        /// </summary>
        public void OnLoad()
        {

            MainForm m = MainForm.Instance;
            this.setSaveDirectory();
            if (this.cacheVersion >= 979)
            {
                string messageText = "[" + this.Name + "] Loading fleet data...";
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);

                this.Fleet = JsonConvert.DeserializeObject<Dictionary<int, CommanderShipLoadout>>(Utils.loadGZip(Path.Combine(this.saveDirectory, "fleet.emj"), true));

                messageText = "[" + this.Name + "] Loading materials data...";
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);

                this.Materials = JsonConvert.DeserializeObject<Dictionary<string, int>>(Utils.loadGZip(Path.Combine(this.saveDirectory, "materials.emj"), true));

                messageText = "[" + this.Name + "] Loading session history data...";
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);
                this.Sessions = JsonConvert.DeserializeObject<List<CommanderSession>>(Utils.loadGZip(Path.Combine(this.saveDirectory, "sessions.emj"), true));

                messageText = String.Format("Applying post-load Journal entry patched for commander '{0}'", this.Name);
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);
            }
            int patchVer = 0;

            if (this.cacheVersion < (patchVer = 680)) // Update MaterialCollected, MaterialDiscovered and MaterialDiscarded
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("MaterialDiscovered") || a.Event.Equals("MaterialCollected") || a.Event.Equals("MaterialDiscarded"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
            if (this.cacheVersion < (patchVer = 770))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("ModuleRetrieve"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 879))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("ReceiveText"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 920))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("CommunityGoalReward") || a.Event.Equals("CommunityGoalJoin"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 941))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("JoinACrew") || a.Event.Equals("QuitACrew") || a.Event.Equals("Loadout"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1009))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("Touchdown") || a.Event.Equals("Liftoff"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1026))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("SetUserShipName"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
        }

        private void updateJournalEntries(List<JournalEntry> toUpdate, MainForm m, int patchVer, Commander c)
        {
            m.journalParser.logger.Log("{0} entries need updating to version {1} for commander '{2}'", toUpdate.Count, patchVer, this.Name);

            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            int lastPercent = 0;
            int cEntry = 0;
            foreach (JournalEntry j in toUpdate)
            {
                double percent = ((double)cEntry++ / (double)toUpdate.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                {
                    TimeSpan ts = (DateTime.Now - timeStarted);
                    double timeLeft = (ts.TotalSeconds / cEntry) * (toUpdate.Count - cEntry);
                    lastETAUpdate = DateTime.Now;
                    lastPercent = (int)percent;
                    m.InvokeIfRequired(() => m.appStatus.Text = String.Format("Updating Journal entries... ({0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft)));
                }
                Commander __;
                JournalEntry nje = m.journalParser.parseEvent(j.Json, out __, true, forcedCommander: c);
                j.Data = nje.Data;
            }
            m.journalParser.logger.Log("{0} entries have been updated to version {1} for commander '{2}'", cEntry, patchVer, this.Name);
        }

        public Commander setHomeSystem(BasicSystem system)
        {
#if DEBUG
            StringBuilder text = new StringBuilder();
            text.AppendLineFormatted("Name: {0}", system.Name);
            text.AppendLineFormatted("ID: {0}", system.ID);
            text.AppendLineFormatted("Coordinates (X, Y, Z): {0}", system.Coordinates.ToString());
            MessageBox.Show(text.ToString());
#endif
            this.HasHomeSystem = true;
            this.HomeSystem = system;
            this.updateDialogDisplays();
            return this;
        }
    }
}
