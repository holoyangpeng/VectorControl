using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGEllipseElement ��ժҪ˵����
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
		/// ��ʾ��Բ�����Cx����
		/// </summary>
		SVG.DataType.SVGLength Cx{get;}

		/// <summary>
		/// ��ʾ��Բ�����Cy����
		/// </summary>
        SVG.DataType.SVGLength Cy { get; }

		/// <summary>
		/// ��ʾ��Բ�����Rx����
		/// </summary>
        SVG.DataType.SVGLength Rx { get; }
		
		/// <summary>
		/// ��ʾ��Բ�����Ry����
		/// </summary>
        SVG.DataType.SVGLength Ry { get; }
	}
}
