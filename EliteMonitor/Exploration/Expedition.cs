using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Logging;
using EliteMonitor.Notifications;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Exploration
{

    public class ExpeditionSystemData
    {
        public string Timestamp { get; set; }
        public string SystemName { get; set; }

        public ExpeditionSystemData(string timestamp, string systemname)
        {
            this.Timestamp = timestamp;
            this.SystemName = systemname;
        }
    }

    public class Expedition : ISavable
    {

        public static int ExpeditionVersion = 5;

        public int Version { get; set; } = ExpeditionVersion;
        public Guid ExpeditionID { get; set; }
        public string ExpeditionName { get; set; } = "New Expedition";

        public List<ExpeditionSystemData> SystemNames = new List<ExpeditionSystemData>();
        public Int64 ExpeditionStartingJournalEntryId { get; set; }
        public Int64 JumpCount { get; set; }
        public float FuelUsed { get; set; }
        public float TotalDistance { get; set; }
        public float AverageJumpDistance { get; set; }
        public Dictionary<string, long> ScanCounts = new Dictionary<string, long>();
        public Int64 BodyScanCount { get; set; }
        public Int64 TotalJournalEntries { get; set; }
        public Int64 LastJournalEntryId { get; set; } = 0;

        public bool AutoComplete { get; set; }
        public string AutoCompleteSystemName { get; set; }
        public string StartingSystemName { get; set; }

        public bool IsCompleted { get; set; } = false;

        [JsonIgnore]
        public bool IsExpeditionLoaded { get; set; } = false;

        public Expedition()
        {
            this.ExpeditionID = Guid.NewGuid();
        }

        //public bool parseJournalEntry(Elite.JournalEntry je) => parseJournalEntry(je.Json);
        public bool parseJournalEntry(JournalEntry entry)
        {
            JObject json = JObject.Parse(entry.Json);
            this.TotalJournalEntries++;
            string eventName = json.GetValue("event").ToString();
            this.LastJournalEntryId = entry.ID;
            if (!(new string[] { "FSDJump", "Scan" }).Contains(eventName)) { return false; }
            string timestamp = json.GetValue("timestamp").ToString();
            switch (eventName)
            {
                case "FSDJump":
                    float jumpDist = json.GetValue("JumpDist").ToObject<float>();
                    float fuel = json.GetValue("FuelUsed").ToObject<float>();
                    string systemName = json.GetValue("StarSystem").ToString();
                    this.FuelUsed += fuel;
                    this.TotalDistance += jumpDist;
                    this.AverageJumpDistance = (this.TotalDistance / (float)this.JumpCount);
                    if (!this.SystemNames.Any(a => a.Timestamp.Equals(timestamp) && a.SystemName.Equals(systemName)))
                    {
                        this.SystemNames.Add(new ExpeditionSystemData(timestamp, systemName));
                        this.JumpCount++;
                    }
                    if (this.IsExpeditionLoaded)
                    {
                        ExpeditionViewer.Instance.updateData();
                        ExpeditionViewer.Instance.updateSystems();
                    }
                    if (this.AutoComplete && this.AutoCompleteSystemName.Equals(systemName))
                    {
                        //this.IsCompleted = true;
                        //(MainForm.Instance.journalParser.viewedCommander == null ? MainForm.Instance.journalParser.activeCommander : MainForm.Instance.journalParser.viewedCommander).HasActiveExpedition = false;
                        this.MarkCompleted();
                        return true;
                    }
                    return false;
                case "Scan":
                    string bodyType = string.Empty;
                    string bodyName = json.GetValue("BodyName").ToString();
                    if (bodyName.ContainsIgnoreCase("Belt Cluster")) return false;
                    try
                    {
                        bool terraformable = json.GetValue("TerraformState").ToString().Equals("Terraformable");
                        bodyType = (terraformable ? "Terraformable " : "") + json.GetValue("PlanetClass").ToString().Replace("Sudarsky class", "Class");

                    }
                    catch { bodyType = MainForm.Instance.Database.GetCorrectStarClassName(json.GetValue("StarType").ToString()); }
                    BodyScanCount++;
                    if (ScanCounts.ContainsKey(bodyType))
                        ScanCounts[bodyType]++;
                    else
                        ScanCounts.Add(bodyType, 1);
                    if (this.IsExpeditionLoaded)
                    {
                        ExpeditionViewer.Instance.updateScans();
                        ExpeditionViewer.Instance.updateData();
                    }
                    return false;
            }
            return true;
        }

        public void MarkCompleted()
        {
            this.IsCompleted = true;
            if (Properties.Settings.Default.NotificationsEnabled)
            {
                Notification n = new Notification("Expedition Completed", $"The expedition '{this.ExpeditionName}' is now complete!");
                Utils.InvokeNotification(n);
            }
        }

        public void OnSave()
        {
            this.Version = ExpeditionVersion;
        }

        public void OnLoad()
        {
            if (this.JumpCount > this.SystemNames.Count - 1)
            {
                this.JumpCount = this.SystemNames.Count - 1;
            }

            if (this.Version < 2 || this.Version < 4) // Update star class names
            {
                Logger l = MainForm.Instance.journalParser.logger.createSubLogger("Expedition");
                foreach (KeyValuePair<string, long> kvp in new Dictionary<string, long>(this.ScanCounts))
                {
                    if (!kvp.Key.Contains("star")) continue;
                    string comp = MainForm.Instance.Database.GetCorrectStarClassName(kvp.Key.Split(' ')[1]);
                    if (!comp.Equals(kvp.Key))
                    {
                        l.Log("Updating entry '{0}' in Expedition '{1}' [{2}] to '{3}'", kvp.Key, this.ExpeditionName, this.ExpeditionID.ToString(), comp);
                        this.ScanCounts.Add(comp, kvp.Value);
                        this.ScanCounts.Remove(kvp.Key);
                    }
                }
                MainForm.Instance.journalParser.viewedCommander.NeedsSaving = true;
            }

            if (this.Version < 3) // Insert expedition start point
            {
                JournalEntry je = MainForm.Instance.journalParser.viewedCommander.JournalEntries.Find(a => a.ID == this.ExpeditionStartingJournalEntryId);
                if (je != null)
                {
                    JObject json = JObject.Parse(je.Json);
                    string ts = json.GetValue("timestamp").ToString();
                    string ss = json.GetValue("StarSystem").ToString();
                    //if (!this.SystemNames[0].SystemName.Equals(ss) && this.SystemNames[0].Timestamp.Equals(ts))
                    //{
                        ExpeditionSystemData esd = new ExpeditionSystemData(ts, ss);
                        this.SystemNames.Insert(0, esd);
                        MainForm.Instance.journalParser.viewedCommander.NeedsSaving = true;
                    //}
                }
            }
            if (this.Version < 5)
            {
                this.StartingSystemName = this.AutoCompleteSystemName;
                MainForm.Instance.journalParser.viewedCommander.NeedsSaving = true;
            }

            this.Version = ExpeditionVersion;

            if (this.IsExpeditionLoaded)
            {
                ExpeditionViewer.Instance.updateData();
                ExpeditionViewer.Instance.updateScans();
                ExpeditionViewer.Instance.updateSystems();
            }
        }
    }
}
