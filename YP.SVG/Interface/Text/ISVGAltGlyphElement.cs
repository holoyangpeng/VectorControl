using System;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// ����altGraph�ڵ��ͨ����Ϊ
	/// </summary>
    public interface ISVGAltGlyphElement :
		ISVGTextPositioningElement,
		ISVGURIReference
	{
		/// <summary>
		/// ��ȡglyglyphRef ����
		/// </summary>
		string GlyphRef{get;}
		
		/// <summary>
		/// ��ȡformat����
		/// </summary>
		string Format{get;}
	}
}
