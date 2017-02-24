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

        public BasicSystem(string name, long ID, SystemCoordinate coordinates)
        {
            this.Name = name;
            this.ID = ID;
            this.Coordinates = coordinates;
        }
    }
}
