using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令s
	/// </summary>
	public interface ISVGPathSegCurvetoCubicSmoothRel:ISVGPathSeg
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
		/// 定义第二控制点的相对横坐标
		/// </summary>
		float X2{set;get;}

		/// <summary>
		/// 定义第二控制点的相对纵坐标
		/// </summary>
		float Y2{set;get;}
	}
}
