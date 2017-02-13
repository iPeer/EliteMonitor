﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EliteMonitor.Utilities;
using System.Drawing;
using EliteMonitor.Extensions;
using System.Globalization;
using System.IO;
using System.Threading;
using EliteMonitor.Elite;
using System.Windows.Forms;
using System.Diagnostics;
using EliteMonitor.Logging;

namespace EliteMonitor.Journal
{
    public class JournalParser
    {

        private MainForm mainForm;
        public Dictionary<string, int> _materialCounts = new Dictionary<string, int>();
        private FileSystemWatcher fileSystemWatcher;
        public Dictionary<string, Commander> commanders = new Dictionary<string, Commander>();
        public Commander activeCommander, viewedCommander;
        public Logger logger;

        public JournalParser(MainForm m)
        {
            this.logger = new Logger("JournalParser");
            this.mainForm = m;
            tailJournal(EliteUtils.JOURNAL_PATH);
        }

        public Commander registerCommander(string commanderName)
        {
            if (commanders.ContainsKey(commanderName))
            {
                Commander _commander = commanders[commanderName];
                activeCommander = _commander;
                return _commander;
            }
            else
            {
                Commander commander = new Commander(commanderName);
                //commander.Name = commanderName;
                activeCommander = commander;
                this.commanders.Add(commanderName, commander);
                return commander;
            }
        }

        public JournalEntry parseEvent(string json, out Commander commander)
        {
            /*
            { "timestamp":"2017-01-26T21:23:18Z", "event":"Fileheader", "part":1, "language":"English\\UK", "gameversion":"2.2", "build":"r131487/r0 " }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"LoadGame", "Commander":"iPeer", "Ship":"Cutter", "ShipID":11, "GameMode":"Group", "Group":"Mobius", "Credits":153430680, "Loan":0 }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"Rank", "Combat":5, "Trade":7, "Explore":5, "Empire":12, "Federation":7, "CQC":0 }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"Progress", "Combat":58, "Trade":27, "Explore":11, "Empire":3, "Federation":3, "CQC":0 }
            { "timestamp":"2017-02-03T04:26:40Z", "event":"MarketBuy", "Type":"superconductors", "Count":728, "BuyPrice":6883, "TotalCost":5010824 }
            { "timestamp":"2017-02-03T04:33:23Z", "event":"MarketSell", "Type":"superconductors", "Count":728, "SellPrice":7265, "TotalSale":5288920, "AvgPricePaid":6883 }
            */
            mainForm.cacheController.addJournalEntryToCache(json);
            commander = activeCommander ?? viewedCommander; // Under normal operating procedures, activeCommander will NEVER be null. It can be null during testing due to "hot inserting" of events (because we don't parse the full log properly), so if it's null, we default the the currently viewed commander.
            JObject j = JObject.Parse(json);
            string @event = (string)j["event"];
            DateTime tsData = (DateTime)j["timestamp"];
            string timestamp = tsData.ToString("G");


            // NOTES: Missions that rank up a player in a major power have RankFed or RamkEmp in the names. This is (currently) the ONLY way to detect a Fed/Empire rank-up

            try
            {

                switch (@event)
                {
                    case "Fileheader":
                        return new JournalEntry(timestamp, @event, "", j);
                    case "LoadGame":
                        bool isGroup = false;
                        try
                        {
                            isGroup = ((string)j["GameMode"]).Equals("Group");
                        }
                        catch { this.logger.Log("GameMode is not set."); }
                        string pGroup = "";
                        try
                        {
                            pGroup = (string)j["Group"];
                        }
                        catch { this.logger.Log("Private group is not set."); }

                        string commanderShip = (string)j["Ship"];
                        try
                        {
                            commanderShip = Ships.RealShipNames[commanderShip.ToLower()];
                        }
                        catch
                        {
                            this.logger.Log("No real name for ship: {0}", LogLevel.ERR, commanderShip == string.Empty || commanderShip == null ? "{!!empty string!!}" : commanderShip);
                            this.logger.Log("{0}", LogLevel.ERR, j.ToString());
                        }
                        long commanderCredits = (long)j["Credits"];
                        string commanderName = (string)j["Commander"];
                        string commanderString = $"{commanderName} | {commanderShip}" + (isGroup ? $" / {pGroup}" : "");
                        Commander _commander = registerCommander(commanderName);
                        _commander.setBasicInfo(commanderShip, commanderCredits, isGroup ? pGroup : "");
                        commander = activeCommander = viewedCommander = _commander;
                        return new JournalEntry(timestamp, @event, $"Commander {commanderString} | Ship: {commanderShip}, Credit balance: {String.Format("{0:n0}", commanderCredits)}", j);
                    case "Rank":

                        int c = (int)j["Combat"];
                        int t = (int)j["Trade"];
                        int e = (int)j["Explore"];
                        int q = (int)j["CQC"];
                        int fed = (int)j["Federation"];
                        int imp = (int)j["Empire"];

                        commander.setRanks(c, t, e, q, fed, imp);

                        return new JournalEntry(timestamp, @event, $"Combat: {commander.combatRankName}, Trading: {commander.tradeRankName}, Exploration: {commander.explorationRankName}, CQC: {commander.cqcRankName} | Federation: {commander.federationRankName}, Empire: {commander.imperialRankName}", j);
                    case "Progress":
                        int cp = (int)j["Combat"];
                        int tp = (int)j["Trade"];
                        int ep = (int)j["Explore"];
                        int fp = (int)j["Federation"];
                        int ip = (int)j["Empire"];
                        int qp = (int)j["CQC"];

                        commander.setRankProgress(cp, tp, ep, qp, fp, ip);

                        return new JournalEntry(timestamp, @event, $"Combat: {cp}, Trade: {tp}, Exploration: {ep}, CQC: {qp} | Federation: {fp}, Empire: {ip}", j);
                    case "MarketBuy":
                    case "MarketSell":
                        bool b = @event.Equals("MarketBuy");
                        string product = (string)j["Type"];
                        int count = (int)j["Count"];
                        long price = 0;
                        if (b)
                            price = (long)j["TotalCost"];
                        else
                            price = (long)j["TotalSale"];
                        string prefix = b ? "Bought" : "Sold";
                        if (b)
                            commander.deductCredits(price);
                        else
                            commander.addCredits(price);
                        return new JournalEntry(timestamp, @event, $"{prefix} {count} {product} for {String.Format("{0:n0}", price)} credits", j);
                    case "SendText":
                        string message = (string)j["Message"];
                        string to = (string)j["To"];
                        if (to.StartsWith("$cmdr_decorate"))
                            to = (string)j["To_Localised"];
                        return new JournalEntry(timestamp, @event, $"TO {to}: {message}", j);
                    case "ReceiveText":
                        string channel = (string)j["Channel"] ?? "-";
                        message = "";
                        string sender = (string)j["From"];
                        if (channel.Equals("player") || sender.StartsWith("$cmdr_decorate"))
                            message = (string)j["Message"];
                        else
                            message = (string)j["Message_Localised"];
                        if (sender.StartsWith("$npc_name_decorate") || sender.StartsWith("$cmdr_decorate") || sender.StartsWith("$ShipName"))
                            sender = (string)j["From_Localised"];
                        else if (sender.StartsWith("&"))
                            sender = sender.Substring(1);

                        return new JournalEntry(timestamp, @event, $"FROM {sender}: {message}", j);
                    case "DockingRequested":
                        return new JournalEntry(timestamp, @event, $"Requested docking at {(string)j["StationName"]}", j);
                    case "DockingGranted":
                        return new JournalEntry(timestamp, @event, $"Docking granted on pad {(string)j["LandingPad"]} at {(string)j["StationName"]}", j);
                    case "Docked":
                        return new JournalEntry(timestamp, @event, $"Docked at {(string)j["StationName"]} ({(string)j["StationType"]}) in {(string)j["StarSystem"]}", j);
                    case "Undocked":
                        return new JournalEntry(timestamp, @event, $"Undocked from {(string)j["StationName"]} ({(string)j["StationType"]})", j);
                    case "RefuelPartial": // Legacy
                    case "RefuelAll":
                        long cost = (long)j["Cost"];
                        float amount = (float)j["Amount"];
                        commander.deductCredits(cost);
                        return new JournalEntry(timestamp, @event, $"Refuelled {String.Format("{0:f2}", amount)} tonnes for {String.Format("{0:n0}", cost)} credits", j);
                    case "BuyAmmo":
                        cost = (long)j["Cost"];
                        commander.deductCredits(cost);
                        return new JournalEntry(timestamp, @event, $"Refilled ammunition for {String.Format("{0:n0}", cost)} credits", j);
                    case "FSDJump":
                        return new JournalEntry(timestamp, @event, $"Jumped to {(string)j["StarSystem"]} ({String.Format("{0:f2}", (float)j["JumpDist"])}Ly)", j);
                    case "RepairPartial": // Legacy
                    case "RepairAll":
                        cost = (long)j["Cost"];
                        commander.deductCredits(cost);
                        return new JournalEntry(timestamp, @event, $"Repaired ship for {String.Format("{0:n0}", cost)} credits", j);
                    case "SupercruiseExit":
                        string system = (string)j["StarSystem"];
                        string body = (string)j["Body"];
                        string bodyType = (string)j["BodyType"];
                        return new JournalEntry(timestamp, @event, $"Exited Supercruise in {system} near {body} ({bodyType})", j);
                    case "SupercruiseEntry":
                        system = (string)j["StarSystem"];
                        return new JournalEntry(timestamp, @event, $"Entered Supercruise in {system}", j);
                    case "ShieldState":
                        bool s = (bool)j["ShieldsUp"];
                        return new JournalEntry(timestamp, @event, String.Format("Shields {0}.", s ? "ONLINE" : "OFFLINE"), j);
                    case "MaterialCollected":
                        string material = (string)j["Name"];
                        count = (int)j["Count"];
                        commander.addMaterial(material, count);
                        /*if (_materialCounts.ContainsKey(material))
                        {
                            _materialCounts[material] += count;
                        }
                        else
                            _materialCounts.Add(material, count);*/
                        return new JournalEntry(timestamp, @event, $"Collected {material}", j);
                    case "MaterialDisgarded":
                        material = (string)j["Name"];
                        count = (int)j["Count"];
                        commander.removeMaterial(material, count);
                        //_materialCounts[material] -= count;
                        return new JournalEntry(timestamp, @event, $"Disgarded {material}", j);
                    case "MissionCompleted":
                        //Console.WriteLine(j.ToString());
                        bool donate = j["Reward"] == null;
                        long credits = donate ? (long)j["Donation"] : (long)j["Reward"];
                        if (donate)
                            commander.deductCredits(credits);
                        else
                            commander.addCredits(credits);
                        prefix = donate ? "Donated" : "Rewarded";
                        return new JournalEntry(timestamp, @event, $"Completed mission. {prefix} {String.Format("{0:n0}", credits)} credits.", j);
                    case "FuelScoop":
                        return new JournalEntry(timestamp, @event, $"Scooped {String.Format("{0:f2}", (float)j["Scooped"])} tonnes of fuel.", j);
                    case "ModuleRetrieve":
                        string shipname = (string)j["Ship"];
                        try
                        {
                            shipname = Ships.RealShipNames[shipname];
                        }
                        catch { }
                        return new JournalEntry(timestamp, @event, String.Format("Transferred module '{0}' to {1}", (string)j["RetrievedItem_Localised"], shipname), j);
                    case "EjectCargo":
                        string cargo = (string)j["Type"];
                        count = (int)j["Count"];
                        bool abandoned = (bool)j["Abandoned"];
                        return new JournalEntry(timestamp, @event, String.Format("{0} {1} {2}", abandoned ? "Abandoned" : "Ejected", count, cargo), j);
                    // { "timestamp":"2017-02-11T20:21:42Z", "event":"Promotion", "Combat":5 }
                    case "Promotion":
                        JProperty jt = j.Last as JProperty;
                        string skill = jt.Name;
                        int rank = Convert.ToInt32(jt.Value);
                        string realRank = Ranks.rankNames[skill.ToLower()][rank];
                        commander.applyPromotion(skill, rank);
                        return new JournalEntry(timestamp, @event, $"Promoted to {realRank} in {skill.Replace("Explore", "Exploration")}", j);
                    default:
                        return new JournalEntry(timestamp, @event, j.ToString(), "UNKNOWN EVENT", j, false);
                }
            }
            catch (Exception e)
            {
                this.logger.Log("Exception while parsing event '{1}': {0}", LogLevel.ERR, e.Message, @event);
                this.logger.Log("{0}", LogLevel.ERR, e.StackTrace);
                throw new Exception(e.Message, e);
            }
           
        }

        public void parseAllJournals()
        {
            string journalPath = EliteUtils.JOURNAL_PATH;
            journalPath = Environment.ExpandEnvironmentVariables(journalPath);
            /*string[] files = Directory.GetFiles(journalPath);
            files.OrderBy(p => new FileInfo(p).CreationTime);*/
            /*foreach (string _f in files)
            {
                Console.WriteLine(_f);
            }*/
            DirectoryInfo di = new DirectoryInfo(journalPath);
            FileInfo[] fileInfo = di.GetFiles().OrderBy(f => f.CreationTime).ToArray();
            mainForm.cacheController._journalLengthCache.Clear();
            List<string> allJournalEntries = new List<string>();
            foreach (FileInfo _file in fileInfo)
            {
                string file = _file.FullName;
                mainForm.cacheController._journalLengthCache.Add(_file.Name, _file.Length);
                mainForm.appStatus.Text = $"Parsing Journal file: {file}...";
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            //parseEvent(line);
                            allJournalEntries.Add(line);
                        }
                        
                    }
                }
            }
            createJournalEntries(allJournalEntries);
            mainForm.appStatus.Text = "Switching commander...";
            switchViewedCommander(activeCommander);
            mainForm.appStatus.Text = "Ready.";
        }

        public void createJournalEntries(List<string> entries, bool checkDuplicates = false)
        {
            mainForm.appStatus.Text = "Generating Journal entries...";
            int cEntry = 0;
            int lastPercent = 0;
            foreach (string s in entries)
            {
                double percent = ((double)cEntry++ / (double)entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if (percent > lastPercent)
                {
                    lastPercent = (int)percent;
                    mainForm.appStatus.Text = String.Format("Generating Journal entries... ({0:n0}%)", percent);
                }
                Commander commander;
                JournalEntry je = parseEvent(s, out commander);
                if (je.Event.Equals("Fileheader"))
                    continue;
                if (!mainForm.comboCommanderList.Items.Contains(commander.Name))
                {
                    mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.Items.Add(commander.Name));
                    commander.JournalEntries = new List<JournalEntry>(entries.Count);
                }
                /*if (!commander.Journal.Contains(s))
                    commander.Journal.Add(je.Json);*/
                /*if (!checkDuplicates || (checkDuplicates && !commander.JournalEntries.Contains(je))) // Holy performance killer
                {
                    if (!commander.EventsList.Contains(je.Event))
                        commander.EventsList.Add(je.Event);
                    je.ID = commander.nextId++;
                    commander.JournalEntries.Add(je);
                }*/
                commander.addJournalEntry(je, checkDuplicates);
            }
        }

        public void switchViewedCommander(string commander)
        {
            this.logger.Log("Commander switch to commander by name '{0}'", commander);
            switchViewedCommander(commanders[commander]);
        }

        public void switchViewedCommander(Commander c)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            mainForm.setAppStatusText(String.Format("Loading commander data for '{0}'", c.Name));
            this.logger.Log("Switching viewed commander to '{0}'", c.Name);
            /*if (c.Journal.Count > 0 && c.JournalEntries.Count == 0)
            {
                this.logger.Log("Journal entries for commander '{0}' are not loaded into memory, loading them in...", c.Name, c.Journal.Count, c.JournalEntries.Count);
                createJournalEntries(c.Journal);
            }*/
            this.viewedCommander = c;
            List<JournalEntry> _entries = c.JournalEntries;
            List<ListViewItem> entries = new List<ListViewItem>();
            int x = 0;
            int lastPercent = 0;
            foreach (JournalEntry j in _entries)
            {
                double percent = ((double)x++ / (double)_entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if (percent > lastPercent)
                {
                    lastPercent = (int)percent;
                    mainForm.appStatus.Text = String.Format("Loading commander data for '{0}' ({1:n0}%)", c.Name, percent);
                }

                /*if (Properties.Settings.Default.showJournalUpdateStatus && (x++ == 1 || x % 100 == 0 || x == _entries.Count))
                    mainForm.appStatus.Text = $"Processing Journal entry {String.Format("{0:n0}", x)} of {String.Format("{0:n0}", _entries.Count)}";*/
                ListViewItem lvi = getListViewEntryForEntry(j);
                entries.Add(lvi);
            }
            mainForm.appStatus.Text = String.Format("Finalising commander data for '{0}'", c.Name);
            entries.Reverse();
            mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.SelectedIndex = mainForm.comboCommanderList.Items.IndexOf(c.Name));
            mainForm.eventList.InvokeIfRequired(() =>
            {
                mainForm.eventList.BeginUpdate();
                mainForm.eventList.Items.Clear();
                mainForm.eventList.Items.AddRange(entries.ToArray());
                foreach (ColumnHeader ch in mainForm.eventList.Columns)
                {
                    ch.Width = -2;
                }

                mainForm.eventList.EndUpdate();
            });
            c.updateDialogDisplays(this.mainForm);
            c.isViewed = true;
            foreach (Commander _c in this.commanders.Values)
            {
                if (_c != viewedCommander)
                    _c.isViewed = false;
            }
            sw.Stop();
            this.logger.Log("Commander Switch completed in {0:n0}ms", sw.ElapsedMilliseconds);
            this.logger.Log("Commander {0} has {1:n0} journal entries", c.Name, c.JournalEntries.Count);
            /*this.logger.Log("Saving commander data...");
            foreach(Commander _c in this.commanders.Values)
            {
                _c.saveData();
            }*/
            mainForm.setAppStatusText("Ready.");
        }

        public ListViewItem getListViewEntryForEntry(JournalEntry j)
        {
            ListViewItem lvi = new ListViewItem(new string[] { j.Timestamp, j.Event, j.Data, j.Notes });
            lvi.ToolTipText = j.Json;
            if (!j.isKnown)
            {
                lvi.BackColor = Color.Pink;
                lvi.SubItems[3].Text = "UNKNOWN EVENT";
            }
            else if (j.Event.Equals("LoadGame"))
                lvi.BackColor = Color.LightGreen;
            return lvi;
        }

        public void tailJournal(string path)
        {
            /*Thread t = new Thread(() =>
            {
                while (true)
                {
                    FileInfo fi = new FileInfo(@"C:\Users\iPeer\Saved Games\Frontier Developments\Elite Dangerous\Journal.170204163640.01.log");
                    Console.WriteLine("--> " + fi.Length);
                    Thread.Sleep(60000);
                }
            });
            t.Start();*/
            this.logger.Log($"Setting up tailer on logfile {path}...");
            path = Environment.ExpandEnvironmentVariables(path);

            fileSystemWatcher = new FileSystemWatcher(path);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite/* | NotifyFilters.Size*/;
            fileSystemWatcher.Changed += fileChanged;
            fileSystemWatcher.Filter = "Journal*.log";
            fileSystemWatcher.EnableRaisingEvents = true;
            mainForm.appStatus.Text = "Tailing started";
        }

        internal void readFileFromByteOffset(FileSystemEventArgs e, long offset = 0)
        {
            readFileFromByteOffset(e.FullPath, e.Name, offset);
        }

        internal void readFileFromByteOffset(string file, long offset = 0)
        {
            FileInfo fi = new FileInfo(file);
            readFileFromByteOffset(file, fi.Name, offset);
        }

        private void readFileFromByteOffset(string file, string fileName, long offset = 0)
        {

            // TODO: Update (properly) for multi-commander(?)
            // { "timestamp":"2017-02-13T16:55:19Z", "event":"TestEvent", "Data":"TEST_EVENT" }

            this.logger.Log($"Reading file {file} ({fileName}) from offset {offset}");
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long byteOffset;
                mainForm.cacheController._journalLengthCache.TryGetValue(fileName, out byteOffset);
                if (byteOffset > 0)
                    fs.Seek(byteOffset, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        mainForm.cacheController._journalLengthCache[fileName] = fs.Position;
                        Commander c;
                        JournalEntry j = parseEvent(s, out c);
                        c.addJournalEntry(j);
                        Console.WriteLine(c.Name);
                        /*if (viewedCommander == c)
                        {
                            ListViewItem lvi = getListViewEntryForEntry(j);
                            mainForm.eventList.InvokeIfRequired(() => mainForm.eventList.Items.Insert(0, lvi));
                        }*/
                    }
                }
            }
        }

        internal void fileChanged(object source, FileSystemEventArgs e)
        {
            long byteOffset;
            mainForm.cacheController._journalLengthCache.TryGetValue(e.Name, out byteOffset);
            readFileFromByteOffset(/*e.FullPath, e.Name, */e, byteOffset);

            /*using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long byteOffset;
                _lastJournalBytes.TryGetValue(e.Name, out byteOffset);
                if (byteOffset > 0)
                    fs.Seek(byteOffset, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        _lastJournalBytes[e.Name] = fs.Position;
                        MainForm.Instance.journalParser.parseEvent(s);
                    }
                }
            }*/
        }
    }
}
