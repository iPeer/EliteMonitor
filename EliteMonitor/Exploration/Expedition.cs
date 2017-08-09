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

    public class Expedition
    {

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

        public bool AutoComplete { get; set; }
        public string AutoCompleteSystemName { get; set; }

        public bool IsCompleted { get; set; } = false;

        [JsonIgnore]
        public bool IsExpeditionLoaded { get; set; } = false;

        public Expedition()
        {
            this.ExpeditionID = Guid.NewGuid();
        }

        public bool parseJournalEntry(Elite.JournalEntry je) => parseJournalEntry(je.Json);
        public bool parseJournalEntry(string entry)
        {
            JObject json = JObject.Parse(entry);
            string timestamp = json.GetValue("timestamp").ToString();
            this.TotalJournalEntries++;
            string eventName = json.GetValue("event").ToString();
            if (!(new string[] { "FSDJump", "Scan" }).Contains(eventName)) { return false; }
            switch (eventName)
            {
                case "FSDJump":
                    float jumpDist = json.GetValue("JumpDist").ToObject<float>();
                    float fuel = json.GetValue("FuelUsed").ToObject<float>();
                    string systemName = json.GetValue("StarSystem").ToString();
                    this.FuelUsed += fuel;
                    this.TotalDistance += jumpDist;
                    this.AverageJumpDistance = (this.TotalDistance / (float)this.JumpCount);
                    this.JumpCount++;
                    if (!this.SystemNames.Any(a => a.Timestamp.Equals(timestamp) && a.SystemName.Equals(systemName)))
                        this.SystemNames.Add(new ExpeditionSystemData(timestamp, systemName));
                    if (this.IsExpeditionLoaded)
                    {
                        ExpeditionViewer.Instance.updateData();
                        ExpeditionViewer.Instance.updateSystems();
                    }
                    if (this.AutoComplete && this.AutoCompleteSystemName.Equals(systemName))
                    {
                        this.IsCompleted = true;
                        MainForm.Instance.journalParser.viewedCommander.HasActiveExpedition = false;
                        return true;
                    }
                    return false;
                case "Scan":
                    string bodyType = string.Empty;
                    try
                    {
                        bool terraformable = json.GetValue("TerraformState").ToString().Equals("Terraformable");
                        bodyType = (terraformable ? "Terraformable " : "") + json.GetValue("PlanetClass").ToString().Replace("Sudarsky class", "Class");

                    }
                    catch { bodyType = string.Format("Class {0} star", json.GetValue("StarType").ToString()); }
                    BodyScanCount++;
                    if (ScanCounts.ContainsKey(bodyType))
                        ScanCounts[bodyType]++;
                    else
                        ScanCounts.Add(bodyType, 1);
                    if (this.IsExpeditionLoaded)
                        ExpeditionViewer.Instance.updateScans();
                    return false;
            }
            return true;
        }

    }
}
