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
    public partial class MaterialList : Form
    {

        public static MaterialList Instance;
        public EventHandler<FormClosingEventArgs> OnMaterialsListClosing;

        ListView[] listViews;

        public MaterialList()
        {
            InitializeComponent();
            Instance = this;
            this.listViews = new ListView[] { this.listViewData, this.listViewElements, this.listViewManufactured, this.listViewUnknown };
            foreach (ListView v in this.listViews)
                v.Sorting = SortOrder.Ascending;
            this.DisplayMaterials();
        }

        public void DisplayMaterials(Dictionary<string, int> materials = null)
        {
            if (materials == null)
                materials = new Dictionary<string, int>(MainForm.Instance.journalParser.viewedCommander.Materials);
            foreach (ListView lv in this.listViews)
            {
                lv.BeginUpdate();
                lv.Items.Clear();
            }
            foreach (KeyValuePair<string, int> kvp in materials)
            {
                string realName = string.Empty;
                bool hasRealName = EliteDatabase.Instance.tryGetMaterialNameFromInternal(kvp.Key, out realName);
                if (!hasRealName)
                {
                    this.listViewUnknown.Items.Add(new ListViewItem(new string[] { kvp.Key, kvp.Value.ToString() }));
                    continue;
                }
                string materialType = EliteDatabase.Instance.getMaterialTypeFromInternalName(kvp.Key);
                switch (materialType)
                {
                    case "Encoded":
                        this.listViewData.Items.Add(new ListViewItem(new string[] { realName, kvp.Value.ToString() }));
                        continue;
                    case "Manufactured":
                        this.listViewManufactured.Items.Add(new ListViewItem(new string[] { realName, kvp.Value.ToString() }));
                        continue;
                    case "Elements":
                    case "Element":
                    case "Raw":
                        this.listViewElements.Items.Add(new ListViewItem(new string[] { realName, kvp.Value.ToString() }));
                        continue;
                    default: // (Gotta) Catch ('em) all
                        this.listViewUnknown.Items.Add(new ListViewItem(new string[] { kvp.Key, kvp.Value.ToString() }));
                        continue;
                }
            }


            foreach (ListView lv in this.listViews)
            {
                foreach (ColumnHeader c in lv.Columns)
                    c.Width = -2;
                lv.EndUpdate();
            }
        }

        private void MaterialList_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (ListView v in this.listViews)
                v.Items.Clear();
            this.OnMaterialsListClosing?.Invoke(this, e);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
