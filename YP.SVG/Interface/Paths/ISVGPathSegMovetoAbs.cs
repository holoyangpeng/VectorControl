using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令M
	/// </summary>
	public interface ISVGPathSegMovetoAbs:ISVGPathSeg
	{
		/// <summary>
		/// 获取或设置目标点的横坐标
		/// </summary>
		float X{get;set;}

		/// <summary>
		/// 获取或设置目标点的纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
