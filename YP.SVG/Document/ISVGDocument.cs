using System;

namespace YP.SVG.Document
{
	/// <summary>
	/// SVGDocument�ӿ�
	/// </summary>
	public interface ISVGDocument//:Document.IRenderDocument
	{
		string Title{get;}
		string Referrer{get;}
		string Domain{get;}
		string Url{get;}
		DocumentStructure.SVGSVGElement RootElement{get;}
	}
}
