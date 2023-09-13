using System;

namespace YP.SVG
{
	/// <summary>
	/// SVGContainerElement 的摘要说明。
	/// </summary>
	public class SVGContainerElement
	{
		public SVGContainerElement()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		#region ..往容器对象中添加子级对象
		/// <summary>
		/// 往容器对象中添加子级对象
		/// </summary>
		/// <param name="container">父级容器对象</param>
		/// <param name="child">欲添加的对象</param>
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
