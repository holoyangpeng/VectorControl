using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Interface.BasicShapes
{
    public interface ISVGConnectionEelement:
        ISVGElement,
        ISVGTests,
        ISVGLangSpace,
        ISVGExternalResourcesRequired,
        ISVGStylable,
        ISVGTransformable
    {
    }
}
