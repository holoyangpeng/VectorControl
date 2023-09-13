using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义命令a
	/// </summary>
	public interface ISVGPathSegArcRel:ISVGPathSeg
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
		/// 定义横半径
		/// </summary>
		float R1{set;get;}

		/// <summary>
		/// 定义第纵半径
		/// </summary>
		float R2{set;get;}

		/// <summary>
		/// 定义第二控制点的相对横坐标
		/// </summary>
		float Angle{set;get;}

		/// <summary>
		/// The value of the large-arc-flag parameter
		/// </summary>
		bool LargeArcFlag {set;get;}

		/// <summary>
		/// The value of the sweep-flag parameter. 
		/// </summary>
		bool SweepFlag{set;get;}
	}
}
