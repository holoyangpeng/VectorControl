using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGLangSpace 的一般行为，这类对象一般都具备xml:lang 和 xml:space属性
	/// </summary>
	public interface ISVGLangSpace:Interface.ISVGElement
	{
		string XmlLang{get;}
		string XmlSpace{get;}
	}
}
