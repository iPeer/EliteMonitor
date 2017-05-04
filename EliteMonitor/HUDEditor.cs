using EliteMonitor.Elite;
using EliteMonitor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EliteMonitor
{

    public enum Ranges
    {
        M0P100, // 0-100
        M100P100, // -100-100
        M200P200 // -200-200
    }


    public partial class HUDEditor : Form
    {
        private bool dontProcessMatrixText = false;
        private bool updateInProgress = false;
        /*private float[][] hudMatrix =
        {   
            //          RR, GR, BR
            new float[] { 1, 0, 0, 0, 0 }, // R
            //          RG, GG, BG
            new float[] { 0, 1, 0, 0, 0 }, // G
            //          BR, BG, BB
            new float[] { 0, 0, 1, 0, 0 }, // B
            new float[] { 0, 0, 0, 1, 0 }, // ??
            new float[] { 0, 0, 0, 0, 1 } // ??
        };*/

        private HUDMatrix hudMatrix = new HUDMatrix();

        private int maxVal = 100;
        private int minVal = 0;

        private Ranges currentRange = Ranges.M0P100;

        private Image hud = Properties.Resources.hud_flight;
        private Image currentHUD;

        private Dictionary<string, HUDMatrix> savedHUDs;
        private Dictionary<string, Image> hudImages = new Dictionary<string, Image>()
        {
            {"Flight", Properties.Resources.hud_flight },
            {"Left panel", Properties.Resources.hud_panelleft },
            {"Right panel", Properties.Resources.hud_panelright },
            {"Station", Properties.Resources.hud_station },
            {"Crotchcam™", Properties.Resources.hud_crotch },
            {"Commodities market", Properties.Resources.hud_commodities }
        };
        private int stockHUDCount = 0;

        public HUDEditor()
        {
            InitializeComponent();
            loadSavedHUDs();
#if !DEBUG
            this.debugLabel.Visible = false;
#endif
            /*this.pictureBox1.Image = applyMatrixToHud(hudMatrix);*/

            // Populate the HUD dropdown

            foreach (string s in this.hudImages.Keys)
            {
                this.comboBoxHUDImage.Items.Add(s);
            }
            this.comboBoxHUDImage.SelectedIndex = this.comboBoxHUDImage.Items.IndexOf("Flight");

            /*applyMatrixToHud(hudMatrix);
            updateSlidersAndTextBoxes();
            updateMatrixTextBox();*/

        }

        private void loadSavedHUDs()
        {
            this.savedHUDs = new Dictionary<string, HUDMatrix>();
            this.savedHUDs.Add("Default", new HUDMatrix(100, 0, 0, 0, 100, 0, 0, 0, 100));
            this.savedHUDs.Add("Default (reversed)", new HUDMatrix(0, 0, 100, 0, 100, 0, 100, 0, 0));
            this.savedHUDs.Add("iPeer", new HUDMatrix(30, 30, 42, 40, 40, 40, 100, -10, 100));
            this.savedHUDs.Add("Kofeyh", new HUDMatrix(30, 30, 40, 20, 60, 10, 90, 15, 15));

            this.stockHUDCount = this.savedHUDs.Count;

            // Load the user's current config into memory (if it exists) so we can add it to the "saved" HUD list

            if (File.Exists(EliteUtils.GRAPHICS_OVERRIDE_PATH))
            {
                XDocument xml = XDocument.Load(EliteUtils.GRAPHICS_OVERRIDE_PATH); // XML is such a pain in the ass to work with.
                try
                {
                    string redMatrix = xml.Element("GraphicsConfig").Element("GUIColour").Element("Default").Element("MatrixRed").Value;
                    string greenMatrix = xml.Element("GraphicsConfig").Element("GUIColour").Element("Default").Element("MatrixGreen").Value;
                    string blueMatrix = xml.Element("GraphicsConfig").Element("GUIColour").Element("Default").Element("MatrixBlue").Value;

                    int rr, rg, rb;
                    int gr, gg, gb;
                    int br, bg, bb;

                    string[] data = redMatrix.Trim().Split(',');

                    rr = (int)Math.Round(Convert.ToSingle(data[0].Trim()) * 100);
                    rg = (int)Math.Round(Convert.ToSingle(data[1].Trim()) * 100);
                    rb = (int)Math.Round(Convert.ToSingle(data[2].Trim()) * 100);

                    data = greenMatrix.Trim().Split(',');

                    gr = (int)Math.Round(Convert.ToSingle(data[0].Trim()) * 100);
                    gg = (int)Math.Round(Convert.ToSingle(data[1].Trim()) * 100);
                    gb = (int)Math.Round(Convert.ToSingle(data[2].Trim()) * 100);

                    data = blueMatrix.Trim().Split(',');

                    br = (int)Math.Round(Convert.ToSingle(data[0].Trim()) * 100);
                    bg = (int)Math.Round(Convert.ToSingle(data[1].Trim()) * 100);
                    bb = (int)Math.Round(Convert.ToSingle(data[2].Trim()) * 100);

                    HUDMatrix hud = new HUDMatrix(rr, rg, rb, gr, gg, gb, br, bg, bb);
                    this.savedHUDs.Add("Current", hud);
                }
                catch {  }
            }

            // Load any custom presets the user might have
            string[] files = Directory.GetFiles(MainForm.Instance.cacheController.hudPresetPath);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                string presetName = fi.Name.Replace(".hud", "");
                HUDMatrix matrix = JsonConvert.DeserializeObject<HUDMatrix>(File.ReadAllText(fi.FullName));
                this.savedHUDs.Add(presetName, matrix);
            }

            this.comboBoxLoadPreset.Items.Clear();
            foreach (KeyValuePair<string, HUDMatrix> kvp in this.savedHUDs)
                this.comboBoxLoadPreset.Items.Add(kvp.Key);
            this.comboBoxLoadPreset.SelectedIndex = this.comboBoxLoadPreset.Items.IndexOf(this.savedHUDs.ContainsKey("Current") ? "Current" : "Default");
        }

        private void switchRange(Ranges range, bool updateSliders = true) // Holy cow.
        {
            if (range == Ranges.M0P100)
            {
                if (this.trackBarRR.Value < 0)
                    this.trackBarRR.Value = 0;
                if (this.trackBarRR.Value > 100)
                    this.trackBarRR.Value = 100;
                this.trackBarRR.Minimum = 0;
                this.trackBarRR.Maximum = 100;

                if (this.trackBarRG.Value < 0)
                    this.trackBarRG.Value = 0;
                if (this.trackBarRG.Value > 100)
                    this.trackBarRG.Value = 100;
                this.trackBarRG.Minimum = 0;
                this.trackBarRG.Maximum = 100;

                if (this.trackBarRB.Value < 0)
                    this.trackBarRB.Value = 0;
                if (this.trackBarRB.Value > 100)
                    this.trackBarRB.Value = 100;
                this.trackBarRB.Minimum = 0;
                this.trackBarRB.Maximum = 100;

                if (this.trackBarGR.Value < 0)
                    this.trackBarGR.Value = 0;
                if (this.trackBarGR.Value > 100)
                    this.trackBarGR.Value = 100;
                this.trackBarGR.Minimum = 0;
                this.trackBarGR.Maximum = 100;

                if (this.trackBarGG.Value < 0)
                    this.trackBarGG.Value = 0;
                if (this.trackBarGG.Value > 100)
                    this.trackBarGG.Value = 100;
                this.trackBarGG.Minimum = 0;
                this.trackBarGG.Maximum = 100;

                if (this.trackBarGB.Value < 0)
                    this.trackBarGB.Value = 0;
                if (this.trackBarGB.Value > 100)
                    this.trackBarGB.Value = 100;
                this.trackBarGB.Minimum = 0;
                this.trackBarGB.Maximum = 100;

                if (this.trackBarBR.Value < 0)
                    this.trackBarBR.Value = 0;
                if (this.trackBarBR.Value > 100)
                    this.trackBarBR.Value = 100;
                this.trackBarBR.Minimum = 0;
                this.trackBarBR.Maximum = 100;

                if (this.trackBarBG.Value < 0)
                    this.trackBarBG.Value = 0;
                if (this.trackBarBG.Value > 100)
                    this.trackBarBG.Value = 100;
                this.trackBarBG.Minimum = 0;
                this.trackBarBG.Maximum = 100;

                if (this.trackBarBB.Value < 0)
                    this.trackBarBB.Value = 0;
                if (this.trackBarBB.Value > 100)
                    this.trackBarBB.Value = 100;
                this.trackBarBB.Minimum = 0;
                this.trackBarBB.Maximum = 100;

                this.maxVal = 100;
                this.minVal = 0;
            }
            else if (range == Ranges.M100P100)
            {
                if (this.trackBarRR.Value < -100)
                    this.trackBarRR.Value = -100;
                if (this.trackBarRR.Value > 100)
                    this.trackBarRR.Value = 100;
                this.trackBarRR.Minimum = -100;
                this.trackBarRR.Maximum = 100;

                if (this.trackBarRG.Value < -100)
                    this.trackBarRG.Value = -100;
                if (this.trackBarRG.Value > 100)
                    this.trackBarRG.Value = 100;
                this.trackBarRG.Minimum = -100;
                this.trackBarRG.Maximum = 100;

                if (this.trackBarRB.Value < -100)
                    this.trackBarRB.Value = -100;
                if (this.trackBarRB.Value > 100)
                    this.trackBarRB.Value = 100;
                this.trackBarRB.Minimum = -100;
                this.trackBarRB.Maximum = 100;

                if (this.trackBarGR.Value < -100)
                    this.trackBarGR.Value = -100;
                if (this.trackBarGR.Value > 100)
                    this.trackBarGR.Value = 100;
                this.trackBarGR.Minimum = -100;
                this.trackBarGR.Maximum = 100;

                if (this.trackBarGG.Value < -100)
                    this.trackBarGG.Value = -100;
                if (this.trackBarGG.Value > 100)
                    this.trackBarGG.Value = 100;
                this.trackBarGG.Minimum = -100;
                this.trackBarGG.Maximum = 100;

                if (this.trackBarGB.Value < -100)
                    this.trackBarGB.Value = -100;
                if (this.trackBarGB.Value > 100)
                    this.trackBarGB.Value = 100;
                this.trackBarGB.Minimum = -100;
                this.trackBarGB.Maximum = 100;

                if (this.trackBarBR.Value < -100)
                    this.trackBarBR.Value = -100;
                if (this.trackBarBR.Value > 100)
                    this.trackBarBR.Value = 100;
                this.trackBarBR.Minimum = -100;
                this.trackBarBR.Maximum = 100;

                if (this.trackBarBG.Value < -100)
                    this.trackBarBG.Value = -100;
                if (this.trackBarBG.Value > 100)
                    this.trackBarBG.Value = 100;
                this.trackBarBG.Minimum = -100;
                this.trackBarBG.Maximum = 100;

                if (this.trackBarBB.Value < -100)
                    this.trackBarBB.Value = -100;
                if (this.trackBarBB.Value > 100)
                    this.trackBarBB.Value = 100;
                this.trackBarBB.Minimum = -100;
                this.trackBarBB.Maximum = 100;

                this.maxVal = 100;
                this.minVal = -100;
            }
            else if (range == Ranges.M200P200)
            {
                if (this.trackBarRR.Value < -200)
                    this.trackBarRR.Value = -200;
                if (this.trackBarRR.Value > 200)
                    this.trackBarRR.Value = 200;
                this.trackBarRR.Minimum = -200;
                this.trackBarRR.Maximum = 200;

                if (this.trackBarRG.Value < -200)
                    this.trackBarRG.Value = -200;
                if (this.trackBarRG.Value > 200)
                    this.trackBarRG.Value = 200;
                this.trackBarRG.Minimum = -200;
                this.trackBarRG.Maximum = 200;

                if (this.trackBarRB.Value < -200)
                    this.trackBarRB.Value = -200;
                if (this.trackBarRB.Value > 200)
                    this.trackBarRB.Value = 200;
                this.trackBarRB.Minimum = -200;
                this.trackBarRB.Maximum = 200;

                if (this.trackBarGR.Value < -200)
                    this.trackBarGR.Value = -200;
                if (this.trackBarGR.Value > 200)
                    this.trackBarGR.Value = 200;
                this.trackBarGR.Minimum = -200;
                this.trackBarGR.Maximum = 200;

                if (this.trackBarGG.Value < -200)
                    this.trackBarGG.Value = -200;
                if (this.trackBarGG.Value > 200)
                    this.trackBarGG.Value = 200;
                this.trackBarGG.Minimum = -200;
                this.trackBarGG.Maximum = 200;

                if (this.trackBarGB.Value < -200)
                    this.trackBarGB.Value = -200;
                if (this.trackBarGB.Value > 200)
                    this.trackBarGB.Value = 200;
                this.trackBarGB.Minimum = -200;
                this.trackBarGB.Maximum = 200;

                if (this.trackBarBR.Value < -200)
                    this.trackBarBR.Value = -200;
                if (this.trackBarBR.Value > 200)
                    this.trackBarBR.Value = 200;
                this.trackBarBR.Minimum = -200;
                this.trackBarBR.Maximum = 200;

                if (this.trackBarBG.Value < -200)
                    this.trackBarBG.Value = -200;
                if (this.trackBarBG.Value > 200)
                    this.trackBarBG.Value = 200;
                this.trackBarBG.Minimum = -200;
                this.trackBarBG.Maximum = 200;

                if (this.trackBarBB.Value < -200)
                    this.trackBarBB.Value = -200;
                if (this.trackBarBB.Value > 200)
                    this.trackBarBB.Value = 200;
                this.trackBarBB.Minimum = -200;
                this.trackBarBB.Maximum = 200;

                this.maxVal = 200;
                this.minVal = -200;
            }

            this.currentRange = range;
            if (updateSliders)
                updateSlidersAndTextBoxes();
            updateRangeCheckboxes();

            //updateMatrixTextBox();
        }

        private void updateSlidersAndTextBoxes()
        {

            // RR

            int val = hudMatrix.RR;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRR.Value = val;
            this.colourRR.Text = val.ToString();

            // RG

            val = hudMatrix.RG;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRG.Value = val;
            this.colourRG.Text = val.ToString();

            // RB

            val = hudMatrix.RB;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRB.Value = val;
            this.colourRB.Text = val.ToString();

            // GR

            val = hudMatrix.GR;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGR.Value = val;
            this.colourGR.Text = val.ToString();

            // GG

            val = hudMatrix.GG;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGG.Value = val;
            this.colourGG.Text = val.ToString();

            // GB

            val = hudMatrix.GB;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGB.Value = val;
            this.colourGB.Text = val.ToString();

            // BR

            val = hudMatrix.BR;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBR.Value = val;
            this.colourBR.Text = val.ToString();

            // BG

            val = hudMatrix.BG;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBG.Value = val;
            this.colourBG.Text = val.ToString();

            // BB

            val = hudMatrix.BB;
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBB.Value = val;
            this.colourBB.Text = val.ToString();

        }

        private void applyMatrixToHud()
        {
            applyMatrixToHud(this.hudMatrix);
        }

        private void applyMatrixToHud(HUDMatrix matrix)
        {
            if (this.updateInProgress)
                return;
            this.updateInProgress = true;
            bool station = this.comboBoxHUDImage.SelectedIndex != this.comboBoxHUDImage.Items.IndexOf("Station");
#if DEBUG
            StringBuilder sb = new StringBuilder();
            int f1 = 0;
            float[][] matrixFloats = matrix.asFloats();
            foreach (float[] f2 in matrixFloats)
            {
                sb.AppendFormat("{0}: ", f1++, f2.ToString());
                string floats = "[ ";
                foreach (float f in f2)
                {

                    floats += (floats.Length > 2 ? ", " : "") + f.ToString();

                }
                floats += " ]";
                sb.AppendFormat("{0}\n", floats);
            }
            sb.AppendFormat("Curve: {0}\n", station ? "active" : "inactive");
            matrixFloats = matrix.asFloats(station);
            f1 = 0;
            foreach (float[] f2 in matrixFloats)
            {
                sb.AppendFormat("{0}: ", f1++, f2.ToString());
                string floats = "[ ";
                foreach (float f in f2)
                {

                    floats += (floats.Length > 2 ? ", " : "") + f.ToString();

                }
                floats += " ]";
                sb.AppendFormat("{0}\n", floats);
            }

            this.debugLabel.Text = sb.ToString();
#endif

            ImageAttributes at = new ImageAttributes();
            ColorMatrix cm = matrix.ColorMatrix(station);
            at.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Default);
            if (this.currentHUD != null)
                this.currentHUD.Dispose();
            this.currentHUD = new Bitmap(this.hud);
            using (Graphics g = Graphics.FromImage(this.currentHUD))
            {
                g.DrawImage(this.hud, new Rectangle(0, 0, hud.Width, hud.Height), 0, 0, hud.Width, hud.Height, GraphicsUnit.Pixel, at);
                g.Flush();
            }
            this.pictureBox1.Image = this.currentHUD;
            //updateMatrixTextBox();
            this.updateInProgress = false;

        }

        private void trackBarRR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRR.Text = val.ToString();
            this.hudMatrix.RR = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarRG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRG.Text = val.ToString();
            this.hudMatrix.RG = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarRB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRB.Text = val.ToString();
            this.hudMatrix.RB = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGR.Text = val.ToString();
            this.hudMatrix.GR = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGG.Text = val.ToString();
            this.hudMatrix.GG = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGB.Text = val.ToString();
            this.hudMatrix.GB = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBR.Text = val.ToString();
            this.hudMatrix.BR = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBG.Text = val.ToString();
            this.hudMatrix.BG = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBB.Text = val.ToString();
            this.hudMatrix.BB = val;
            applyMatrixToHud(this.hudMatrix);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            applyMatrixToHud(this.hudMatrix);
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            switchRange(Ranges.M0P100);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            switchRange(Ranges.M100P100);
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            switchRange(Ranges.M200P200);
        }

        private void updateMatrixTextBox()
        {
            /*string matrix = createXMLMatrixText();
            this.matrixTextBox.Text = matrix;*/
        }

        private void updateRangeCheckboxes()
        {
            radioButton1.Checked = this.currentRange == Ranges.M0P100;
            radioButton2.Checked = this.currentRange == Ranges.M100P100;
            radioButton3.Checked = this.currentRange == Ranges.M200P200;
        }

        /*[Obsolete("Use HUDMatrix.getXMLMatrix() instead", true)]
        private string createXMLMatrixText()
        {
            string[] lines = new string[3]
            {
                string.Format("<MatrixRed> {0}, {1}, {2} </MatrixRed>", this.hudMatrix[0][0], this.hudMatrix[0][1], this.hudMatrix[0][2]),
                string.Format("<MatrixGreen> {0}, {1}, {2} </MatrixGreen>", this.hudMatrix[1][0], this.hudMatrix[1][1], this.hudMatrix[1][2]),
                string.Format("<MatrixBlue> {0}, {1}, {2} </MatrixBlue>", this.hudMatrix[2][0], this.hudMatrix[2][1], this.hudMatrix[2][2])
            };

            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
                sb.AppendLine(l);
            return sb.ToString();

        }*/

        private void matrixTextBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: Idiot proof

            if (this.updateInProgress || this.dontProcessMatrixText)
                return;
            this.updateInProgress = true;
            if (this.matrixTextBox.Lines.Length == 1)
                this.matrixTextBox.Text = this.matrixTextBox.Text.Replace("</MatrixRed>", "</MatrixRed>\n").Replace("</MatrixGreen>", "</MatrixGreen>\n").Replace("</MatrixBlue>", "</MatrixBlue>\n");
            try
            {
                int x = 0;
                Ranges highestRange = Ranges.M0P100;
                float[][] matrix = this.hudMatrix.asFloats();
                foreach (String s in this.matrixTextBox.Lines)
                {
                    int start = s.IndexOf('>') + 1;
                    int end = s.LastIndexOf('<') - start;
                    string pars = s.Substring(start, end);
                    int y = 0;
                    foreach (string m in pars.Split(','))
                    {
                        float f = /*Convert.ToSingle(m);*/float.Parse(m);
                        Console.WriteLine(string.Format("{0} / {1}", m, f));
                        if (f < 0 && highestRange < Ranges.M100P100)
                            highestRange = Ranges.M100P100;
                        else if ((f < -1 || f > 1) && highestRange < Ranges.M200P200)
                            highestRange = Ranges.M200P200;
                        matrix[x][y++] = f;
                    }
                    x++;

                }
                this.hudMatrix.setFromFloats(matrix);
                switchRange(highestRange);
                updateSlidersAndTextBoxes();
                this.updateInProgress = false;
                applyMatrixToHud();
                this.dontProcessMatrixText = true;
                this.matrixTextBox.Text = this.hudMatrix.getXMLMatrix();
                this.dontProcessMatrixText = false;
            }
            catch (Exception _e) { Console.WriteLine(_e.Message + "\n" + _e.StackTrace); }
        }

        private void buttonSavePreset_Click(object sender, EventArgs e)
        {
            string presetName = Utils.Prompt("Please enter a name for this preset", "Enter preset name");
            string hudsPath = MainForm.Instance.cacheController.hudPresetPath;
            string hudPath = Path.Combine(hudsPath, String.Format("{0}.hud", presetName));
            bool fileExists = File.Exists(hudPath);
            if (!fileExists || (fileExists && MessageBox.Show($"A preset with the name '{presetName}' already exists, do you want to overwrite it?", "Confirm overwrite", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                using (StreamWriter sw = new StreamWriter(hudPath))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(this.hudMatrix, Formatting.Indented));
                }
                if (this.savedHUDs.ContainsKey(presetName))
                    this.savedHUDs[presetName] = new HUDMatrix(this.hudMatrix);
                else
                    this.savedHUDs.Add(presetName, new HUDMatrix(this.hudMatrix));
                if (!this.comboBoxLoadPreset.Items.Contains(presetName))
                {
                    this.comboBoxLoadPreset.Items.Add(presetName);
                    this.comboBoxLoadPreset.SelectedIndex = this.comboBoxLoadPreset.Items.IndexOf(presetName);
                }
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to apply this HUD matrix?", "Confirm HUD apply", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.hudMatrix.writeToXMLFile();
        }

        private void comboBoxLoadPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            string hmName = ((ComboBox)sender).SelectedItem.ToString();
            if (((ComboBox)sender).SelectedIndex <= this.stockHUDCount)
                this.buttonDeletePreset.Enabled = false;
            else
                this.buttonDeletePreset.Enabled = true;
            HUDMatrix hm = new HUDMatrix(this.savedHUDs[hmName]); // so we don't "pollute" the defaults/saved huds
#if DEBUG
            Console.WriteLine(string.Format("Switching to matrix {0} {1} {2} // {3} {4} {5} // {6} {7} {8}", hm.RR, hm.GR, hm.BR, hm.RG, hm.GG, hm.BG, hm.RB, hm.GB, hm.BB));
#endif
            this.updateInProgress = true;
            switchRange(hm.getHighestRange()); // Build 890: The order of this and the statement below were switched to fix an issue with switching back to the default matrix while having matricies out of range of the default range.
            this.hudMatrix = hm;
            updateSlidersAndTextBoxes();
            this.updateInProgress = false;
            applyMatrixToHud();
        }

        private void comboBoxHUDImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Image toSwitch = this.hudImages[this.comboBoxHUDImage.SelectedItem.ToString()];
            this.hud = toSwitch;
            applyMatrixToHud();
            updateRangeCheckboxes();
            updateRangeCheckboxes();
            updateMatrixTextBox();
        }

        private void buttonDeletePreset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this preset?", "Confirm deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string presetName = this.comboBoxLoadPreset.SelectedItem.ToString();
                string hudsPath = MainForm.Instance.cacheController.hudPresetPath;
                string hudPath = Path.Combine(hudsPath, String.Format("{0}.hud", presetName));
                this.savedHUDs.Remove(presetName);
                this.comboBoxLoadPreset.Items.Remove(presetName);
                File.Delete(hudPath);
                this.comboBoxLoadPreset.SelectedIndex = 0;
            }
        }
    }
}
