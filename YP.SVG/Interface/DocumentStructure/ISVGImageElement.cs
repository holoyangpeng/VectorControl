using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGImageElement ��ժҪ˵����
	/// </summary>
	public interface ISVGImageElement:
		ISVGElement,
		ISVGTests,
		ISVGStylable,
		ISVGTransformable,
		ISVGLangSpace
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

		CTS.ISVGPreserveAspectRatio PreserveAspectRatio{get;}

		/// <summary>
		/// ��ȡͼƬԴ
		/// </summary>
		//System.Drawing.Bitmap ImageSource{get;}
	}
}
