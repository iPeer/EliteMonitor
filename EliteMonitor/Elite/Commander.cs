using EliteMonitor.Extensions;
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
using EliteMonitor.Exploration;
using EliteMonitor.Journal;

namespace EliteMonitor.Elite
{

    public class CommanderSession { } // TODO

    public class BodyDiscovery
    {

        public string BodyName { get; set; }
        public DateTime DiscoveredTime { get; set; } = DateTime.UtcNow;

        public BodyDiscovery() { }

        public BodyDiscovery(string bodyName, string gameTimestamp)
        {
            this.BodyName = bodyName;
            this.DiscoveredTime = DateTime.Parse(gameTimestamp, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
    }

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

    public class Commander : ISavable
    {

        //[JsonIgnore]
        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        public List<string> EventsList { get; set; } = new List<string>();
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
        [JsonIgnore]
        public List<BodyDiscovery> DiscoveredBodies = new List<BodyDiscovery>();
        [JsonIgnore]
        public Dictionary<Guid, Expedition> Expeditions = new Dictionary<Guid, Expedition>();
        [JsonIgnore]
        public bool HasActiveExpedition {
            get
            {
                return this.Expeditions.Values.Any(a => !a.IsCompleted);
            }
        }
        [JsonIgnore]
        public Guid ActiveExpeditionGuid { get; set; }
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
        [JsonIgnore]
        public bool NeedsSaving { get; set; } = false;
        public string Name { get; set; }
        public long Credits { get; set; }
        public CommanderShip ShipData { get; set; }
        public string Ship { get; set; }
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
        public long RescuedThargoidRefugees { get; set; } = 0L;

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

            this.saveDirectory = Path.Combine(MainForm.Instance.cacheController.commanderDataPath, this.Name);
            //this.MarkDirty();
            return this;
        }

        public Commander SetShip(string nV, string sI, string sN)
        {
            this.ShipData = new CommanderShip(nV, sI, sN);
            //this.MarkDirty();
            return this;
        }

        public Commander SetShip(CommanderShip ship)
        {
            this.ShipData = ship;
            //this.MarkDirty();
            return this;
        }

        public Commander setBasicInfo(string ship, long credits, string privateGroup)
        {
            this.ShipData = new CommanderShip(ship);
            this.Credits = credits;
            this.PrivateGroup = privateGroup;
            //this.MarkDirty();
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
            //this.MarkDirty();
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
            //this.MarkDirty();
            return this;
        }

        public Commander registerFirstDiscovery(string bodyName, string gameTimestamp)
        {
            if (this.DiscoveredBodies == null)
                this.DiscoveredBodies = new List<BodyDiscovery>();
            if (!this.DiscoveredBodies.Any(b => b.BodyName.Equals(bodyName))) {
                this.DiscoveredBodies.Add(new BodyDiscovery(bodyName, gameTimestamp));
            }
            this.MarkDirty();
            return this;
        }

        public long saveData(bool uncompressed = false)
        {
            this.OnSave();

#if DEBUG
            Utils.saveDataFile("journal", this.saveDirectory, JsonConvert.SerializeObject(this, Formatting.Indented), compressed: false);
#endif

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
            this.MarkDirty();
            return this;
        }

        public Commander addCredits(long credits)
        {
            this.Credits += credits;
            this.lastCreditsChange = credits;
            this.MarkDirty();
            return this;
        }

        public DirectoryInfo CreateSaveDirectory()
        {
            string mainCacheDir = MainForm.Instance.cacheController.commanderDataPath;
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
            this.MarkDirty();
            return this;
        }

        public Commander RemoveMaterials(List<JournalMaterialsEventKVP> materials)
        {
            foreach (JournalMaterialsEventKVP kvp in materials)
            {
                if (this.Materials.ContainsKey(kvp.Name)) 
                    this.Materials[kvp.Name] -= kvp.Count;
            }
            this.MarkDirty();
            return this;
        }

        public Commander addMaterial(string material, int count)
        {
            if (this.Materials.ContainsKey(material))
                this.Materials[material] += count;
            else
                this.Materials.Add(material, count);
            this.MarkDirty();
            return this;
        }

        /*[Obsolete]
        public Commander addEvent(string @event)
        {
            if (!this.Journal.Contains(@event))
                this.Journal.Add(@event);
            return this;
        }*/

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
                    this.combatProgress = (rank == 8 ? 100 : 0);
                }
            }
            else if (type.Equals("trade"))
            {
                if (rank > this.tradeRank)
                {
                    this.tradeRank = rank;
                    this.tradeProgress = (rank == 8 ? 100 : 0);
                }
            }
            else if (type.Equals("explore"))
            {
                if (rank > this.explorationRank)
                {
                    this.explorationRank = rank;
                    this.explorationProgress = (rank == 8 ? 100 : 0);
                }
            }
            else if (type.Equals("cqc"))
            {
                if (rank > this.cqcRank)
                {
                    this.cqcRank = rank;
                    this.cqcProgress = (rank == 8 ? 100 : 0);
                }
            }
            else if (type.Equals("federation"))
            {
                if (rank > this.federationRank)
                {
                    this.federationRank = rank;
                    this.federationProgress = (rank == 14 ? 100 : 0);
                }
            }
            else if (type.Equals("empire"))
            {
                if (rank > this.imperialRank)
                {
                    this.imperialRank = rank;
                    this.imperialProgress = (rank == 14 ? 100 : 0);
                }
            }
            this.MarkDirty();
            return this;
        }

        public void updateDialogDisplays(MainForm m)
        {

            // Home system tooltip

            m.InvokeIfRequired(() =>
            {
                //m.homeSystemTooltip.SetToolTip(m.commanderLabel, "Click to set or change home system.");
                m.homeSystemTooltip.SetToolTip(m.commanderLocationLabel, /*"Click to set or change home system."*/Utils.GetLandmarkDistancesString());
            });


            // Commander data

            m.commanderLabel.InvokeIfRequired(() => {
                /*string format = "{0} {1} | {2}";
                if (isInMulticrew)
                    format = "{0} {1} | {2}";
                if (string.IsNullOrEmpty(this.CurrentLocation) && string.IsNullOrEmpty(this.CurrentSystem))
                    format = "{0} {1} | {2}";*/

                m.commanderLabel.Text = string.Format("{0}{1} | {2}", this.Name, (this.isInMulticrew ? string.Format(" ({0})", this.MultiCrewCommanderName) : (string.IsNullOrEmpty(this.PrivateGroup) ? "" : string.Format(" ({0})", this.PrivateGroup))), (this.ShipData != null ? this.ShipData.getFormattedShipString() : m.Database.getShipNameFromInternalName(this.Ship)));
                if (!(string.IsNullOrEmpty(this.CurrentLocation) && string.IsNullOrEmpty(this.CurrentSystem)))
                {
                    //m.commanderLocationLabel.Text = string.Format("{0}{1}{2}", this.isDocked || this.isLanded ? this.isDocked ? "Docked at " : "Landed on " : "", String.Format("{0} | ", this.CurrentLocation), this.CurrentSystem);
                    string cmdrLocString = string.Empty;
                    if (this.isLanded || this.isDocked)
                        cmdrLocString = string.Format("{0}", this.isLanded ? "Landed on " : "Docked at ");
                    if (!string.IsNullOrEmpty(this.CurrentLocation))
                        cmdrLocString += this.CurrentLocation+" | ";
                    cmdrLocString += this.CurrentSystem;
                    if (m.Database.ThargoidStationLocations.Contains(this.CurrentSystem))
                        cmdrLocString += string.Format(" [{0:n0} refugees saved]", this.RescuedThargoidRefugees);
                    m.commanderLocationLabel.Text = cmdrLocString; 
                    if (this.HasHomeSystem && this.CurrentSystemCoordinates != null && !this.CurrentSystem.Equals(this.HomeSystem.Name))
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

        }

        public Commander addJournalEntry(JournalEntry je, bool checkDuplicates = false, bool dontUpdateDialogDisplays = false)
        {
            if (!checkDuplicates || (checkDuplicates && !this.JournalEntries.Contains(je)))
            {
                if (!this.EventsList.Contains(je.Event))
                {
                    this.EventsList.Add(je.Event);
                }
                je.ID = this.nextId++;
                this.JournalEntries.Add(je);
                if (this.isViewed)
                {
                    if (!dontUpdateDialogDisplays)
                        this.updateDialogDisplays();
                    MainForm m = MainForm.Instance;
                    if (!dontUpdateDialogDisplays)
                    {
                        m.eventList.InvokeIfRequired(() =>
                        {
                            m.eventList.BeginUpdate();
                            m.eventList.Rows.Insert(0, m.journalParser.getListViewEntryForEntry(je));
                            m.eventList.EndUpdate();
                        });
                    }
                }
                this.MarkDirty();
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
                this.MarkDirty();
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
            this.MarkDirty();
        }

        /// <summary>
        /// Do things before saving commander data here
        /// </summary>
        public void OnSave()
        {
            this.NeedsSaving = false;

            MainForm m = MainForm.Instance;

            this.cacheVersion = Utils.getBuildNumber();

            string messageText = "[" + this.Name + "] Saving fleet data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("fleet", this.saveDirectory, JsonConvert.SerializeObject(this.Fleet));
#if DEBUG
            Utils.saveDataFile("fleet", this.saveDirectory, JsonConvert.SerializeObject(this.Fleet, Formatting.Indented), compressed: false);
#endif

            messageText = "[" + this.Name + "] Saving materials data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("materials", this.saveDirectory, JsonConvert.SerializeObject(this.Materials));
#if DEBUG
            Utils.saveDataFile("materials", this.saveDirectory, JsonConvert.SerializeObject(this.Materials, Formatting.Indented), compressed: false);
#endif

            messageText = "[" + this.Name + "] Saving session history data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("sessions", this.saveDirectory, JsonConvert.SerializeObject(this.Sessions));

#if DEBUG
            Utils.saveDataFile("sessions", this.saveDirectory, JsonConvert.SerializeObject(this.Sessions, Formatting.Indented), compressed: false);
#endif

            messageText = "[" + this.Name + "] Saving discoveries data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("discoveries", this.saveDirectory, JsonConvert.SerializeObject(this.DiscoveredBodies));

#if DEBUG
            Utils.saveDataFile("discoveries", this.saveDirectory, JsonConvert.SerializeObject(this.DiscoveredBodies, Formatting.Indented), compressed: false);
#endif

            messageText = "[" + this.Name + "] Saving expedition data...";
            m.InvokeIfRequired(() => m.appStatus.Text = messageText);
            m.journalParser.logger.Log(messageText);

            Utils.saveGZip("expeditions", this.saveDirectory, JsonConvert.SerializeObject(this.Expeditions));
#if DEBUG
            Utils.saveDataFile("expeditions", this.saveDirectory, JsonConvert.SerializeObject(this.Expeditions, Formatting.Indented), compressed: false);
#endif

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

                messageText = "[" + this.Name + "] Loading discoveries data...";
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);
                this.DiscoveredBodies = JsonConvert.DeserializeObject<List<BodyDiscovery>>(Utils.loadGZip(Path.Combine(this.saveDirectory, "discoveries.emj"), true));

                messageText = "[" + this.Name + "] Loading expedition data...";
                m.InvokeIfRequired(() => m.appStatus.Text = messageText);
                m.journalParser.logger.Log(messageText);
                this.Expeditions = JsonConvert.DeserializeObject<Dictionary<Guid, Expedition>>(Utils.loadGZip(Path.Combine(this.saveDirectory, "expeditions.emj"), true));

                messageText = String.Format("Applying post-load Journal entry patches for commander '{0}'", this.Name);
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
            if (this.cacheVersion < (patchVer = 1034))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("SellExplorationData"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
            if (this.cacheVersion < (patchVer = 1327))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("StartJump") || a.Event.Equals("FSDJump"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
            if (this.cacheVersion < (patchVer = 1426))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("Repair") || a.Event.Equals("DockSRV") || a.Event.Equals("LaunchSRV") || a.Event.Equals("DatalinkVoucher") || a.Event.Equals("DatalinkScan"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
            if (this.cacheVersion < (patchVer = 1720))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("NavBeaconScan") || a.Event.Equals("Bounty") || a.Event.Equals("MaterialCollected") || a.Event.Equals("MaterialDiscovered") || a.Event.Equals("MaterialDiscarded") || a.Event.Equals("MarketSell") || a.Event.Equals("MarketBuy") || a.Event.Equals("Materials") || a.Event.Equals("EngineerProgress") || a.Event.Equals("EngineerCraft") || a.Event.Equals("EngineerApply"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }
            if (this.cacheVersion < (patchVer = 1783))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("ReceiveText") || a.Event.Equals("CommunityGoal"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1796))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => !a.isKnown || a.Event.Equals("HullDamage") || a.Event.Equals("CommunityGoal"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1803))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("HullDamage"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1833))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => a.Event.Equals("MissionAccepted"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1855))
            {
                List<JournalEntry> toUpdate = this.JournalEntries.FindAll(a => /*a.Event.Equals("ShipyardNew") || a.Event.Equals("ShipyardBuy") || */a.Event.Equals("ShipyardSwap"));
                updateJournalEntries(toUpdate, m, patchVer, this);
            }

            if (this.cacheVersion < (patchVer = 1881)) // Fix non-vanity ship names using internal name
            {
                string fixed_shipname = MainForm.Instance.Database.getShipNameFromInternalName(this.ShipData.ShipNonVanityName);
                if (this.ShipData.ShipName.Equals(this.ShipData.ShipNonVanityName)) this.ShipData.ShipName = fixed_shipname;
                this.ShipData.ShipNonVanityName = fixed_shipname;
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
                try
                {
                    JournalEntry nje = m.journalParser.parseEvent(j.Json, out __, true, forcedCommander: c, bypassRegisterCheck: true, showNotifications: false, doNotPlaySounds: true);
                    if (j.Data.Equals(j.Json))
                        j.Data = string.Empty;
                    j.Data = nje.Data;
                }
                catch (InvalidJSONException) { continue; }
            }
            if (cEntry > 0)
                this.MarkDirty();
            m.journalParser.logger.Log("{0} entries have been updated to version {1} for commander '{2}'", cEntry, patchVer, this.Name);
            toUpdate.Clear();
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
            this.MarkDirty();
            return this;
        }

        public Commander SetMaterials(Dictionary<string, int> materials)
        {
            this.MarkDirty();
            this.Materials = new Dictionary<string, int>(materials);
            return this;
        }

        public void setSaveRequired() => this.MarkDirty();
        public void MarkDirty()
        {
            this.NeedsSaving = true;
        }

        public List<Expedition> getActiveExpeditions()
        {
            if (this.Expeditions == null)
                return new List<Expedition>();
            return this.Expeditions.Where(a => !a.Value.IsCompleted).Select(b => b.Value).ToList();
        }
    }
}
