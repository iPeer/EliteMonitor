using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{

    public class NoClassOrSizeAvailableException : Exception { }

    public class EliteUtils
    {
        public static readonly string JOURNAL_PATH = Environment.ExpandEnvironmentVariables(@"%userprofile%\Saved Games\Frontier Developments\Elite Dangerous\");
        public static readonly string GRAPHICS_OVERRIDE_PATH = Path.Combine(Environment.ExpandEnvironmentVariables(@"%localappdata%\Frontier Developments\Elite Dangerous\Options\Graphics"), "GraphicsConfigurationOverride.xml");
        public static readonly string[] CLASS_NAMES = { "", "E", "D", "C", "B", "A" };
        public static List<string> htpList = new List<string>();

        public static bool IsEliteRunning()
        {
            return Process.GetProcessesByName("EliteDangerous32").Length > 0 || Process.GetProcessesByName("EliteDangerous64").Length > 0;
        }

        public static string[] getSizeAndClassFromInternalName(string internalName, bool applySettings = false)
        {

            string size = string.Empty;
            string className = string.Empty;

            if (internalName.Contains("_size") && internalName.Contains("_class"))
            {
                int sizePos = internalName.IndexOf("_size") + 5;
                int classPos = internalName.IndexOf("_class") + 6;

                int @class = Convert.ToInt32(internalName.Substring(classPos, 1));
                size = internalName.Substring(sizePos, 1);

                className = CLASS_NAMES[@class];

                //Console.WriteLine(string.Format("{0}: {1} {2} -> {3}{4}", internalName, sizePos, classPos, className, size));
            }
            else
            {

                return new string[0];                
                //throw new NoClassOrSizeAvailableException();
                // "StoredItem": "$hpt_multicannon_gimbal_large_name;"
                /*Console.WriteLine(internalName);
                switch (internalName.Substring(5, internalName.Length - 5))
                {
                    case "multicannon_gimbal_small":
                        size = "1";
                        className = "F";
                        break;
                    case "multicannon_gimbal_medium":
                        size = "2";
                        className = "E";
                        break;
                    case "multicannon_gimbal_large":
                        size = "3";
                        className = "C";
                        break;
                    case "multicannon_gimbal_huge":
                        size = "4";
                        className = "A";
                        break;
                }*/
            }

            if (applySettings && Properties.Settings.Default.moduleClassBeforeSize)
                return new string[] { className, size };
            return new string[] { size, className };
        }

    }
}
