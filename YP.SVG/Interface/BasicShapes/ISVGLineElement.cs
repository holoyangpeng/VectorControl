using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGLineElement 的摘要说明。
	/// </summary>
	public interface ISVGLineElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		/// <summary>
		/// 表示直线对象的X1属性（起始横坐标）
		/// </summary>
		SVG.DataType.SVGLength X1{get;}

		/// <summary>
		/// 表示直线对象的Y1属性（起始纵坐标）
		/// </summary>
		SVG.DataType.SVGLength Y1{get;}

		/// <summary>
		/// 表示直线对象的X2属性（终止横坐标）
		/// </summary>
		SVG.DataType.SVGLength X2{get;}

		/// <summary>
		/// 表示直线对象的Y2属性（终止纵坐标）
		/// </summary>
		SVG.DataType.SVGLength Y2{get;}
	}
}
