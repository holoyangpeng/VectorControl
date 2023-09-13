using System;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// 定义glyphRef节点的通用行为
	/// </summary>
    public interface ISVGGlyphRefElement :
		ISVGElement,
		ISVGURIReference,
		ISVGStylable 
	{
		/// <summary>
		/// 获取或设置glyphRef属性
		/// </summary>
		string GlyphRef{get;set;}
		// raises DOMException on setting

		/// <summary>
		/// 获取或设置format属性
		/// </summary>
		string Format{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// 获取或设置x属性
		/// </summary>
		float    X{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// 获取或设置y属性
		/// </summary>
		float    Y{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// 获取或设置dx属性
		/// </summary>
		float    Dx{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// 获取或设置dy属性
		/// </summary>
		float    Dy{set;get;}
		// raises DOMException on setting

	}
}
