using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    public class JournalEntry
    {

        public string Timestamp { get; set; }
        public string Event { get; set; }
        public string Data { get; set; }
        public string Json { get; set; }
        public string Notes { get; set; }
        public bool isKnown { get; set; } = true;
        public long ID { get; set; }

        [JsonConstructor]
        public JournalEntry() { }

        public JournalEntry(string timestamp, string @event, string data, string notes, string json, bool known = true)
        {
            this.Timestamp = timestamp;
            this.Event = @event;
            this.Data = data;
            this.Notes = notes;
            this.Json = json;
            this.isKnown = known;
        }

        public JournalEntry(string timestamp, string @event, string data, string notes, JObject json, bool known = true)
        {
            this.Timestamp = timestamp;
            this.Event = @event;
            this.Data = data;
            this.Notes = notes;
            this.Json = json.ToString();
            this.isKnown = known;
        }

        public JournalEntry(string timestamp, string @event, string data, string json, bool known = true)
        {
            this.Timestamp = timestamp;
            this.Event = @event;
            this.Data = data;
            this.Json = json;
            this.isKnown = known;
        }

        public JournalEntry(string timestamp, string @event, string data, JObject json, bool known = true)
        {
            this.Timestamp = timestamp;
            this.Event = @event;
            this.Data = data;
            this.Json = json.ToString();
            this.isKnown = known;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return this.Json.Equals(((JournalEntry)obj).Json);
            }
            catch { return false; }
        }

        public override int GetHashCode()
        {
            return this.Json.GetHashCode();
        }

    }
}
