using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.DataType
{
    public struct SVGViewport
    {
        #region ..fields
        public DataType.SVGRect Bounds;
        public float RotateAngle;
        #endregion

        #region ..Constructor
        public SVGViewport(DataType.SVGRect bounds, float angle)
        {
            this.RotateAngle = angle;
            this.Bounds = bounds;
        }
        #endregion
    }
}
