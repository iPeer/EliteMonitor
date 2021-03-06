﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class SystemCoordinate
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public SystemCoordinate(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", this.X, this.Y, this.Z);
        }

    }

    public class DSSScanBodyMaterialsData // What a trainwreck of a name that is
    {
        public string Name { get; set; }
        public double Percent { get; set; }
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
