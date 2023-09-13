using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGEllipseElement 的摘要说明。
	/// </summary>
	public interface ISVGEllipseElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		/// <summary>
		/// 表示椭圆对象的Cx属性
		/// </summary>
		SVG.DataType.SVGLength Cx{get;}

		/// <summary>
		/// 表示椭圆对象的Cy属性
		/// </summary>
        SVG.DataType.SVGLength Cy { get; }

		/// <summary>
		/// 表示椭圆对象的Rx属性
		/// </summary>
        SVG.DataType.SVGLength Rx { get; }
		
		/// <summary>
		/// 表示椭圆对象的Ry属性
		/// </summary>
        SVG.DataType.SVGLength Ry { get; }
	}
}
