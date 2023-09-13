using System;

namespace YP.SVG
{
	/// <summary>
	/// SVGContainerElement ��ժҪ˵����
	/// </summary>
	public class SVGContainerElement
	{
		public SVGContainerElement()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		#region ..����������������Ӽ�����
		/// <summary>
		/// ����������������Ӽ�����
		/// </summary>
		/// <param name="container">������������</param>
		/// <param name="child">����ӵĶ���</param>
		public static void AddChildElement(YP.SVG.Interface.ISVGContainer container,YP.SVG.Interface.ISVGElement child)
		{
			if(container != null && child != null)
			{
				if(container.ValidChild(child))
				{
					YP.SVG.SVGElement childelement = (YP.SVG.SVGElement)child;
					System.Xml.XmlNode next = childelement.NextSibling;
					while(true)
					{
						if(next == null)
							break;
						if(next is YP.SVG.Interface.ISVGElement)
						{
							if(container.ValidChild((YP.SVG.Interface.ISVGElement)next))
								break;
						}

						next = next.NextSibling;
					}

					if(next == null)
						container.ChildElements.Add(child);
					else if(next is YP.SVG.Interface.ISVGElement)
					{
						int index = container.ChildElements.IndexOf((YP.SVG.Interface.ISVGElement)next);
						if(index >= 0 && index < container.ChildElements.Count)
							container.ChildElements.Insert(index,(SVGElement)child);
						else
							container.ChildElements.Add(child);
					}
				}
			}
		}
		#endregion
	}
}
