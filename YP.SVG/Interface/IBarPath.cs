using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Interface
{
    public interface IOutlookBarPath
    {
        System.Drawing.Drawing2D.GraphicsPath GPath { get; }

        string ID { get; set; }

        string Title { get; }
    }
}
