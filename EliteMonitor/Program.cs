using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    static class Program
    {
        public static MainForm mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(mainForm = new MainForm());
            }
            catch (Exception e)
            {
                string crashLogDirectory = Path.Combine(Utils.getApplicationEXEFolderPath(), "crashes");
                Directory.CreateDirectory(crashLogDirectory);
                string crashFile = Path.Combine(crashLogDirectory, string.Format("{0}.log", new DateTime().ToString("YYYYMMdd-HHmmss")));
                using (StreamWriter sw = new StreamWriter(crashFile))
                {
                    sw.WriteLine(String.Format("Application crashed at {0}", new DateTime().ToLongTimeString()));
                    sw.WriteLine(String.Format("Application version: {0}, built: {1}", Utils.getApplicationVersion(), Utils.getAssemblyBuildTime()));
                    sw.WriteLine(e.Message);
                    sw.WriteLine(e.StackTrace);
                    sw.WriteLine();
                    foreach (KeyValuePair<object, object> kvp in e.Data)
                    {
                        sw.WriteLine(String.Format("{0}: {1}", kvp.Key.ToString(), kvp.Value.ToString()));
                    }
                }
                MessageBox.Show("Oh no! Elite Monitor has crashed, that's unfortunate. A crash log has been created at the location below, please forward this file to iPeer so this can be fixed!\n\n"+crashFile, "Crash!", MessageBoxButtons.OK);
                Process.Start(crashFile);
            }
        }
    }
}
