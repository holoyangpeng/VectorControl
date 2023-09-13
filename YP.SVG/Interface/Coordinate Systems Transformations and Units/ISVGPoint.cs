using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// 定义ISVGPoint 的一般行为
	/// </summary>
	public interface ISVGPoint:Interface.DataType.ISVGType
	{
		/// <summary>
		/// 获取或设置横坐标
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 获取或设置纵坐标
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// 根据指定的SVGMatrix进行变换
		/// </summary>
		ISVGPoint MatrixTransform(ISVGMatrix matrix);
	}
}
