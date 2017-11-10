using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class Material
    {

        public Material() { }
        public string Name { get; set; }
        public string Type { get; set; }

        /*public string InternalName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Rarity Rarity { get; set; }
        public string Symbol { get; set; }

        public Material(string internalName, string type, string realName, Rarity rarity, string symbol = null)
        {
            this.InternalName = internalName;
            this.Name = realName;
            this.Type = type;
            this.Rarity = rarity;
            this.Symbol = symbol;
        }

        public Material()
        {
        }*/
    }
}
