using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Utilities
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBuildTime : Attribute
    {

        public string BuildTime { get; private set; }
        public AssemblyBuildTime(string date) { this.BuildTime = date; }

    }

}
