using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令H
	/// </summary>
	public interface ISVGPathSegLinetoHorizontalAbs:ISVGPathSeg
	{
		/// <summary>
		/// 终点的绝对横坐标
		/// </summary>
		float X{set;get;}
	}
}
