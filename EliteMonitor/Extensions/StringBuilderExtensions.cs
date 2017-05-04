using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Extensions
{
    public static class StringBuilderExtensions
    {

        public static StringBuilder AppendLineFormatted(this StringBuilder sb, string format, params object[] formats)
        {
            sb.AppendLine(string.Format(format, formats));
            return sb;
        }

    }
}
