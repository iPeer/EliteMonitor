using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{

    public class NoElementMatchesSymbolException : Exception { }

    public class Materials
    {
        // There are some materials missing from here as they don't seem to actually exist in the game
        // List in order of rarity and alphabetically for readability
        private static List<Material> MATERIALS = new List<Material>() // Oh boy!
        { 

            // Elements

            new Material("carbon", "Element", "Carbon", Rarity.VeryCommon, "C"),
            new Material("iron", "Element", "Iron", Rarity.VeryCommon, "Fe"),
            new Material("nickel", "Element", "Nickel", Rarity.VeryCommon, "Ni"),
            new Material("phosphorus", "Element", "Phosphorus", Rarity.VeryCommon, "P"),
            new Material("sulphur", "Element", "Sulphur", Rarity.VeryCommon, "S"),

            new Material("chromium", "Element", "Chromium", Rarity.Common, "Cr"),
            new Material("germanium", "Element", "Germanium", Rarity.Common, "Ge"),
            new Material("manganese", "Element", "Manganese", Rarity.Common, "Mn"),
            new Material("vanadium", "Element", "Vanadium", Rarity.Common, "V"),
            new Material("zinc", "Element", "Zinc", Rarity.Common, "Zn"),

            new Material("arsenic", "Element", "Arsenic", Rarity.Standard, "As"),
            new Material("niobium", "Element", "Niobium", Rarity.Standard, "Nb"),
            new Material("selenium", "Element", "Selenium", Rarity.Standard, "Se"),
            new Material("tungsten", "Element", "Tungsten", Rarity.Standard, "W"),
            new Material("zirconium", "Element", "Zirconium", Rarity.Standard, "Zr"),

            new Material("cadmium", "Element", "Cadmium", Rarity.Rare, "Cd"),
            new Material("mercury", "Element", "Mercury", Rarity.Rare, "Hg"),
            new Material("molybdenum", "Element", "Molybdenum", Rarity.Rare, "Mo"),
            new Material("tin", "Element", "Tin", Rarity.Rare, "Sn"),
            new Material("yttrium", "Element", "Yttrium", Rarity.Rare, "Y"),

            new Material("antimony", "Element", "Antimony", Rarity.VeryRare, "Sb"),
            new Material("polonium", "Element", "Polonium", Rarity.VeryRare, "Po"),
            new Material("ruthenium", "Element", "Ruthenium", Rarity.VeryRare, "Ru"),
            new Material("technetium", "Element", "Technetium", Rarity.VeryRare, "Tc"),
            new Material("tellurium", "Element", "Tellurium", Rarity.VeryRare, "Te"),

            // Manufactured

            new Material("basicconductors", "Manufactured", "Basic Conductors", Rarity.VeryCommon),
            new Material("crystalshards", "Manufactured", "Crystal Shards", Rarity.VeryCommon),
            new Material("gridresistors", "Manufactured", "Grid Resistors", Rarity.VeryCommon),
            new Material("heatconductionwiring", "Manufactured", "Heat Conduction Wiring", Rarity.VeryCommon),
            new Material("mechanicalscrap", "Manufactured", "Mechanical Scrap", Rarity.VeryCommon),
            new Material("salvagedalloys", "Manufactured", "Salvaged Alloys", Rarity.VeryCommon),
            new Material("wornshieldemitters", "Manufactured", "Worn Shield Emitters", Rarity.VeryCommon),

            new Material("chemicalprocessors", "Manufactured", "Chemical Processors", Rarity.Common),
            new Material("conductivecomponents", "Manufactured", "Conductive Components", Rarity.Common),
            new Material("uncutfocuscrystals", "Manufactured", "Flawed Focus Crystals", Rarity.Common),
            new Material("galvanisingalloys", "Manufactured", "Galvanising Alloys", Rarity.Common),
            new Material("heatdispersionplate", "Manufactured", "Heat Dispersion Plate", Rarity.Common),
            new Material("heatresistantceramics", "Manufactured", "Heat Resistant Ceramics", Rarity.Common),
            new Material("hybridcapacitors", "Manufactured", "Hybrid Capacitors", Rarity.Common),
            new Material("mechanicalequipment", "Manufactured", "Mechanical Equipment", Rarity.Common),
            new Material("shieldemitters", "Manufactured", "Shield Emitters", Rarity.Common),

            new Material("chemicaldistillery", "Manufactured", "Chemical Distillery", Rarity.Standard),
            new Material("conductiveceramics", "Manufactured", "Conductive Ceramics", Rarity.Standard),
            new Material("electrochemicalarrays", "Manufactured", "Electrochemical Arrays", Rarity.Standard),
            new Material("focuscrystals", "Manufactured", "Focus Crystals", Rarity.Standard),
            new Material("heatexchangers", "Manufactured", "Heat Exchangers", Rarity.Standard),
            new Material("highdensitycomposites", "Manufactured", "High Density Composites", Rarity.Standard),
            new Material("mechanicalcomponents", "Manufactured", "Mechanical Components", Rarity.Standard),
            new Material("phasealloys", "Manufactured", "Phase Alloys", Rarity.Standard),
            new Material("precipitatedalloys", "Manufactured", "Precipitated Alloys", Rarity.Standard),
            new Material("shieldingsensors", "Manufactured", "Shielding Sensors", Rarity.Standard),

            new Material("chemicalmanipulators", "Manufactured", "Chemical Manipulators", Rarity.Rare),
            new Material("compoundshielding", "Manufactured", "Compound Shielding", Rarity.Rare),
            new Material("conductivepolymers", "Manufactured", "Conductive Polymers", Rarity.Rare),
            new Material("configurablecomponents", "Manufactured", "Configurable Components", Rarity.Rare),
            new Material("heatvanes", "Manufactured", "Heat Vanes", Rarity.Rare),
            new Material("polymercapacitors", "Manufactured", "Polymer Capacitors", Rarity.Rare),
            new Material("fedproprietarycomposites", "Manufactured", "Proprietary Composites", Rarity.Rare),
            new Material("protolightalloys", "Manufactured", "Proto Light Alloys", Rarity.Rare),
            new Material("refinedfocuscrystals", "Manufactured", "Refined Focus Crystals", Rarity.Rare),
            new Material("thermicalloys", "Manufactured", "Thermic Alloys", Rarity.Rare),

            new Material("biotechconductors", "Manufactured", "Biotech Conductors", Rarity.VeryRare),
            new Material("fedcorecomposites", "Manufactured", "Core Dynamics Composites", Rarity.VeryRare),
            new Material("exquisitefocuscrystals", "Manufactured", "Exquisite Focus Crystals", Rarity.VeryRare),
            new Material("imperialshielding", "Manufactured", "Imperial Shielding", Rarity.VeryRare),
            new Material("improvisedcomponents", "Manufactured", "Improvised Components", Rarity.VeryRare),
            new Material("militarygradealloys", "Manufactured", "Military Grade Alloys", Rarity.VeryRare),
            new Material("militarysupercapacitors", "Manufactured", "Military Supercapacitors", Rarity.VeryRare),
            new Material("pharmaceuticalisolators", "Manufactured", "Pharmaceutical Isolators", Rarity.VeryRare),
            new Material("protoheatradiators", "Manufactured", "Proto Heat Radiators", Rarity.VeryRare),
            new Material("protoradiolicalloys", "Manufactured", "Proto Radiolic Alloys", Rarity.VeryRare),
            new Material("unknownenergysource", "Manufactured", "Unknown Fragment", Rarity.VeryRare),

            // Data

            new Material("bulkscandata", "Data", "Anomalous Bulk Scan Data", Rarity.VeryCommon),
            new Material("disruptedwakeechoes", "Data", "Atypical Disrupted Wake Echoes", Rarity.VeryCommon),
            new Material("shieldcyclerecordings", "Data", "Distorted Shield Cycle Recordings", Rarity.VeryCommon),
            new Material("scrambledemissiondata", "Data", "Exceptional Scrambled Emission Data", Rarity.VeryCommon),
            new Material("legacyfirmware", "Data", "Specialised Legacy Firmware", Rarity.VeryCommon),
            new Material("encryptedfiles", "Data", "Unusual Encrypted Files", Rarity.VeryCommon),

            new Material("fsdtelemetry", "Data", "Anomalous FSD Telemetry", Rarity.Common),
            new Material("shieldsoakanalysis", "Data", "Inconsistent Shield Soak Analysis", Rarity.Common),
            new Material("archivedemissiondata", "Data", "Irregular Emission Data", Rarity.Common),
            new Material("consumerfirmware", "Data", "Modified Consumer Firmware", Rarity.Common),
            new Material("encryptioncodes", "Data", "Tagged Encryption Codes", Rarity.Common),
            new Material("scanarchives", "Data", "Unidentified Scan Archives", Rarity.Common),

            new Material("scandatabanks", "Data", "Classified Scan Databanks", Rarity.Standard),
            new Material("industrialfirmware", "Data", "Cracked Industrial Firmware", Rarity.Standard),
            new Material("symmetrickeys", "Data", "Open Symmetric Keys", Rarity.Standard),
            new Material("wakesolutions", "Data", "Strange Wake Solutions", Rarity.Standard),
            new Material("emissiondata", "Data", "Unexpected Emission Data", Rarity.Standard),
            new Material("shielddensityreports", "Data", "Untypical Shield Scans", Rarity.Standard),
            new Material("unknownshipsignature", "Data", "Unknown Ship Signature", Rarity.Standard),
            new Material("unknownwakedata", "Data", "Unknown Wake Data", Rarity.Standard),

            new Material("shieldpatternanalysis", "Data", "Aberrant Shield Pattern Analysis", Rarity.Rare),
            new Material("encryptionarchives", "Data", "Atypical Encryption Archives", Rarity.Rare),
            new Material("decodedemissiondata", "Data", "Decoded Emission Data", Rarity.Rare),
            new Material("encodedscandata", "Data", "Divergent Scan Data", Rarity.Rare),
            new Material("hyperspacetrajectories", "Data", "Eccentric Hyperspace Trajectories", Rarity.Rare),
            new Material("securityfirmware", "Data", "Security Firmware Patch", Rarity.Rare),

            new Material("compactemissionsdata", "Data", "Abnormal Compact Emission Data", Rarity.VeryRare),
            new Material("adaptiveencryptors", "Data", "Adaptive Encryptors Capture", Rarity.VeryRare),
            new Material("classifiedscandata", "Data", "Classified Scan Fragment", Rarity.VeryRare),
            new Material("dataminedwake", "Data", "Datamined Wake Exceptions", Rarity.VeryRare),
            new Material("embeddedfirmware", "Data", "Modified Embedded Firmware", Rarity.VeryRare),
            new Material("shieldfrequencydata", "Data", "Peculiar Shield Frequency Data", Rarity.VeryRare)
        };

        public static string getMaterialNameFromInternalName(string internalName)
        {
            Material m = MATERIALS.FirstOrDefault(a => a.InternalName.Equals(internalName));
            if (m == null) return internalName;
            return m.Name;
        }

        public static string getInternalNameFromRealName(string realName)
        {
            Material m = MATERIALS.FirstOrDefault(a => a.Name.Equals(realName));
            if (m == null) return realName;
            return m.InternalName;
        }

        public static Material getMaterialFromElementSymbol(string symbol)
        {
            Material m = MATERIALS.FirstOrDefault(a => a.Symbol.Equals(symbol));
            if (m == null) throw new NoElementMatchesSymbolException();
            return m;
        }

        public static string getElementNameFromSymbol(string symbol)
        {
            try
            {
                Material m = getMaterialFromElementSymbol(symbol);
                return m.Name;
            }
            catch (NoElementMatchesSymbolException) { return symbol; }
        }

        public static object getTypeForMaterialByInternalName(string internalName)
        {
            Material m = MATERIALS.FirstOrDefault(a => a.InternalName.Equals(internalName));
            if (m == null) return "Unknown";
            return m.Type;
        }
    }
}
