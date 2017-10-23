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

        private WebClient EDSMWebClient = new WebClient();
        public event EventHandler<List<BasicSystem>> OnEDSMDataDownloadComplete;
        public event EventHandler<Int64[]> OnEDSMSystemParseProgress;

        public List<Material> Materials;
        public Dictionary<string, string> Ships;
        //public List<Commodity> Commodities; // Coming soon™
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
        public static EliteDatabase Instance;
        public Logger logger;
        private MainForm mainForm;

        private List<DownloadableDataItem> needsUpdate = new List<DownloadableDataItem>();

        public EliteDatabase(MainForm m)
        {
            this.mainForm = m;
            this.logger = new Logger("EliteDatabase");
            Instance = this;
            this.Materials = new List<Material>();
            this.Ships = new Dictionary<string, string>();
            //this.Commodities = new List<Commodity>();

            loadMaterials();
            loadShips();
            loadCommodities();

            runUpdatesIfNeeded();
        }

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
                this.Materials = JsonConvert.DeserializeObject<List<Material>>(sr.ReadToEnd());
            }
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

        public void loadCommodities()
        {
            /*using (StreamReader sr = new StreamReader(Path.Combine(MainForm.Instance.cacheController.dataPath, "commodities.json")))
            {
                this.Commodities = JsonConvert.DeserializeObject<List<Commodity>>(sr.ReadToEnd());
            }*/
        }

        public string getMaterialNameFromInternalName(string internalName)
        {
            Material m = new Material();
            try
            {
                m = this.Materials.First(a => a.InternalName.Equals(internalName));
            }
            catch { return internalName; }
            if (m == null) return internalName;
            return m.Name;
        }

        public string getInternalNameFromRealName(string realName)
        {
            Material m = this.Materials.First(a => a.Name.Equals(realName));
            if (m == null) return realName;
            return m.InternalName;
        }

        public Material getMaterialFromElementSymbol(string symbol)
        {
            Material m = this.Materials.First(a => a.Symbol.Equals(symbol));
            if (m == null) throw new NoElementMatchesSymbolException();
            return m;
        }

        public string getElementNameFromSymbol(string symbol)
        {
            try
            {
                Material m = getMaterialFromElementSymbol(symbol);
                return m.Name;
            }
            catch (NoElementMatchesSymbolException) { return symbol; }
        }

        public string getTypeForMaterialByInternalName(string internalName)
        {
            Material m = this.Materials.First(a => a.InternalName.Equals(internalName));
            if (m == null) return "Unknown";
            return m.Type;
        }

        public string getShipNameFromInternalName(string internalName)
        {
            internalName = internalName.ToLower();
            if (this.Ships.ContainsKey(internalName))
                return this.Ships[internalName];
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
                } catch { continue; }
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
                    case "Y": // Brown dwarf, episode 2
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
                    default: // Some mystery class we've never seen before :O thargoids.jpg
                        return 3120;
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

    }
}
