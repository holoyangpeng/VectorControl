using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令m
	/// </summary>
	public interface ISVGPathSegMovetoRel:ISVGPathSeg
	{
		/// <summary>
		/// 定义目标点的相对横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 定义目标点的相对纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
