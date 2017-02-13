using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Logging;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Caching
{
    public class CacheController
    {

        public string cachePath { get; private set; }
        public string journalLengthCache { get; private set; }
        [Obsolete]
        public string journalEntryCache { get; private set; }
        [Obsolete]
        public string materialCache { get; private set; }
        public string commandersPath { get; private set; }
        public Logger logger;
        public Dictionary<string, long> _journalLengthCache = new Dictionary<string, long>();

        public List<string> _journalCache = new List<string>();
        public Dictionary<string, string> commanderCaches = new Dictionary<string, string>();
        public Commander switchOnLoad;

        private MainForm mainForm;

        public CacheController(MainForm main)
        {
            this.mainForm = main;
            this.logger = new Logging.Logger("CacheController");
            this.cachePath = Path.Combine(Utils.getApplicationEXEFolderPath(), "cache");
            this.journalLengthCache = Path.Combine(this.cachePath, "journal.emc");
            //this.journalEntryCache = Path.Combine(this.cachePath, "journal_entries.emc");
            //this.materialCache = Path.Combine(this.cachePath, "material.emc");
            this.commandersPath = Path.Combine(this.cachePath, "commanders.emc");
            if (!Directory.Exists(this.cachePath))
                Directory.CreateDirectory(this.cachePath);
        }

        public bool cacheExists()
        {
            return File.Exists(this.commandersPath) && File.Exists(this.journalLengthCache);
        }

        public void addJournalEntryToCache(string journalEntry)
        {
            if (!_journalCache.Contains(journalEntry))
                _journalCache.Add(journalEntry);
        }

        internal void verifyFileLengths()
        {
            // TODO: Finish
            this.logger.Log("Verifying Journal file lengths...");
            string journalPath = EliteUtils.JOURNAL_PATH;
            journalPath = Environment.ExpandEnvironmentVariables(journalPath);
            DirectoryInfo di = new DirectoryInfo(journalPath);
            FileInfo[] fileInfo = di.GetFiles().OrderBy(f => f.CreationTime).ToArray();
            int failed = 0;
            List<FileInfo> failedFiles = new List<FileInfo>();
            foreach (FileInfo fi in fileInfo)
            {
                if (!_journalLengthCache.ContainsKey(fi.Name) || fi.Length > _journalLengthCache[fi.Name])
                {
                    failed++;
                    failedFiles.Add(fi);
                }
            }
            this.logger.Log("{0} file(s) failed verification.", failed);
            if (failed > 0)
            {
                List<string> newEntries = new List<string>();
                foreach(FileInfo f in failedFiles)
                {
                    this.logger.Log("Verifying file '{0}'...", f.FullName);
                    using (StreamReader sr = new StreamReader(f.FullName, Encoding.UTF8))
                    {
                        string l = "";
                        while ((l = sr.ReadLine()) != null)
                        {
                            newEntries.Add(l);
                        }
                    }
                    if (this._journalLengthCache.ContainsKey(f.Name))
                        this._journalLengthCache[f.Name] = f.Length;
                    else
                        this._journalLengthCache.Add(f.Name, f.Length);
                }
                this.logger.Log("Creating up to {0} new entries from files that failed verification", newEntries.Count);
                mainForm.journalParser.createJournalEntries(newEntries, true);
                this.saveAllCaches();
            }
        }

        /*public void saveJournalCache()
        {
            if (File.Exists(this.journalCache))
                File.Delete(this.journalCache);
            if (File.Exists(this.journalEntryCache))
                File.Delete(this.journalEntryCache);
            if (File.Exists(this.materialCache))
                File.Delete(this.materialCache);

            using (StreamWriter sw = new StreamWriter(this.journalCache))
            {
                sw.WriteLine(JsonConvert.SerializeObject(this.mainForm.journalParser._lastJournalBytes, Formatting.Indented));
            }
            using (StreamWriter sw = new StreamWriter(this.journalEntryCache))
            {
                sw.WriteLine(JsonConvert.SerializeObject(_journalCache, Formatting.Indented));
            }
            using (StreamWriter sw = new StreamWriter(this.materialCache))
            {
                sw.WriteLine(JsonConvert.SerializeObject(this.mainForm.journalParser._materialCounts, Formatting.Indented));
            }

        }*/
        public void saveAllCaches()
        {
            string journalPath = Path.Combine(this.cachePath, "commanders.emc");
            Dictionary<string, string> commanderData = new Dictionary<string, string>();
            foreach (Commander c in mainForm.journalParser.commanders.Values)
            {
                //Console.WriteLine($"COMMANDER: {c.Name} | journal_{c.Name}.emc");
                commanderData.Add(c.Name, $"journal_{c.Name}.emc");
                c.saveData();
            }
            using (StreamWriter sw = new StreamWriter(journalPath, false, Encoding.UTF8))
            {
                sw.WriteLine(JsonConvert.SerializeObject(commanderData, Formatting.Indented));
            }
            using (StreamWriter sw = new StreamWriter(this.journalLengthCache, false, Encoding.UTF8))
            {
                sw.WriteLine(JsonConvert.SerializeObject(this._journalLengthCache, Formatting.Indented));
            }
        }

        public Commander loadCommanderFromFile(string filePath)
        {
            this.logger.Log("Loading commander data for commander from {0}", filePath);
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                Commander c = JsonConvert.DeserializeObject<Commander>(sr.ReadToEnd());
                logger.Log("Loaded Commander data for commander '{0}' - Entries: {1:n0}", LogLevel.DEBUG, c.Name, c.JournalEntries.Count);
                c.OnLoad();
                return c;
            }
        }

        public bool loadCaches()
        {

            // TODO update caching for new save methods
            this.logger.Log("Loading commander cache list...");
            this.commanderCaches.Clear();
            using (StreamReader sr = new StreamReader(this.commandersPath, Encoding.UTF8))
            {
                this.commanderCaches = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }

            this.logger.Log("Loading commander data...");
            foreach (KeyValuePair<string, string> kvp in this.commanderCaches)
            {
                string commanderPath = Path.Combine(this.cachePath, kvp.Value);
                if (!File.Exists(commanderPath))
                {
                    this.logger.Log("Commander cache file '{0}' doesn't exist. Treating caches as invalid.", LogLevel.ERR, commanderPath);
                    return false;
                }
                Commander c = loadCommanderFromFile(commanderPath);
                this.logger.Log("Checking for unknown events and updating them where necessary...");
                int updated = 0;
                List<JournalEntry> needsUpdating = c.JournalEntries.FindAll(j => !j.isKnown);
                foreach (JournalEntry je in needsUpdating)
                {
                    Commander __ = c;
                    JournalEntry nje = mainForm.journalParser.parseEvent(je.Json, out __);
                    if (nje.isKnown)
                    {
                        updated++;
                        if (je.Notes.Equals("UNKNOWN EVENT"))
                            je.Notes = null;
                        je.Data = nje.Data;
                        je.isKnown = true;
                        /*List<JournalEntry> tmp = c.JournalEntries;
                        tmp.Reverse();
                        int listIndex = tmp.IndexOf(je);
                        mainForm.eventList.InvokeIfRequired(() =>
                        {
                            ListViewItem lvi = mainForm.eventList.Items[listIndex];
                            lvi.SubItems[2].Text = je.Data;
                        });*/
                    }
                }
                this.logger.Log("{0}/{1} entries have been updated.", updated, needsUpdating.Count);
                this.logger.Log("{0}: {1}", LogLevel.DEBUG, c.Name, c.isViewed);
                if (c.isViewed)
                    this.switchOnLoad = c;
                mainForm.journalParser.commanders.Add(c.Name, c);
                if (!mainForm.comboCommanderList.Items.Contains(c.Name))
                    mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.Items.Add(c.Name));
            }
            if (switchOnLoad == null)
            {
                switchOnLoad = mainForm.journalParser.commanders.Values.First();
            }
            this.logger.Log("Loading Journal length data...");
            this._journalLengthCache.Clear();
            using (StreamReader sr = new StreamReader(this.journalLengthCache, Encoding.UTF8))
            {
                this._journalLengthCache = JsonConvert.DeserializeObject<Dictionary<string, long>>(sr.ReadToEnd());
            }

            return true;
        }

        internal void clearCaches()
        {
            string[] files = Directory.GetFiles(this.cachePath);
            foreach (string f in files)
                File.Delete(f);
            mainForm.journalParser.commanders.Clear();
            mainForm.eventList.Items.Clear();
        }
    }
}
