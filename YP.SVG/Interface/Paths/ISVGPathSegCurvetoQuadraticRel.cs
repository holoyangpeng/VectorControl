using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令q
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticRel:ISVGPathSeg
	{
		/// <summary>
		/// 定义终点的相对横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 定义终点的相对纵坐标
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// 定义控制点的相对横坐标
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// 定义控制点的相对纵坐标
		/// </summary>
		float Y1{set;get;}
	}
}
