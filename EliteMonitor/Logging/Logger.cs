﻿using EliteMonitor.Extensions;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EliteMonitor.Logging
{
    public enum LogLevel
    {
        INFO,
        INF,
        WARNING,
        WRN,
        ERROR,
        ERR,
        DEBUG,
        DBG
    }

    public class Logger
    {

        public string Name { get; private set; }
        public string LogPath { get; private set; }
        private bool alwaysPrefix = false;

        public Logger(String name) : this(name, Path.Combine(Utils.getApplicationEXEFolderPath(), "logs", String.Format("{0}.log", name))) { }

        public Logger(String name, String path)
        {

            this.Name = name;
            this.LogPath = path;

            // Check if the logs directy exists, if not, make it so.
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            this.ArchiveAndRemoveOldLogs();
            // At this point, everything is save to use, so we can start writing stuff to the log
            this.LogToEngineAndModule("Logger created with name '{0}' -- File path: {1}", name, path);

        }

        public Logger createSubLogger(string name)
        {
            Logger l = new Logger(name, this.LogPath);
            l.alwaysPrefix = true;
            return l;
        }

        public void ArchiveAndRemoveOldLogs()
        {
            if (!File.Exists(this.LogPath))
                return;

            /*List<string> numberedFiles = Directory.GetFiles(Path.GetDirectoryName(this.LogPath)).Where(a => Regex.IsMatch(a, @"log\.[0-9]+")).ToList();
            if (numberedFiles.Count > 0)
                foreach (string s in numberedFiles)
                    File.Delete(s);

            if (File.Exists(String.Format("{0}.last", this.LogPath))) {
                File.Delete(String.Format("{0}.last", this.LogPath));
            }

            File.Copy(this.LogPath, String.Format("{0}.last", this.LogPath));
            File.WriteAllText(this.LogPath, string.Empty);*/
            if (File.Exists(String.Format("{0}.last", this.LogPath)))
                File.Move(String.Format("{0}.last", this.LogPath), String.Format("{0}.1", this.LogPath));
            if (File.Exists(String.Format("{0}.5", this.LogPath)))
                File.Delete(String.Format("{0}.5", this.LogPath));
            for (int x = 4; x > 0; x--)
            {
                if (File.Exists(String.Format("{0}.{1}", this.LogPath, x)))
                    File.Move(String.Format("{0}.{1}", this.LogPath, x), String.Format("{0}.{1}", this.LogPath, x + 1));
            }
            File.Copy(this.LogPath, String.Format("{0}.1", this.LogPath));
            File.WriteAllText(this.LogPath, String.Empty);

        }

        public void LogToEngineAndModule(string message, params object[] fillers)
        {
            if (MainForm.Instance != null)
                MainForm.Instance.logger.Log(message, LogLevel.INFO, fillers);
            this.Log(message, LogLevel.INFO, fillers);
        }

        public void Log(string message, params object[] fillers)
        {
            Log(message, LogLevel.INFO, fillers);
        }

        public void Log(string message, LogLevel level, params object[] fillers)
        {

            // Before we do anything, lock to prevent conflicts

            lock (this)
            {

                string _msg = String.Format(message, fillers);
                string type = "INF";
                switch (level)
                {
                    case LogLevel.ERR:
                    case LogLevel.ERROR:
                        type = "ERR";
                        break;
                    case LogLevel.DBG:
                    case LogLevel.DEBUG:
                        type = "DBG";
                        break;
                    case LogLevel.WRN:
                    case LogLevel.WARNING:
                        type = "WRN";
                        break;
                    default:
                        type = "INF";
                        break;
                }

                string time = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss");

                string @string = string.Empty;
                if (this.alwaysPrefix)
                    @string = String.Format("[{0}] {1}: [{3}] {2}", time, type, _msg, this.Name.ToUpper());
                else
                    @string = String.Format("[{0}] {1}: {2}", time, type, _msg);

#if DEBUG
                Console.WriteLine(@string);
                System.Diagnostics.Debug.WriteLine(@string);
#endif

                // Write the juicy goodness to file
                using (StreamWriter w = new StreamWriter(this.LogPath, true, Encoding.UTF8))
                {
                    w.WriteLine(@string);
                }


            }
        }

    }
}
