using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGRectElement 的摘要说明。
	/// </summary>
	public interface ISVGRectElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
        /// <summary>
        /// 表示矩形对象的X属性
        /// </summary>
        SVG.DataType.SVGLength X { get; }

        /// <summary>
        /// 表示矩形对象的Y属性
        /// </summary>
        SVG.DataType.SVGLength Y { get; }

        /// <summary>
        /// 表示矩形对象的Width对象
        /// </summary>
        SVG.DataType.SVGLength Width { get; }

        /// <summary>
        /// 表示矩形对象的Height属性
        /// </summary>
        SVG.DataType.SVGLength Height { get; }

		/// <summary>
		/// 表示矩形对象的Rx对象
		/// </summary>
        SVG.DataType.SVGLength Rx { get; }
		
		/// <summary>
		/// 表示矩形对象的Ry属性
		/// </summary>
        SVG.DataType.SVGLength Ry { get; }
	}
}
