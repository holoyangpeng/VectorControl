using System;
using System.Drawing;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义二次贝塞尔曲线
	/// </summary>
	public interface ISVGPathSegQuadratic:Paths.ISVGPathSegCubic
	{
		/// <summary>
		/// 获取二次曲线控制点
		/// </summary>
		PointF GetQuadraticControl(ISVGPathSegList svgPathList);
	}
}
