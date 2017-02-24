using EliteMonitor.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Utilities
{
    public class Utils
    {
        public static string getAssemblyBuildTime()
        {
            AssemblyBuildTime att = (AssemblyBuildTime)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyBuildTime));
            return att.BuildTime;
        }

        public static string getApplicationVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }

        public static int getBuildNumber()
        {
            Version v = new Version(getApplicationVersion());
            return v.Revision;
        }

        public static string getApplicationEXEFolderPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        internal static string getCreditsString(int v)
        {
            return String.Format("{0:n0} Credits", v);
        }

        internal static int getCredits()
        {
            return Convert.ToInt32(MainForm.Instance.creditsLabel.Text.Split(' ')[0].Replace(",", ""));
        }

        internal static void deductCredits(int price)
        {
            int credits = getCredits();
            credits -= price;
            MainForm.Instance.creditsLabel.InvokeIfRequired(() => MainForm.Instance.creditsLabel.Text = getCreditsString(credits));
        }

        internal static void addCredits(int price)
        {
            int credits = getCredits();
            credits += price;
            MainForm.Instance.creditsLabel.InvokeIfRequired(() => MainForm.Instance.creditsLabel.Text = getCreditsString(credits));
        }

        internal static string formatTimeFromSeconds(double time)
        {
            TimeSpan ts = TimeSpan.FromSeconds(time);
            string t = ts.ToString(@"hh\:mm\:ss");
            while (t.StartsWith("00:"))
            {
                t = t.Substring(3);
            }
            
            return t+(t.Length == 2 ? "s" : "");
        }
    }
}
