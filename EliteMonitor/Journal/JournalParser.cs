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
using System.Media;
using EliteMonitor.Exploration;
using EliteMonitor.Notifications;

namespace EliteMonitor.Journal
{

    public class NoRegisteredCommanderException : Exception { }
    public class FragmentedJsonStringException : Exception { }
    public class InvalidJSONException : Exception { }

    public class JournalParser
    {

        private MainForm mainForm;
        private FileSystemWatcher fileSystemWatcher;
        public Dictionary<string, Commander> commanders = new Dictionary<string, Commander>();
        public Commander activeCommander, viewedCommander;
        public Logger logger;
        private bool fullParseInProgress = false;
        private string currentTailFile = "";
        private bool hasChangedLogFile = false;
        private bool tailerRunning = false;
        private Thread journalTailThread;
        private bool abortJournalTailing = false;
        private bool fileWatcherRunning = false;
        private bool betaJournal = false;
        private bool isCommanderRegistered = false;
        private DateTime lastJumpStartTime = DateTime.MinValue;
        public string LastParsedJson = string.Empty;
        private int scanEntriesToIgnore = 0;
        public bool noCommandersOrJournals = false;
        private string JsonToAppend = string.Empty;

        List<ParsableJournalEntry> preLoadCommanderData = new List<ParsableJournalEntry>(3);

        public JournalParser(MainForm m)
        {
            this.logger = new Logger("JournalParser");
            this.mainForm = m;
        }

        public void startTailer()
        {
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

        public JournalEntry parseEvent(string json, out Commander commander, bool isReparse = false, Commander forcedCommander = null, bool doNotPlaySounds = false, bool isBeta = false, bool bypassRegisterCheck = false, bool showNotifications = true)
        {
            /*
            { "timestamp":"2017-01-26T21:23:18Z", "event":"Fileheader", "part":1, "language":"English\\UK", "gameversion":"2.2", "build":"r131487/r0 " }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"LoadGame", "Commander":"iPeer", "Ship":"Cutter", "ShipID":11, "GameMode":"Group", "Group":"Mobius", "Credits":153430680, "Loan":0 }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"Rank", "Combat":5, "Trade":7, "Explore":5, "Empire":12, "Federation":7, "CQC":0 }
            { "timestamp":"2017-01-26T21:24:47Z", "event":"Progress", "Combat":58, "Trade":27, "Explore":11, "Empire":3, "Federation":3, "CQC":0 }
            { "timestamp":"2017-02-03T04:26:40Z", "event":"MarketBuy", "Type":"superconductors", "Count":728, "BuyPrice":6883, "TotalCost":5010824 }
            { "timestamp":"2017-02-03T04:33:23Z", "event":"MarketSell", "Type":"superconductors", "Count":728, "SellPrice":7265, "TotalSale":5288920, "AvgPricePaid":6883 }
            */
            //mainForm.cacheController.addJournalEntryToCache(json);
            this.LastParsedJson = json;
            // Fix EM crashing when it comes across fragmented JSON lines
            if (!json.TrimEnd().EndsWith("}"))
            {
                throw new FragmentedJsonStringException();
            }
            if (forcedCommander == null)
                commander = activeCommander ?? viewedCommander; // Under normal operating procedures, activeCommander will NEVER be null. It can be null during testing due to "hot inserting" of events (because we don't parse the full log properly), so if it's null, we default the the currently viewed commander.
            else
                commander = forcedCommander;
            JObject j;
            try
            {
                j = JObject.Parse(json);
            }
            catch (JsonReaderException _e)
            {
                this.logger.Log("Supplied JSON is not valid JSON!", LogLevel.ERROR);
                this.logger.Log("Supplied JSON: '{0}'", LogLevel.ERROR, json);
                this.logger.Log("{0}", LogLevel.ERROR, _e.ToString());
                throw new InvalidJSONException();
            }
            catch (Exception e)
            {
                throw e;
            }
            string @event = (string)j["event"];
            DateTime tsData = (DateTime)j["timestamp"];
            string timestamp = tsData.ToString("G");


            if (!bypassRegisterCheck && (!new string[] { "Fileheader", "LoadGame" }.Contains(@event) && !this.isCommanderRegistered))
            {
                //Console.WriteLine($"- {@event}");
                throw new NoRegisteredCommanderException();
            }
            //Console.WriteLine($"+ {@event}");

            switch (@event)
            {
                case "Fileheader":
                    this.betaJournal = j.GetValue("gameversion").ToString().Contains("Beta");
                    this.isCommanderRegistered = !j.GetValue("part").ToString().Equals("1");
                    return new JournalEntry(timestamp, @event, "", j);
                case "Commander":
                    string commanderName = j.GetValue("Name").ToString();
                    if (isBeta || this.betaJournal)
                        commanderName += " [BETA]";
                    Commander _commander = registerCommander(commanderName);
                    this.isCommanderRegistered = true;
                    return new JournalEntry(timestamp, @event, string.Format("Welcome back, CMDR {0}.", commanderName), j);
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
                        commanderShip = mainForm.Database.getShipNameFromInternalName(commanderShip.ToLower());
                    }
                    catch
                    {
                        this.logger.Log("No real name for ship: {0}", LogLevel.ERR, commanderShip == string.Empty || commanderShip == null ? "{!!empty string!!}" : commanderShip);
                        this.logger.Log("{0}", LogLevel.ERR, j.ToString());
                    }
                    long commanderCredits = (long)j["Credits"];
                    commanderName = (string)j["Commander"];
                    if (isBeta || this.betaJournal)
                        commanderName += " [BETA]";
                    string shipName = (string)j["ShipName"];
                    string shipID = (string)j["ShipIdent"];
                    string commanderString = $"{commanderName}" + (isGroup ? $" ({pGroup})" : "");

                    if (!this.isCommanderRegistered)
                    {
                        _commander = registerCommander(commanderName);
                        commander = activeCommander = _commander;
                        this.isCommanderRegistered = true;
                    }
                    commander.setBasicInfo(commanderShip, commanderCredits, isGroup ? pGroup : "");
                    commander.SetShip(commanderShip, shipID, shipName);
                    string commanderShipFormatted = commander.ShipData.getFormattedShipString();
                    commander.isInMulticrew = false;
                    return new JournalEntry(timestamp, @event, $"Commander {commanderString} | {commanderShipFormatted} | Credit balance: {String.Format("{0:n0}", commanderCredits)}", j);
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
                    if (!isReparse)
                    {
                        if (b)
                            commander.deductCredits(price);
                        else
                            commander.addCredits(price);
                    }
                    string productName = mainForm.Database.getCommodityNameFromInternal(product);
                    return new JournalEntry(timestamp, @event, $"{prefix} {count} {productName} for {String.Format("{0:n0}", price)} credits", j);
                case "SendText":
                    string message = (string)j["Message"];
                    string to = (string)j["To"];
                    if (to.StartsWith("$cmdr_decorate"))
                        to = (string)j["To_Localised"];
                    return new JournalEntry(timestamp, @event, $"TO {to}: {message}", j);
                case "ReceiveText": // 1.0.0.1770 - If frontier could stop changing the format of the journal, that'd be fucking great.
                    string channel = (string)j["Channel"] ?? "-";
                    string sender = (string)j["From"];
                    message = string.Empty;
                    /*if (channel.Equals("player") || sender.StartsWith("$cmdr_decorate") || channel.Equals("wing"))
                        message = (string)j["Message"];
                    else
                        message = (string)j["Message_Localised"];*/

                    /*message = j.GetValue("Message").ToString();
                    if (message.StartsWith("$"))
                        message = j.GetValue("Message_Localised").ToString();*/
                    if (sender.StartsWith("$npc_name_decorate") || sender.StartsWith("$cmdr_decorate") || sender.StartsWith("$ShipName"))
                        sender = (string)j["From_Localised"];
                    else if (sender.StartsWith("&"))
                        sender = sender.Substring(1);
                    if (channel.Equals("local") && !sender.StartsWith("CMDR "))
                        sender = "CMDR " + sender;
                    if (channel.Equals("npc"))
                    {
                        try
                        {
                            message = j.GetValue("Message_Localised").ToString();
                        }
                        catch
                        {
                            message = j.GetValue("Message").ToString();
                        }
                    }
                    else
                        message = j.GetValue("Message").ToString();

                    if (string.IsNullOrWhiteSpace(sender))
                        sender = "UNKNOWN SENDER";

                    return new JournalEntry(timestamp, @event, $"FROM {sender}: {message}", j);
                case "DockingRequested":
                    return new JournalEntry(timestamp, @event, $"Requested docking at {(string)j["StationName"]}", j);
                case "DockingGranted":
                    int landingPad = j.GetValue("LandingPad").ToObject<int>();
                    //   "StationType": "Coriolis"
                    JToken stationType;
                    bool hasStationType = j.TryGetValue("StationType", out stationType);
                    if (showNotifications && Properties.Settings.Default.NotificationsEnabled && Properties.Settings.Default.DockingNotifications && hasStationType && (new string[] { "Coriolis", "Orbis", "Ocellus" }).Contains(stationType.ToString()))
                    {
                        string padTime = MainForm.Instance.Database.getLandingPadPosition(landingPad);
                        string padDist = MainForm.Instance.Database.getLandingPadDistance(landingPad);
                        Notification n = new Notification("Docking granted", $"Greens on the right, pad {landingPad} located at {padTime} {padDist}", 15);
                        Utils.InvokeNotification(n);
                    }
                    return new JournalEntry(timestamp, @event, $"Docking granted on pad {landingPad} at {(string)j["StationName"]}", j);
                case "Docked":
                    string stationName = (string)j["StationName"];
                    stationType = (string)j["StationType"];
                    string starSystem = (string)j["StarSystem"];
                    if (!isReparse)
                    {
                        commander.isDocked = true;
                        commander.CurrentSystem = starSystem;
                        commander.CurrentLocation = stationName;
                        if (this.viewedCommander != null)
                            mainForm.UpdateThargoidEasterEggIfRequired();
                    }
                    string eventText = String.Format("Docked at {0}{1} in {2}", stationName, stationType == null || stationType.Equals(string.Empty) ? "" : $" ({stationType})", starSystem);
                    return new JournalEntry(timestamp, @event, eventText, j);
                case "Undocked":
                    stationName = (string)j["StationName"];
                    stationType = (string)j["StationType"];
                    if (!isReparse)
                    {
                        commander.isDocked = false;
                    }
                    eventText = String.Format("Undocked from {0}{1}", stationName, stationType == null || stationType.Equals(string.Empty) ? "" : $" ({stationType})");
                    return new JournalEntry(timestamp, @event, eventText, j);
                case "RefuelPartial": // Legacy
                case "RefuelAll":
                    long cost = (long)j["Cost"];
                    float amount = (float)j["Amount"];
                    if (!isReparse)
                        commander.deductCredits(cost);
                    return new JournalEntry(timestamp, @event, $"Refuelled {String.Format("{0:f2}", amount)} tonnes for {String.Format("{0:n0}", cost)} credits", j);
                case "BuyAmmo":
                    cost = (long)j["Cost"];
                    if (!isReparse)
                        commander.deductCredits(cost);
                    return new JournalEntry(timestamp, @event, $"Refilled ammunition for {String.Format("{0:n0}", cost)} credits", j);
                case "FSDJump":
                    if (!isReparse)
                    {
                        commander.CurrentSystem = (string)j["StarSystem"];
                        commander.CurrentLocation = string.Empty;
                        List<JToken> coords = j["StarPos"].ToList();
                        float X = coords[0].ToObject<float>();
                        float Y = coords[1].ToObject<float>();
                        float Z = coords[2].ToObject<float>();

                        SystemCoordinate sc = new SystemCoordinate(X, Y, Z);

                        commander.CurrentSystemCoordinates = sc;
                        commander.isLanded = commander.isDocked = false;
                        JToken addr;
                        if (j.TryGetValue("SystemAddress", out addr))
                        {
                            commander.LastSystemAddress = addr.ToObject<long>();
                        }
                    }
                    string Append = string.Empty;
                    if (this.lastJumpStartTime > DateTime.MinValue)
                    {
                        string JumpDuration = Utils.formatTimeFromSeconds((int)(DateTime.Parse(timestamp) - this.lastJumpStartTime).TotalSeconds - 5);
                        Append = $", {JumpDuration}";
                        this.lastJumpStartTime = DateTime.MinValue;
                    }
                    return new JournalEntry(timestamp, @event, $"Jumped to {(string)j["StarSystem"]} ({String.Format("{0:f2}", (float)j["JumpDist"])}Ly{Append})", j);
                case "RepairPartial": // Legacy
                case "RepairAll":
                    cost = (long)j["Cost"];
                    if (!isReparse)
                        commander.deductCredits(cost);
                    return new JournalEntry(timestamp, @event, $"Repaired ship for {String.Format("{0:n0}", cost)} credits", j);
                case "SupercruiseExit":
                    string system = (string)j["StarSystem"];
                    string body = (string)j["Body"];
                    string bodyType = (string)j["BodyType"];
                    if (!isReparse)
                    {
                        commander.CurrentSystem = system;
                        commander.CurrentLocation = body;
                    }
                    return new JournalEntry(timestamp, @event, $"Exited Supercruise in {system} near {body} ({bodyType})", j);
                case "SupercruiseEntry":
                    system = (string)j["StarSystem"];
                    if (!isReparse)
                    {
                        commander.CurrentSystem = system;
                        commander.CurrentLocation = string.Empty;
                    }
                    return new JournalEntry(timestamp, @event, $"Entered Supercruise in {system}", j);
                case "ShieldState":
                    bool s = (bool)j["ShieldsUp"];
                    return new JournalEntry(timestamp, @event, String.Format("Shields {0}.", s ? "ONLINE" : "OFFLINE"), j);
                case "MaterialCollected":
                    string material = (string)j["Name"];
                    count = (int)j["Count"];
                    string category = (string)j["Category"];
                    if (!isReparse)
                        commander.addMaterial(material, count);
                    if (mainForm.MaterialsGUIOpen)
                        MaterialList.Instance.DisplayMaterials();
                    if (!isReparse && showNotifications && Properties.Settings.Default.ShowMaterialCountNotifications)
                    {
                        Notification n = new Notification("Material Count Update", string.Format("{0}: {1:n0}", mainForm.Database.getMaterialNameFromInternal(material), commander.Materials[material]));
                        Utils.InvokeNotification(n);
                    }
                    //mainForm.Database.saveMaterialTypeToDatabase(material, category);
                    return new JournalEntry(timestamp, @event, String.Format("Collected. {0} : {1} ({2})", category, mainForm.Database.getMaterialNameFromInternal(material), count), j);
                case "MaterialDiscovered":
                    material = (string)j["Name"];
                    category = (string)j["Category"];
                    //mainForm.Database.saveMaterialTypeToDatabase(material, category);
                    if (mainForm.MaterialsGUIOpen)
                        MaterialList.Instance.DisplayMaterials();
                    return new JournalEntry(timestamp, @event, String.Format("Discovered new material: {0} : {1}", category, mainForm.Database.getMaterialNameFromInternal(material)), j);
                case "MaterialDiscarded":
                    material = (string)j["Name"];
                    count = (int)j["Count"];
                    if (!isReparse)
                        commander.removeMaterial(material, count);
                    if (mainForm.MaterialsGUIOpen)
                        MaterialList.Instance.DisplayMaterials();
                    if (!isReparse && showNotifications && Properties.Settings.Default.ShowMaterialCountNotifications)
                    {
                        Notification n = new Notification("Material Count Update", string.Format("{0}: {1:n0}", mainForm.Database.getMaterialNameFromInternal(material), commander.Materials[material]));
                        Utils.InvokeNotification(n);
                    }
                    string matOutString = String.Format("Discarded. {0} : {1} ({2})", mainForm.Database.getMaterialTypeFromInternalName(material), mainForm.Database.getMaterialNameFromInternal(material), count);
                    return new JournalEntry(timestamp, @event, matOutString, j);
                case "MissionCompleted":
                    bool donate = j["Reward"] == null;
                    long credits = donate ? (long)j["Donation"] : (long)j["Reward"];
                    if (!isReparse)
                    {
                        if (donate)
                            commander.deductCredits(credits);
                        else
                            commander.addCredits(credits);
                    }
                    prefix = donate ? "Donated" : "Rewarded";
                    return new JournalEntry(timestamp, @event, $"Completed mission. {prefix} {String.Format("{0:n0}", credits)} credits.", j);
                case "FuelScoop":
                    return new JournalEntry(timestamp, @event, $"Scooped {String.Format("{0:f2}", (float)j["Scooped"])} tonnes of fuel.", j);
                case "ModuleRetrieve":
                    string shipname = (string)j["Ship"];
                    string module = (string)j["RetrievedItem_Localised"];
                    string moduleInternal = (string)j["RetrievedItem"];
                    string[] moduleClassandSize = EliteUtils.getSizeAndClassFromInternalName(moduleInternal, true);
                    bool hasStored = true;
                    string moduleStored, moduleStoredInternal, fullStoredModuleName = string.Empty;
                    string[] moduleStoredClassandSize;
                    try
                    {
                        moduleStored = (string)j["SwapOutItem_Localised"];
                        moduleStoredInternal = (string)j["SwapOutItem"];
                        moduleStoredClassandSize = EliteUtils.getSizeAndClassFromInternalName(moduleStoredInternal, true);
                        fullStoredModuleName = string.Format("{0} {1}", string.Join("", moduleStoredClassandSize), moduleStored);
                    }
                    catch { hasStored = false; }

                    try
                    {
                        shipname = mainForm.Database.getShipNameFromInternalName(shipname);
                    }
                    catch { }
                    string fullModuleName = string.Format("{0} {1}", string.Join("", moduleClassandSize), module);
                    string eText = string.Empty;
                    if (hasStored)
                        eText = string.Format("Swapped '{0}' with '{1}' in your {2}", fullStoredModuleName, fullModuleName, shipname);
                    else
                        eText = String.Format("Transferred module '{0}' to your {1}", fullModuleName, shipname);
                    return new JournalEntry(timestamp, @event, eText, j);
                case "ModuleStore":
                    shipname = (string)j["Ship"];
                    module = (string)j["StoredItem_Localised"];
                    moduleInternal = (string)j["StoredItem"];
                    moduleClassandSize = EliteUtils.getSizeAndClassFromInternalName(moduleInternal, true);
                    try
                    {
                        shipname = mainForm.Database.getShipNameFromInternalName(shipname);
                    }
                    catch { }
                    fullModuleName = string.Format("{0} {1}", string.Join("", moduleClassandSize), module);
                    return new JournalEntry(timestamp, @event, String.Format("Stored module '{0}' from your {1}", fullModuleName, shipname), j);
                case "ModuleBuy":
                    bool partExchange = true;
                    string soldModule = string.Empty, soldModuleFull = string.Empty;
                    long soldModulePrice = 0;
                    try
                    {
                        soldModulePrice = (long)j["SellPrice"];
                        soldModule = (string)j["SellItem_Localised"];
                        string sMI = (string)j["SellItem"];
                        string[] sMICS = EliteUtils.getSizeAndClassFromInternalName(sMI, true);
                        soldModuleFull = string.Format("{0} {1}", string.Join("", sMICS), soldModule);
                    }
                    catch { partExchange = false; }

                    long buyModulePrice = (long)j["BuyPrice"];
                    string buyModule = (string)j["BuyItem_Localised"];
                    string bMI = (string)j["BuyItem"];
                    string[] bMICS = EliteUtils.getSizeAndClassFromInternalName(bMI, true);
                    string buyModuleFull = string.Format("{0} {1}", string.Join("", bMICS), buyModule);

                    long totalCost = buyModulePrice - soldModulePrice;

                    if (!isReparse)
                    {
                        if (totalCost < 0L || !partExchange)
                            commander.deductCredits(Math.Abs(totalCost));
                        else
                            commander.addCredits(totalCost);
                    }

                    if (partExchange)
                        eText = string.Format("Traded in '{0}' for '{1}'. {3}: {2:n0}", soldModuleFull, buyModuleFull, Math.Abs(totalCost), totalCost < 0L ? "Gain" : "Loss");
                    else
                        eText = string.Format("Purchased '{0}' for {1:n0} credits", buyModuleFull, totalCost);

                    return new JournalEntry(timestamp, @event, eText, j);
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
                case "FactionKillBond":
                    int value = (int)j["Reward"];
                    string issuingFaction = (string)j["AwardingFaction"];
                    string againstFaction = (string)j["VictimFaction"];
                    eText = string.Format("Combat Bond valued at {0:n0} awarded from {1} for destruction of ship from {2}.", value, issuingFaction, againstFaction);
                    return new JournalEntry(timestamp, @event, eText, j);
                case "Bounty":
                    try
                    {
                        value = (int)j["TotalReward"];
                    }
                    catch { value = (int)j["Reward"]; }
                    againstFaction = (string)j["VictimFaction"];
                    string ship = string.Empty;
                    try
                    {
                        ship = (string)j["Target"];
                    }
                    catch { }
                    if (string.IsNullOrEmpty(ship))
                        eText = string.Format("Bounty credit of {0:n0} awarded.", value);
                    else
                        eText = string.Format("Bounty credit of {0:n0} awarded for destruction of {1} {2}.", value, againstFaction, mainForm.Database.getShipNameFromInternalName(ship));
                    return new JournalEntry(timestamp, @event, eText, j);
                case "RedeemVoucher":
                    value = (int)j["Amount"];
                    string voucherType = (string)j["Type"];
                    try
                    {
                        voucherType = mainForm.Database.VoucherTypes[voucherType];
                    }
                    catch { }
                    if (!isReparse)
                        commander.addCredits(value);
                    eText = string.Format("Redeemed {0}s valued at {1:n0}", voucherType, value);
                    return new JournalEntry(timestamp, @event, eText, j);
                case "HeatWarning":
                    return new JournalEntry(timestamp, @event, "Heat levels critical!", j);
                case "HullDamage":
                    bool fighter = false;
                    string hullDmgString = string.Format("Took hull damage! Hull health: {0:n0}%", (float)j["Health"] * 100.00);
                    try { fighter = j.GetValue("Fighter").ToObject<bool>(); if (fighter) { hullDmgString = string.Format("Fighter took hull damage! Fighter's hull health: {0:n0}%", (float)j["Health"] * 100.00); } }
                    catch { }
                    return new JournalEntry(timestamp, @event, hullDmgString, j);
                case "ShipyardSwap":
                    string oldShip = (string)j["StoreOldShip"];
                    string newShip = (string)j["ShipType"];
                    eText = string.Format("Switch from ship {0} to {1}.", mainForm.Database.getShipNameFromInternalName(oldShip), mainForm.Database.getShipNameFromInternalName(newShip));
                    if (!isReparse)
                        commander.SetShip(mainForm.Database.getShipNameFromInternalName(newShip), "", "");
                    return new JournalEntry(timestamp, @event, eText, j);
                case "StartJump": // { "timestamp":"2017-04-29T01:28:25Z", "event":"StartJump", "JumpType":"Hyperspace", "StarSystem":"CD-77 1073", "StarClass":"K" }
                    string jumpType = (string)j["JumpType"];
                    eventText = "";
                    if (jumpType.Equals("Supercruise"))
                    {
                        eventText = "Jumped to Supercruise";
                    }
                    else
                    {
                        this.lastJumpStartTime = DateTime.Parse(timestamp);
                        system = (string)j["StarSystem"];
                        string starClass = (string)j["StarClass"];
                        eventText = string.Format("Preparing to jump to {0} | Star Class: {1}", system, starClass);
                    }
                    return new JournalEntry(timestamp, @event, eventText, j);
                /*case "Materials":
                    return new JournalEntry(timestamp, @event, "Updated material counts.", j, false);*/ // TODO
                case "Loadout":
                    if (commander != null)
                    {
                        ship = (string)j["Ship"];
                        shipName = (string)j["ShipName"];
                        shipID = (string)j["ShipIdent"];
                        commander.SetShip(new CommanderShip(mainForm.Database.getShipNameFromInternalName(ship), shipID, shipName));
                        int commanderShipID = (int)j["ShipID"];
                        commander.UpdateShipLoadout(commanderShipID, mainForm.Database.getShipNameFromInternalName(ship), shipID, shipName.Equals(ship) ? "" : shipName, j["Modules"].ToString());
                    }
                    return new JournalEntry(timestamp, @event, "Updated vessel loadout", j);
                case "CommunityGoalJoin":
                    string name = (string)j["Name"];
                    return new JournalEntry(timestamp, @event, string.Format("Signed up for community goal '{0}'", name), j);
                case "CommunityGoalReward":
                    name = (string)j["Name"];
                    credits = (long)j["Reward"];
                    eventText = string.Format("Completed community goal '{0}', awarded {1:n0} credits", name, credits);
                    if (commander != null && !isReparse)
                        commander.addCredits(credits);
                    return new JournalEntry(timestamp, @event, eventText, j);
                case "JoinACrew":
                    name = (string)j["Captain"];
                    if (!isReparse)
                    {
                        commander.isInMulticrew = true;
                        commander.MultiCrewCommanderName = name;
                    }
                    return new JournalEntry(timestamp, @event, $"Joined {name}'s ship in multicrew", j);
                case "QuitACrew":
                    name = (string)j["Captain"];
                    if (!isReparse)
                        commander.isInMulticrew = false;
                    return new JournalEntry(timestamp, @event, $"Left {name}'s ship", j);
                case "Touchdown":
                    bool player = true;
                    try
                    {
                        player = (bool)j["PlayerControlled"];
                    }
                    catch { }
                    if (player && !isReparse)
                        commander.isLanded = true;
                    return new JournalEntry(timestamp, @event, "Touchdown!", j);
                case "Liftoff":
                    player = true;
                    try
                    {
                        player = (bool)j["PlayerControlled"];
                    }
                    catch { }
                    if (player && !isReparse)
                        commander.isLanded = false;
                    return new JournalEntry(timestamp, @event, "Liftoff!", j);
                case "Location":
                    // { "timestamp":"2017-07-17T15:53:35Z", "event":"Location", "Docked":true, "StationName":"Ising Vision", "StationType":"Coriolis", "StarSystem":"Neto", "StarPos":[-41.188,7.656,36.313], "SystemAllegiance":"Independent", "SystemEconomy":"$economy_HighTech;", "SystemEconomy_Localised":"High Tech", "SystemGovernment":"$government_Democracy;", "SystemGovernment_Localised":"Democracy", "SystemSecurity":"$SYSTEM_SECURITY_high;", "SystemSecurity_Localised":"High Security", "Body":"Ising Vision", "BodyType":"Station", "Factions":[ { "Name":"Neto Blue Power Limited", "FactionState":"Boom", "Government":"Corporate", "Influence":0.060000, "Allegiance":"Federation", "PendingStates":[ { "State":"Outbreak", "Trend":0 } ] }, { "Name":"72 Herculis Free", "FactionState":"Boom", "Government":"Democracy", "Influence":0.049000, "Allegiance":"Federation", "RecoveringStates":[ { "State":"Outbreak", "Trend":0 } ] }, { "Name":"Wolf 1509 Blue Power Inc", "FactionState":"Boom", "Government":"Corporate", "Influence":0.051000, "Allegiance":"Federation" }, { "Name":"G 139-50 Electronics Partners", "FactionState":"Boom", "Government":"Corporate", "Influence":0.057000, "Allegiance":"Federation" }, { "Name":"Alliance of V816 Herculis", "FactionState":"None", "Government":"Confederacy", "Influence":0.075000, "Allegiance":"Federation", "RecoveringStates":[ { "State":"Boom", "Trend":0 } ] }, { "Name":"Neto Regulatory State", "FactionState":"Boom", "Government":"Dictatorship", "Influence":0.047000, "Allegiance":"Independent" }, { "Name":"Workers of Neto Progressive Party", "FactionState":"Boom", "Government":"Democracy", "Influence":0.098000, "Allegiance":"Federation" }, { "Name":"Neto Mob", "FactionState":"Boom", "Government":"Anarchy", "Influence":0.018000, "Allegiance":"Independent" }, { "Name":"Pixel Bandits Security Force", "FactionState":"Expansion", "Government":"Democracy", "Influence":0.545000, "Allegiance":"Independent", "PendingStates":[ { "State":"Boom", "Trend":1 } ] } ], "SystemFaction":"Pixel Bandits Security Force", "FactionState":"Expansion" }
                    // { "timestamp":"2017-07-16T21:33:00Z", "event":"Location", "Docked":false, "StarSystem":"Wolf 918", "StarPos":[-19.000,-23.906,25.344], "SystemAllegiance":"Independent", "SystemEconomy":"$economy_Colony;", "SystemEconomy_Localised":"Colony", "SystemGovernment":"$government_Feudal;", "SystemGovernment_Localised":"Feudal", "SystemSecurity":"$SYSTEM_SECURITY_medium;", "SystemSecurity_Localised":"Medium Security", "Body":"Wolf 918", "BodyType":"Star", "Factions":[ { "Name":"Wolf 918 Gold Bridge Network", "FactionState":"Boom", "Government":"Corporate", "Influence":0.224224, "Allegiance":"Federation" }, { "Name":"LHS 4058 Confederacy", "FactionState":"Retreat", "Government":"Confederacy", "Influence":0.039039, "Allegiance":"Federation", "RecoveringStates":[ { "State":"Boom", "Trend":0 } ] }, { "Name":"Barons of Wolf 918", "FactionState":"None", "Government":"Feudal", "Influence":0.297297, "Allegiance":"Independent", "PendingStates":[ { "State":"Boom", "Trend":1 } ], "RecoveringStates":[ { "State":"Famine", "Trend":0 } ] }, { "Name":"Youming Solutions", "FactionState":"None", "Government":"Corporate", "Influence":0.186186, "Allegiance":"Federation", "RecoveringStates":[ { "State":"Boom", "Trend":1 } ] }, { "Name":"Confederation of Wolf 918", "FactionState":"CivilWar", "Government":"Confederacy", "Influence":0.079079, "Allegiance":"Federation", "RecoveringStates":[ { "State":"Boom", "Trend":0 } ] }, { "Name":"Wolf 918 PLC", "FactionState":"Boom", "Government":"Corporate", "Influence":0.095095, "Allegiance":"Independent", "PendingStates":[ { "State":"Outbreak", "Trend":0 } ] }, { "Name":"Pirates of Wolf 918", "FactionState":"CivilWar", "Government":"Anarchy", "Influence":0.079079, "Allegiance":"Independent", "RecoveringStates":[ { "State":"Boom", "Trend":0 } ] } ], "SystemFaction":"Barons of Wolf 918" }
                    if (!isReparse)
                    {
                        commander.CurrentSystem = (string)j["StarSystem"];
                        commander.CurrentLocation = (string)j["Body"];
                        commander.isDocked = (bool)j["Docked"];
                        List<JToken> coords = j["StarPos"].ToList();
                        float X = coords[0].ToObject<float>();
                        float Y = coords[1].ToObject<float>();
                        float Z = coords[2].ToObject<float>();

                        SystemCoordinate sc = new SystemCoordinate(X, Y, Z);

                        commander.CurrentSystemCoordinates = sc;
                    }
                    return new JournalEntry(timestamp, @event, "Location data updated", j);
                case "SetUserShipName":
                    // { "timestamp":"2017-07-17T18:07:27Z", "event":"SetUserShipName", "Ship":"anaconda", "ShipID":36, "UserShipName":"IEV Roxanne", "UserShipId":"IP-22A" }
                    string newShipName = (string)j["UserShipName"];
                    string newShipId = (string)j["UserShipId"];
                    int fleetID = (int)j["ShipID"];
                    if (!isReparse)
                    {
                        commander.UpdateCurrentShip(fleetID, newShipId, newShipName);
                    }
                    return new JournalEntry(timestamp, @event, string.Format("Updated ship naming: [{0}] {1}", newShipId, newShipName), j);
                case "SellExplorationData":
                    List<string> systems = j.GetValue("Systems").ToObject<List<string>>();
                    int systemCount = systems.Count;
                    List<string> discovered = j.GetValue("Discovered").ToObject<List<string>>();
                    if (discovered.Count > 0 && commander != null)
                        foreach (string sys in discovered)
                            commander.registerFirstDiscovery(sys, timestamp);
                    long baseReward = (long)j["BaseValue"];
                    long bonusReward = (long)j["Bonus"];
                    long totalCredits = baseReward + bonusReward;
                    if (!isReparse)
                        commander.addCredits(totalCredits);
                    eventText = string.Format("Turned in exploration data for {0:n0} systems totalling {1:n0} credits", systemCount, totalCredits);
                    return new JournalEntry(timestamp, @event, eventText, j);
                //{ "timestamp":"2017-07-18T23:09:40Z", "event":"SellExplorationData", "Systems":[ "Byua Euq ES-W b1-14", "Byua Euq DF-H b10-16", "Traikaae QD-T b58-16", "Trifid Sector NI-S b4-17", "Trifid Sector DH-K a9-3", "Drojia MA-P a115-3", "Lagoon Sector WW-H a11-4", "Trifid Sector RD-R a5-7", "Trifid Sector TL-B a14-11" ], "Discovered":[  ], "BaseValue":32871, "Bonus":0 }
                case "Scan": // We have multi-line journal entries now, let's abuse the heck out of them!
                             // Planet: { "timestamp":"2017-08-11T17:46:06Z", "event":"Scan", "BodyName":"Synookio OP-K b10-2 A 6", "DistanceFromArrivalLS":107.124298, "TidalLock":false, "TerraformState":"", "PlanetClass":"Water world", "Atmosphere":"thick argon rich atmosphere", "AtmosphereType":"ArgonRich", "AtmosphereComposition":[ { "Name":"Nitrogen", "Percent":96.575706 }, { "Name":"Argon", "Percent":2.764786 }, { "Name":"CarbonDioxide", "Percent":0.626221 } ], "Volcanism":"major water magma volcanism", "MassEM":1.340439, "Radius":8274225.000000, "SurfaceGravity":7.803720, "SurfaceTemperature":353.210022, "SurfacePressure":80850008.000000, "Landable":false, "SemiMajorAxis":32115288064.000000, "Eccentricity":0.000016, "OrbitalInclination":-0.000502, "Periapsis":229.444931, "OrbitalPeriod":5383803.500000, "RotationPeriod":-5384022.000000, "AxialTilt":-1.736953 }
                             // Star: { "timestamp":"2017-08-11T17:46:29Z", "event":"Scan", "BodyName":"Synookio OP-K b10-2 A", "DistanceFromArrivalLS":0.000000, "StarType":"M", "StellarMass":0.339844, "Radius":329793632.000000, "AbsoluteMagnitude":9.631912, "Age_MY":12912, "SurfaceTemperature":2778.000000, "SemiMajorAxis":1099630116864.000000, "Eccentricity":0.045134, "OrbitalInclination":34.794361, "Periapsis":77.388535, "OrbitalPeriod":4289333248.000000, "RotationPeriod":142939.281250, "AxialTilt":0.000000, "Rings":[ { "Name":"Synookio OP-K b10-2 A A Belt", "RingClass":"eRingClass_MetalRich", "MassMT":6.794e+09, "InnerRad":6.0099e+08, "OuterRad":1.684e+09 } ] }
                    string bodyName = j.GetValue("BodyName").ToString();
                    if (bodyName.Contains("Belt Cluster"))
                        return new JournalEntry(timestamp, @event, bodyName, j);
                    long orbitalPeriod = 0;
                    try
                    {
                        orbitalPeriod = j.GetValue("OrbitalPeriod").ToObject<long>();
                    }
                    catch { }
                    long rotationPeriod = j.GetValue("RotationPeriod").ToObject<long>();
                    bool retroRotate = false;
                    bool retroOrbit = false;
                    if (rotationPeriod < 0)
                        retroRotate = true;
                    if (orbitalPeriod < 0)
                        retroOrbit = true;
                    double distanceFromArrival = j.GetValue("DistanceFromArrivalLS").ToObject<double>();
                    bool isStar = false;
                    string bodyClass = string.Empty;
                    try
                    {
                        bodyClass = j.GetValue("PlanetClass").ToString().Replace("Sudarsky class", "Class");
                    }
                    catch
                    {
                        bodyClass = mainForm.Database.GetCorrectStarClassName(j.GetValue("StarType").ToString());
                        isStar = true;
                    }
                    Dictionary<string, double> materials = new Dictionary<string, double>();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLineFormatted("Body: {0}", bodyName);
                    sb.AppendLineFormatted("Type: {0}", bodyClass);
                    sb.AppendLineFormatted("Distance from arrival: {0:n2}", distanceFromArrival);
                    List<string> orbitCriteria = new List<string>();
                    List<string> rotationCriteria = new List<string>();
                    if (retroOrbit)
                        orbitCriteria.Add("retrograde");
                    if (retroRotate)
                        rotationCriteria.Add("retrograde");
                    if (!isStar)
                    {
                        bool terraformable = false;
                        try { terraformable = j.GetValue("TerraformState").ToString().Equals("Terraformable"); }
                        catch { }
                        if (this.scanEntriesToIgnore == 0 && Properties.Settings.Default.SoundsEnabled && !doNotPlaySounds)
                        {
                            string soundFile = string.Empty;
                            switch (bodyClass)
                            {
                                case "Earthlike body":
                                    soundFile = "elw_scanned.wav";
                                    break;
                                case "Ammonia world":
                                    soundFile = "aw_scanned.wav";
                                    break;
                                case "Water world":
                                    soundFile = terraformable ? "tww_scanned.wav" : "ww_scanned.wav";
                                    break;
                                case "High metal content body":
                                    soundFile = terraformable ? "thmc_scanned.wav" : "hmc_scanned.wav";
                                    break;
                            }
                            Utils.PlaySound(soundFile);
                        }
                        // { "timestamp":"2017-04-06T10:33:23Z", "event":"Scan", "BodyName":"Akandi BC 2 a", "DistanceFromArrivalLS":81403.218750, "PlanetClass":"Icy body", "MassEM":0.027412, "Radius":2631372.500000, "SurfaceGravity":1.577917, "SemiMajorAxis":635405376.000000, "Eccentricity":0.000000, "OrbitalInclination":76.420624, "Periapsis":109.436974, "OrbitalPeriod":2750407.500000, "RotationPeriod":2761699.750000 }
                        bool tidallyLocked = false;
                        try { tidallyLocked = j.GetValue("TidalLock").ToObject<bool>(); }
                        catch { }
                        if (tidallyLocked)
                            rotationCriteria.Add("tidally locked");
                        bool landable = false;
                        try { landable = j.GetValue("Landable").ToObject<bool>(); }
                        catch { }
                        sb.AppendLineFormatted("Is Landable: {0}", landable ? "yes" : "no");
                        if (landable)
                        {
                            try // Who the actual fuck decides it's a good idea to change the format of something randomly IN A GOD DAMNED API?!
                            {
                                foreach (JProperty jp in j.GetValue("Materials"))
                                {
                                    materials.Add(jp.Name, jp.Value.ToObject<double>());
                                }
                            }
                            catch
                            {
                                List<DSSScanBodyMaterialsData> frontier = j.GetValue("Materials").ToObject<List<DSSScanBodyMaterialsData>>();
                                foreach (DSSScanBodyMaterialsData fuck in frontier) // Fuckin' Frontier, man.
                                    materials.Add(fuck.Name, fuck.Percent);
                            }
                        }
                        sb.AppendLineFormatted("Terraformable: {0}", terraformable ? "yes" : "no");
                        sb.AppendLineFormatted("Earth masses: {0:n3}", j.GetValue("MassEM").ToObject<double>());
                        sb.AppendLineFormatted("Gravity: {0:n2} g", (j.GetValue("SurfaceGravity").ToObject<double>() / 10d));
                    }
                    if (orbitalPeriod > 0)
                        sb.AppendLineFormatted("Orbital period: {0}{1}", Utils.formatTimeFromGalacticSeconds(Math.Abs(orbitalPeriod)), orbitCriteria.Count > 0 ? string.Format(" ({0})", string.Join(", ", orbitCriteria.ToArray())) : "");
                    sb.AppendLineFormatted("Rotational period: {0}{1}", Utils.formatTimeFromGalacticSeconds(Math.Abs(rotationPeriod)), rotationCriteria.Count > 0 ? string.Format(" ({0})", string.Join(", ", rotationCriteria.ToArray())) : "");
                    int tempKelvin = Int32.MaxValue;
                    try { tempKelvin = j.GetValue("SurfaceTemperature").ToObject<int>(); }
                    catch { }
                    if (tempKelvin != Int32.MaxValue)
                    {
                        double tempCelsius = tempKelvin - 273.15;
                        double tempFahrenheit = (tempKelvin * 9 / 5) - 459.67;
                        sb.AppendLineFormatted("Surface temperature: {0:n0} K ({1:n2} C / {2:n2} F)", tempKelvin, tempCelsius, tempFahrenheit);
                    }
                    sb.AppendLineFormatted("Radius: {0:n3} km", j.GetValue("Radius").ToObject<double>() / 1000d);
                    if (isStar)
                    {
                        sb.AppendLineFormatted("Solar masses: {0:n5}", j.GetValue("StellarMass").ToObject<double>());
                        sb.AppendLineFormatted("Age: {0:n0} million years", j.GetValue("Age_MY").ToObject<int>());
                    }
                    if (materials.Count > 0)
                    {
                        sb.AppendLine();
                        foreach (KeyValuePair<string, double> kvp in materials)
                            sb.AppendLineFormatted("{0}: {1:n2}%", kvp.Key.CapitaliseFirst(), kvp.Value);
                    }
                    if (this.scanEntriesToIgnore == 0 && showNotifications && !isReparse && Properties.Settings.Default.NotificationsEnabled && Properties.Settings.Default.ScanNotifications)
                    {
                        Notification n = new Notification(bodyName, sb.ToString(), 20);
                        Utils.InvokeNotification(n);
                    }
                    if (this.scanEntriesToIgnore > 0)
                        this.scanEntriesToIgnore--;
                    return new JournalEntry(timestamp, @event, sb.ToString().Trim(), j);
                case "NavBeaconScan":
                    int bodies = j.GetValue("NumBodies").ToObject<int>();
                    if (!isReparse)
                        this.scanEntriesToIgnore = bodies;
                    return new JournalEntry(timestamp, @event, $"Nav Beacon scan complete: {bodies} data entries downloaded.", j);
                case "Friends":
                    string status = j.GetValue("Status").ToString();
                    string friendString = string.Empty;
                    if (status.Equals("Added"))
                        friendString = string.Format("Now friends with CMDR {0}", j.GetValue("Name").ToString());
                    else
                        friendString = string.Format("CMDR {0} has {1} {2}.", j.GetValue("Name").ToString(), status.Equals("Offline") ? "gone" : "come", status.Equals("Offline") ? "offline" : "online");
                    if (!status.Equals("Added") && showNotifications && !isReparse && Properties.Settings.Default.NotificationsEnabled && Properties.Settings.Default.FriendNotifications)
                    {
                        Utils.InvokeNotification(new Notification(string.Format("Friend {0}", status), friendString));
                    }
                    return new JournalEntry(timestamp, @event, friendString, j);
                case "Music":
                    string track = j.GetValue("MusicTrack").ToString();
                    return new JournalEntry(timestamp, @event, $"New music track: {track}", j);
                case "Repair":
                    long repairCost = j.GetValue("Cost").ToObject<long>();
                    if (!isReparse)
                        commander.deductCredits(repairCost);
                    return new JournalEntry(timestamp, @event, string.Format("Spent {0:n0} credits on advanced maintenance", repairCost), j);
                case "LaunchSRV":
                    return new JournalEntry(timestamp, @event, "Deployed SRV.", j);
                case "DockSRV":
                    return new JournalEntry(timestamp, @event, "Docked SRV.", j);
                case "DatalinkScan":
                    return new JournalEntry(timestamp, @event, j.GetValue("Message_Localised").ToString(), j);
                case "DatalinkVoucher":
                    return new JournalEntry(timestamp, @event, string.Format("Awarded {0} intel package worth {1:n0} credits", j.GetValue("PayeeFaction").ToString(), j.GetValue("Reward").ToObject<long>()), j);
                case "Materials": // Finally?!
                    if (/*!isReparse && */commander != null)
                    {
                        Dictionary<string, int> _materials = new Dictionary<string, int>();

                        // This is really inefficient, but the journal is shit, so we practically have to do it this way
                        /*List<GenericStringIntKVP> raw = new List<GenericStringIntKVP>();
                        List<GenericStringIntKVP> manufactured = new List<GenericStringIntKVP>();
                        List<GenericStringIntKVP> encoded = new List<GenericStringIntKVP>();*/

                        List<JournalMaterialsEventKVP> all = new List<JournalMaterialsEventKVP>();

                        all.AddRange(JsonConvert.DeserializeObject<List<JournalMaterialsEventKVP>>(j.GetValue("Raw").ToString()));
                        all.AddRange(JsonConvert.DeserializeObject<List<JournalMaterialsEventKVP>>(j.GetValue("Manufactured").ToString()));
                        all.AddRange(JsonConvert.DeserializeObject<List<JournalMaterialsEventKVP>>(j.GetValue("Encoded").ToString()));

                        foreach (JournalMaterialsEventKVP kvp in all)
                        {
                            //if (kvp.Name == null) continue;
                            if (_materials.ContainsKey(kvp.Name)) { this.logger.Log("Duplicate material entry '{0}' - skipping", LogLevel.WARNING, kvp.Name); continue; }
                            _materials.Add(kvp.Name, kvp.Count);
                        }
                        commander.SetMaterials(_materials);
                        _materials.Clear();
                    }
                    return new JournalEntry(timestamp, @event, "Updated material counts", j);
                /*
                 * { "timestamp":"2017-11-06T17:43:46Z", "event":"EngineerCraft", "Engineer":"Lei Cheung", "Blueprint":"Sensor_Sensor_LightWeight", "Level":5, "Ingredients":[ { "Name":"conductiveceramics", "Count":1 }, { "Name":"protolightalloys", "Count":1 }, { "Name":"protoradiolicalloys", "Count":1 } ] }
{ "timestamp":"2017-11-06T17:43:55Z", "event":"EngineerApply", "Engineer":"Lei Cheung", "Blueprint":"Sensor_Sensor_LightWeight", "Level":5 }
*/
                case "EngineerCraft":
                    string engineer = j.GetValue("Engineer").ToString();
                    string blueprint = string.Empty;
                    JToken @out;
                    if (j.TryGetValue("Blueprint", out @out))
                        blueprint = @out.ToString();
                    else
                        blueprint = j.GetValue("BlueprintName").ToString();
                    int level = j.GetValue("Level").ToObject<int>();
                    if (!isReparse && commander != null)
                    {
                        try
                        {
                            List<JournalMaterialsEventKVP> materialCosts = new List<JournalMaterialsEventKVP>();
                            materialCosts = JsonConvert.DeserializeObject<List<JournalMaterialsEventKVP>>(j.GetValue("Ingredients").ToString());
                            commander.RemoveMaterials(materialCosts);
                        }
                        catch
                        {
                            //this.logger.Log("Unable to load engineer blueprint data via normal method, falling back to old method.", LogLevel.WARNING);
                            Dictionary<string, int> materialCosts = new Dictionary<string, int>();
                            materialCosts = JsonConvert.DeserializeObject<Dictionary<string, int>>(j.GetValue("Ingredients").ToString());
                            List<JournalMaterialsEventKVP> converted = new List<JournalMaterialsEventKVP>();
                            foreach (KeyValuePair<string, int> kvp in materialCosts)
                            {
                                JournalMaterialsEventKVP jme = new JournalMaterialsEventKVP();
                                jme.Name = kvp.Key;
                                jme.Count = kvp.Value;
                                converted.Add(jme);
                            }
                            commander.RemoveMaterials(converted);
                        }
                    }
                    if (mainForm.MaterialsGUIOpen)
                        MaterialList.Instance.DisplayMaterials();
                    string engineerString = string.Format("Crafted Grade {0} {1} upgrade at {2}", level, blueprint, engineer);
                    return new JournalEntry(timestamp, @event, engineerString, j);
                case "EngineerApply":
                    engineer = j.GetValue("Engineer").ToString();
                    blueprint = j.GetValue("Blueprint").ToString();
                    level = j.GetValue("Level").ToObject<int>();
                    engineerString = string.Format("Applied Grade {0} {1} upgrade from {2}", level, blueprint, engineer);
                    return new JournalEntry(timestamp, @event, engineerString, j);
                case "EngineerProgress":
                    engineer = j.GetValue("Engineer").ToString();
                    try
                    {
                        level = j.GetValue("Rank").ToObject<int>();
                        engineerString = string.Format("Granted Grade {0} access at {1}", level, engineer);
                        if (showNotifications &&  Properties.Settings.Default.NotificationsEnabled)
                        {
                            Notification n = new Notification("Engineer Rank Progress", string.Format("You now have grade {0} access with {1}", level, engineer));
                            Utils.InvokeNotification(n);
                        }
                    }
                    catch (NullReferenceException)
                    {
                        engineerString = string.Format("You have learned of the engineer {0}", engineer);
                    }
                    return new JournalEntry(timestamp, @event, engineerString, j);
                case "CommunityGoal":
                    List<JournalCommunityGoal> cgs = JsonConvert.DeserializeObject<List<JournalCommunityGoal>>(j.GetValue("CurrentGoals").ToString());
                    sb = new StringBuilder();
                    int iCG = 0;
                    foreach (JournalCommunityGoal cg in cgs)
                    {
                        if (iCG > 0)
                            sb.AppendLine();

                        sb.AppendLine(string.Format("{0}{1}", cg.Title, cg.IsComplete ? " [COMPLETE]" : ""));
                        if (cg.PlayerContribution > 0)
                            sb.AppendLineFormatted(@"[{0} / {1}] Global progress: {2} ({3:n0}) | Your contribution: {4:n0}, reward tier: {5}, payout: {6:n0}", cg.SystemName, cg.MarketName, cg.TierReached, cg.CurrentTotal, cg.PlayerContribution, cg.PlayerPercentileBand, cg.Bonus);
                        else
                            sb.AppendLineFormatted(@"[{0} / {1}] Global progress: {2} ({3:n0})", cg.SystemName, cg.MarketName, cg.TierReached, cg.CurrentTotal);
                        iCG++;
                    }
                    return new JournalEntry(timestamp, @event, sb.ToString().TrimEnd(), j);
                // { "timestamp":"2017-12-15T20:46:45Z", "event":"MissionAccepted", "Faction":"Aegis Research", "Name":"Mission_DS_PassengerBulk", "LocalisedName":"17 Refugees looking to get off the starport", "DestinationSystem":"Taygeta", "DestinationStation":"Rescue Ship - Titan's Daughter", "Expiry":"2017-12-15T21:58:53Z", "Influence":"High", "Reputation":"Low", "Reward":125732, "PassengerCount":17, "PassengerVIPs":false, "PassengerWanted":false, "PassengerType":"Refugee", "MissionID":263713525 }
                case "MissionAccepted":
                    string missionName = string.Empty;
                    try
                    {
                        missionName = j.GetValue("LocalisedName").ToString();
                    }
                    catch { }
                    string factionName = j.GetValue("Faction").ToString();
                    long missionReward = -1L;
                    try
                    {
                        missionReward = j.GetValue("Reward").ToObject<long>();
                    }
                    catch { }
                    string missionIntName = j.GetValue("Name").ToString();
                    if (commander != null && (missionIntName.Equals("Mission_DS_PassengerBulk") || missionName.EndsWithIgnoreCase(" Refugees looking to get off the starport")))
                    {
                        int passengerCount = j.GetValue("PassengerCount").ToObject<int>();
                        commander.RescuedThargoidRefugees += passengerCount;
                    }
                    string output = string.Empty;
                    if (string.IsNullOrWhiteSpace(missionName) && missionReward == -1L)
                    {
                        output = "Mission accepted.";
                    }
                    else if (string.IsNullOrWhiteSpace(missionName) && missionReward > -1L)
                    {
                        output = string.Format("Accepted mission for faction {1} - Payout: {2:n0}", missionName, factionName, missionReward);
                    }
                    else { output = string.Format("Accepted mission '{0}' for faction {1} - Payout: {2:n0}", missionName, factionName, missionReward); }
                    return new JournalEntry(timestamp, @event, output, j);
                case "ShipTargeted":
                    bool locked = j.GetValue("TargetLocked").ToObject<bool>();
                    if (!locked)
                        return new JournalEntry(timestamp, @event, "Targeting reset.", j);
                    int scanStage = j.GetValue("ScanStage").ToObject<int>();
                    shipName = mainForm.Database.getShipNameFromInternalName(j.GetValue("Ship").ToString());
                    StringBuilder td = new StringBuilder();
                    td.AppendFormat("{0}", shipName);
                    if (scanStage >= 1)
                    {
                        string pilotName = string.Empty;
                        if (j.TryGetValue("PilotName_Localised", out @out))
                            pilotName = @out.ToString();
                        else
                            pilotName = j.GetValue("PilotName").ToString();
                        string pilotRank = string.Empty; // Dear Frontier, if you're going to put something in your API documentation, AT LEAST MAKE SURE IT ACTUALLY SHOWS UP ALL THE DAMN TIME.
                        if (j.TryGetValue("PilotRank", out @out))
                            pilotRank = @out.ToString();
                        if (string.IsNullOrWhiteSpace(pilotRank))
                            td.AppendFormat("\n{0}", pilotName);
                        else
                            td.AppendFormat("\n{0}\n{1}", pilotName, pilotRank);
                    }
                    if (scanStage >= 2)
                    {
                        int shieldHealth = j.GetValue("ShieldHealth").ToObject<int>();
                        int hullHealth = j.GetValue("HullHealth").ToObject<int>();
                        td.AppendFormat("\nShields: {0}%\nHull: {1}%", shieldHealth, hullHealth);
                    }
                    if (scanStage >= 3)
                    {
                        bool wanted = j.GetValue("LegalStatus").ToString().EqualsIgnoreCase("wanted");
                        Int64 bounty = 0;
                        if (wanted)
                        {
                            // Try-catch here because sometimes the Journal forgets to add it or something (???).
                            try
                            {
                                bounty = j.GetValue("Bounty").ToObject<Int64>();
                            }
                            catch
                            {
                                this.logger.Log("Ship is wanted but no bounty ammount is present, setting bounty as 0", LogLevel.ERR);
                            }
                            if (bounty == 0)
                                td.AppendFormat("\n{0}", wanted ? "WANTED" : "CLEAN");
                            else
                                td.AppendFormat("\n{0} ({1:n0})", wanted ? "WANTED" : "CLEAN", bounty);
                        }
                        else
                            td.AppendFormat("\n{0}", wanted ? "WANTED" : "CLEAN");
                        JToken _owningFaction;
                        if (j.TryGetValue("Faction", out _owningFaction))
                        {

                            td.AppendFormat("\n{0}", _owningFaction.ToString());
                        }
                    }
                    return new JournalEntry(timestamp, @event, td.ToString(), j);
                case "EngineerLegacyConvert":
                    bool preview = j.GetValue("IsPreview").ToObject<bool>();
                    blueprint = j.GetValue("Blueprint").ToString();
                    if (preview)
                        return new JournalEntry(timestamp, @event, string.Format("Previewing 3.0 engineering upgrade for blueprint {0}", blueprint), j);
                    return new JournalEntry(timestamp, @event, String.Format("Applied 3.0 version of blueprint {0}", blueprint), j);
                case "MaterialTrade":
                    string traderType = j.GetValue("TraderType").ToString();
                    MaterialTradeData paid = j.GetValue("Paid").ToObject<MaterialTradeData>();
                    MaterialTradeData received = j.GetValue("Received").ToObject<MaterialTradeData>();
                    if (!isReparse && commander != null)
                    {
                        commander.addMaterial(received.Material, received.Quantity);
                        commander.removeMaterial(paid.Material, paid.Quantity);
                    }
                    string tradeString = string.Format("Traded in {0:n0} {1} for {2:n0} {3}", paid.Quantity, paid.Material_Localised, received.Quantity, received.Material_Localised);
                    return new JournalEntry(timestamp, @event, tradeString, j);
                case "NpcCrewPaidWage":
                    credits = j.GetValue("Amount").ToObject<Int64>();
                    return new JournalEntry(timestamp, @event, string.Format("Paid crew wages of {0:n0} credits", credits), j);
                case "Shutdown":
                    if (commander == null)
                        return new JournalEntry(timestamp, @event, "Game closed.", j);
                    else
                        return new JournalEntry(timestamp, @event, string.Format("Farewell, Commander {0}", commander.Name), j);
                case "Powerplay":
                    /*
                      "timestamp": "2018-01-30T17:32:26Z",
                      "event": "Powerplay",
                      "Power": "Aisling Duval",
                      "Rank": 0,
                      "Merits": 0,
                      "Votes": 0,
                      "TimePledged": 1517333472
                    */
                    string power = j.GetValue("Power").ToString();
                    rank = j.GetValue("Rank").ToObject<int>();
                    long merits = j.GetValue("Merits").ToObject<long>();
                    if (commander != null)
                        commander.SetPowerPlayData(true, power, rank, merits);
                    return new JournalEntry(timestamp, @event, string.Format("Pledged to {0} | Rank {1} ({2:n0} merits)", power, rank, merits), j);
                case "PowerplayJoin":
                    power = j.GetValue("Power").ToString();
                    if (commander != null)
                        commander.SetBasicPowerPlayData(true, power);
                    return new JournalEntry(timestamp, @event, string.Format("Pledged allegiance to {0}", power), j);
                case "PowerplayLeave":
                    power = j.GetValue("Power").ToString();
                    if (commander != null)
                        commander.SetNotPledgedInPowerPlay();
                    return new JournalEntry(timestamp, @event, string.Format("No longer pledged to {0}", power), j);
                case "PowerplayDefect":
                    power = j.GetValue("FromPower").ToString();
                    string power2 = j.GetValue("ToPower").ToString();
                    if (commander != null)
                        commander.SetNotPledgedInPowerPlay();
                    return new JournalEntry(timestamp, @event, string.Format("Defected from {0} to {1}", power, power2), j);
                case "DiscoveryScan":
                    long systemAddress = j.GetValue("SystemAddress").ToObject<long>();
                    count = j.GetValue("Bodies").ToObject<Int32>();
                    if (commander != null && systemAddress == commander.LastSystemAddress)
                        return new JournalEntry(timestamp, @event, string.Format("{0}: {1:n0} new astronomical object(s) found.", commander.CurrentSystem, count), j);
                    return new JournalEntry(timestamp, @event, string.Format("{0:n0} new astronomical object(s) found.", count), j);
                case "Reputation":
                    // Coder's note: This has to be like this because evidently Frontier think omiting an entry because it is 0 is okay.
                    double _fed, emp, alliance, indep;
                    _fed = emp = alliance = indep = 0.0d;
                    if (j.TryGetValue("Federation", out @out))
                        _fed = Math.Truncate(@out.ToObject<double>());
                    if (j.TryGetValue("Empire", out @out))
                        emp = Math.Truncate(@out.ToObject<double>());
                    if (j.TryGetValue("Alliance", out @out))
                        alliance = Math.Truncate(@out.ToObject<double>());
                    if (j.TryGetValue("Independent", out @out))
                        indep = Math.Truncate(@out.ToObject<double>());

                    return new JournalEntry(timestamp, @event, string.Format("Reputations | Federation: {0}, Empire: {1}, Alliance: {2}, Independent: {3}", _fed, emp, alliance, indep), j);
                default:
                    return new JournalEntry(timestamp, @event, string.Empty, j, false);
            }
           
        }

        public void parseAllJournals()
        {
            this.fullParseInProgress = true;
            string journalPath = EliteUtils.JOURNAL_PATH;
            journalPath = Environment.ExpandEnvironmentVariables(journalPath);
            if (!Directory.Exists(journalPath))
                Directory.CreateDirectory(journalPath);
            /*string[] files = Directory.GetFiles(journalPath);
            files.OrderBy(p => new FileInfo(p).CreationTime);*/
                    /*foreach (string _f in files)
                    {
                        Console.WriteLine(_f);
                    }*/
                    DirectoryInfo di = new DirectoryInfo(journalPath);
            FileInfo[] fileInfo = di.GetFiles().OrderBy(f => f.CreationTime).ToArray();
            if (fileInfo.Length == 0)
            {
                mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = $"No journals to load yet!");
                this.noCommandersOrJournals = true;
                return;
            }
            mainForm.cacheController._journalLengthCache.Clear();
            List<ParsableJournalEntry> allJournalEntries = new List<ParsableJournalEntry>();
            int x = 0;
            foreach (FileInfo _file in fileInfo)
            {
                string file = _file.FullName;
                mainForm.cacheController._journalLengthCache.Add(_file.Name, _file.Length);
                mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = $"Parsing Journal file ({x++}/{fileInfo.Length}): {_file.Name}...");
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            allJournalEntries.Add(new ParsableJournalEntry(line, _file.Name.StartsWith("JournalBeta")));
                        }
                        
                    }
                }
            }
            /*try
            {*/
                createJournalEntries(allJournalEntries, false, true, dontPlaySounds: true, showNotifications: false);
            /*}
            catch (Exception e)
            {
                throw e;
            }*/
            allJournalEntries.Clear();
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Switching commander...");
            switchViewedCommander(activeCommander);
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Ready.");
            this.fullParseInProgress = false;
        }

        /*public void createJournalEntries(List<Tuple<string, bool>> entries, bool checkDupes = false, bool dontUpdateDisplays = false, bool dontUpdatePercentage = false, bool dontPlaySounds = false, bool isBetaJournal = false)
        {
            // This is a bit inefficient, but hey, what're you gonna do - we only use it for bulk updates
            //List<string> toParse = entries.Select(a => a.Item1).ToList();
            foreach (Tuple<string, bool> t in entries)
                createJournalEntries(new List<string> { t.Item1 }, checkDupes, dontUpdateDisplays, dontUpdatePercentage, dontPlaySounds, t.Item2);
        }*/

        public void createJournalEntries(List<ParsableJournalEntry> entries, bool checkDuplicates = false, bool dontUpdateDisplays = false, bool dontUpdatePercentage = false, bool dontPlaySounds = false, bool showNotifications = true)
        {
            if (!dontUpdatePercentage)
                mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Generating Journal entries...");
            int cEntry = 0;
            int lastPercent = 0;
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            Commander commander = activeCommander ?? new Commander("blank");
            //logger.Log("{0}", commander == null);
            if (dontUpdateDisplays)
                mainForm.eventList.InvokeIfRequired(() => mainForm.eventList.BeginUpdate());
            //logger.Log("Number of log entries needing parsing: {0}", entries.Count);
            foreach (ParsableJournalEntry en in entries)
            {
                string s = en.JSON;
                if (!string.IsNullOrWhiteSpace(this.JsonToAppend))
                {
                    s = this.JsonToAppend + s;
                    this.JsonToAppend = string.Empty;
                }
                bool isBetaJournalEntry = en.IsBetaJournal;
                double percent = ((double)cEntry++ / (double)entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if (!dontUpdatePercentage && ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00))
                {
                    //TimeSpan ts = (DateTime.Now - timeStarted);
                    //double timeLeft = (ts.TotalSeconds / cEntry) * (entries.Count - cEntry);
                    lastETAUpdate = DateTime.Now;
                    lastPercent = (int)percent;
                }
                if (!dontUpdatePercentage)
                {
                    TimeSpan ts = (DateTime.Now - timeStarted);
                    double timeLeft = (ts.TotalSeconds / cEntry) * (entries.Count - cEntry);
                    mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = String.Format("Generating Journal entries... ({2}/{3}, {0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft), cEntry, entries.Count));
                }
                /*string @event = JObject.Parse(en.JSON).GetValue("event").ToString();
                if (!@event.Equals("LoadGame") && this.isNewGameSession)
                {
                    preLoadCommanderData.Add(en);
                    continue;
                }*/
                JournalEntry je;
                try
                {
                    try
                    {
                        je = parseEvent(s, out commander, doNotPlaySounds: dontPlaySounds, isBeta: isBetaJournalEntry, showNotifications: showNotifications);
                    }
                    catch (InvalidJSONException) { continue; }
                    catch (FragmentedJsonStringException)
                    {
                        this.JsonToAppend = s;
                        continue;
                    }
                    if (je.Event.Equals("Fileheader") || (je.Event.Equals("Music") && Properties.Settings.Default.HideMusicEvents))
                        continue;
                    if (je.Event.Equals("LoadGame") && this.isCommanderRegistered && this.preLoadCommanderData.Count > 0)
                    {
                        logger.Log("There are {0} entries waiting in pre-load, parsing them now...", this.preLoadCommanderData.Count);
                        createJournalEntries(this.preLoadCommanderData, checkDuplicates, dontUpdateDisplays, true, true, showNotifications: false);
                        this.preLoadCommanderData.Clear();
                        logger.Log("Done parsing pre-load journal entries.");
                    }

                    if (!mainForm.comboCommanderList.Items.Contains(commander.Name))
                    {
                        mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.Items.Add(commander.Name));
                        commander.JournalEntries = new List<JournalEntry>(entries.Count);
                    }
                    commander.addJournalEntry(je, checkDuplicates, dontUpdateDisplays);
                }
                catch (NoRegisteredCommanderException)
                {
                    string @event = JObject.Parse(en.JSON).GetValue("event").ToString();
                    this.logger.Log("No commander is registered yet, holding on to journal entry with event of '{0}' until one is.", @event);
                    this.preLoadCommanderData.Add(en);
                    continue;
                }
                if (!(je.Event.Equals("Scan") && this.scanEntriesToIgnore > 0))
                {
                    foreach (Expedition e in commander.getActiveExpeditions())
                        e.parseJournalEntry(je);
                }

            }
            entries.Clear();
            if (dontUpdateDisplays)
                mainForm.eventList.InvokeIfRequired(() => mainForm.eventList.EndUpdate());
            if (this.noCommandersOrJournals && !fullParseInProgress && viewedCommander == null && this.commanders.Count > 0)
            {
                this.noCommandersOrJournals = false;
                Commander com = this.commanders.Values.First();
                if (com != null)
                    switchViewedCommander(com);
            }
            else if (viewedCommander != null && activeCommander != null && commander != null && !fullParseInProgress && Properties.Settings.Default.autoSwitchActiveCommander && !viewedCommander.Name.Equals(activeCommander.Name))
            {
                switchViewedCommander(commander);
                if (!Properties.Settings.Default.autoSwitchMessagesDisplayed)
                {
                    Thread _t = new Thread(() =>
                    {
                        bool keepUpdating = MessageBox.Show("The commander you were viewing doesn't match the commander you appear to be playing on, so the view has been switched to the one you're playing automatically.\n\nWould you like to auto-switch whenever there's a commander difference is detected?\n\nYou can change this at any time in the options dialog.", "Auto-switch Commander", MessageBoxButtons.YesNo) == DialogResult.Yes;
                        Properties.Settings.Default.autoSwitchActiveCommander = keepUpdating;
                        Properties.Settings.Default.autoSwitchMessagesDisplayed = true;
                        Properties.Settings.Default.Save();
                    });
                    _t.Start();
                }
            }
        }

        public void switchViewedCommander(string commander)
        {
            this.logger.Log("Commander switch to commander by name '{0}'", commander);
            switchViewedCommander(commanders[commander]);
        }

        public void switchViewedCommander(Commander c)
        {
            mainForm.buttonDiscoveredBodies.InvokeIfRequired(() => mainForm.buttonDiscoveredBodies.Enabled = false);
            mainForm.buttonExpeditions.InvokeIfRequired(() => mainForm.buttonExpeditions.Enabled = false);
            mainForm.buttonSearch.InvokeIfRequired(() => mainForm.buttonSearch.Enabled = false);
            mainForm.buttonMaterials.InvokeIfRequired(() => mainForm.buttonMaterials.Enabled = false);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            mainForm.setAppStatusText(String.Format("Loading commander data for '{0}'", c.Name));
            this.logger.Log("Switching viewed commander to '{0}'", c.Name);
            /*if (c.Journal.Count > 0 && c.JournalEntries.Count == 0)
            {
                this.logger.Log("Journal entries for commander '{0}' are not loaded into memory, loading them in...", c.Name, c.Journal.Count, c.JournalEntries.Count);
                createJournalEntries(c.Journal);
            }*/
            if (this.viewedCommander != null)
            {
                this.viewedCommander.isViewed = false;
                this.viewedCommander.MarkDirty();
            }
            this.viewedCommander = c;
            List<JournalEntry> _entries = c.JournalEntries;
            List<DataGridViewRow> entries = new List<DataGridViewRow>();
            int x = 0;
            int lastPercent = 0;
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            foreach (JournalEntry j in _entries)
            {
                double percent = ((double)x++ / (double)_entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                {
                    TimeSpan ts = (DateTime.Now - timeStarted);
                    double timeLeft = (ts.TotalSeconds / x) * (_entries.Count - x);
                    lastETAUpdate = DateTime.Now;
                    lastPercent = (int)percent;
                    mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = String.Format("Loading commander data for '{0}' ({1:n0}%) [ETA: {2}]", c.Name, percent, Utils.formatTimeFromSeconds(timeLeft)));
                }

                /*if (Properties.Settings.Default.showJournalUpdateStatus && (x++ == 1 || x % 100 == 0 || x == _entries.Count))
                    mainForm.appStatus.Text = $"Processing Journal entry {String.Format("{0:n0}", x)} of {String.Format("{0:n0}", _entries.Count)}";*/
                DataGridViewRow lvi = getListViewEntryForEntry(j);
                entries.Add(lvi);
            }
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Populating entry list...");
            entries.Reverse();
            mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.SelectedIndex = mainForm.comboCommanderList.Items.IndexOf(c.Name));
            mainForm.eventList.InvokeIfRequired(() =>
            {
                mainForm.eventList.BeginUpdate();
                mainForm.eventList.Rows.Clear();
                mainForm.eventList.Rows.AddRange(entries/*.GetRange(0, 1000)*/.ToArray());
                entries.Clear();
                /*mainForm.eventList.AutoSize = true;
                mainForm.eventList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;*/
                /*mainForm.eventList.DataSource = c.JournalEntries;
                mainForm.eventList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                mainForm.eventList.AutoSize = true;*/
                /*int _x = 0;
                foreach (DataGridViewRow d in entries)
                {
                    if (_x++ >= 1000) break;
                    mainForm.eventList.Rows.Insert(0, d);
                }*/
                /*foreach (DataGridViewColumn ch in mainForm.eventList.Columns)
                {
                    ch.Width = -2;
                }*/
                mainForm.eventList.EndUpdate();
            });
            c.updateDialogDisplays(this.mainForm);
            if (!c.isViewed)
                c.MarkDirty();
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
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            mainForm.setAppStatusText("Ready.");
            mainForm.buttonDiscoveredBodies.InvokeIfRequired(() => mainForm.buttonDiscoveredBodies.Enabled = true);
            mainForm.buttonExpeditions.InvokeIfRequired(() => mainForm.buttonExpeditions.Enabled = true);
            mainForm.buttonSearch.InvokeIfRequired(() => mainForm.buttonSearch.Enabled = true);
            mainForm.buttonMaterials.InvokeIfRequired(() => mainForm.buttonMaterials.Enabled = true);
            mainForm.UpdateThargoidEasterEggIfRequired();
        }

        public DataGridViewRow getListViewEntryForEntry(JournalEntry j)
        {
            /*string dataString = j.Data;
            if (dataString.Length > Utils.LIST_VIEW_MAX_STRING_LENGTH)
            {
                string hoverString = "[HOVER FOR FULL INFO] ";
                dataString = string.Format("{0}{1}...", hoverString, j.Data.Substring(0, (Utils.LIST_VIEW_MAX_STRING_LENGTH - hoverString.Length) - 3));
            }*/
            DataGridViewRow lvi = (DataGridViewRow)mainForm.eventList.RowTemplate.Clone();
            lvi.CreateCells(mainForm.eventList, j.Timestamp, j.Event, string.IsNullOrWhiteSpace(j.Data) ? j.Json : j.Data, j.Notes);
            //lvi.ToolTipText = j.Json;
            if (!j.isKnown)
            {
                lvi.DefaultCellStyle.BackColor = Color.Pink;
                lvi.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                //lvi.SubItems[3].Text = "UNKNOWN EVENT";
            }
            else
            {
                lvi.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                int h = TextRenderer.MeasureText(j.Data, mainForm.eventList.DefaultCellStyle.Font).Height + 5;
                if (h > 18)
                    lvi.Height = h;
            }
            if (Properties.Settings.Default.enableEntryHighlighting)
            {
                if (j.Event.Equals("LoadGame"))
                    lvi.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (j.Event.Equals("SendText") || j.Event.Equals("ReceiveText"))
                    lvi.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                else if (j.Event.Equals("FSDJump") || j.Event.Equals("StartJump"))
                    lvi.DefaultCellStyle.BackColor = Color.LightGray;
                else if (j.Event.Equals("Friends"))
                    lvi.DefaultCellStyle.BackColor = Color.PeachPuff;
            }
            if (Properties.Settings.Default.darkModeEnabled && lvi.DefaultCellStyle.BackColor != Color.Empty)
                lvi.DefaultCellStyle.ForeColor = Color.FromArgb(0, 0, 0);
            lvi.Tag = j.ID;
            return lvi;
        }

        public void switchTailFile(FileInfo fi)
        {
            this.logger.Log("Requested change to Journal file {0}", fi.FullName);
            this.hasChangedLogFile = true;
            this.currentTailFile = fi.FullName;
            if (this.journalTailThread == null)
                tailFile(this.currentTailFile);
        }

        public void tailJournal(string path)
        {

            if (fileWatcherRunning)
                return;
            this.fileWatcherRunning = true;

            if (abortJournalTailing)
            {
                string err = "Tailing not started due to Journal error.";
                mainForm.InvokeIfRequired(() =>
                {
                    mainForm.appStatus.Text = err;
                    mainForm.toolStripTailingFailed.Visible = true;
                });
                this.logger.Log(err);
                return;
            }

            //this.logger.Log("Most recent Journal file is {0}, setting up to tail that file...", last.Name);


            DirectoryInfo di = new DirectoryInfo(EliteUtils.JOURNAL_PATH);
            FileInfo last = null;
            try
            {
                last = di.GetFiles("Journal*").OrderBy(f => f.CreationTime).ToArray().Last();
                this.currentTailFile = last.FullName;
                tailFile(this.currentTailFile);
            }
            catch { }

            this.logger.Log($"Setting up file watcher on directory {path}...");
            path = Environment.ExpandEnvironmentVariables(path);

            fileSystemWatcher = new FileSystemWatcher(path);
            //fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            //fileSystemWatcher.Changed += fileChanged;
            fileSystemWatcher.Created += fileCreated;
            fileSystemWatcher.Filter = "Journal*.log";
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void tailFile(string filePath)
        {
            if (this.tailerRunning)
                return;
            this.tailerRunning = true;

            FileInfo last = new FileInfo(filePath);
            this.logger.Log("Tailer running on {0}...", last.Name);
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = $"Tailing '{last.Name}'...");

            journalTailThread = new Thread(() =>
            {
                long lastSize = 0L;
                mainForm.cacheController._journalLengthCache.TryGetValue(last.Name, out lastSize);
                using (FileStream fs = new FileStream(last.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (mainForm.cacheController._journalLengthCache.ContainsKey(last.Name))
                        fs.Seek(mainForm.cacheController._journalLengthCache[last.Name], SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        bool tailerRestartRequired = false;
                        while (!this.abortJournalTailing && !tailerRestartRequired)
                        {
                            while (!sr.EndOfStream)
                            {
                                string newEntry = sr.ReadLine();
                                createJournalEntries(new List<ParsableJournalEntry>() { new ParsableJournalEntry(newEntry, last.Name.StartsWith("JournalBeta")) }, checkDuplicates: true, dontUpdatePercentage: true);
                                if (mainForm.cacheController._journalLengthCache.ContainsKey(last.Name))
                                    mainForm.cacheController._journalLengthCache[last.Name] = fs.Position;
                                else
                                    mainForm.cacheController._journalLengthCache.Add(last.Name, fs.Position);
                            }
                            while (sr.EndOfStream)
                            {
                                if (this.hasChangedLogFile)
                                {
                                    /*fs.Close();
                                    sr.Close();*/
                                    this.hasChangedLogFile = false;
                                    Console.WriteLine("Tailer restart requested");
                                    tailerRestartRequired = true;
                                    this.tailerRunning = false;
                                    tailFile(this.currentTailFile);
                                    break;
                                }
                                Thread.Sleep(100);
                            }
                            /*if (this.abortJournalTailing)
                                break;
                            last.Refresh();
                            if (this.hasChangedLogFile || last.Length > lastSize)
                            {
                                readFile:
                                readFileFromByteOffset(last.FullName, last.Name, lastSize);
                                lastSize = last.Length;
                                if (mainForm.cacheController._journalLengthCache.ContainsKey(last.Name))
                                    mainForm.cacheController._journalLengthCache[last.Name] = lastSize;
                                else
                                    mainForm.cacheController._journalLengthCache.Add(last.Name, lastSize);
                                if (this.hasChangedLogFile)
                                {
                                    this.logger.Log("Journal file change is pending to change to Journal {0}", this.currentTailFile);
                                    last = new FileInfo(this.currentTailFile);
                                    lastSize = 0L;
                                    this.hasChangedLogFile = false;
                                    goto readFile;
                                }
                            }
                            //Console.WriteLine("--> " + last.Length);
                            Thread.Sleep(Properties.Settings.Default.tailFilePollInterval);*/
                        }
                    }
                }
            });
            journalTailThread.Name = "JOURNAL TAIL THREAD: " + last.Name;
            journalTailThread.IsBackground = true; // do not block the application from terminating
            journalTailThread.Start();
        }

        public void stopTailing()
        {
            this.fileWatcherRunning = false;
            this.tailerRunning = false;
            this.abortJournalTailing = true;
            if (this.fileSystemWatcher != null)
                this.fileSystemWatcher.Dispose();
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Tailing aborted due to error.");
        }

        private void fileCreated(object sender, FileSystemEventArgs e)
        {
            this.logger.Log("New Journal file detected: {0}", e.Name);
            switchTailFile(new FileInfo(e.FullPath));
        }

        internal void fileChanged(object source, FileSystemEventArgs e)
        {
            long byteOffset;
            mainForm.cacheController._journalLengthCache.TryGetValue(e.Name, out byteOffset);
            readFileFromByteOffset(e, byteOffset);
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
            this.logger.Log($"Reading file {file} ({fileName}) from offset {offset}");
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (offset == 0) // if an offset isn't supplied (or it's sent as zero), see if we already have one stored
                    mainForm.cacheController._journalLengthCache.TryGetValue(fileName, out offset);
                if (offset > 0)
                    fs.Seek(offset, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string s = "";
                    List<ParsableJournalEntry> entries = new List<ParsableJournalEntry>();
                    while ((s = sr.ReadLine()) != null)
                    {
                        mainForm.cacheController._journalLengthCache[fileName] = fs.Position;
                        /*Commander c;
                        JournalEntry j = parseEvent(s, out c);
                        if (j.Event.Equals("FileHeader")) continue;
                        c.addJournalEntry(j);*/
                        entries.Add(new ParsableJournalEntry(s, file.StartsWith("JournalBeta")));
                    }
                    createJournalEntries(entries, true, dontPlaySounds: true, showNotifications: false);
                }
            }
        }
    }
}
