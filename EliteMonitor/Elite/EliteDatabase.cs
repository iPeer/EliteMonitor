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
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using EliteMonitor.Extensions;
using System.Drawing;

namespace EliteMonitor.Elite
{

    public class NoElementMatchesSymbolException : Exception { }
    public class NoResultsForSystemSearchException : Exception { }

    public class EliteDatabase
    {

        /*#if DEBUG
                public const string EDSM_API_URL = "https://beta.edsm.net:8080/api-v1/"; // Seems to be deprecated?
        #else*/
        public const string EDSM_API_URL = "https://www.edsm.net/api-v1/";
        /*#endif*/
        public const string VOLATILES_API_URL = "https://ipeer.auron.co.uk/EliteMonitor/volatiles.json";

        public readonly Bitmap[] MATERIAL_GRADE_IMAGES = new Bitmap[] { Properties.Resources.grade_1, Properties.Resources.grade_2, Properties.Resources.grade_3, Properties.Resources.grade_4, Properties.Resources.grade_5 };

        private WebClient EDSMWebClient = new WebClient();
        public event EventHandler<List<BasicSystem>> OnEDSMDataDownloadComplete;
        public event EventHandler<Int64[]> OnEDSMSystemParseProgress;
        private JObject current_volatiles;

        //public List<Material> Materials;
        public Dictionary<string, Material> Materials;
        //public Dictionary<string, string> MaterialTypes;
        public Dictionary<string, string> Ships;
        public Dictionary<string, string> Commodities;
        public Dictionary<string, string> VoucherTypes = new Dictionary<string, string>()
        {
            { "bounty", "Bounty Voucher" },
            { "CombatBond", "Combat Bond" },
            { "settlement", "Intel Package" }
        };
        public Dictionary<string, string> StarClassNames = new Dictionary<string, string>()
        {
            { "H", "Black hole" },
            { "N", "Neutron star" },
            { "AeBe", "Herbig Ae/Be star" },
            { "DA", "White Dwarf" },
            { "DAV", "Pulsating White Dwarf" },
            { "DB", "White Dwarf" },
            { "DBV", "Pulsating White Dwarf" },
            { "DAB", "White Dwarf" },
            { "DC", "White Dwarf" },
            { "DCV", "Pulsating White Dwarf" },
            { "DQ", "White Dwarf" },
            { "DAZ", "White Dwarf" },
            { "TTS", "T Tauri star" },
            { "SupermassiveBlackHole", "Supermassive Black Hole" },
            { "W", "Wolf-Rayet star" },
            { "WC", "Wolf-Rayet C star" }, // I don't advise using these as a bathoom.
            { "WO", "Wolf-Rayet O star" },
            { "WNC", "Wolf-Rayet NC star" },
            { "WN", "Wolf-Rayet N star" },
            { "C", "Carbon star" },
            { "C-N", "Carbon star" },
            { "CN", "Carbon star" },
            { "C-J", "Carbon star" },
            { "CJ", "Carbon star" },
            { "A_BlueWhiteSuperGiant", "Class A supergiant" },
            { "F_WhiteSuperGiant", "Class F supergiant" },
            { "M_RedSuperGiant", "Class M supergiant" },
            { "M_RedGiant", "Class M red giant" },
            { "K_OrangeGiant", "Class K orange giant" },

        };
        public string[] ThargoidStationLocations = { "HIP 16753", "Pleiades Sector IR-W d1-55", "Taygeta" };
        public static EliteDatabase Instance;
        public Logger logger;
        private MainForm mainForm;

        private List<DownloadableDataItem> needsUpdate = new List<DownloadableDataItem>();

        public EliteDatabase(MainForm m)
        {
            this.mainForm = m;
            this.logger = new Logger("EliteDatabase");
            Instance = this;
            this.Materials = new Dictionary<string, Material>();
            //this.MaterialTypes = new Dictionary<string, string>();
            this.Ships = new Dictionary<string, string>();
            this.Commodities = new Dictionary<string, string>();

            loadDataFromVolatilesOrDisk();

            /*loadMaterials();
            loadShips();
            loadCommodities();*/
            /*loadMaterialTypes();*/
        }

        /*public bool loadDataFromVolatiles()
        {
            if (!volatilesAreValid())
            {

            }
        }*/

        public void loadDataFromDataCache()
        {
            loadMaterials();
            loadShips();
            loadCommodities();
        }

        public void loadDataFromVolatiles(bool download = true)
        {
            if (download)
            {
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        mainForm.InvokeIfRequired(() => mainForm.volatilesLabel.Text = "Downloading updated volatiles...");
                        this.logger.Log("Downloading volatiles from server...");
                        this.logger.Log("Volatiles URL is {0}", VOLATILES_API_URL);
                        string new_volatiles = wc.DownloadString(new Uri(VOLATILES_API_URL));
                        JObject _volatiles = JObject.Parse(new_volatiles);
                        string volatiles_etag = wc.ResponseHeaders.Get("ETag");
                        volatiles_etag = volatiles_etag.Substring(1, volatiles_etag.Length - 2);
                        this.logger.Log("Volatile remote etag is {0}", volatiles_etag);
                        _volatiles.Add("version", volatiles_etag);
                        string backup_path = Path.Combine(MainForm.Instance.cacheController.dataPath, "volatiles.json.bak");
                        string volatiles_path = Path.Combine(MainForm.Instance.cacheController.dataPath, "volatiles.json");
                        if (File.Exists(backup_path))
                            File.Delete(backup_path); //FIXME: This will error since EM is using the file
                        if (File.Exists(volatiles_path))
                        {
                            File.Copy(volatiles_path, backup_path);
                            File.Delete(volatiles_path);
                        }
                        using (StreamWriter sw = new StreamWriter(volatiles_path))
                        {
                            sw.WriteLine(JsonConvert.SerializeObject(_volatiles, Formatting.Indented));
                        }
                        this.logger.Log("Volatiles have been saved to disk", volatiles_etag);
                        this.current_volatiles = _volatiles;
                        _volatiles = null;
                    }
                    catch (Exception e) {
                        this.logger.Log("Unable to download volatiles from URL: ", LogLevel.ERR);
                        this.logger.Log("{1}{2}{0}", LogLevel.ERR, e.StackTrace, e.ToString(), Environment.NewLine);
                        loadDataFromVolatiles(false);
                    }
                }
            }
            if (!File.Exists(Path.Combine(MainForm.Instance.cacheController.dataPath, "volatiles.json"))) // Failsafe
            {
                loadDataFromDataCache();
                mainForm.journalParser.viewedCommander?.updateDialogDisplays();
                return;
            }
            if (!download)
            {
                using (StreamReader sr = new StreamReader(Path.Combine(MainForm.Instance.cacheController.dataPath, "volatiles.json")))
                {
                    this.current_volatiles = JObject.Parse(sr.ReadToEnd());
                }
            }
            this.Commodities.Clear();
            this.Commodities = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.current_volatiles.GetValue("commodities").ToString());
            this.Ships.Clear();
            this.Ships = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.current_volatiles.GetValue("ships").ToString());
            this.Materials.Clear();
            this.Materials = JsonConvert.DeserializeObject<Dictionary<string, Material>>(this.current_volatiles.GetValue("materials").ToString());
            this.ThargoidStationLocations = JsonConvert.DeserializeObject<string[]>(this.current_volatiles.GetValue("attack_systems").ToString());
#if DEBUG
            Debug.WriteLine("---------- DISPLAYING LOADED DATA FROM VOLATILES ----------");
            Debug.WriteLine("COMMODITIES:");
            foreach (KeyValuePair<string, string> kvp in this.Commodities)
            {
                Debug.WriteLine(string.Format("\t{0}: {1}", kvp.Key, kvp.Value));
            }
            Debug.WriteLine("");

            Debug.WriteLine("SHIPS:");
            foreach (KeyValuePair<string, string> kvp in this.Ships)
            {
                Debug.WriteLine(string.Format("\t{0}: {1}", kvp.Key, kvp.Value));
            }
            Debug.WriteLine("");

            Debug.WriteLine("MATERIALS:");
            foreach (KeyValuePair<string, Material> kvp in this.Materials)
            {
                Debug.WriteLine(string.Format("\t{0}: {1}", kvp.Key, kvp.Value.Name));
            }
            Debug.WriteLine("");


            Debug.WriteLine("THARGOID ATTACKS:");
            foreach (string s in this.ThargoidStationLocations)
            {
                Debug.WriteLine(string.Format("\t{0}", s));
            }
            Debug.WriteLine("---------- ----------");

#endif
            mainForm.InvokeIfRequired(() => mainForm.volatilesLabel.Text = "");
            mainForm.journalParser.viewedCommander?.updateDialogDisplays();
        }

        public void loadDataFromVolatilesOrDisk()
        {
            /*Thread volatile_thread = new Thread(new ThreadStart(() =>
            {*/
            string volatilesPath = Path.Combine(MainForm.Instance.cacheController.dataPath, "volatiles.json");
            if (!File.Exists(volatilesPath))
            {
                loadDataFromVolatiles(true); return;
            }
            try
            {
                StreamReader sr = new StreamReader(volatilesPath); // Don't use USING here as it creates a lock on the file when we try to delete it
                JObject volatiles = JObject.Parse(sr.ReadToEnd());
                sr.Close();   // v
                sr.Dispose(); // Clear lock on file in case we want to delete it.
                string etag = volatiles.GetValue("version").ToString();
                /*using (HttpClient c = new HttpClient())
                {*/
                mainForm.InvokeIfRequired(() => mainForm.volatilesLabel.Text = "Checking if volatiles need updating...");
                //HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Head, VOLATILES_API_URL);
                //this.logger.Log("Looking for volatiles at {0}", req.RequestUri);
                HttpResponseMessage res = Task.Run(() => GetVolatilesETag()).Result;
                string remote_etag = res.Headers.ETag.Tag;
                remote_etag = remote_etag.Substring(1, remote_etag.Length - 2);
                this.logger.Log("Remove volatiles returned with etag of {0}", remote_etag);
                this.logger.Log("local = {0}, remote = {1}", etag, remote_etag);
                if (!etag.Equals(remote_etag)) { mainForm.InvokeIfRequired(() => mainForm.volatilesLabel.Text = "Preparing to download updated volatiles..."); loadDataFromVolatiles(true); }
                else { this.logger.Log("Volatiles are up to date."); mainForm.InvokeIfRequired(() => mainForm.volatilesLabel.Text = "Volatiles are up to date."); loadDataFromVolatiles(false); }
                /*}*/
            }
            catch { loadDataFromDataCache(); }
            /*}));
            volatile_thread.IsBackground = true;
            volatile_thread.Priority = ThreadPriority.BelowNormal;
            volatile_thread.Start();*/
        }

        public async Task<HttpResponseMessage> GetVolatilesETag()
        {
            using (HttpClient c = new HttpClient())
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Head, VOLATILES_API_URL);
                this.logger.Log("Looking for volatiles at {0}", req.RequestUri);
                return await c.SendAsync(req);
            }
        }

        /*private void loadDataFromVolatilesOrDisk()
        {
            if (loadVolatiles())
        }*/

        // TODO

        public void runUpdatesIfNeeded()
        {
            if (this.needsUpdate.Count > 0)
            {
                Thread t = new Thread(() =>
                {
                    foreach (DownloadableDataItem d in this.needsUpdate)
                    {
                        FileDownloadHandler fdh = new FileDownloadHandler(d);
                        fdh.OnFileDownloadComplete += OnDatabaseDownloadComplete;
                        fdh.OnFileDownloadProgressChanged += OnDownloadProgressChanged;
                    }
                });
            }
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDatabaseDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void reloadMaterials() => loadMaterials();
        public void loadMaterials()
        {
            using (StreamReader sr = new StreamReader(Path.Combine(MainForm.Instance.cacheController.dataPath, "materials.json")))
            {
                this.Materials = JsonConvert.DeserializeObject<Dictionary<string, Material>>(sr.ReadToEnd());
            }
        }

        public void loadCommodities()
        {
            using (StreamReader sr = new StreamReader(Path.Combine(MainForm.Instance.cacheController.dataPath, "commodities.json")))
            {
                this.Commodities = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }
        }

        /*public void loadMaterialTypes()
        {
            if (File.Exists(Path.Combine(MainForm.Instance.cacheController.dataPath, "materialtypes.json")))
            {
                using (StreamReader sr = new StreamReader(Path.Combine(MainForm.Instance.cacheController.dataPath, "materialtypes.json")))
                {
                    this.MaterialTypes = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                }
            }
        }*/

        /*public void saveMaterialTypeToDatabase(string material, string type)
        {
            if (this.MaterialTypes.ContainsKey(material)) return;
            this.MaterialTypeDBIsDirty = true;
            this.MaterialTypes.Add(material, type);
        }*/

        /*public void saveMaterialTypeDatabaseToDisk()
        {
            if (this.MaterialTypeDBIsDirty)
            {
                this.MaterialTypeDBIsDirty = false;
                using (StreamWriter sw = new StreamWriter(Path.Combine(MainForm.Instance.cacheController.dataPath, "materialtypes.json")))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(this.MaterialTypes, Formatting.Indented));
                }
            }
        }*/

        /*public bool getMaterialTypeFromInternal(string material, out string type)
        {
            if (!this.MaterialTypes.ContainsKey(material)) { type = string.Empty;  return false; }
            type = this.MaterialTypes[material];
            return true;
        }*/

        public Int32 getMaterialGradeFromInternalName(string @internal)
        {
            return this.Materials[@internal].Grade;
        }

        public string getMaterialNameFromInternal(string @internal)
        {
            if (this.Materials.ContainsKey(@internal))
                return this.Materials[@internal].Name;
            return @internal;
        }

        public bool tryGetMaterialNameFromInternal(string @internal, out string realName)
        {
            if (this.Materials.ContainsKey(@internal))
            {
                realName = this.Materials[@internal].Name;
                return true;
            }
            realName = @internal;
            return false;
        }

        public string getMaterialTypeFromInternalName(string @internal)
        {
            if (this.Materials.ContainsKey(@internal))
                return this.Materials[@internal].Type;
            return "Unknown";
        }

        public string getCommodityNameFromInternal(string @internal)
        {
            if (this.Commodities.ContainsKey(@internal))
                return this.Commodities[@internal];
            return @internal;
        }

        public void loadShips()
        {
            string path = Path.Combine(MainForm.Instance.cacheController.dataPath, "ships.json");
            if (!File.Exists(path))
            {
                scheduleShipsDataUpdate();
            }
            using (StreamReader sr = new StreamReader(path))
            {
                this.Ships = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }
        }

        public void scheduleShipsDataUpdate()
        {
            DownloadableDataItem ddi = new DownloadableDataItem("https://ipeer.auron.co.uk/EliteMonitor/data/ships.json", Path.Combine(mainForm.cacheController.dataPath, "ships.json"));
            this.needsUpdate.Add(ddi);
        }

        public void scheduleMaterialsDataUpdate()
        {
            DownloadableDataItem ddi = new DownloadableDataItem("https://ipeer.auron.co.uk/EliteMonitor/data/materials.json", Path.Combine(mainForm.cacheController.dataPath, "materials.json"));
            this.needsUpdate.Add(ddi);
        }

        public void scheduleCommoditiesDataUpdate()
        {
            DownloadableDataItem ddi = new DownloadableDataItem("https://ipeer.auron.co.uk/EliteMonitor/data/commodities.json", Path.Combine(mainForm.cacheController.dataPath, "commodities.json"));
            this.needsUpdate.Add(ddi);
        }

        public string getShipNameFromInternalName(string internalName)
        {
            //internalName = internalName.ToLower();
            if (this.Ships.ContainsKey(internalName.ToLower()))
                return this.Ships[internalName.ToLower()];
            else return internalName;
        }

        public string getShipInternalNameFromRealName(string realName)
        {
            try { return this.Ships.First(a => a.Value.Equals(realName)).Key; }
            catch { return realName; }
        }

        public string GetCorrectStarClassName(string starClass)
        {
            if (this.StarClassNames.ContainsKey(starClass))
            {
                return this.StarClassNames[starClass];
            }
            else
                return string.Format("{0} class star", starClass);
        }

        public void getSystemSearchResultsFromEDSMAPI(string searchText, EventHandler<List<BasicSystem>> onEDSMDataDoneDownloading, DownloadProgressChangedEventHandler downloadProgress = null, EventHandler<Int64[]> parseProgresshandler = null)
        {
            EDSMWebClient = new WebClient();
            string url = string.Format("https://www.edsm.net/api-v1/systems?systemName={0}&showCoordinates=1&showInformation=1", searchText.Replace(" ", "%20"));
            if (downloadProgress != null)
                EDSMWebClient.DownloadProgressChanged += downloadProgress;
            if (parseProgresshandler != null)
                this.OnEDSMSystemParseProgress += parseProgresshandler;
            EDSMWebClient.DownloadStringCompleted += EDSMDownloadCompleted;
            EDSMWebClient.DownloadStringAsync(new Uri(url));
            OnEDSMDataDownloadComplete += onEDSMDataDoneDownloading;
        }

        private void EDSMDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            JArray json = JArray.Parse(e.Result);
            List<BasicSystem> results = new List<BasicSystem>();
            long totalEntries = json.Count;
            long currentEntry = 0;
            foreach (JObject j in json)
            {
                this.OnEDSMSystemParseProgress(this, new Int64[] { currentEntry++, totalEntries });
                string systemName = j.GetValue("name").ToString();
                //Debug.WriteLine($"{currentEntry}: {systemName}");
                SystemCoordinate coords;
                try
                {
                    coords = JsonConvert.DeserializeObject<SystemCoordinate>(j.GetValue("coords").ToString());
                }
                catch { continue; }
                string Allegiance = "Independent";
                string Economy = "None";
                Dictionary<string, object> systemInfo = new Dictionary<string, object>();
                try
                {
                    systemInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(j.GetValue("information").ToString());
                }
                catch { }

                if (systemInfo.Count > 0)
                {
                    Allegiance = systemInfo["allegiance"].ToString();
                    Economy = systemInfo["economy"].ToString();
                }
                results.Add(new BasicSystem(systemName, 0, coords, Allegiance, Economy));
            }

            json.Clear();

            OnEDSMDataDownloadComplete(this, results);

            results.Clear();

            // Unsubscribe all event listeners, otherwise shit gets crazy.
            EDSMWebClient.Dispose();
            this.OnEDSMDataDownloadComplete = null;
            this.OnEDSMSystemParseProgress = null;

        }

        public BasicSystem getSystemDataFromEDSMAPI(string text)
        {
            string apiURL = string.Format("{0}system", EDSM_API_URL);
            string urlWithParams = string.Format("{0}?systemName={1}&showCoordinates=1&showInformation=1", apiURL, text.Replace(" ", "%20"));

            WebClient wc = new WebClient();
            string _json = wc.DownloadString(urlWithParams);
#if DEBUG
            this.logger.Log("EDSM SYSTEM API RESPONSE: {0}", _json);
#endif
            if (_json.Equals("[]"))
            {
                throw new ArgumentException();
            }
            JObject json = JObject.Parse(_json);
            string name = json.GetValue("name").ToString();
            Dictionary<string, float> systemCoords = JsonConvert.DeserializeObject<Dictionary<string, float>>(json.GetValue("coords").ToString());
            Dictionary<string, object> systemInfo = new Dictionary<string, object>();
            try
            {
                systemInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.GetValue("information").ToString());
            }
            catch { }

            float x = systemCoords["x"];
            float y = systemCoords["y"];
            float z = systemCoords["z"];

            long systemID = 0;
            if (systemInfo.Keys.Count > 0)
            {
                try
                {
                    systemID = Convert.ToInt64(systemInfo["eddbId"]);
                }
                catch { }
            }

            SystemCoordinate sc = new SystemCoordinate(x, y, z);
            BasicSystem bs = new BasicSystem(name, systemID, sc);
            return bs;

        }

        public int getBodyPayout(string bodyType, bool isStar)
        {
            if (isStar)
            {
                switch (bodyType)
                {
                    case "SupermassiveBlackHole": // Sagittarius A*
                        return 628318;
                    // White dwarves
                    case "D":
                    case "DA":
                    case "DAB":
                    case "DAO":
                    case "DAZ":
                    case "DAV":
                    case "DB":
                    case "DBZ":
                    case "DBV":
                    case "DO":
                    case "DOV":
                    case "DQ":
                    case "DC":
                    case "DCV":
                    case "DX":
                        return 35000;
                    case "N": // Neutron
                        return 55000;
                    case "H": // Black hole
                        return 60000;
                    case "AeBe": // Herbig Ae/Be
                        return 3000;
                    // Carbon stars
                    case "C":
                    case "C-N":
                    case "CN":
                    case "C-J":
                    case "CJ":
                        return 2930;
                    // Wolf-Rayet
                    case "W":
                    case "WC":
                    case "WO":
                    case "WNC":
                    case "WN":
                        return 2930;
                    case "L": // Brown Dwarf, episode 1
                        return 2890;
                    case "TTS": // T Tauri
                        return 2900;
                    case "Y": // Brown dwarf, episode 2 & 3
                    case "T":
                        return 2880;
                    case "O":
                        return 6140;
                    case "B":
                        return 3000;
                    case "A":
                        return 2950;
                    case "F":
                        return 2930;
                    case "K":
                        return 2920;
                    case "G":
                        return 2920;
                    case "M":
                        return 2900;
                    case "A_BlueWhiteSuperGiant":
                    case "F_WhiteSuperGiant":
                    case "M_RedSuperGiant":
                    case "M_RedGiant":
                    case "K_OrangeGiant":
                        return 3120;
                    default: // Some mystery class we've never seen before :O thargoids.jpg
                        return 0;
                }
            }
            else
            {
                // hold on to your butts!
                bodyType = bodyType.Replace("Sudarsky class", "Class");
                switch (bodyType)
                {
                    case "High metal content body":
                        return 34310;
                    case "Terraformable High metal content body":
                        return 412250;
                    case "Water world":
                        return 301410;
                    case "Terraformable Water world":
                        return 694970;
                    case "Ammonia world":
                        return 230200;
                    case "Earthlike body":
                        return 627890;
                    case "Metal rich body":
                        return 65000;
                    case "Rocky body":
                        return 930;
                    case "Terraformable Rocky body":
                        return 181100;
                    case "Rocky ice body":
                    case "Icy body":
                        return 1250;
                    case "Water giant": // No idea if this is actually their designation.
                        return 1820;
                    // Gas giants
                    case "Class I gas giant":
                        return 7000;
                    case "Class II gas giant":
                        return 53670;
                    case "Class III gas giant":
                        return 2690;
                    case "Class IV gas giant":
                        return 2800;
                    case "Class V gas giant":
                        return 2100;
                    case "Gas giant with ammonia based life":
                        return 1720;
                    case "Helium rich gas giant": // Again, no idea if this is their actual designation
                        return 2100;
                    case "Gas giant with water based life":
                        return 2300;

                    default:
                        return 0;
                }
            }
        }

        public string getLandingPadPosition(int pad)
        {
            switch (pad)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    return "6 o'clock";
                case 5:
                case 6:
                case 7:
                case 8:
                    return "7 o'clock";
                case 9:
                case 10:
                    return "8 o'clock";
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return "9 o'clock";
                case 16:
                case 17:
                case 18:
                case 19:
                    return "10 o'clock";
                case 20:
                case 21:
                case 22:
                case 23:
                    return "11 o'clock";
                case 24:
                case 25:
                    return "12 o'clock";
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    return "1 o'clock";
                case 31:
                case 32:
                case 33:
                case 34:
                    return "2 o'clock";
                case 35:
                case 36:
                case 37:
                case 38:
                    return "3 o'clock";
                case 39:
                case 40:
                    return "4 o'clock";
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                    return "5 o'clock";
                default:
                    return "No idea!";
            }
        }

        public string getLandingPadDistance(int pad)
        {
            switch (pad)
            {
                case 1:
                case 2:
                case 5:
                case 6:
                case 9:
                case 11:
                case 12:
                case 16:
                case 17:
                case 20:
                case 21:
                case 24:
                case 26:
                case 27:
                case 31:
                case 32:
                case 35:
                case 36:
                case 39:
                case 41:
                case 42:
                    return "near";
                case 3:
                case 4:
                case 7:
                case 8:
                case 10:
                case 14:
                case 15:
                case 18:
                case 19:
                case 22:
                case 23:
                case 25:
                case 29:
                case 30:
                case 33:
                case 34:
                case 37:
                case 38:
                case 40:
                case 44:
                case 45:
                    return "far";
                default:
                    return "mid";
            }
        }
    }
}
