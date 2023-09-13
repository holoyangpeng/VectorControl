using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// SVG中的颜色
	/// </summary>
	public interface IRgbColor:ISVGType
	{
		/// <summary>
		/// 获取颜色的Red颜色分量值
		/// </summary>
		short R{get;}

		/// <summary>
		/// 获取颜色的Green颜色分量值
		/// </summary>
		short G{get;}

		/// <summary>
		/// 获取颜色的Blue颜色分量值
		/// </summary>
		short B{get;}

		System.Drawing.Color GDIColor{get;}
	}
}
