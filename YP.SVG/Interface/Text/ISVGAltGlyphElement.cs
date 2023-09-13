using System;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义altGraph节点的通用行为
	/// </summary>
    public interface ISVGAltGlyphElement :
		ISVGTextPositioningElement,
		ISVGURIReference
	{
		/// <summary>
		/// 获取glyglyphRef 属性
		/// </summary>
		string GlyphRef{get;}
		
		/// <summary>
		/// 获取format属性
		/// </summary>
		string Format{get;}
	}
}
