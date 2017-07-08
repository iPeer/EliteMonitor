using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Elite
{
    class Ranks
    {

        public static Image[] combat = new Image[] { Properties.Resources.rank_1_combat, Properties.Resources.rank_2_combat, Properties.Resources.rank_3_combat, Properties.Resources.rank_4_combat, Properties.Resources.rank_5_combat, Properties.Resources.rank_6_combat, Properties.Resources.rank_7_combat, Properties.Resources.rank_8_combat, Properties.Resources.rank_9_combat };
        public static Image[] trade = new Image[] { Properties.Resources.rank_1_trading, Properties.Resources.rank_2_trading, Properties.Resources.rank_3_trading, Properties.Resources.rank_4_trading, Properties.Resources.rank_5_trading, Properties.Resources.rank_6_trading, Properties.Resources.rank_7_trading, Properties.Resources.rank_8_trading, Properties.Resources.rank_9_trading };
        public static Image[] explore = new Image[] { Properties.Resources.rank_1, Properties.Resources.rank_2, Properties.Resources.rank_3, Properties.Resources.rank_4, Properties.Resources.rank_5, Properties.Resources.rank_6, Properties.Resources.rank_7, Properties.Resources.rank_8, Properties.Resources.rank_9 };
        public static Image[] cqc = new Image[] { Properties.Resources.rank_1_cqc, Properties.Resources.rank_2_cqc, Properties.Resources.rank_3_cqc, Properties.Resources.rank_4_cqc, Properties.Resources.rank_5_cqc, Properties.Resources.rank_6_cqc, Properties.Resources.rank_7_cqc, Properties.Resources.rank_8_cqc, Properties.Resources.rank_9_cqc };


        public static Dictionary<string, string[]> rankNames = new Dictionary<string, string[]>()
        {
            //                              1                   2                   3               4                       5                   6               7                   8               9
            { "combat", new string[] {  "Harmless",     "Mostly Harmless",      "Novice",       "Competent",            "Expert",           "Master",       "Dangerous",        "Deadly",       "Elite" } },
            { "trade", new string[] {   "Penniless",    "Mostly Penniless",     "Peddler",      "Dealer",               "Merchant",         "Broker",       "Entrepreneur",     "Tycoon",       "Elite" } },
            { "explore", new string[] { "Aimless",      "Mostly Aimless",       "Scout",        "Surveyor",             "Trailblazer",      "Pathfinder",   "Ranger",           "Pioneer",      "Elite" } },
            { "cqc" , new string[] {    "Helpless",     "Mostly Helpless",      "Amateur",      "Semi Professional",    "Professional",     "Champion",     "Hero",             "Legend",       "Elite" } },
            { "federation", new string[] { "None", "Recruit", "Cadet", "Midshipman", "Petty Officer", "Chief Petty Officer", "Warrant Officer", "Ensign", "Lieutenant", "Lieutenant Commander", "Post Commander", "Post Captain", "Rear Admiral", "Vice Admiral", "Admiral" } },
            { "empire", new string[] { "None", "Outsider", "Serf", "Master", "Squire", "Knight", "Lord", "Baron", "Viscount", "Count", "Earl", "Marquis", "Duke", "Prince", "King" } }

    };


        // TODO: Make this fix more legit (these were moved into the rankNames dictionary)
        public static string[] federation = new string[] { "None", "Recruit", "Cadet", "Midshipman", "Petty Officer", "Chief Petty Officer", "Warrant Officer", "Ensign", "Lieutenant", "Lieutenant Commander", "Post Commander", "Post Captain", "Rear Admiral", "Vice Admiral", "Admiral" };
        public static string[] empire = new string[] { "None", "Outsider", "Serf", "Master", "Squire", "Knight", "Lord", "Baron", "Viscount", "Count", "Earl", "Marquis", "Duke", "Prince", "King" };

        public static long[] promotionValuesExploration = new long[] { 0, 20000, 135000, 570000, 2070000, 5070000, 17500000, 58000000, 156500000 };
        public static long[] promotionValuesTrading = new long[] { 0, 5000, 100000, 800000, 3700000, 30000000, 133510138, 381000000, 1011828840 };

        public static long[] calculateRankCredits(RankType type, int currentRank, int currentRankPercentage)
        {
            long[] ret = new long[3];
            long[] dataArray = (type == RankType.EXPLORATION ? promotionValuesExploration : promotionValuesTrading);

            double rankProgression = (currentRankPercentage / 100d);
            long creditsAtNextRank = dataArray[currentRank < 8 ? currentRank + 1 : currentRank];
            long baseCreditsForCurrentRank = dataArray[currentRank];

            long creditsDifferenceBetweenRanks = creditsAtNextRank - baseCreditsForCurrentRank;

            long creditsPastCurrentRank = (long)Math.Floor((double)creditsDifferenceBetweenRanks * rankProgression);

            //long creditsToNextRank = creditsAtNextRank - creditsPastCurrentRank;
            long creditsToNextRank = creditsDifferenceBetweenRanks - creditsPastCurrentRank;
            long creditsFrom0Rank = (creditsDifferenceBetweenRanks - creditsToNextRank) + dataArray[currentRank];
            if (currentRank == 8)
            {
                creditsFrom0Rank = dataArray[currentRank];
            }
            else {
                for (int x = 0; x < currentRank; x++)
                {
                    creditsFrom0Rank += dataArray[x];
                }
            }

            return new long[] { creditsPastCurrentRank, creditsToNextRank, creditsFrom0Rank };

        }

        public enum RankType
        {
            TRADE,
            EXPLORATION
        }

    }

    [Serializable]
    internal class UnsuitableRankException : Exception
    {
        public UnsuitableRankException()
        {
        }

        public UnsuitableRankException(string message) : base(message)
        {
        }

        public UnsuitableRankException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsuitableRankException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
