using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令h
	/// </summary>
	public interface ISVGPathSegLinetoHorizontalRel:ISVGPathSeg
	{
		/// <summary>
		/// 定义终点的相对横坐标
		/// </summary>
		float X{set;get;}
	}
}
