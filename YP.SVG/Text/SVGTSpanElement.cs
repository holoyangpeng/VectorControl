using System;

namespace YP.SVG.Text
{
	/// <summary>
	/// 定义tspan节点
	/// </summary>
	public class SVGTSpanElement:YP.SVG.Text.SVGTextPositionElement,Interface.Text.ISVGTSpanElement
	{
		#region ..构造及消除
		public SVGTSpanElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion
	}
}
