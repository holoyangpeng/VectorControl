using System;

using YP.SVG.Interface.DataType;
using YP.SVG.Interface.CTS;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义SVGTextElement, SVGTSpanElement, SVGTRefElement, SVGAltGlyphElement and SVGTextPathElement的通用行为
	/// </summary>
	public interface ISVGTextContentElement
	{
		/// <summary>
		/// 获取长度调整类别
		/// </summary>
		System.Enum LengthAdjust{get;}

		/// <summary>
		/// 获取TextLength属性
		/// </summary>
		ISVGLength TextLength{get;}

		/// <summary>
		/// 获取当前节点中将要绘制的总字符数目，包括由“tref”所指向的文本节点字符长度
		/// </summary>
		/// <returns></returns>
		int     GetNumberOfChars ();

		/// <summary>
		/// 当绘制全部字符时，所需要的总的GDI长度
		/// </summary>
		/// <returns></returns>
		float    GetComputedTextLength ();

		/// <summary>
		/// 绘制子字符串所需要的长度
		/// </summary>
		/// <param name="charnum">起始字符索引</param>
		/// <param name="nchars">子字符串长度</param>
		/// <returns></returns>
		float    GetSubStringLength (int charnum,int nchars );

		/// <summary>
		/// 获取指定索引处字符的开始绘制位置
		/// </summary>
		ISVGPoint GetStartPositionOfChar (int charnum );

		/// <summary>
		/// 获取指定索引处字符绘制结束时的位置
		/// </summary>
		ISVGPoint GetEndPositionOfChar (int charnum );

		/// <summary>
		/// 获取指定索引处字符的绘制边界
		/// </summary>
		ISVGRect  GetExtentOfChar (int charnum );

		/// <summary>
		/// 获取指定索引处字符相对于当前用户空间的旋转角度
		/// </summary>
		/// <param name="charnum">字符索引</param>
		/// <returns></returns>
		float    GetRotationOfChar (int charnum );

		/// <summary>
		/// 获取指定位置处的字符索引
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		//int     GetCharNumAtPosition (ISVGPoint point,ref YP.SVGDom.Text.TextContentInfo info);

		/// <summary>
		/// 选择子字符串
		/// </summary>
		/// <param name="charnum">开始字符索引</param>
		/// <param name="nchars">字符串长度</param>
		void     SelectSubString (int charnum,int nchars );
	}
}
