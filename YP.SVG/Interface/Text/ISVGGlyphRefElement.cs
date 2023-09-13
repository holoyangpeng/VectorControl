using System;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// ����glyphRef�ڵ��ͨ����Ϊ
	/// </summary>
    public interface ISVGGlyphRefElement :
		ISVGElement,
		ISVGURIReference,
		ISVGStylable 
	{
		/// <summary>
		/// ��ȡ������glyphRef����
		/// </summary>
		string GlyphRef{get;set;}
		// raises DOMException on setting

		/// <summary>
		/// ��ȡ������format����
		/// </summary>
		string Format{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// ��ȡ������x����
		/// </summary>
		float    X{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// ��ȡ������y����
		/// </summary>
		float    Y{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// ��ȡ������dx����
		/// </summary>
		float    Dx{set;get;}
		// raises DOMException on setting

		/// <summary>
		/// ��ȡ������dy����
		/// </summary>
		float    Dy{set;get;}
		// raises DOMException on setting

	}
}
