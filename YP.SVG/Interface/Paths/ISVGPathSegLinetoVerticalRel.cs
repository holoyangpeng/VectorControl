using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令v
	/// </summary>
	public interface ISVGPathSegLinetoVerticalRel:ISVGPathSeg
	{
		/// <summary>
		/// 终点的相对纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
