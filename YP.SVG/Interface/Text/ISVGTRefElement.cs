using System;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义tref节点的通用行为
	/// </summary>
	public interface ISVGTRefElement:
		ISVGTextPositioningElement,
		ISVGURIReference
	{
	}
}
