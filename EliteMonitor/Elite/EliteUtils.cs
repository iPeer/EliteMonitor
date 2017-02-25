using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class EliteUtils
    {
        public static readonly string JOURNAL_PATH = Environment.ExpandEnvironmentVariables(@"%userprofile%\Saved Games\Frontier Developments\Elite Dangerous\");
        public static readonly string GRAPHICS_OVERRIDE_PATH = Path.Combine(Environment.ExpandEnvironmentVariables(@"%localappdata%\Frontier Developments\Elite Dangerous\Options\Graphics"), "GraphicsConfigurationOverride.xml");

        public static bool IsEliteRunning()
        {
            return Process.GetProcessesByName("EliteDangerous32").Length > 0 || Process.GetProcessesByName("EliteDangerous64").Length > 0;
        }

    }
}
