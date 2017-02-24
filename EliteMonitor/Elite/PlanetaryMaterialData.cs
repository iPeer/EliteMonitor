using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class PlanetaryMaterialData // Util class
    {

        //{"material_id":19,"material_name":"Tungsten","share":0.6}

        public long material_id { get; set; }
        public string material_name { get; set; }
        public double share { get; set; }

    }
}
