using System;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义textPath节点的通用行为
	/// </summary>
    public interface ISVGTextPathElement :
		ISVGTextContentElement,
		ISVGURIReference 
	{
		/// <summary>
		/// 获取startOffset属性
		/// </summary>
		ISVGLength StartOffset{get;}

		/// <summary>
		/// 获取method属性
		/// </summary>
		System.Enum Method{get;}

		/// <summary>
		/// 获取spacing属性
		/// </summary>
		System.Enum Spacing{get;}
	}
}
