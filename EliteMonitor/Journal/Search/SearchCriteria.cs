using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Journal.Search
{
    public class SearchCriteria
    {

        public string[] Events { get; private set; }
        public string SearchText { get; private set; }
        public string OriginalQuery { get; private set; }

        public SearchCriteria(string query)
        {
            this.CreateSearchFromQuery(query);
        }

        public SearchCriteria(string[] events, string searchText)
        {
            this.OriginalQuery = string.Join(" ", string.Join(" ", events), searchText);
        }

        private void CreateSearchFromQuery(string query)
        {
            this.OriginalQuery = query;
            List<string> events = new List<string>();
            //List<string> texts = new List<string>();

            List<string> data = this.OriginalQuery.Split(' ').ToList();
            foreach (string s in data.ToList())
            {
                if (s.StartsWithIgnoreCase("event:"))
                {
                    events.Add(s.Split(':')[1].ToLower());
                    data.Remove(s);
                }
            }
            this.SearchText = string.Join(" ", data);
            this.Events = events.ToArray();
        }

        public bool EntryMatches(JournalEntry j)
        {
            if (this.Events.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(this.SearchText))
                    return this.Events.Contains(j.Event.ToLower());
                else
                    return this.Events.Contains(j.Event.ToLower()) && (j.Data.ContainsIgnoreCase(this.SearchText) || (!Properties.Settings.Default.JournalsSearchJson ? false : j.Json.ContainsIgnoreCase(this.SearchText)));
            }
            else/* if (this.Events.Length == 0)*/
            {
                if (string.IsNullOrWhiteSpace(this.SearchText)) return false;
                return j.Data.ContainsIgnoreCase(this.SearchText) || (!Properties.Settings.Default.JournalsSearchJson ? false : j.Json.ContainsIgnoreCase(this.SearchText));
            }
            //return false;
        }

    }
}
