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

namespace EliteMonitor.Elite
{

    public class NoElementMatchesSymbolException : Exception { }

    public class EliteDatabase
    {

        public List<Material> Materials;
        public Dictionary<string, string> Ships;
        //public List<Commodity> Commodities; // Coming soon™
        public Dictionary<string, string> VoucherTypes = new Dictionary<string, string>()
        {
            { "bounty", "Bounty Voucher" },
            { "CombatBond", "Combat Bond" },
            { "settlement", "Intel Package" }
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
            Material m = this.Materials.First(a => a.InternalName.Equals(internalName));
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

    }
}
