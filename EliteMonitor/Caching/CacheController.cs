using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Logging;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace EliteMonitor.Caching
{
    public class CacheController
    {
        /// <summary>
        /// The path for the directory in which caches are saved.
        /// </summary>
        public string commanderDataPath { get; private set; }
        [Obsolete]
        public string cachePath { get; private set; }
        /// <summary>
        /// The path for the directory in which HUD presets are saved
        /// </summary>
        public string hudPresetPath { get; private set; }
        public string journalLengthCache { get; private set; }
        [Obsolete]
        public string journalEntryCache { get; private set; }
        [Obsolete]
        public string materialCache { get; private set; }
        public string commandersPath { get; private set; }
        public string dataPath { get; private set; }
        public string bodyDataCache { get; private set; }
        public string systemDataCache { get; private set; }
        public string rawBodyDataCache { get; private set; }
        public string rawSystemDataCache { get; private set; }
        public string soundsPath { get; private set; }
        public Logger logger;
        public Dictionary<string, long> _journalLengthCache = new Dictionary<string, long>();

        public List<string> _journalCache = new List<string>();
        public Dictionary<string, Tuple<string, long>> commanderCaches = new Dictionary<string, Tuple<string, long>>();
        public Commander switchOnLoad;

        private MainForm mainForm;

        public CacheController(MainForm main)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            this.mainForm = main;
            this.logger = new Logging.Logger("CacheController");
            this.commanderDataPath = Path.Combine(Utils.getApplicationEXEFolderPath(), "Commander_Data");
            this.cachePath = Path.Combine(Utils.getApplicationEXEFolderPath(), "cache");
            this.dataPath = Path.Combine(Utils.getApplicationEXEFolderPath(), "data");
            this.hudPresetPath = Path.Combine(Utils.getApplicationEXEFolderPath(), "huds");
            this.soundsPath = Path.Combine(Utils.getApplicationEXEFolderPath(), "sounds");
            this.journalLengthCache = Path.Combine(this.commanderDataPath, "journal.emc");
            this.bodyDataCache = Path.Combine(this.dataPath, "bodies.sqlite");
            this.systemDataCache = Path.Combine(this.dataPath, "systems.sqlite");
            this.rawSystemDataCache = Path.Combine(this.dataPath, "raw_systems.csv");
            this.rawBodyDataCache = Path.Combine(this.dataPath, "raw_bodies.jsonl");
            //this.journalEntryCache = Path.Combine(this.cachePath, "journal_entries.emc");
            //this.materialCache = Path.Combine(this.cachePath, "material.emc");
            this.commandersPath = Path.Combine(this.commanderDataPath, "commanders.emc");
            if (Directory.Exists(this.cachePath))
            {
                Directory.Move(this.cachePath, this.commanderDataPath);
            }
#pragma warning restore CS0612 // Type or member is obsolete
            if (!Directory.Exists(this.commanderDataPath))
                Directory.CreateDirectory(this.commanderDataPath);
            if (!Directory.Exists(this.dataPath))
                Directory.CreateDirectory(this.dataPath);
            if (!Directory.Exists(this.hudPresetPath))
                Directory.CreateDirectory(this.hudPresetPath);
        }

        public bool cacheExists()
        {
            return /*File.Exists(this.commandersPath) && */File.Exists(this.journalLengthCache);
        }

        public void addJournalEntryToCache(string journalEntry)
        {
            if (!_journalCache.Contains(journalEntry))
                _journalCache.Add(journalEntry);
        }

        internal void verifyFileLengths()
        {
            Dictionary<string, long> newJournalLengthCache = new Dictionary<string, long>();
            try
            {
                this.logger.Log("Verifying Journal file lengths...");
                mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Verifying Journal file lengths...");
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
                    foreach (FileInfo f in failedFiles)
                    {
                        this.logger.Log("Verifying file '{0}'...", f.FullName);
                        using (FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // Fix for IO error (file in use) when updating cache while the game is running
                        {
                            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                            {
                                string l = "";
                                while ((l = sr.ReadLine()) != null)
                                {
                                    newEntries.Add(l);
                                }
                            }
                        }
                        newJournalLengthCache.Add(f.Name, f.Length);
                        /*if (this._journalLengthCache.ContainsKey(f.Name))
                            this._journalLengthCache[f.Name] = f.Length;
                        else
                            this._journalLengthCache.Add(f.Name, f.Length);*/
                    }
                    this.logger.Log("Creating up to {0} new entries from files that failed verification", newEntries.Count);
                    mainForm.journalParser.createJournalEntries(newEntries, true, true);
                    foreach (KeyValuePair<string, long> kvp in newJournalLengthCache)
                    {
                        if (this._journalLengthCache.ContainsKey(kvp.Key))
                            this._journalLengthCache[kvp.Key] = kvp.Value;
                        else
                            this._journalLengthCache.Add(kvp.Key, kvp.Value);
                    }
                    this.saveAllCaches();
                }
            }
            catch (Exception e)
            {
                newJournalLengthCache.Clear();
                this.logger.Log("{0}", LogLevel.ERROR, e.ToString());
                throw e;
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
            //string journalPath = Path.Combine(this.cachePath, "commanders.emc");
            //Dictionary<string, Tuple<string, long>> commanderData = new Dictionary<string, Tuple<string, long>>();
            foreach (Commander c in mainForm.journalParser.commanders.Values)
            {
                if (!c.NeedsSaving) // Don't save commander that has no changes
                {
                    this.logger.Log("CMDR {0} has no changes and doesn't require saving", c.Name);
                    continue;
                } 
                c.CreateSaveDirectory();
                long saveBytes = c.saveData();
            }
            /*using (StreamWriter sw = new StreamWriter(journalPath, false, Encoding.UTF8))
            {
                sw.WriteLine(JsonConvert.SerializeObject(commanderData, Formatting.Indented));
            }*/
            using (StreamWriter sw = new StreamWriter(this.journalLengthCache, false, Encoding.UTF8))
            {
                sw.WriteLine(JsonConvert.SerializeObject(this._journalLengthCache, Formatting.Indented));
            }
        }

        public Commander loadCommanderFromFile(string filePath, long decompressedBytes)
        {
            this.logger.Log("Loading commander data for commander from {0}", filePath);
            try
            {
                if (decompressedBytes < 2)
                    decompressedBytes = 2; // This ensures we read the magic number for GZip and the code errors if it's not right so we can attempt a legacy load
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        byte[] bytes = new byte[decompressedBytes];
                        gz.Read(bytes, 0, (int)decompressedBytes);
                        string commanderData = Encoding.UTF8.GetString(bytes);
                        Commander c = JsonConvert.DeserializeObject<Commander>(commanderData);
                        logger.Log("[GZIP] Loaded Commander data for commander '{0}' - Entries: {1:n0}", LogLevel.DEBUG, c.Name, c.JournalEntries.Count);
                        c.OnLoad();
                        return c;
                    }
                }
            }
            catch (InvalidDataException)
            {
                this.logger.Log("Saved Commander data is not GZip, attempting legacy load on file.");
                try
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                    {
                        Commander c = JsonConvert.DeserializeObject<Commander>(sr.ReadToEnd());
                        logger.Log("Loaded Commander data for commander '{0}' - Entries: {1:n0}", LogLevel.DEBUG, c.Name, c.JournalEntries.Count);
                        c.OnLoad();
                        return c;
                    }
                }
                catch
                {
                    this.logger.Log("FATAL: Unable to load commander data from file '{0}'", filePath);
                    throw new UnableToLoadCommanderException();
                }
            }
            //throw new UnableToLoadCommanderException("Unknown issue");
        }

        public bool loadCaches()
        {

            this.logger.Log("Loading Journal length data...");
            this._journalLengthCache.Clear();
            using (StreamReader sr = new StreamReader(this.journalLengthCache, Encoding.UTF8))
            {
                this._journalLengthCache = JsonConvert.DeserializeObject<Dictionary<string, long>>(sr.ReadToEnd());
            }

            DirectoryInfo di = new DirectoryInfo(this.commanderDataPath);
            if (di.GetDirectories().Length > 0)
            {
                MainForm m = MainForm.Instance;
                this.logger.Log("Loading commanders...");
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    string currentDirectory = d.Name;
                    m.InvokeIfRequired(() => m.appStatus.Text = "Loading commander data for CMDR "+currentDirectory);
                    Commander c = JsonConvert.DeserializeObject<Commander>(Utils.loadGZip(Path.Combine(d.FullName, "journal.emj")));
                    c.OnLoad();
                    m.journalParser.commanders.Add(c.Name, c);
                    /* New loading is done now (isn't it so much simpler?! - Now we do the same checks we did on the old loading */
                    List<JournalEntry> unknownEvents = c.JournalEntries.FindAll(j => !j.isKnown);
                    this.logger.Log("Number of entries requiring update: {0}", unknownEvents.Count);
                    long updated = 0;
                    foreach (JournalEntry j in unknownEvents)
                    {
                        Commander __ = c;
                        JournalEntry je = m.journalParser.parseEvent(j.Json, out __, true);
                        if (je.isKnown)
                        {
                            updated++;
                            j.isKnown = true;
                            j.Data = je.Data;
                            if (j.Notes.Equals("UNKNOWN EVENT") || j.Notes.Equals("KNOWN-ISH EVENT"))
                                j.Notes = null;
                        }
                    }
                    this.logger.Log("Number of unknown journal events updated: {0}/{1}", updated, unknownEvents.Count);
                    if (updated > 0)
                        c.setSaveRequired();
                    if (!m.comboCommanderList.Items.Contains(c.Name))
                        m.comboCommanderList.InvokeIfRequired(() => m.comboCommanderList.Items.Add(c.Name));
                    if (c.isViewed)
                        this.switchOnLoad = c;
                }

            }
            else {
                this.logger.Log("Loading commander cache list...");
                this.commanderCaches.Clear();
                try
                {
                    using (StreamReader sr = new StreamReader(this.commandersPath, Encoding.UTF8))
                    {
                        this.commanderCaches = JsonConvert.DeserializeObject<Dictionary<string, Tuple<string, long>>>(sr.ReadToEnd());
                    }
                }
                catch (JsonSerializationException)
                {
                    using (StreamReader sr = new StreamReader(this.commandersPath, Encoding.UTF8))
                    {
                        Dictionary<string, string> tmp = new Dictionary<string, string>();
                        tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                        foreach (KeyValuePair<string, string> kvp in tmp)
                        {
                            this.commanderCaches.Add(kvp.Key, Tuple.Create<string, long>(kvp.Value, 0L));
                        }
                    }
                }


                this.logger.Log("Loading commander data...");
                foreach (KeyValuePair<string, Tuple<string, long>> kvp in new Dictionary<string, Tuple<string, long>>(this.commanderCaches))
                {
                    string commanderPath = Path.Combine(this.commanderDataPath, kvp.Value.Item1);
                    if (!File.Exists(commanderPath))
                    {
                        this.logger.Log("Commander cache file '{0}' doesn't exist. Treating caches as invalid.", LogLevel.ERR, commanderPath);
                        return false;
                    }
                    Commander c = loadCommanderFromFile(commanderPath, kvp.Value.Item2);
                    if (!c.newSaveMethod)
                    {
                        this.logger.Log("Updating Commander save path to new version...");
                        this.logger.Log("Creating commander data directory...");
                        c.CreateSaveDirectory();
                        this.logger.Log("Moving existing cache data for this commander to new path...");
                        string currentPath = string.Format("./cache/{0}", kvp.Value.Item1);
                        string newPath = string.Format("./cache/{0}/journal.emj", c.Name);
                        this.logger.Log("{0} -> {1}", LogLevel.DEBUG, currentPath, newPath);
                        File.Move(currentPath, newPath);
                        this.logger.Log("Updating commander registry to new path...");
                        Tuple<string, long> cTuple = this.commanderCaches[c.Name];
                        cTuple = new Tuple<string, long>(string.Format("./{0}/journal.emj", c.Name), cTuple.Item2);
                        this.commanderCaches[c.Name] = cTuple;
                        c.newSaveMethod = true;
                    }
                    this.logger.Log("Checking for unknown events and updating them where necessary...");
                    int updated = 0;
                    List<JournalEntry> needsUpdating = c.JournalEntries.FindAll(j => !j.isKnown);
                    foreach (JournalEntry je in needsUpdating)
                    {
                        Commander __ = c;
                        JournalEntry nje = mainForm.journalParser.parseEvent(je.Json, out __, true);
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
            }

            if (switchOnLoad == null)
            {
                switchOnLoad = mainForm.journalParser.commanders.Values.First();
            }

            return true;
        }

        internal void clearCaches()
        {
            string[] files = Directory.GetFiles(this.commanderDataPath);
            foreach (string f in files)
                File.Delete(f);
            foreach (string d in Directory.GetDirectories(this.commanderDataPath))
                Directory.Delete(d, true);
            mainForm.InvokeIfRequired(() =>
            {
                mainForm.journalParser.commanders.Clear();
                mainForm.eventList.Rows.Clear();
                mainForm.comboCommanderList.Items.Clear();
                mainForm.eventFilterDropdown.Items.Clear();
            });
            this.commanderCaches.Clear();
        }
    }

    [Serializable]
    internal class UnableToLoadCommanderException : Exception
    {
        public UnableToLoadCommanderException()
        {
        }

        public UnableToLoadCommanderException(string message) : base(message)
        {
        }

        public UnableToLoadCommanderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToLoadCommanderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
