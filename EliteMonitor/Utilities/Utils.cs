using EliteMonitor.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EliteMonitor.Elite;
using System.Media;

namespace EliteMonitor.Utilities
{
    public class Utils
    {

        public const int LIST_VIEW_MAX_STRING_LENGTH = 259;

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

        public static string formatTimeFromGalacticSeconds(double time)
        {
            double seconds = Math.Floor(time % 60);
            double minutes = Math.Floor((time % 3600) / 60);
            double hours = Math.Floor(time / 3600);
            double days = Math.Floor(hours / 24);
            hours -= Math.Floor(days * 24);

            double years = Math.Floor(days / 365);
            days -= Math.Floor(years * 365);

            double weeks = Math.Floor(days / 7);
            days -= Math.Floor(weeks * 7);

            List<string> elements = new List<string>();

            if (years > 0)
                elements.Add(string.Format("{0:n0} {1}", years, years == 1 ? "year" : "years"));
            if (weeks > 0)
                elements.Add(string.Format("{0:n0} {1}", weeks, weeks == 1 ? "week" : "weeks"));
            if (days > 0)
                elements.Add(string.Format("{0:n0} {1}", days, days == 1 ? "day" : "days"));
            if (hours > 0)
                elements.Add(string.Format("{0:n0} {1}", hours, hours == 1 ? "hour" : "hours"));
            if (minutes > 0)
                elements.Add(string.Format("{0:n0} {1}", minutes, minutes == 1 ? "minute" : "minutes"));
            if (seconds > 0)
                elements.Add(string.Format("{0:n0} {1}", seconds, seconds == 1 ? "second" : "seconds"));
            return elements.ToArray().JoinWithDifferingLast(", ");
        }

        public static long saveDataFile(string fileName, string filePath, string data, string fileExtension = ".emj", bool compressed = true)
        {
            if (compressed)
                return writeGZip(fileName, filePath, data, fileExtension);
            else
            {
                string fp = Path.Combine(filePath, string.Format("{0}{1}.uncompressed", fileName, fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension));
                using (StreamWriter sw = new StreamWriter(fp))
                {
                    sw.WriteLine(data);
                }
                return 0;
            }
        }

        public static long saveGZip(string fileName, string filePath, string data, string fileExtension = ".emj") => writeGZip(fileName, filePath, data, fileExtension);
        public static long writeGZip(string fileName, string filePath, string data, string fileExtension = ".emj")
        {
            if (new string[] { ".eml", "eml" }.Contains(fileExtension))
            {
                throw new InvalidOperationException("Files cannot use the \"eml\" extension as it is reserved for length data for GZipped files.");
            }
            string fp = Path.Combine(filePath, string.Format("{0}{1}", fileName, fileExtension.StartsWith(".") ? fileExtension : "."+fileExtension));
            string bp = Path.Combine(filePath, string.Format("{0}.eml", fileName));

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            using (FileStream fs = new FileStream(fp, FileMode.Create))
            {
                using (GZipStream gz = new GZipStream(fs, CompressionLevel.Optimal))
                {
                    gz.Write(bytes, 0, bytes.Length);
                }
            }

            using (StreamWriter sw = new StreamWriter(bp, false, Encoding.UTF8))
            {
                sw.WriteLine(bytes.LongLength);
            }

            return bytes.LongLength;

        }

        public static string loadGZip(string filePath, bool noErrorIfNotFound = false) => readGZip(filePath, noErrorIfNotFound);
        public static string readGZip(string filePath, bool noErrorIfNotFound = false)
        {
            if (!File.Exists(filePath))
            {
                if (noErrorIfNotFound)
                {
                    return string.Empty;
                }
                else
                {
                    throw new FileNotFoundException("File '" + filePath + "' was not found.");
                }
            }
            FileInfo fi = new FileInfo(filePath);
            string byteCountFile = fi.FullName.Replace(fi.Extension, ".eml");
            if (!File.Exists(byteCountFile))
            {
                if (MessageBox.Show("There was an error while attempting to load commander data. EliteMonitor will need to generate the cache again from scratch.\nNote: If you have deleted or lost Journal files since the cache was generated, this will result in lost entries") == DialogResult.OK)
                {
                    MainForm.Instance.cacheController.clearCaches();
                    MainForm.Instance.startNoCacheLoadThread();
                }
                throw new FileNotFoundException("No matching byte count file was found for file '" + filePath + "'");
            }
            long byteCount = 0L;
            using (StreamReader sr = new StreamReader(byteCountFile, Encoding.UTF8))
            {
                string byteData = sr.ReadToEnd();
                byteCount = Convert.ToInt64(byteData);
            }

            string output = string.Empty;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {

                byte[] bytes = new byte[byteCount];
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    gz.Read(bytes, 0, (int)byteCount);
                    output = Encoding.UTF8.GetString(bytes);
                }

            }

            return output;

        }

        //http://stackoverflow.com/a/5427121
        public static string Prompt(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            textLabel.AutoSize = true;
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        public static string CreateSafeFilename(string v)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                v.Replace(c, '_');
            return v;
        }

        public static double CalculateLyDistance(SystemCoordinate to, SystemCoordinate from)
        {
            return Math.Sqrt(Math.Pow((from.X - to.X), 2) + Math.Pow((from.Y - to.Y), 2) + Math.Pow((from.Z - to.Z), 2));
        }

        public static string GetLandmarkDistancesString()
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, SystemCoordinate> landmarkSystems = new Dictionary<string, SystemCoordinate>()
            {
                { "Sol", new SystemCoordinate(0, 0, 0) },
                { "Great Annihilator", new SystemCoordinate(354.84375, -42.4375, 22997.21875) },
                { "Sagittarius A*", new SystemCoordinate(25.21875, -20.90625, 25899.96875) },
                { "Beagle Point", new SystemCoordinate(-1111.5625, -134.21875, 65269.75) },
            };

            foreach (KeyValuePair<string, SystemCoordinate> kvp in landmarkSystems)
            {
                Commander commander = MainForm.Instance.journalParser.viewedCommander;
                if (commander == null) break;
                if (commander.HasHomeSystem && kvp.Key.Equals(commander.HomeSystem.Name)) continue;
                sb.AppendFormat("Distance from {0}: {1:n2} Ly\n", kvp.Key, CalculateLyDistance(kvp.Value, commander.CurrentSystemCoordinates));
            }


            return sb.ToString().TrimEnd();
        }

        internal static void PlaySound(string soundFile)
        {
            SoundPlayer sp = new SoundPlayer();
            string soundPath = Path.Combine(MainForm.Instance.cacheController.soundsPath, soundFile);
            if (File.Exists(soundPath))
            {
                sp.SoundLocation = Path.Combine(MainForm.Instance.cacheController.soundsPath, soundFile);
                sp.Play();
            }
        }
    }
}
