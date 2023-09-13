using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令V
	/// </summary>
	public interface ISVGPathSegLinetoVerticalAbs:ISVGPathSeg
	{
		/// <summary>
		/// 终点的绝对纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
