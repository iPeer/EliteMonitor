using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private float[][] hudMatrix =
        {   
            //          RR, GR, BR
            new float[] { 1, 0, 0, 0, 0 }, // R
            //          RG, GG, BG
            new float[] { 0, 1, 0, 0, 0 }, // G
            //          BR, BG, BB
            new float[] { 0, 0, 1, 0, 0 }, // B
            new float[] { 0,  0,  0,  1F, 0}, // ??
            new float[] { 0, 0, 0, 0, 1F } // ??
        };

        private int maxVal = 100;
        private int minVal = 0;

        private Image hud = Properties.Resources.elitehud;

        public HUDEditor()
        {
            InitializeComponent();
            /*this.pictureBox1.Image = applyMatrixToHud(hudMatrix);*/
            applyMatrixToHud(hudMatrix);
            updateSlidersAndTextBoxes();

        }

        private void switchRange(Ranges range)
        {
            if (range == Ranges.M0P100)
            {
                this.trackBarRR.Minimum = 0;
                this.trackBarRR.Maximum = 100;

                this.trackBarRG.Minimum = 0;
                this.trackBarRG.Maximum = 100;

                this.trackBarRB.Minimum = 0;
                this.trackBarRB.Maximum = 100;

                this.trackBarGR.Minimum = 0;
                this.trackBarGR.Maximum = 100;

                this.trackBarGG.Minimum = 0;
                this.trackBarGG.Maximum = 100;

                this.trackBarGB.Minimum = 0;
                this.trackBarGB.Maximum = 100;

                this.trackBarBR.Minimum = 0;
                this.trackBarBR.Maximum = 100;

                this.trackBarBG.Minimum = 0;
                this.trackBarBG.Maximum = 100;

                this.trackBarBB.Minimum = 0;
                this.trackBarBB.Maximum = 100;

                this.maxVal = 100;
                this.minVal = 0;
            }
            else if (range == Ranges.M100P100)
            {
                this.trackBarRR.Minimum = -100;
                this.trackBarRR.Maximum = 100;

                this.trackBarRG.Minimum = -100;
                this.trackBarRG.Maximum = 100;

                this.trackBarRB.Minimum = -100;
                this.trackBarRB.Maximum = 100;

                this.trackBarGR.Minimum = -100;
                this.trackBarGR.Maximum = 100;

                this.trackBarGG.Minimum = -100;
                this.trackBarGG.Maximum = 100;

                this.trackBarGB.Minimum = -100;
                this.trackBarGB.Maximum = 100;

                this.trackBarBR.Minimum = -100;
                this.trackBarBR.Maximum = 100;

                this.trackBarBG.Minimum = -100;
                this.trackBarBG.Maximum = 100;

                this.trackBarBB.Minimum = -100;
                this.trackBarBB.Maximum = 100;

                this.maxVal = 100;
                this.minVal = -100;
            }
            else if (range == Ranges.M200P200)
            {
                this.trackBarRR.Minimum = -200;
                this.trackBarRR.Maximum = 200;

                this.trackBarRG.Minimum = -200;
                this.trackBarRG.Maximum = 200;

                this.trackBarRB.Minimum = -200;
                this.trackBarRB.Maximum = 200;

                this.trackBarGR.Minimum = -200;
                this.trackBarGR.Maximum = 200;

                this.trackBarGG.Minimum = -200;
                this.trackBarGG.Maximum = 200;

                this.trackBarGB.Minimum = -200;
                this.trackBarGB.Maximum = 200;

                this.trackBarBR.Minimum = -200;
                this.trackBarBR.Maximum = 200;

                this.trackBarBG.Minimum = -200;
                this.trackBarBG.Maximum = 200;

                this.trackBarBB.Minimum = -200;
                this.trackBarBB.Maximum = 200;
                this.maxVal = 200;
                this.minVal = -200;
            }

            updateSlidersAndTextBoxes();
        }

        private void updateSlidersAndTextBoxes()
        {

            // RR

            int val = (int)(hudMatrix[0][0] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRR.Value = val;
            this.colourRR.Text = val.ToString();

            // RG

            val = (int)(hudMatrix[1][0] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRG.Value = val;
            this.colourRG.Text = val.ToString();

            // RB

            val = (int)(hudMatrix[2][0] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarRB.Value = val;
            this.colourRB.Text = val.ToString();

            // GR

            val = (int)(hudMatrix[0][1] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGR.Value = val;
            this.colourGR.Text = val.ToString();

            // GG

            val = (int)(hudMatrix[1][1] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGG.Value = val;
            this.colourGG.Text = val.ToString();

            // GB

            val = (int)(hudMatrix[2][1] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarGB.Value = val;
            this.colourGB.Text = val.ToString();

            // BR

            val = (int)(hudMatrix[0][2] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBR.Value = val;
            this.colourBR.Text = val.ToString();

            // BG

            val = (int)(hudMatrix[1][2] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBG.Value = val;
            this.colourBG.Text = val.ToString();

            // BB

            val = (int)(hudMatrix[2][2] * 100F);
            if (val > this.maxVal)
                val = this.maxVal;
            this.trackBarBB.Value = val;
            this.colourBB.Text = val.ToString();

        }

        private void applyMatrixToHud()
        {
            applyMatrixToHud(this.hudMatrix);
        }

        private void applyMatrixToHud(float[][] matrix)
        {

            // TODO FIXME: There is a pretty bad memory leak here.

            StringBuilder sb = new StringBuilder();
            int f1 = 0;
            foreach (float[] f2 in matrix)
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

            ImageAttributes at = new ImageAttributes();
            ColorMatrix cm = new ColorMatrix(matrix);
            at.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            Bitmap newBitmap = new Bitmap(this.hud);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(this.hud, new Rectangle(0, 0, hud.Width, hud.Height), 0, 0, hud.Width, hud.Height, GraphicsUnit.Pixel, at);
                g.DrawString("PLACEHOLDER IMAGE", new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), Brushes.Red, new Rectangle(0, 0, hud.Width, hud.Height));
                g.Flush();
            }
            this.pictureBox1.Image = newBitmap;

        }

        private void trackBarRR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRR.Text = val.ToString();
            this.hudMatrix[0][0] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarRG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRG.Text = val.ToString();
            this.hudMatrix[1][0] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarRB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourRB.Text = val.ToString();
            this.hudMatrix[2][0] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGR.Text = val.ToString();
            this.hudMatrix[0][1] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGG.Text = val.ToString();
            this.hudMatrix[1][1] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarGB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourGB.Text = val.ToString();
            this.hudMatrix[2][1] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBR_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBR.Text = val.ToString();
            this.hudMatrix[0][2] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBG_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBG.Text = val.ToString();
            this.hudMatrix[1][2] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void trackBarBB_ValueChanged(object sender, EventArgs e)
        {
            int val = ((TrackBar)sender).Value;
            this.colourBB.Text = val.ToString();
            this.hudMatrix[2][2] = (float)((float)val / 100F);
            applyMatrixToHud(this.hudMatrix);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            applyMatrixToHud(this.hudMatrix);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            switchRange(Ranges.M0P100);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            switchRange(Ranges.M100P100);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            switchRange(Ranges.M200P200);
        }
    }
}
