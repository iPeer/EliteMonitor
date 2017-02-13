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

namespace EliteMonitor.Elite
{
    public class Commander
    {

        //[JsonIgnore]
        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        public List<string> EventsList { get; set; } = new List<string>();
        [JsonIgnore]
        [Obsolete("No longer needed", true)]
        public List<string> Journal { get; set; } = new List<string>();
        public Dictionary<string, int> Materials { get; set; } = new Dictionary<string, int>();
        public int cacheVersion = Utils.getBuildNumber();
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
        public string Ship { get; set; }
        [JsonIgnore] // We don't need to save this because it's kind of irrelevant
        public string PrivateGroup { get; set; } = "";

        public int combatRank { get; set; }
        public int tradeRank { get; set; }
        public int explorationRank { get; set; }
        public int cqcRank { get; set; }
        public int federationRank { get; set; }
        public int imperialRank { get; set; }

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
        }

        public Commander setBasicInfo(string ship, long credits, string privateGroup)
        {
            this.Ship = ship;
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

        public void saveData()
        {
            this.OnSave();
            string commanderSavePath = Path.Combine(MainForm.Instance.cacheController.cachePath, $"journal_{this.Name}.emc");
            using (StreamWriter sw = new StreamWriter(commanderSavePath, false, Encoding.UTF8))
            {
                sw.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public Commander deductCredits(long credits)
        {
            this.Credits -= credits;
            return this;
        }

        public Commander addCredits(long credits)
        {
            this.Credits += credits;
            return this;
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
                if (rank > this.explorationRank)
                {
                    this.explorationRank = rank;
                    this.explorationProgress = 0;
                }
            }
            return this;
        }

        public void updateDialogDisplays(MainForm m)
        {
            // Commander data

            m.commanderLabel.InvokeIfRequired(() => m.commanderLabel.Text = String.Format("{0} | {1} {2}", this.Name, this.Ship, !this.PrivateGroup.Equals(string.Empty) && this.PrivateGroup != null ? $"/ {this.PrivateGroup}" : ""));

            // Credits

            m.creditsLabel.InvokeIfRequired(() => m.creditsLabel.Text = string.Format("{0:n0} credits", this.Credits));

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

            m.combatRankName.InvokeIfRequired(() => m.combatRankName.Text = combatRank);
            m.tradeRankName.InvokeIfRequired(() => m.tradeRankName.Text = tradeRank);
            m.exploreRankName.InvokeIfRequired(() => m.exploreRankName.Text  = explorationRank);
            m.cqcRankName.InvokeIfRequired(() => m.cqcRankName.Text = cqcRank);
            m.fedRankName.InvokeIfRequired(() => m.fedRankName.Text = federationRank);
            m.empireRankName.InvokeIfRequired(() => m.empireRankName.Text = imperialRank);

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

        public Commander addJournalEntry(JournalEntry je, bool checkDuplicates = false)
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
                    this.updateDialogDisplays();
                    MainForm m = MainForm.Instance;
                    m.eventList.InvokeIfRequired(() => m.eventList.Items.Insert(0, m.journalParser.getListViewEntryForEntry(je)));
                }
            }

            return this;
        }
        /// <summary>
        /// Do things before saving commander data here
        /// </summary>
        public void OnSave()
        {
            this.cacheVersion = Utils.getBuildNumber();
        }

        /// <summary>
        /// Do things after loading commander data here
        /// </summary>
        public void OnLoad()
        {

        }
    }
}
