using EliteMonitor.Elite;
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
            /*if (Properties.Settings.Default.darkModeEnabled)
                Utils.toggleNightModeForForm(this);*/
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
                Int32 materialGrade = EliteDatabase.Instance.getMaterialGradeFromInternalName(kvp.Key);
                ImageList images = new ImageList();
                images.Images.Add(EliteDatabase.Instance.MATERIAL_GRADE_IMAGES[0]);
                images.Images.Add(EliteDatabase.Instance.MATERIAL_GRADE_IMAGES[1]);
                images.Images.Add(EliteDatabase.Instance.MATERIAL_GRADE_IMAGES[2]);
                images.Images.Add(EliteDatabase.Instance.MATERIAL_GRADE_IMAGES[3]);
                images.Images.Add(EliteDatabase.Instance.MATERIAL_GRADE_IMAGES[4]);
                this.listViewData.SmallImageList = this.listViewElements.SmallImageList = this.listViewManufactured.SmallImageList = images;
                EliteDatabase.Instance.logger.Log("Setting Material icon index to {0:n0}", Logging.LogLevel.DEBUG, materialGrade - 1);
                ListViewItem lvi = new ListViewItem(new string[] { realName, kvp.Value.ToString() });
                lvi.ImageIndex = materialGrade - 1;
                switch (materialType)
                {
                    case "Encoded":
                        this.listViewData.Items.Add(lvi);
                        continue;
                    case "Manufactured":
                        this.listViewManufactured.Items.Add(lvi);
                        continue;
                    case "Elements":
                    case "Element":
                    case "Raw":
                        this.listViewElements.Items.Add(lvi);
                        continue;
                    default: // (Gotta) Catch ('em) all
                        this.listViewUnknown.Items.Add(lvi);
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
