﻿using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Journal;
using EliteMonitor.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Exploration
{
    public partial class ScanJournalDialog : Form
    {
        public ScanJournalDialog()
        {
            InitializeComponent();
        }

        public void startScan(JournalEntry startPoint, Expedition expedition, Commander commander)
        {
            Console.WriteLine(string.Format("--> {0}", startPoint.ID));
            Console.WriteLine(string.Format("--> {0}", expedition.ExpeditionID));
            this.progressBar1.InvokeIfRequired(() => this.progressBar1.Style = ProgressBarStyle.Marquee);
            long startId = startPoint.ID;
            if (startId <= commander.JournalEntries.Count)
            {
                commander.HasActiveExpedition = true;
                commander.ActiveExpeditionGuid = expedition.ExpeditionID;
                string[] validEvents = new string[] { "FSDJump", "Scan" };
                List<JournalEntry> entries = commander.JournalEntries.FindAll(a => a.ID >= startId && validEvents.Contains(a.Event));
                this.progressBar1.InvokeIfRequired(() => this.progressBar1.Style = ProgressBarStyle.Continuous);
                this.progressBar1.InvokeIfRequired(() => this.progressBar1.Maximum = entries.Count);
                this.progressBar1.InvokeIfRequired(() => this.progressBar1.Step = 1);
                this.progressBar1.InvokeIfRequired(() => this.progressBar1.Value = 0);
                foreach (JournalEntry je in entries)
                {
                    if (expedition.parseJournalEntry(je))
                    {
                        Console.WriteLine(string.Format("--> {0}", je.ID));
                        this.progressBar1.InvokeIfRequired(() => this.progressBar1.Value = this.progressBar1.Maximum);
                        break;
                    }
                }
            }
            if (commander.Expeditions == null)
                commander.Expeditions = new Dictionary<Guid, Expedition>();
            commander.Expeditions.Add(expedition.ExpeditionID, expedition);
            MainForm.Instance.cacheController.saveAllCaches();
            this.InvokeIfRequired(() => this.Close());
            //TODO: show expedition GUI
        }

    }
}
