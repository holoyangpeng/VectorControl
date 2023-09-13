using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGCircleElement ��ժҪ˵����
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
		/// ��ȡԲ�ĺ�����
		/// </summary>
		SVG.DataType.SVGLength Cx{get;}

		/// <summary>
		/// ��ȡԲ��������
		/// </summary>
        SVG.DataType.SVGLength Cy { get; }

		/// <summary>
		/// ��ȡԲ�뾶
		/// </summary>
        SVG.DataType.SVGLength R { get; }
	}
}
