﻿using EliteMonitor.Elite;
using EliteMonitor.Logging;
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

        public static int ExpeditionVersion = 2;

        public int Version { get; set; } = 1;
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

        public void OnSave()
        {
            this.Version = ExpeditionVersion;
        }

        public void OnLoad()
        {
            if (this.JumpCount > this.SystemNames.Count)
            {
                this.JumpCount = this.SystemNames.Count;
            }

            int version = 1;
            if (this.Version < (version = 2))
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
            }
            if (this.IsExpeditionLoaded)
            {
                ExpeditionViewer.Instance.updateData();
                ExpeditionViewer.Instance.updateScans();
                ExpeditionViewer.Instance.updateSystems();
            }
        }
    }
}