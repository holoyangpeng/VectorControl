using System;
using System.Xml;

using YP.SVG.Interface;
using YP.SVG.Interface.DocumentStructure;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// ʵ��һ��SVGElementInstance������ʵ��
	/// һ������ʵ����Ӧ���ĵ���һ���ض��Ľڵ㡣
	/// </summary>
	public class SVGElementInstance:Interface.DocumentStructure.ISVGElementInstance
	{
		#region ..���켰����
		public SVGElementInstance(Interface.ISVGElement corresponding,Interface.DocumentStructure.ISVGUseElement correspondingUse,Interface.DocumentStructure.ISVGElementInstance parentInstance)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.correspondingElement = corresponding;
			this.correspondingUseElement = correspondingUse;
			this.parentNode = parentInstance;
		}

		public SVGElementInstance(Interface.ISVGElement corresponding,Interface.DocumentStructure.ISVGUseElement correspondingUse):this(corresponding,correspondingUse,null)
		{
		}
		#endregion

		#region ..˽�б���
		Interface.ISVGElement correspondingElement;
		Interface.DocumentStructure.ISVGUseElement correspondingUseElement;
		Interface.DocumentStructure.ISVGElementInstance parentNode;
		YP.SVG.DocumentStructure.SVGGElement g = null;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ��Use���ƹ����������滻Use�����G����
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
		/// ��ȡ��Ӧ��SVGElement
		/// </summary>
		public ISVGElement CorrespondingElement
		{
			get
			{
				return this.correspondingElement;
			}
		}

		/// <summary>
		/// ��ȡ����Ӧ��Use����
		/// </summary>
		public ISVGUseElement CorrespondingUseElement
		{
			get
			{
				return this.correspondingUseElement;
			}
		}

		/// <summary>
		/// ��ȡ����ʵ���ĸ�����ÿһ������ʵ�����߱�һ��������ʵ�������Ǹö���ʵ��ֱ�ӱ�UseӦ�ã���������£�����ʵ���ĸ�����ʵ��Ϊnull
		/// </summary>
		public ISVGElementInstance ParentNode
		{
			get
			{
				return this.parentNode;
			}
		}

		/// <summary>
		/// ��ȡ����ʵ�����Ӽ��б�
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
		/// ��ȡ��һ���Ӽ�����
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
		/// ��ȡ���һ���Ӽ�����
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
		/// ��ȡ��ʵ����������Ӧ��ǰһ������ʵ��
		/// </summary>
		public ISVGElementInstance PreviousSibling
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// ��ȡ�ö���ʵ������Ӧ�ĺ�һ������ʵ��
		/// </summary>
		public ISVGElementInstance NextSibling
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��������
		/// </summary>
		public void UpdateRefElement()
		{
			this.g = null;
		}
		#endregion
	}
}
