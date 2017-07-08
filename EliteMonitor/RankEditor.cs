using EliteMonitor.Elite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    public partial class RankEditor : Form
    {
        public RankEditor()
        {
            InitializeComponent();
        }

        private void RankEditor_Load(object sender, EventArgs e)
        {

            //Load ranks

            foreach (KeyValuePair<string, string[]> kvp in Ranks.rankNames)
            {
                foreach (string r in kvp.Value)
                {
                    if (kvp.Key.Equals("combat"))
                        this.combatRankName.Items.Add(r);
                    else if (kvp.Key.Equals("trade"))
                        this.tradeRankName.Items.Add(r);
                    else if (kvp.Key.Equals("explore"))
                        this.exploreRankName.Items.Add(r);
                    else if (kvp.Key.Equals("cqc"))
                        this.cqcRankName.Items.Add(r);
                    else if (kvp.Key.Equals("federation"))
                        this.fedRankName.Items.Add(r);
                    else if (kvp.Key.Equals("empire"))
                        this.empRankName.Items.Add(r);
                }
            }

            Commander c = MainForm.Instance.journalParser.viewedCommander;

            this.combatRankName.SelectedIndex = c.combatRank;
            this.combatProgress.Value = c.combatProgress;

            this.tradeRankName.SelectedIndex= c.tradeRank;
            this.tradeProgress.Value = c.tradeProgress;

            this.exploreRankName.SelectedIndex = c.explorationRank;
            this.exploreProgress.Value = c.explorationProgress;

            this.cqcRankName.SelectedIndex = c.cqcRank;
            this.cqcProgress.Value = c.cqcProgress;

            this.fedRankName.SelectedIndex = c.federationRank;
            this.fedProgress.Value = c.federationProgress;

            this.empRankName.SelectedIndex = c.imperialRank;
            this.empProgress.Value = c.imperialProgress;

        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            Commander c = MainForm.Instance.journalParser.viewedCommander;
            c.setRanks(this.combatRankName.SelectedIndex, this.tradeRankName.SelectedIndex, this.exploreRankName.SelectedIndex, this.cqcRankName.SelectedIndex, this.fedRankName.SelectedIndex, this.empRankName.SelectedIndex);
            c.setRankProgress((int)this.combatProgress.Value, (int)this.tradeProgress.Value, (int)this.exploreProgress.Value, (int)this.cqcProgress.Value, (int)this.fedProgress.Value, (int)this.empProgress.Value);
            c.updateDialogDisplays();

            this.Close();
            this.Dispose();
        }
    }
}
