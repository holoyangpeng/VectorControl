using System;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// ����SVGTextElement, SVGTSpanElement, SVGTRefElement and SVGAltGlyphElement��ͨ����Ϊ
	/// </summary>
	public interface ISVGTextPositioningElement:ISVGTextContentElement
	{
		/// <summary>
		/// ��ȡx����
		/// </summary>
		ISVGLengthList X{get;}

		/// <summary>
		/// ��ȡy����
		/// </summary>
		ISVGLengthList Y{get;}

		/// <summary>
		/// ��ȡdx����
		/// </summary>
		ISVGLengthList Dx{get;}

		/// <summary>
		/// ��ȡdy����
		/// </summary>
		ISVGLengthList Dy{get;}

		/// <summary>
		/// ��ȡrotate����
		/// </summary>
		ISVGNumberList Rotate{get;}
	}
}
