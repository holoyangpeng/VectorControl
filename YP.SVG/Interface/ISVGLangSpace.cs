using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����ISVGLangSpace ��һ����Ϊ���������һ�㶼�߱�xml:lang �� xml:space����
	/// </summary>
	public interface ISVGLangSpace:Interface.ISVGElement
	{
		string XmlLang{get;}
		string XmlSpace{get;}
	}
}
