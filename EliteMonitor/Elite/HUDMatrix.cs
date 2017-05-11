using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EliteMonitor.Elite
{
    public class HUDMatrix
    {

        public static float CONTRAST = .2f;

        public int RR { get; set; } = 100;
        public int RG { get; set; }
        public int RB { get; set; }

        public int GR { get; set; }
        public int GG { get; set; } = 100;
        public int GB { get; set; }

        public int BR { get; set; }
        public int BG { get; set; }
        public int BB { get; set; } = 100;

        public HUDMatrix() { }
        public HUDMatrix(float rr, float gr, float br, float rg, float gg, float bg, float rb, float gb, float bb)
        {
            this.RR = (int)Math.Round(rr * 100.0);
            this.RG = (int)Math.Round(rg * 100.0);
            this.RB = (int)Math.Round(rb * 100.0);

            this.GR = (int)Math.Round(gr * 100.0);
            this.GB = (int)Math.Round(gb * 100.0);
            this.GG = (int)Math.Round(gg * 100.0);

            this.BR = (int)Math.Round(br * 100.0);
            this.BG = (int)Math.Round(bg * 100.0);
            this.BB = (int)Math.Round(bb * 100.0);
        }

        public HUDMatrix(int rr, int gr, int br, int rg, int gg, int bg, int rb, int gb, int bb)
        {
            this.RR = rr;
            this.RG = rg;
            this.RB = rb;

            this.GR = gr;
            this.GB = gb;
            this.GG = gg;

            this.BR = br;
            this.BG = bg;
            this.BB = bb;
        }

        public HUDMatrix(HUDMatrix hm) : this(hm.RR, hm.GR, hm.BR, hm.RG, hm.GG, hm.BG, hm.RB, hm.GB, hm.BB) { }

        public string getXMLMatrix()
        {
            return String.Join("\n", new string[]
            {
            string.Format("<MatrixRed> {0}, {1}, {2} </MatrixRed>", (float)(this.RR / 100.0), (float)(this.GR / 100.0), (float)(this.BR / 100.0)),
            string.Format("<MatrixGreen> {0}, {1}, {2} </MatrixGreen>", (float)(this.RG / 100.0), (float)(this.GG / 100.0), (float)(this.BG / 100.0)),
            string.Format("<MatrixBlue> {0}, {1}, {2} </MatrixBlue>", (float)(this.RB / 100.0), (float)(this.GB / 100.0), (float)(this.BB / 100.0))
            });
        }

        internal float[][] asFloats(bool applyCurve = false)
        {
            float[][] f = new float[][]
            {   
                //          RR, GR, BR
                new float[] { (float)(this.RR / 100.0), (float)(this.GR / 100.0), (float)(this.BR / 100.0), 0, 0 }, // R
                //          RG, GG, BG
                new float[] { (float)(this.RG / 100.0), (float)(this.GG / 100.0), (float)(this.BG / 100.0), 0, 0 }, // G
                //          RB, BG, BB
                new float[] { (float)(this.RB / 100.0), (float)(this.GB / 100.0), (float)(this.BB / 100.0), 0, 0 }, // B

                /*//          RR, GR, BR
                new float[] { (float)(this.RR / 100.0), (float)(this.RG / 100.0), (float)(this.RB / 100.0), 0, 0 }, // R
                //          RG, GG, BG
                new float[] { (float)(this.GR / 100.0), (float)(this.GG / 100.0), (float)(this.GB / 100.0), 0, 0 }, // G
                //          RB, BG, BB
                new float[] { (float)(this.BR / 100.0), (float)(this.BG / 100.0), (float)(this.BB / 100.0), 0, 0 }, // B*/

                new float[] { 0, 0, 0, 1, 0 }, // ??
                new float[] { 0, 0, 0, 0, 1 } // ??
            };
            if (applyCurve)
            {
                if (f[0][0] < 1f)
                    f[0][0] += (float)(1.00 * f[0][0]);
                if (f[0][1] < 1f)
                    f[0][1] += (float)(1.00 * f[0][1]);
                if (f[0][2] < 1f)
                    f[0][2] += (float)(1.00 * f[0][2]);
            }
            return f;
        }

        public ColorMatrix ColourMatrix() => ColorMatrix();
        public ColorMatrix ColorMatrix(bool applyCurve = false)
        {
            return new ColorMatrix(this.asFloats(applyCurve));
        }

        internal void setFromFloats(float[][] matrix)
        {
            this.RR = (int)Math.Round(matrix[0][0] * 100.0);
            this.RG = (int)Math.Round(matrix[1][0] * 100.0);
            this.RB = (int)Math.Round(matrix[2][0] * 100.0);

            this.GR = (int)Math.Round(matrix[0][1] * 100.0);
            this.GG = (int)Math.Round(matrix[1][1] * 100.0);
            this.GB = (int)Math.Round(matrix[2][1] * 100.0);

            this.BR = (int)Math.Round(matrix[0][2] * 100.0);
            this.BG = (int)Math.Round(matrix[1][2] * 100.0);
            this.BB = (int)Math.Round(matrix[2][2] * 100.0);
        }

        public Ranges getHighestRange()
        {
            Ranges range = Ranges.M0P100;
            foreach (float[] f2 in asFloats())
            {
                foreach (float f in f2)
                {
                    if (f < 0 && range < Ranges.M100P100)
                        range = Ranges.M100P100;
                    else if ((f < -1 || f > 1) && range < Ranges.M200P200)
                        return Ranges.M200P200; // this is the higest range we can have, so if we hit this, just return.
                }
            }

            return range;
        }

        public void writeToXMLFile()
        {
            try
            {
                string xmlPath = EliteUtils.GRAPHICS_OVERRIDE_PATH;

                File.Copy(xmlPath, xmlPath.Replace(".xml", String.Format("-{0}.xml", DateTime.Now.ToString("yyyyMMdd-HHmmss"))));

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineOnAttributes = true
                };

                using (XmlWriter xml = XmlWriter.Create(xmlPath, xmlWriterSettings))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("GraphicsConfig");

                    xml.WriteStartElement("GUIColour");

                    xml.WriteStartElement("Default");
                    //          0:0 0:1 0:2
                    //          RR, GR, BR
                    //          1:0 1:1 1:2
                    //          RG, GG, BG
                    //          2:0 2:1 2:2
                    //          RB, BG, BB

                    xml.WriteElementString("LocalisationName", "Standard");
                    float[][] floats = asFloats();

                    xml.WriteElementString("MatrixRed", String.Format(" {0}, {1}, {2} ", floats[0][0], floats[0][1], floats[0][2]));
                    xml.WriteElementString("MatrixGreen", String.Format(" {0}, {1}, {2} ", floats[1][0], floats[1][1], floats[1][2]));
                    xml.WriteElementString("MatrixBlue", String.Format(" {0}, {1}, {2} ", floats[2][0], floats[2][1], floats[2][2]));

                     xml.WriteEndElement();
                    xml.WriteEndElement();
                    xml.WriteEndElement();
                    xml.WriteEndDocument();

                }
                MessageBox.Show("HUD matrix was applied successfully!", "Complete", MessageBoxButtons.OK);
            }
            catch (Exception e) { MessageBox.Show($"Couldn't apply HUD matrix!\n\n{e.Message}", "Failure", MessageBoxButtons.OK); }

        }

        public override string ToString()
        {
            return $"{this.RR} {this.RG} {this.RB} / {this.GR} {this.GG} {this.GB} / {this.BR} {this.BG} {this.BB}";
        }
    }
}
