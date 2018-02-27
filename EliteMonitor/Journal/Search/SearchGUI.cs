using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Journal.Search
{
    public partial class SearchGUI : Form
    {

        public static SearchGUI Instance;
        public EventHandler<FormClosingEventArgs> OnSearchGUIClosing;
        private bool searchHasFocus = false;

        public SearchGUI()
        {
            InitializeComponent();
            if (Properties.Settings.Default.darkModeEnabled)
                Utils.toggleNightModeForForm(this);
            foreach (DataGridViewColumn column in this.searchResults.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            Instance = this;
        }

        private void SearchGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.OnSearchGUIClosing?.Invoke(this, e);
            this.Dispose();
        }

        private void SearchGUI_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                this.performSearch();
            }
        }

        private void performSearch()
        {

            SearchCriteria sc = new SearchCriteria(this.textBoxSearch.Text);
#if DEBUG
            MessageBox.Show(string.Format("Events: [{0}]\nSearch text: {1}", string.Join(", ", sc.Events), sc.SearchText));
#endif

            List<JournalEntry> matches = MainForm.Instance.journalParser.viewedCommander.JournalEntries.FindAll(a => sc.EntryMatches(a));
            if (matches.Count == 0) { MessageBox.Show("There were no results found for your query. Try being less specific or different keywords.", "No results"); return; }
            List<DataGridViewRow> items = new List<DataGridViewRow>();

            foreach (JournalEntry j in matches)
            {
                DataGridViewRow r = MainForm.Instance.journalParser.getListViewEntryForEntry(j);
                //r.Tag = j.ID;
                items.Add(r);
            }
            this.searchResults.BeginUpdate();
            //items.OrderByDescending(a => Convert.ToInt64(a.Tag));
            items.Reverse();
            this.searchResults.Rows.Clear();
            this.searchResults.Rows.AddRange(items.ToArray());
            this.Text = string.Format("Journal Search - {0:n0} results", matches.Count);
            this.searchResults.EndUpdate();
            items.Clear();
            matches.Clear();

        }

        private void searchResults_DoubleClick(object sender, EventArgs e)
        {
            SelectRowMatchingResult();
        }

        public void SelectRowMatchingResult()
        {
            long journalID = Convert.ToInt64(this.searchResults.SelectedRows[0].Tag);
            Utils.EnsureFormIsVisible(MainForm.Instance);
            MainForm m = MainForm.Instance;
            int rowIndex = -1;
            foreach (DataGridViewRow r in m.eventList.Rows)
            {
                if (r.Tag.Equals(this.searchResults.SelectedRows[0].Tag))
                {
                    rowIndex = r.Index;
                    break;
                }
            }
            if (rowIndex == -1) { MessageBox.Show("Something went wrong while attempting to find the row in the main interface (row index was not found), report this as a bug!", "Oops!"); return; }
            m.eventList.InvokeIfRequired(() =>
            {
                m.eventList.Rows[rowIndex].Selected = true;
                m.eventList.FirstDisplayedScrollingRowIndex = rowIndex;
            });
        }

        private void showRowInMainJournalWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectRowMatchingResult();
        }

        private void searchResults_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = this.searchResults.HitTest(e.Location.X, e.Location.Y).RowIndex;
                if (rowIndex == -1) return;
                this.searchResults.Rows[rowIndex].Selected = true;
                contextMenuStrip1.Show(this.searchResults, e.Location);
            }
        }

        private void SearchGUI_Shown(object sender, EventArgs e)
        {
            this.textBoxSearch.Focus();
        }

        private void textBoxSearch_Click(object sender, EventArgs e)
        {
            if (!this.searchHasFocus)
            {
                this.searchHasFocus = true;
                this.textBoxSearch.SelectAll();
            }
        }

        private void textBoxSearch_Leave(object sender, EventArgs e)
        {
            this.searchHasFocus = false;
        }
    }
}
