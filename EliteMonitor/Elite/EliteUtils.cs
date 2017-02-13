using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class EliteUtils
    {
        public static readonly string JOURNAL_PATH = Environment.ExpandEnvironmentVariables(@"%userprofile%\Saved Games\Frontier Developments\Elite Dangerous\");

        public static bool IsEliteRunning()
        {
            return Process.GetProcessesByName("EliteDangerous32").Length > 0 || Process.GetProcessesByName("EliteDangerous64").Length > 0;
        }

    }
}
