using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA.UI
{
    public static class GdiEx
    {
        public static bool IsFinite(this PointF point)
        {
            return !float.IsNaN(point.X)
                && !float.IsNaN(point.Y)
                && !float.IsInfinity(point.X)
                && !float.IsInfinity(point.Y);
        }
    }
}
