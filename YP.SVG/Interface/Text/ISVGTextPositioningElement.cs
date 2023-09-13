using System;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义SVGTextElement, SVGTSpanElement, SVGTRefElement and SVGAltGlyphElement的通用行为
	/// </summary>
	public interface ISVGTextPositioningElement:ISVGTextContentElement
	{
		/// <summary>
		/// 获取x属性
		/// </summary>
		ISVGLengthList X{get;}

		/// <summary>
		/// 获取y属性
		/// </summary>
		ISVGLengthList Y{get;}

		/// <summary>
		/// 获取dx属性
		/// </summary>
		ISVGLengthList Dx{get;}

		/// <summary>
		/// 获取dy属性
		/// </summary>
		ISVGLengthList Dy{get;}

		/// <summary>
		/// 获取rotate属性
		/// </summary>
		ISVGNumberList Rotate{get;}
	}
}
