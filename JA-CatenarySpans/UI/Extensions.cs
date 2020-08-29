using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JA.Engineering;

namespace JA.UI
{
    public static class Extensions
    {
        public static ToolStripMenuItem ToMenuItem(this ProjectUnits projectUnits, System.Drawing.Image icon,
            EventHandler<ProjectUnits.SetEventArgs> select_units)
        {
            void click(object tsi, EventArgs ev)
            {
                select_units?.Invoke(tsi, new ProjectUnits.SetEventArgs(projectUnits));
            }
            return new ToolStripMenuItem(projectUnits.ToString(), icon, click) { Tag=projectUnits };
        }
    }
}
