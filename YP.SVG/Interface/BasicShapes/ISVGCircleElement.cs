using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGCircleElement 的摘要说明。
	/// </summary>
	public interface ISVGCircleElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		/// <summary>
		/// 获取圆心横坐标
		/// </summary>
		SVG.DataType.SVGLength Cx{get;}

		/// <summary>
		/// 获取圆心纵坐标
		/// </summary>
        SVG.DataType.SVGLength Cy { get; }

		/// <summary>
		/// 获取圆半径
		/// </summary>
        SVG.DataType.SVGLength R { get; }
	}
}
