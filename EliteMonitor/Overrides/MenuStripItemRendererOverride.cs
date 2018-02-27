using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Overrides
{
    public class MenuStripItemRendererOverride : ToolStripProfessionalRenderer
    {
        public MenuStripItemRendererOverride() : base(new OverrideColours()) { }
    }

    public class OverrideColours : ProfessionalColorTable
    {

        public override Color MenuItemSelected
        {
            get
            {
                return Color.FromArgb(77, 77, 77);
            }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                return Color.FromArgb(77, 77, 77);
            }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                return Color.FromArgb(77, 77, 77);
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                return Color.FromArgb(99, 99, 99);
            }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                return Color.FromArgb(99, 99, 99);
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return Color.FromArgb(99, 99, 99);
            }
        }

    }
}
