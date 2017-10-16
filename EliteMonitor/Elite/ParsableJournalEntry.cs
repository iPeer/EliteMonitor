using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class ParsableJournalEntry
    {

        public string JSON { get; set; }
        public bool IsBetaJournal { get; set; } = false;

        public ParsableJournalEntry(string json, bool isBetaJournal)
        {
            this.JSON = json;
            this.IsBetaJournal = isBetaJournal;
        }
    }
}
