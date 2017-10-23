using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class BasicSystem
    {

        public string Name { get; private set; }
        public long ID { get; private set; }
        public SystemCoordinate Coordinates { get; private set; }
        public string Allegiance { get; set; } = "Independent";
        public string Economy { get; set; } = "None";
        public double DistanceFromSol {
            get
            {
                return Utilities.Utils.CalculateLyDistance(this.Coordinates, 0, 0, 0);
            }
        }

        public BasicSystem(string name, long ID, SystemCoordinate coordinates, string allegiance = "", string economy = "")
        {
            this.Name = name;
            this.ID = ID;
            this.Coordinates = coordinates;
            this.Allegiance = allegiance;
            this.Economy = economy;
        }
    }
}
