using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义SVG中构成路径的对象的一般行为
	/// </summary>
	public interface ISVGPathable
	{
		/// <summary>
		/// 获取绘制路径
		/// </summary>
		System.Drawing.Drawing2D.GraphicsPath GPath{get;}

        //ISVGRenderer Render { get; }
	}
}
