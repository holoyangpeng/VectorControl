using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	///定一命令t
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticSmoothRel:ISVGPathSeg
	{/// <summary>
		/// 终点相对横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 终点相对纵坐标
		/// </summary>
		float Y{set;get;}
	}
}
