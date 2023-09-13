using System;
using System.Drawing;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义三次贝塞尔一般行为
	/// </summary>
	public interface ISVGPathSegCubic:ISVGPathSeg
	{
		/// <summary>
		/// 获取第一个控制点
		/// </summary>
		/// <param name="svgPathSegList">路径单元所在的单元列表</param>
		PointF GetFirstControl(ISVGPathSegList svgPathSegList);

		/// <summary>
		/// 获取第二个控制点
		/// </summary>
		PointF GetSecondControl(ISVGPathSegList svgPathSegList);
	}
}
