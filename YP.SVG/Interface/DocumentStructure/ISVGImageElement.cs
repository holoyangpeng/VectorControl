using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGImageElement 的摘要说明。
	/// </summary>
	public interface ISVGImageElement:
		ISVGElement,
		ISVGTests,
		ISVGStylable,
		ISVGTransformable,
		ISVGLangSpace
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

		CTS.ISVGPreserveAspectRatio PreserveAspectRatio{get;}

		/// <summary>
		/// 获取图片源
		/// </summary>
		//System.Drawing.Bitmap ImageSource{get;}
	}
}
