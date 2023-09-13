using System;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// ����textPath�ڵ��ͨ����Ϊ
	/// </summary>
    public interface ISVGTextPathElement :
		ISVGTextContentElement,
		ISVGURIReference 
	{
		/// <summary>
		/// ��ȡstartOffset����
		/// </summary>
		ISVGLength StartOffset{get;}

		/// <summary>
		/// ��ȡmethod����
		/// </summary>
		System.Enum Method{get;}

		/// <summary>
		/// ��ȡspacing����
		/// </summary>
		System.Enum Spacing{get;}
	}
}
