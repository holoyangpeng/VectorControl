using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令C
	/// </summary>
	public interface ISVGPathSegCurvetoCubicAbs:ISVGPathSeg
	{
		/// <summary>
		/// 定义终点的绝对横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 定义终点的绝对纵坐标
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// 定义第一控制点的绝对横坐标
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// 定义第一控制点的绝对纵坐标
		/// </summary>
		float Y1{set;get;}

		/// <summary>
		/// 定义第二控制点的绝对横坐标
		/// </summary>
		float X2{set;get;}

		/// <summary>
		/// 定义第二控制点的绝对纵坐标
		/// </summary>
		float Y2{set;get;}
	}
}
