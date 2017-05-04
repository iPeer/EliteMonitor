using EliteMonitor.Caching;
using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Journal;
using EliteMonitor.Logging;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace EliteMonitor
{
    public partial class MainForm : Form
    {

        private ListViewItem.ListViewSubItem _lastListViewItem;
        private Point _lastToolTipMousePos;
        public static MainForm Instance { get; private set; }
        private Dictionary<string, Color> defaultColours = new Dictionary<string, Color>();
        public JournalParser journalParser;
        public CacheController cacheController;
        public Logger logger;
        private Thread eliteCheckerThread;
        private SQLiteConnection sql;
        private const int PRELOAD_BODY_DATA_LIMIT = 100000;
        private string lastHighlight = "";
        public EliteDatabase Database;

        public MainForm()
        {
            this.logger = new Logger("Main");
            InitializeComponent();
#if !DEBUG
            dEBUGToolStripMenuItem.Visible = false;
#endif
            appVersionStatusLabel.Text = Utils.getApplicationVersion();
            eventFilterDropdown.SelectedIndex = eventFilterDropdown.Items.IndexOf("NONE");
            Instance = this;
            journalParser = new JournalParser(this);
            cacheController = new CacheController(this);
            Database = new EliteDatabase(this);
            if (!cacheController.cacheExists())
            {
                startNoCacheLoadThread();
            }
            else
            {
                startCacheLoadThread();
            }
            eliteCheckerThread = new Thread(() =>
            {
                while (true)
                {
                    bool r = EliteUtils.IsEliteRunning();
                    eliteRunningStatus.Text = String.Format("Elite: Dangerous is{0}running", r ? " " : " not ");
                    Thread.Sleep(60000);
                }
            });
            eliteCheckerThread.IsBackground = true;
            eliteCheckerThread.Start();

            Thread updateCheck = new Thread(() =>
            {
                WebClient wc = new WebClient();
                JObject json = JObject.Parse(wc.DownloadString("https://ipeer.auron.co.uk/EliteMonitor/version.json"));
                string version = (string)json["version"];
                Version v = new Version(version);
                this.logger.Log("Version check came back with {0}", version);
                if (v > new Version(Utils.getApplicationVersion()))
                {
                    this.appVersionStatusLabel.Text = "UPDATE AVAILABLE!";
                    this.logger.Log("Version running is out of date, prompting user for update.");
                    if (MessageBox.Show("An update is available for EliteMonitor, would you like to download it now?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start("https://ipeer.auron.co.uk/EliteMonitor/EliteMonitor.zip");
                    }
                }

            });
            updateCheck.IsBackground = true;
            updateCheck.Start();

            /*string[] lines = new string[]
            {
                "{ \"timestamp\":\"2017-01-26T21:23:18Z\", \"event\":\"Fileheader\", \"part\":1, \"language\":\"English\\UK\", \"gameversion\":\"2.2\", \"build\":\"r131487/r0 \" }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"LoadGame\", \"Commander\":\"iPeer\", \"Ship\":\"Cutter\", \"ShipID\":11, \"GameMode\":\"Group\", \"Group\":\"Mobius\", \"Credits\":153430680, \"Loan\":0 }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"Rank\", \"Combat\":5, \"Trade\":7, \"Explore\":5, \"Empire\":12, \"Federation\":7, \"CQC\":0 }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"Progress\", \"Combat\":58, \"Trade\":27, \"Explore\":11, \"Empire\":3, \"Federation\":3, \"CQC\":0 }"
            };
            foreach (string s in lines)
                jp.parseEvent(s.Replace(@"\", ""));*/

            // I'm too lazy to write these JSON files, so I just export them :)
            /*using (StreamWriter sw = new StreamWriter(Path.Combine(cacheController.dataPath, "commodities.json")))
            {
                sw.WriteLine(JsonConvert.SerializeObject(Materials.MATERIALS, Formatting.Indented));
            }*/

        }

        public void startNoCacheLoadThread()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    journalParser.parseAllJournals();
                }
                catch
                {
                    journalParser.stopTailing();
                    MessageBox.Show("Unfortunately, an error occurred while attempting to load data from the journal files. Please try loading it again by restarting EliteMonitor.\nIf this issue persists, please report it as a bug so it can be fixed!", "Journal parsing error", MessageBoxButtons.OK);
                }
                cacheController.saveAllCaches();
                onCacheLoadComplete();
            });
            t.Start();
        }

        public void startCacheLoadThread()
        {
            Thread t = new Thread(() =>
            {
                if (!cacheController.loadCaches())
                {
                    cacheController.clearCaches();
                    startNoCacheLoadThread();
                }
                else
                {
                    try {
                        cacheController.verifyFileLengths();
                    }
                    catch
                    {
                        journalParser.stopTailing();
                        MessageBox.Show("Unfortunately, an error occurred while attempting to load new data from the journal files. Old, existing data will still be available to view, but new data will not be added until the issue is resolved.\nIf this issue persists, please report it as a bug so it can be fixed!", "Journal parsing error", MessageBoxButtons.OK);
                        this.InvokeIfRequired(() => this.eventList.EndUpdate()); // Force end update otherwise the list becomes blank after load failure of new data
                    }
                    journalParser.switchViewedCommander(cacheController.switchOnLoad);
                    cacheController.switchOnLoad = null;
                    onCacheLoadComplete();
                }
            });
            t.Start();
        }

        public void onCacheLoadComplete()
        {
            this.journalParser.startTailer();
        }

        [Obsolete]
        internal void addEvent(params string[] v)
        {
            addEvent(v[2], v);
        }

        [Obsolete]
        internal void addEvent(string json, params string[] v)
        {
            try
            {
                if (v.Length == 0) { throw new InvalidOperationException("Data array cannot be empty."); }
                ListViewItem lvi = new ListViewItem(v);
                if (v.Length >= 4 && v[3].Equals("UNKNOWN EVENT"))
                {
                    lvi.BackColor = Color.Pink;
                }
                else if (v[1].Equals("LoadGame"))
                    lvi.BackColor = Color.LightGreen;
                lvi.ToolTipText = json;
                this.eventList.InvokeIfRequired(() =>
                {
                    eventList.Items.Insert(0, lvi);
                    foreach (ColumnHeader ch in eventList.Columns)
                    {

                        ch.Width = -2;
                    }
                });
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }

        private void eventList_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo ht = eventList.HitTest(e.Location);
            if (ht != null)
            {
                if (/*_lastToolTipMousePos != e.Location || */(ht.SubItem != null && ht.SubItem != _lastListViewItem))
                {
                    _lastListViewItem = ht.SubItem;
                    _lastToolTipMousePos = e.Location;
                    string tipText = /*ht.Item.SubItems[2].Text;*/ht.Item.ToolTipText;
                    toolTip.Show(tipText, (ListView)sender, e.Location.X + 10, e.Location.Y + 10, 5000);
                }
            }
        }

        private void MainForm_MouseEnter(object sender, EventArgs e)
        {
            if (toolTip.Active && !toolTip.ShowAlways)
                toolTip.Hide(eventList);
        }

        private void eventFilterDropdown_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selected = eventFilterDropdown.SelectedItem.ToString();

            if (selected.Equals(this.lastHighlight)) return;

            IEnumerable<ListViewItem> items = this.eventList.Items.Cast<ListViewItem>();
            if (!this.lastHighlight.Equals(string.Empty))
            {
                List<ListViewItem> toRemove = items.Where(a => a.SubItems[1].Text.Equals(this.lastHighlight)).ToList();
                foreach (ListViewItem i in toRemove)
                {
                    if (defaultColours.ContainsKey(i.SubItems[1].Text))
                        i.BackColor = defaultColours[i.SubItems[1].Text];
                }
                this.lastHighlight = selected;
            }

            List<ListViewItem> toAdd = items.Where(a => a.SubItems[1].Text.Equals(selected)).ToList();
            if (toAdd.Count > 0)
            {
                foreach (ListViewItem i in toAdd)
                {
                    if (!defaultColours.ContainsKey(i.SubItems[1].Text))
                        defaultColours.Add(i.SubItems[1].Text, i.BackColor);
                    i.BackColor = Color.LightBlue;
                }
                this.eventList.EnsureVisible(this.eventList.Items.IndexOf(toAdd.First()));
            }

            /*foreach (ListViewItem i in eventList.Items)
            {
                if (i.SubItems[1].Text.Equals(selected))
                {
                    if (!defaultColours.ContainsKey(selected))
                    {
                        defaultColours.Add(selected, i.BackColor);

                    }
                    i.BackColor = Color.LightBlue;
                }
                else
                {
                    if (defaultColours.ContainsKey(i.SubItems[1].Text))
                        i.BackColor = defaultColours[i.SubItems[1].Text];
                }
            }*/
        }

        delegate void ThreadSafeAppStatusLabelTextChange(string text);

        public void setAppStatusText(string text)
        {
            if (this.InvokeRequired)
            {
                ThreadSafeAppStatusLabelTextChange tsasltc = new ThreadSafeAppStatusLabelTextChange(setAppStatusText);
                this.Invoke(tsasltc, new object[] { text });
            }
            else
            {
                this.appStatus.Text = text;
            }
        }

        private void openCacheDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Utils.getApplicationEXEFolderPath(), "cache/"));
        }

        private void clearCachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all caches?\nThis may result in missing information if Journal files have been deleted.\n\nTHIS PROCESS CANNOT BE UNDONE.", "Confirm Cache Clear", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cacheController.clearCaches();
                this.startNoCacheLoadThread();
            }
        }

        private void realoadJournalEntriesFromCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reload all Journal entries? This might take a while.", "Confirm reload", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eventList.Items.Clear();
                cacheController.loadCaches();
            }
        }

        private void debugRefreshJournal_Click(object sender, EventArgs e)
        {
            string newCommander = this.comboCommanderList.SelectedItem.ToString();
            Thread t = new Thread(() =>
            {
                this.journalParser.switchViewedCommander(newCommander);
            });
            t.Start();

        }

        private void comboCommanderList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string newCommander = this.comboCommanderList.SelectedItem.ToString();
            Thread t = new Thread(() =>
            {
                this.journalParser.switchViewedCommander(newCommander);
            });
            t.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cacheController.saveAllCaches();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hUDEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
                HUDEditor hud = new HUDEditor();
                hud.Show(this);
        }

        private void testBodiesDatabaseReadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // TODO: Make the application download the files required for this

            /* EXAMPLE LINE FROM THIS FILE:
             * 
             * 
             * {"id":63,"created_at":1466612919,"updated_at":1482859581,"name":"Arro Naga 5 F","system_id":1317,"group_id":6,"group_name":"Planet","type_id":31,"type_name":"Icy body","distance_to_arrival":null,"full_spectral_class":null,"spectral_class":null,"spectral_sub_class":null,"luminosity_class":null,"luminosity_sub_class":null,"surface_temperature":null,"is_main_star":null,"age":null,"solar_masses":null,"solar_radius":null,"catalogue_gliese_id":null,"catalogue_hipp_id":null,"catalogue_hd_id":null,"volcanism_type_id":null,"volcanism_type_name":null,"atmosphere_type_id":1,"atmosphere_type_name":"No atmosphere","terraforming_state_id":null,"terraforming_state_name":null,"earth_masses":null,"radius":null,"gravity":null,"surface_pressure":null,"orbital_period":null,"semi_major_axis":null,"orbital_eccentricity":null,"orbital_inclination":null,"arg_of_periapsis":null,"rotational_period":null,"is_rotational_period_tidally_locked":false,"axis_tilt":null,"eg_id":null,"belt_moon_masses":null,"rings":[],"atmosphere_composition":[],"solid_composition":[],"materials":[{"material_id":1,"material_name":"Carbon","share":22.7},{"material_id":2,"material_name":"Iron","share":11.8},{"material_id":3,"material_name":"Nickel","share":8.9},{"material_id":4,"material_name":"Phosphorus","share":14.5},{"material_id":5,"material_name":"Sulphur","share":27},{"material_id":7,"material_name":"Chromium","share":5.3},{"material_id":8,"material_name":"Germanium","share":3.4},{"material_id":10,"material_name":"Selenium","share":4.2},{"material_id":16,"material_name":"Molybdenum","share":0.8},{"material_id":19,"material_name":"Tungsten","share":0.6},{"material_id":22,"mater,"share":0.7}],"is_landable":1}
             * 
             * 
             * EXAMPLE LINE FROM systems.csv
             * id,edsm_id,name,x,y,z,population,is_populated,government_id,government,allegiance_id,allegiance,state_id,state,security_id,security,primary_economy_id,primary_economy,power,power_state,power_state_id,needs_permit,updated_at,simbad_ref,controlling_minor_faction_id,controlling_minor_faction,reserve_type_id,reserve_type
             * 17,60,"10 Ursae Majoris",0.03125,34.90625,-39.09375,0,0,176,None,5,None,80,None,16,Low,10,None,,,,0,1466593246,"10 Ursae Majoris",,,,
             * 
             * FORMULA TO CALCULATE DISTANCE IS
             * distance_in_ly=sqrt( (x2-x1)2 + (y2-y1)2 + (z2-z1)2 )
             * 
             * 
             * */
            readSystems();
        }

        public void readSystems()
        {
            FileInfo fi = new FileInfo(cacheController.rawSystemDataCache);
            long len = fi.Length; // We basically have to use bytes to determine how far along the file we are here as it is a BIG file.
            int lastPercent = 0;
            DateTime timeStarted;
            DateTime lastETAUpdate = DateTime.Now;

            Thread systemsThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                List<BasicSystem> systems = new List<BasicSystem>();

                using (FileStream fs = File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        string line = "";
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            int lineN = 0;
                            string[] headers = new string[0];
                            timeStarted = DateTime.Now;
                            while ((line = sr.ReadLine()) != null)
                            {
                                double percent = ((double)sr.BaseStream.Position / (double)len) * 100.00;
                                //Console.WriteLine(String.Format("{0} | {1} | {2} / {3}", (int)percent > lastPercent, (int)percent, lastPercent, DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00));
                                //Console.WriteLine(percent + " / " + lastPercent);
                                if ((int)percent == 100 || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                                {
                                    TimeSpan ts = (DateTime.Now - timeStarted);
                                    double timeLeft = (ts.TotalSeconds / sr.BaseStream.Position) * (len - sr.BaseStream.Position);
                                    lastETAUpdate = DateTime.Now;
                                    lastPercent = (int)percent;
                                    this.InvokeIfRequired(() => appStatus.Text = String.Format("Parsing systems.csv... ({0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft)));
                                }
                                if (lineN++ == 0)
                                {
                                    headers = new string[line.Split(',').Length];
                                    headers = line.Split(',');
                                }
                                else
                                {
                                    try
                                    {
                                        string sysName = line.Split(',')[Array.IndexOf(headers, "name")];
                                        long sysID = Convert.ToInt64(line.Split(',')[Array.IndexOf(headers, "id")]);
                                        //long edsmID = Convert.ToInt64(line.Split(',')[Array.IndexOf(headers, "edsm_id")]);
                                        if (sysID == 1317)
                                        {
                                            Console.WriteLine("MATCH: "+line);
                                        }
                                        float x, y, z;
                                        try
                                        {
                                            x = Convert.ToSingle(line.Split(',')[Array.IndexOf(headers, "x")]);
                                            y = Convert.ToSingle(line.Split(',')[Array.IndexOf(headers, "y")]);
                                            z = Convert.ToSingle(line.Split(',')[Array.IndexOf(headers, "z")]);
                                        }
                                        catch (FormatException) { continue; }

                                        BasicSystem _bs = new BasicSystem(sysName, sysID, new SystemCoordinate(x, y, z));
                                        systems.Add(_bs);
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine(line);
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        Console.WriteLine("Exception with line: "+line);
                                    }

                                }
                            }

                        }
                    }
                }
                saveSystemData(systems);
                sw.Stop();
                this.logger.Log("systems.csv read in {0:n0}ms", sw.ElapsedMilliseconds);
                readBodies(systems);
            });
            systemsThread.Start();
        }

        public void readBodies(List<BasicSystem> systemData)
        {
            this.InvokeIfRequired(() => appStatus.Text = "Preparing to parse bodies.jsonl...");
            FileInfo fi = new FileInfo(cacheController.rawBodyDataCache);
            long len = fi.Length; // We basically have to use bytes to determine how far along the file we are here as it is a BIG file.
            int lastPercent = 0;
            DateTime lastETAUpdate = DateTime.Now;

            Thread t = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                List<Body> bodyList = new List<Body>();

                using (FileStream fs = File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        string line = "";
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            Stopwatch startTimer = new Stopwatch();
                            startTimer.Start();
                            ILookup<Int64, BasicSystem> lookup = systemData.ToLookup<BasicSystem, long>(a => a.ID);
                            long parsed = 0;
                            long skipped = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (bodyList.Count >= PRELOAD_BODY_DATA_LIMIT)
                                {
                                    startTimer.Stop();
                                    saveBodyData(bodyList);
                                    bodyList.Clear();
                                    startTimer.Start();
                                }
                                bool brokenContinue = false;
                                double percent = ((double)sr.BaseStream.Position / (double)len) * 100.00;
                                //Console.WriteLine(percent + " / " + lastPercent);
                                if ((int)percent == 100 || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                                {
                                    double timeLeft = (startTimer.Elapsed.TotalSeconds / sr.BaseStream.Position) * (len - sr.BaseStream.Position);
                                    lastETAUpdate = DateTime.Now;
                                    lastPercent = (int)percent;
                                    this.InvokeIfRequired(() => appStatus.Text = String.Format("Parsing bodies.jsonl... ({0:n0}%) [ETA: {1}] - Skipped {2:n0}/{3:n0} ({4:n0}%)", percent, Utils.formatTimeFromSeconds(timeLeft), skipped, parsed, ((double)skipped / (double)parsed) * 100.00));
                                }
                                JObject json = JObject.Parse(line);
                                parsed++;
                                if (!json["materials"].HasValues)
                                {
                                    skipped++;
                                    continue; // We don't need bodies with no material data
                                }
                                long systemID = (long)json["system_id"];
                                int count = lookup[systemID].ToList().Count;
                                if (count == 0) { Console.WriteLine(String.Format("System ID {0} has no matches in the lookup, skipping this body.", systemID)); continue; } // We don't care about bodies that don't have an accompanying system
                                string planetName = (string)json["name"];
                                long bodyID = (long)json["id"];
                                string planetType = (string)json["type_name"];
                                int distanceFromStar = -1;
                                if (!json["distance_to_arrival"].IsNullOrEmpty())
                                    distanceFromStar = (int)json["distance_to_arrival"];
                                Dictionary<string, double> materials = new Dictionary<string, double>();
                                JArray jarray = (JArray)json["materials"];
                                /*jarray.
                                List<PlanetaryMaterialData> materialData = JsonConvert.DeserializeObject<List<PlanetaryMaterialData>>(json["materials"].ToString());*/
                                foreach (JObject jt in jarray) {
                                    //Console.WriteLine(jt.ToString());
                                    if (string.IsNullOrEmpty(jt["share"].ToString()))
                                    {
                                        brokenContinue = true;
                                        break;
                                    }
                                    double share = (double)jt["share"];
                                    string material = (string)jt["material_name"];
                                    if (materials.ContainsKey(material)) {
                                        if (share > materials[material])
                                            materials[material] = share;
                                    }
                                    else {
                                        materials.Add(material, share);
                                    }
                                }
                                if (brokenContinue)
                                { // True is the body has a material parameter that is nulled.
                                    skipped++;
                                    continue;
                                }
                                BasicSystem system = lookup[systemID].ToList().First(); // There should never be multiple systems with the same ID, if there is, that's on EDDB, not me.
                                Body body = new Body(planetName, planetType, system.Name.Replace("\"", ""), distanceFromStar, system.Coordinates, materials); // Now that I think about it, this class creation is redundant ¯\_(ツ)_/¯
                                bodyList.Add(body);
                            }
                            saveBodyData(bodyList);

                        }
                    }
                }
                sql.Close();
                systemData.Clear(); // Free up this memory.
                sw.Stop();
                this.logger.Log("bodies.jsonl read in {0:n0}ms", sw.ElapsedMilliseconds);
            });
            t.Start();
        }

        private void saveSystemData(List<BasicSystem> systems)
        {
            int bCount = systems.Count;
            int cBody = 0;
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            int lastPercent = 0;
            // If this process is running, we are recreating the cache, we don't "update it"
            SQLiteConnection.CreateFile(cacheController.systemDataCache);
            sql = new SQLiteConnection(String.Format("Data Source={0};Version=3;", cacheController.systemDataCache));
            sql.Open();
            SQLiteCommand com = new SQLiteCommand("CREATE TABLE IF NOT EXISTS systems (system text, ID bigint, X real, Y real, Z real)", sql);
            com.ExecuteNonQuery();
            com.Dispose();
            using (SQLiteCommand insert = new SQLiteCommand("INSERT INTO systems VALUES(@sysName,@ID,@coordsx,@coordsy,@coordsz)", sql))
            {
                using (SQLiteTransaction t = sql.BeginTransaction())
                {
                    foreach (BasicSystem b in systems)
                    {

                        double percent = ((double)cBody++ / (double)bCount) * 100.00;
                        if ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                        {
                            TimeSpan ts = (DateTime.Now - timeStarted);
                            double timeLeft = (ts.TotalSeconds / cBody) * (bCount - cBody);
                            lastETAUpdate = DateTime.Now;
                            lastPercent = (int)percent;
                            this.InvokeIfRequired(() => appStatus.Text = String.Format("Preparing system data for saving... ({0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft)));
                        }

                        insert.Parameters.Clear(); // Just in case ;)
                        insert.Parameters.AddWithValue("@sysName", b.Name.Replace("\"", ""));
                        insert.Parameters.AddWithValue("@ID", b.ID);
                        insert.Parameters.AddWithValue("@coordsx", b.Coordinates.X);
                        insert.Parameters.AddWithValue("@coordsy", b.Coordinates.Y);
                        insert.Parameters.AddWithValue("@coordsz", b.Coordinates.Z);

                        insert.ExecuteNonQueryAsync();
                    }
                    this.InvokeIfRequired(() => appStatus.Text = "Saving system data...");
                    t.Commit();
                }
            }
        }

        private void saveBodyData(List<Body> bodyList)
        {
            int bCount = bodyList.Count;
            int cBody = 0;
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            int lastPercent = 0;
            if (!File.Exists(cacheController.bodyDataCache))
                SQLiteConnection.CreateFile(cacheController.bodyDataCache);
            sql = new SQLiteConnection(String.Format("Data Source={0};Version=3;", cacheController.bodyDataCache));
            sql.Open();
            SQLiteCommand com = new SQLiteCommand("CREATE TABLE IF NOT EXISTS bodies (system text, body text, bodyType text, distanceFromStar bigint, X real, Y real, Z real, materialData text)", sql);
            com.ExecuteNonQuery();
            com.Dispose();
            using (SQLiteCommand insert = new SQLiteCommand("INSERT INTO bodies VALUES(@sysName,@bodyName,@bodyType,@distance,@coordsx,@coordsy,@coordsz,@materials)", sql))
            {
                using (SQLiteTransaction t = sql.BeginTransaction())
                {
                    foreach (Body b in bodyList)
                    {

                        double percent = ((double)cBody++ / (double)bCount) * 100.00;
                        if ((int)percent == 100 || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                        {
                            TimeSpan ts = (DateTime.Now - timeStarted);
                            double timeLeft = (ts.TotalSeconds / cBody) * (bCount - cBody);
                            lastETAUpdate = DateTime.Now;
                            lastPercent = (int)percent;
                            this.InvokeIfRequired(() => appStatus.Text = String.Format("Preparing body data for saving... ({0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft)));
                        }

                        insert.Parameters.Clear(); // Just in case ;)
                        insert.Parameters.AddWithValue("@sysName", b.SystemName);
                        insert.Parameters.AddWithValue("@bodyName", b.Name);
                        insert.Parameters.AddWithValue("@bodyType", b.Type);
                        insert.Parameters.AddWithValue("@distance", b.EntryDistance);
                        insert.Parameters.AddWithValue("@coordsx", b.Coordinates.X);
                        insert.Parameters.AddWithValue("@coordsy", b.Coordinates.Y);
                        insert.Parameters.AddWithValue("@coordsz", b.Coordinates.Z);
                        insert.Parameters.AddWithValue("@materials", JsonConvert.SerializeObject(b.Materials));

                        insert.ExecuteNonQueryAsync();
                    }
                    this.InvokeIfRequired(() => appStatus.Text = "Saving body data to database...");
                    t.Commit();
                }
            }
            bodyList.Clear();
        }

        private void showDistanceFromSolToNetoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //(x2-x1)2 + (y2-y1)2 + (z2-z1)2
            // -41.1875 / 7.65625 / 36.3125
            float netox = -41.1875f;
            float netoy = 7.65625f;
            float netoz = 36.3125f;

            float solx = 0, soly = 0, solz = 0;
            double distance = Math.Sqrt(Math.Pow((solx - netox), 2) + Math.Pow((soly - netoy), 2) + Math.Pow((solz - netoz), 2));
            MessageBox.Show(distance.ToString());
        }

        private void resetSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }
    }
}
