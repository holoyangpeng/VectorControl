using System;
using System.Xml;

using YP.SVG.Interface;
using YP.SVG.Interface.DocumentStructure;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// 实现一个SVGElementInstance，对象实例
	/// 一个对象实例对应于文档中一个特定的节点。
	/// </summary>
	public class SVGElementInstance:Interface.DocumentStructure.ISVGElementInstance
	{
		#region ..构造及消除
		public SVGElementInstance(Interface.ISVGElement corresponding,Interface.DocumentStructure.ISVGUseElement correspondingUse,Interface.DocumentStructure.ISVGElementInstance parentInstance)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.correspondingElement = corresponding;
			this.correspondingUseElement = correspondingUse;
			this.parentNode = parentInstance;
		}

		public SVGElementInstance(Interface.ISVGElement corresponding,Interface.DocumentStructure.ISVGUseElement correspondingUse):this(corresponding,correspondingUse,null)
		{
		}
		#endregion

		#region ..私有变量
		Interface.ISVGElement correspondingElement;
		Interface.DocumentStructure.ISVGUseElement correspondingUseElement;
		Interface.DocumentStructure.ISVGElementInstance parentNode;
		YP.SVG.DocumentStructure.SVGGElement g = null;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取在Use绘制过程中用于替换Use对象的G对象
		/// </summary>
		public YP.SVG.DocumentStructure.SVGGElement BackGroundDrawElement
		{
			get
			{
				if(this.g == null)
				{
					if(this.correspondingUseElement != null)
					{
						Document.SVGDocument doc = this.correspondingElement.OwnerDocument;
						bool oldCreate = doc.inUseCreate;
						doc.inUseCreate = true;
						bool old = doc.AcceptNodeChanged;
						doc.AcceptNodeChanged = false;
						this.g = (YP.SVG.DocumentStructure.SVGGElement)doc.CreateElement("", "g", Document.SVGDocument.SvgNamespace);
						YP.SVG.SVGElement refElm = (YP.SVG.SVGElement)this.correspondingElement;
						YP.SVG.DocumentStructure.SVGUseElement use = (YP.SVG.DocumentStructure.SVGUseElement)this.correspondingUseElement;
						if(refElm is YP.SVG.DocumentStructure.SVGSymbolElement)
						{
							SVGSVGElement svgElm = (SVGSVGElement)refElm.OwnerDocument.CreateElement(this.correspondingElement.OwnerDocument.Prefix, "svg", Document.SVGDocument.SvgNamespace);
							string width = "100%";
							string height = "100%";
							if(use != null)
							{
								if(use.HasAttribute("width"))
									width = use.GetAttribute("width").Trim();
								if(use.HasAttribute("height"))
									height = use.GetAttribute("height").Trim();
							}
							svgElm.InternalSetAttribute("width", width);
							svgElm.InternalSetAttribute("height", height);

                            //foreach(XmlNode att in ((YP.SVG.SVGElement)this.correspondingElement).Attributes )
                            //{
                            //    if(att.LocalName != "x" && 
                            //        att.LocalName != "y" &&
                            //        att.LocalName != "width" &&
                            //        att.LocalName != "height" &&
                            //        att.LocalName != "transform" && 
                            //        !(att.LocalName == "href" && att.NamespaceURI == Document.SVGDocument.XLinkNamespace && !(((YP.SVG.SVGElement)this.correspondingUseElement) is SVGImageElement)))
                            //    {
                            //    }
                            //}
							svgElm.refedElement = refElm.Clone() as SVGStyleable;
							g.InternalAppendChild(svgElm);
                            if (!g.ChildElements.Contains(svgElm))
								g.ChildElements.Add(svgElm);
						}
						else if(refElm != null)
						{
							doc.BeginProcess();
							System.Xml.XmlNode node1 = refElm.CloneNode(true);
							doc.EndProcess();
							bool old2 = doc.inLoadProcess;
							doc.inLoadProcess = true;
							node1 = doc.ImportNode(node1,true);
							this.g.InternalAppendChild(node1);
							doc.inLoadProcess = old2;
						}
						doc.AcceptNodeChanged = old;
						doc.inUseCreate = oldCreate;
					}
				}
				return this.g;
			}
		}
		/// <summary>
		/// 获取对应的SVGElement
		/// </summary>
		public ISVGElement CorrespondingElement
		{
			get
			{
				return this.correspondingElement;
			}
		}

		/// <summary>
		/// 获取所对应的Use对象
		/// </summary>
		public ISVGUseElement CorrespondingUseElement
		{
			get
			{
				return this.correspondingUseElement;
			}
		}

		/// <summary>
		/// 获取对象实例的父对象，每一个对象实例都具备一个父对象实例，除非该对象实例直接被Use应用，这种情况下，对象实例的父对象实例为null
		/// </summary>
		public ISVGElementInstance ParentNode
		{
			get
			{
				return this.parentNode;
			}
		}

		/// <summary>
		/// 获取对象实例的子级列表
		/// </summary>
		public ISVGElementInstanceList ChildNodes
		{
			get
			{
				if(this.correspondingElement != null)
				{
					return new SVGElementInstanceList(((YP.SVG.SVGElement)this.correspondingElement).ChildNodes,this.correspondingUseElement,this.parentNode);
				}
				return null;
			}
		}

		/// <summary>
		/// 获取第一个子级对象
		/// </summary>
		public ISVGElementInstance FirstChild
		{
			get
			{
				Interface.DocumentStructure.ISVGElementInstanceList cn = ChildNodes;
				if(cn != null)
				{
					if(cn.Length > 0)
						return cn.Item(0);
				}
                
				return null;
			}
		}

		/// <summary>
		/// 获取最后一个子级对象
		/// </summary>
		public ISVGElementInstance LastChild
		{
			get
			{
				Interface.DocumentStructure.ISVGElementInstanceList cn = ChildNodes;
				if(cn != null)
				{
					if(cn.Length > 0)
						return cn.Item(cn.Length - 1);
				}
				return null;
			}
		}

		/// <summary>
		/// 获取该实例对象所对应的前一个对象实例
		/// </summary>
		public ISVGElementInstance PreviousSibling
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// 获取该对象实例所对应的后一个对象实例
		/// </summary>
		public ISVGElementInstance NextSibling
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region ..更新引用
		/// <summary>
		/// 更新引用
		/// </summary>
		public void UpdateRefElement()
		{
			this.g = null;
		}
		#endregion
	}
}
