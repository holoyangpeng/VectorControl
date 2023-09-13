using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGElementInstance ��ժҪ˵����
	/// </summary>
	public interface ISVGElementInstance
	{
		ISVGElement CorrespondingElement{get;}
		ISVGUseElement CorrespondingUseElement{get;}
		ISVGElementInstance ParentNode{get;}
		ISVGElementInstanceList ChildNodes{get;}
		ISVGElementInstance FirstChild{get;}
		ISVGElementInstance LastChild{get;}
		ISVGElementInstance PreviousSibling{get;}
		ISVGElementInstance NextSibling{get;}
	}
}
