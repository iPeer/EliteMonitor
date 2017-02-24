using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class SystemCoordinate
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SystemCoordinate(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
    public class Body
    {

        public string Name { get; private set; }
        public string Type { get; private set; }
        public string SystemName { get; private set; }
        public int EntryDistance { get; private set; }

        public SystemCoordinate Coordinates { get; private set; } = new SystemCoordinate(0, 0, 0);

        public Dictionary<string, double> Materials { get; private set; } = new Dictionary<string, double>();

        public Body(string name, string type, string SystemName, int entryDistance, SystemCoordinate coordinates, Dictionary<string, double> materials)
        {
            this.Name = name;
            this.Type = type;
            this.SystemName = SystemName;
            this.EntryDistance = entryDistance;
            this.Coordinates = coordinates;
            this.Materials = materials;
        }


    }
}
