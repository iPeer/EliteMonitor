using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Journal
{
    public class JournalCommunityGoal
    {
        public int CGID { get; set; }
        public string Title { get; set; }
        public string SystemName { get; set; }
        public string MarketName { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsComplete { get; set; }
        public long CurrentTotal { get; set; }
        public int PlayerContribution { get; set; }
        public int NumContributors { get; set; }
        public int TopRankSize { get; set; }
        public bool PlayerInTopRank { get; set; }
        public string TierReached { get; set; }
        public int PlayerPercentileBand { get; set; }
        public int Bonus { get; set; }
    }
}
