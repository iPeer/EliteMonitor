using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Utilities
{
    public interface ISavable
    {
        void OnSave();
        void OnLoad();
    }
}
