using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ISVGRectElement ��ժҪ˵����
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
        /// ��ʾ���ζ����X����
        /// </summary>
        SVG.DataType.SVGLength X { get; }

        /// <summary>
        /// ��ʾ���ζ����Y����
        /// </summary>
        SVG.DataType.SVGLength Y { get; }

        /// <summary>
        /// ��ʾ���ζ����Width����
        /// </summary>
        SVG.DataType.SVGLength Width { get; }

        /// <summary>
        /// ��ʾ���ζ����Height����
        /// </summary>
        SVG.DataType.SVGLength Height { get; }

		/// <summary>
		/// ��ʾ���ζ����Rx����
		/// </summary>
        SVG.DataType.SVGLength Rx { get; }
		
		/// <summary>
		/// ��ʾ���ζ����Ry����
		/// </summary>
        SVG.DataType.SVGLength Ry { get; }
	}
}
