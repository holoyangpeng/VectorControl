using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Interface
{
    public interface ISVGTextBlockContainer:ISVGContainer
    {
        SVG.DataType.SVGViewport Viewport { get; }


    }
}
