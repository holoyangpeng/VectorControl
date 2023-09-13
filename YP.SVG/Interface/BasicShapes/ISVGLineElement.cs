using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGLineElement ��ժҪ˵����
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
		/// ��ʾֱ�߶����X1���ԣ���ʼ�����꣩
		/// </summary>
		SVG.DataType.SVGLength X1{get;}

		/// <summary>
		/// ��ʾֱ�߶����Y1���ԣ���ʼ�����꣩
		/// </summary>
		SVG.DataType.SVGLength Y1{get;}

		/// <summary>
		/// ��ʾֱ�߶����X2���ԣ���ֹ�����꣩
		/// </summary>
		SVG.DataType.SVGLength X2{get;}

		/// <summary>
		/// ��ʾֱ�߶����Y2���ԣ���ֹ�����꣩
		/// </summary>
		SVG.DataType.SVGLength Y2{get;}
	}
}
