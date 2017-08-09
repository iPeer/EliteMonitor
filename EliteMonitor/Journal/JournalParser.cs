using System;
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
        private bool fullParseInProgress = false;
        private string currentTailFile = "";
        private bool hasChangedLogFile = false;
        private bool tailerRunning = false;
        private Thread journalTailThread;
        private bool abortJournalTailing = false;
        private bool fileWatcherRunning = false;

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

        public JournalEntry parseEvent(string json, out Commander commander, bool isReparse = false, Commander forcedCommander = null)
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
            if (forcedCommander == null)
                commander = activeCommander ?? viewedCommander; // Under normal operating procedures, activeCommander will NEVER be null. It can be null during testing due to "hot inserting" of events (because we don't parse the full log properly), so if it's null, we default the the currently viewed commander.
            else
                commander = forcedCommander;
            JObject j;
            try
            {
                j = JObject.Parse(json);
            }
            catch (Exception e)
            {
                throw e;
            }

            // FIXME: Expedition entries are doubled when updating from journals that are ahead of our cache
            if (commander != null && !isReparse && commander.HasActiveExpedition)
                if (commander.Expeditions == null) // Commander's expedition file cannot be loaded after previously being present
                    commander.HasActiveExpedition = false;
                else
                    commander.Expeditions[commander.ActiveExpeditionGuid].parseJournalEntry(json);

            string @event = (string)j["event"];
            DateTime tsData = (DateTime)j["timestamp"];
            string timestamp = tsData.ToString("G");


            // NOTES: Missions that rank up a player in a major power have RankFed or RamkEmp in the names. This is (currently) the ONLY way to detect a Fed/Empire rank-up

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
                        commanderShip = mainForm.Database.getShipNameFromInternalName(commanderShip.ToLower());
                    }
                    catch
                    {
                        this.logger.Log("No real name for ship: {0}", LogLevel.ERR, commanderShip == string.Empty || commanderShip == null ? "{!!empty string!!}" : commanderShip);
                        this.logger.Log("{0}", LogLevel.ERR, j.ToString());
                    }
                    long commanderCredits = (long)j["Credits"];
                    string commanderName = (string)j["Commander"];
                    string shipName = (string)j["ShipName"];
                    string shipID = (string)j["ShipIdent"];
                    string commanderString = $"{commanderName}" + (isGroup ? $" ({pGroup})" : "");
                    Commander _commander = registerCommander(commanderName);
                    _commander.setBasicInfo(commanderShip, commanderCredits, isGroup ? pGroup : "");
                    _commander.SetShip(commanderShip, shipID, shipName);
                    commander = activeCommander = _commander;
                    string commanderShipFormatted = commander.ShipData.getFormattedShipString();
                    _commander.isInMulticrew = false;
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
                    if (channel.Equals("player") || sender.StartsWith("$cmdr_decorate") || channel.Equals("wing"))
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
                    string stationName = (string)j["StationName"];
                    string stationType = (string)j["StationType"];
                    string starSystem = (string)j["StarSystem"];
                    if (!isReparse)
                    {
                        commander.isDocked = true;
                        commander.CurrentSystem = starSystem;
                        commander.CurrentLocation = stationName;
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
                    }
                    return new JournalEntry(timestamp, @event, $"Jumped to {(string)j["StarSystem"]} ({String.Format("{0:f2}", (float)j["JumpDist"])}Ly)", j);
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
                    return new JournalEntry(timestamp, @event, String.Format("Collected. {0} : {1} ({2})", category, mainForm.Database.getMaterialNameFromInternalName(material), count), j);
                case "MaterialDiscovered":
                    material = (string)j["Name"];
                    category = (string)j["Category"];
                    return new JournalEntry(timestamp, @event, String.Format("Discovered new material: {0} : {1}", category, mainForm.Database.getMaterialNameFromInternalName(material)), j);
                case "MaterialDiscarded":
                    material = (string)j["Name"];
                    count = (int)j["Count"];
                    if (!isReparse)
                        commander.removeMaterial(material, count);
                    //_materialCounts[material] -= count;
                    return new JournalEntry(timestamp, @event, String.Format("Discarded. {0} : {1} ({2})", mainForm.Database.getTypeForMaterialByInternalName(material), mainForm.Database.getMaterialNameFromInternalName(material), count), j);
                case "MissionCompleted":
                    //Console.WriteLine(j.ToString());
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
                        eText = string.Format("Bountry credit of {0:n0} awarded.", value);
                    else
                        eText = string.Format("Bountry credit of {0:n0} awarded for destruction of {1} {2}.", value, againstFaction, mainForm.Database.getShipNameFromInternalName(ship));
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
                    return new JournalEntry(timestamp, @event, string.Format("Took hull damage! Hull health: {0:n0}%", (float)j["Health"] * 100.00), j);
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
                        commander.SetShip(new CommanderShip(ship, shipID, shipName));
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
                default:
                    return new JournalEntry(timestamp, @event, j.ToString(), "UNKNOWN EVENT", j, false);
            }
           
        }

        public void parseAllJournals()
        {
            this.fullParseInProgress = true;
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
                            allJournalEntries.Add(line);
                        }
                        
                    }
                }
            }
            try
            {
                createJournalEntries(allJournalEntries, false, true);
            }
            catch (Exception e)
            {
                throw e;
            }
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Switching commander...");
            switchViewedCommander(activeCommander);
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Ready.");
            this.fullParseInProgress = false;
        }

        public void createJournalEntries(List<string> entries, bool checkDuplicates = false, bool dontUpdateDisplays = false, bool dontUpdatePercentage = false)
        {
            if (!dontUpdatePercentage)
                mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Generating Journal entries...");
            mainForm.eventList.InvokeIfRequired(() => mainForm.eventList.BeginUpdate());
            int cEntry = 0;
            int lastPercent = 0;
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            Commander commander = activeCommander;
            List<string> preLoadCommanderData = new List<string>(3);
            foreach (string s in entries)
            {
                double percent = ((double)cEntry++ / (double)entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if (!dontUpdatePercentage && ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00))
                {
                    TimeSpan ts = (DateTime.Now - timeStarted);
                    double timeLeft = (ts.TotalSeconds / cEntry) * (entries.Count - cEntry);
                    lastETAUpdate = DateTime.Now;
                    lastPercent = (int)percent;
                    mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = String.Format("Generating Journal entries... ({0:n0}%) [ETA: {1}]", percent, Utils.formatTimeFromSeconds(timeLeft)));
                }
                JournalEntry je = parseEvent(s, out commander);
                if (je.Event.Equals("Fileheader"))
                    continue;
                if (je.Event.Equals("LoadGame") && /*!hasAlreadyLoaded*/commander != null)
                {
                    createJournalEntries(preLoadCommanderData, checkDuplicates, dontUpdateDisplays, true);
                    preLoadCommanderData.Clear();
                }
                if (/*!hasLoadedCommander*/commander == null)
                {
                    preLoadCommanderData.Add(s);
                    continue;
                }
                if (!mainForm.comboCommanderList.Items.Contains(commander.Name))
                {
                    mainForm.comboCommanderList.InvokeIfRequired(() => mainForm.comboCommanderList.Items.Add(commander.Name));
                    commander.JournalEntries = new List<JournalEntry>(entries.Count);
                }
                commander.addJournalEntry(je, checkDuplicates, dontUpdateDisplays);
            }
            mainForm.eventList.InvokeIfRequired(() => mainForm.eventList.EndUpdate());
            if (viewedCommander != null && activeCommander != null && commander != null && !fullParseInProgress && Properties.Settings.Default.autoSwitchActiveCommander && !viewedCommander.Name.Equals(activeCommander.Name))
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
            DateTime timeStarted = DateTime.Now;
            DateTime lastETAUpdate = DateTime.Now;
            foreach (JournalEntry j in _entries)
            {
                double percent = ((double)x++ / (double)_entries.Count) * 100.00;
                //Console.WriteLine(percent + " / " + lastPercent);
                if ((int)percent > lastPercent || DateTime.Now.Subtract(lastETAUpdate).TotalSeconds >= 1.00)
                {
                    TimeSpan ts = (DateTime.Now - timeStarted);
                    double timeLeft = (ts.TotalSeconds / x) * (entries.Count - x);
                    lastETAUpdate = DateTime.Now;
                    lastPercent = (int)percent;
                    mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = String.Format("Loading commander data for '{0}' ({1:n0}%) [ETA: {2}]", c.Name, percent, Utils.formatTimeFromSeconds(timeLeft)));
                }

                /*if (Properties.Settings.Default.showJournalUpdateStatus && (x++ == 1 || x % 100 == 0 || x == _entries.Count))
                    mainForm.appStatus.Text = $"Processing Journal entry {String.Format("{0:n0}", x)} of {String.Format("{0:n0}", _entries.Count)}";*/
                ListViewItem lvi = getListViewEntryForEntry(j);
                entries.Add(lvi);
            }
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = String.Format("Finalising commander data for '{0}'", c.Name));
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
            mainForm.buttonDiscoveredBodies.InvokeIfRequired(() => mainForm.buttonDiscoveredBodies.Enabled = true);
            mainForm.buttonExpeditions.InvokeIfRequired(() => mainForm.buttonExpeditions.Enabled = true);
        }

        public ListViewItem getListViewEntryForEntry(JournalEntry j)
        {
            ListViewItem lvi = new ListViewItem(new string[] { j.Timestamp, j.Event, j.Data, j.Notes });
            lvi.ToolTipText = j.Json;
            if (!j.isKnown)
            {
                lvi.BackColor = Color.Pink;
                //lvi.SubItems[3].Text = "UNKNOWN EVENT";
            }
            if (Properties.Settings.Default.enableEntryHighlighting)
            {
                if (j.Event.Equals("LoadGame"))
                    lvi.BackColor = Color.LightGreen;
                else if (j.Event.Equals("SendText") || j.Event.Equals("ReceiveText"))
                    lvi.BackColor = Color.LightGoldenrodYellow;
                else if (j.Event.Equals("FSDJump") || j.Event.Equals("StartJump"))
                    lvi.BackColor = Color.LightGray;
            }
            return lvi;
        }

        public void switchTailFile(FileInfo fi)
        {
            this.logger.Log("Requested change to Journal file {0}", fi.FullName);
            this.hasChangedLogFile = true;
            this.currentTailFile = fi.FullName;
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
            FileInfo last = di.GetFiles().OrderBy(f => f.CreationTime).ToArray().Last();

            this.currentTailFile = last.FullName;

            tailFile(this.currentTailFile);


            this.logger.Log($"Setting up file watcher on directory {path}...");
            path = Environment.ExpandEnvironmentVariables(path);

            fileSystemWatcher = new FileSystemWatcher(path);
            //fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            //fileSystemWatcher.Changed += fileChanged;
            fileSystemWatcher.Created += fileCreated;
            fileSystemWatcher.Filter = "Journal*.log";
            fileSystemWatcher.EnableRaisingEvents = true;
            mainForm.InvokeIfRequired(() => mainForm.appStatus.Text = "Tailing started...");
        }

        public void tailFile(string filePath)
        {
            if (this.tailerRunning)
                return;
            this.tailerRunning = true;

            FileInfo last = new FileInfo(filePath);

            this.logger.Log("Tailer running on {0}...", last.Name);

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
                                createJournalEntries(new List<string>() { newEntry }, checkDuplicates: true, dontUpdatePercentage: true);
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
                    List<string> entries = new List<string>();
                    while ((s = sr.ReadLine()) != null)
                    {
                        mainForm.cacheController._journalLengthCache[fileName] = fs.Position;
                        /*Commander c;
                        JournalEntry j = parseEvent(s, out c);
                        if (j.Event.Equals("FileHeader")) continue;
                        c.addJournalEntry(j);*/
                        entries.Add(s);
                    }
                    createJournalEntries(entries, true);
                }
            }
        }
    }
}
