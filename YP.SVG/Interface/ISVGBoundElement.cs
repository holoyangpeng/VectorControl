using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����һ�����������󶼾߱�X,Y,Width,Height����
	/// </summary>
	public interface ISVGBoundElement
	{
		/// <summary>
		/// ��ʾ���ζ����X����
		/// </summary>
		SVG.DataType.SVGLength X{get;}

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
	}
}
