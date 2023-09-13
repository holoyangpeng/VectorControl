using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令T
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticSmoothAbs:ISVGPathSeg
	{
		/// <summary>
		/// 终点绝对横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 终点绝对纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
