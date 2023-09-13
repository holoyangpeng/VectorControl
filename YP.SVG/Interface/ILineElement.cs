using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YP.SVG.Interface
{
    /// <summary>
    /// 定义线条对象
    /// </summary>
    public interface ILineElement
    {
        /// <summary>
        /// 线条的总长度
        /// </summary>
        float Distance { get; }

        /// <summary>
        /// 求在指定长度的线段两端端点
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        PointF[] GetAnchorsWithDistance(float distance);
    }
}
