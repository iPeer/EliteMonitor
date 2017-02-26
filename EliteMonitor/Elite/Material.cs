using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public enum Rarity
    {
        Unknown, // Unknown rarity for unknown materials
        Standard,
        VeryCommon,
        Common,
        Uncommon,
        Rare,
        VeryRare
    }
    public class Material
    {

        public string InternalName { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public Rarity Rarity { get; private set; }
        public string Symbol { get; private set; }

        public Material(string internalName, string type, string realName, Rarity rarity, string symbol = null)
        {
            this.InternalName = internalName;
            this.Name = realName;
            this.Type = type;
            this.Rarity = rarity;
            this.Symbol = symbol;
        }

    }
}
