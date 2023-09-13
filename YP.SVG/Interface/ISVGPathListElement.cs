using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义一组对象，这组对象都具备了SVGPathSegList的结构，包括SVGPathElement 和SVGAnimateMotionElement
	/// </summary>
	public interface ISVGPathSegListElement:ISVGElement
	{
		/// <summary>
		/// 获取其SVGPathSegList构成
		/// </summary>
		Interface.Paths.ISVGPathSegList SVGPathSegList{get;}

		/// <summary>
		/// 获取路径
		/// </summary>
		System.Drawing.Drawing2D.GraphicsPath SegPath{get;}
	}
}
