using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义SVG中的容器对象，这类对象除了动画之外一般还允许包含其他元素
	/// </summary>
	public interface ISVGContainer
	{
		/// <summary>
		/// 获取子级元素
		/// </summary>
		YP.SVG.SVGElementCollection ChildElements{get;}

		/// <summary>
		/// 判断节点是否是有效的子级节点
		/// </summary>
		/// <param name="child">子级节点</param>
		/// <returns></returns>
		bool ValidChild(Interface.ISVGElement child);
	}
}
