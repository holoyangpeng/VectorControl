using System;
using System.Collections;
using System.Xml;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Drawing;

namespace YP.SVG
{
	/// <summary>
	/// 所有SVG图元对象的基类，当使用SVGDocument导入SVG内容时，文档中的所有SVG节点都将被创建为SVGElement对象
	/// </summary>
	[Serializable]
	[Browsable(false),ComVisible(false)]
	public class SVGElement:System.Xml.XmlElement,
        Interface.ISVGElement,
        Interface.ISVGTests,
        Base.Interface.IWebElement,
        Interface.ISVGExternalResourcesRequired, 
        System.IDisposable,
        System.ComponentModel.ICustomTypeDescriptor
    {
        #region ..构造及消除
        public SVGElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			this.ownerDocument = doc; 
		}
		#endregion

		#region ..私有变量
		Document.SVGDocument ownerDocument;
		string id = string.Empty;
		public bool canRender = false;
		#endregion

		#region ..WebElement属性
		/// <summary>
		/// 获取节点的父级节点
		/// </summary>
		public SVGElement ParentElement
		{
			get
			{
				if(this == this.OwnerDocument.DocumentElement)
					return null;
				return this.ParentNode as SVGElement;
			}
		}

		/// <summary>
		/// 获取对象邻近的前一个节点
		/// </summary>
        public SVGElement PreviousElement
		{
			get
			{
				YP.SVG.SVGElement temp = null;
				if(this.ParentNode is YP.SVG.SVGElement)
				{
					System.Xml.XmlNode node = this.PreviousSibling;
					while(true)
					{
						if(node == null || node is YP.SVG.SVGElement)
						{
							temp = (YP.SVG.SVGElement)node;
							break;
						}
						node = node.PreviousSibling;
					}
				}
				return temp;
			}
		}

		/// <summary>
		/// 获取对象邻近的后一个节点
		/// </summary>
		public Interface.ISVGElement NextElement
		{
			get
			{
				YP.SVG.SVGElement temp = null;
				if(this.ParentNode != null)
				{
					System.Xml.XmlNode node = this.NextSibling;
					while(true)
					{
						if(node == null || node is YP.SVG.SVGElement)
						{
							temp = (YP.SVG.SVGElement)node;
							break;
						}
						node = node.NextSibling;
					}
				}
				return temp;
			}
		}
		#endregion

		#region ..公共属性
        /// <summary>
        /// 与SVGElement对象相关联的用户自定义数据.
        /// 注意,该值是一个运行态属性,并不会写入SVG文档中,当文档被重新打开时,该属性值将被重置为空
        /// </summary>
        public object Tag { set; get; }

		/// <summary>
		/// 获取对象的ID名称
		/// </summary>
		public string ID
		{
			get
			{
				return this.GetAttribute("id");
			}
            set
            {
                this.SetAttribute("id", value, false);
            }
		}

		/// <summary>
		/// gets or sets the property category the element has
		/// </summary>
		//public YP.Forms.PropertyCategory HasCategory 
		//{
		//	set
		//	{
		//		this.hasCategory = value;
		//	}
		//	get
		//	{
		//		return this.hasCategory & this.ownerDocument.DefaultProperties;
		//	}
		//}

		/// <summary>
		/// 获取节点所属文档
		/// </summary>
		public new Document.SVGDocument OwnerDocument
		{
			get
			{
				return this.ownerDocument;
			}
		}

		/// <summary>
		/// 获取第一个子节点
		/// </summary>
		public Interface.ISVGElement FirstChildElement
		{
			get
			{
				foreach(System.Xml.XmlNode node in this.ChildNodes)
					if(node is SVGElement)
						return node as SVGElement;
				return null;
			}
		}

		/// <summary>
		/// 判断节点是否可以绘制
		/// </summary>
		public bool CanRender
		{
			get
			{
				return this.canRender;
			}
		}

		/// <summary>
		/// 获取节点的XmlSpace空间
		/// </summary>
		public string XmlSpace
		{
			get
			{
				string s = this.GetAttribute("xml:space");
				if(s.Length == 0)
				{
					if(ParentNode is SVGElement)
					{
						SVGElement par = (SVGElement) ParentNode;
						s = par.XmlSpace;
					}
					else s = "default";
				}
				
				return s;
			}
		}

        public SVGElement OwnerStructureElement
        {
            get
            {
                if (this is DocumentStructure.SVGGElement || this is DocumentStructure.SVGSymbolElement)
                    return this;
                XmlNode parent = this.ParentNode;
                while (parent != null)
                {
                    if (parent is DocumentStructure.SVGGElement
                        || parent is DocumentStructure.SVGSymbolElement)
                        return (SVGElement)parent;
                    parent = parent.ParentNode;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取当前对象所属的SVG图元(SVGSVGElement)对象
        /// </summary>
		public Interface.DocumentStructure.ISVGSVGElement OwnerSvgElement
		{
			get
			{
				//return (SvgSvgElement)this.SelectSingleNode("ancestor::svg:svg", this.OwnerDocument.namespaceManager);
				if(this.Equals(this.OwnerDocument.DocumentElement))
				{
					return null;
				}
				else
				{
                    //var elm = this.SelectSingleNode(".//ancestor::svg") ;
                    //return elm as DocumentStructure.SVGSVGElement;
                    XmlNode parent = this.ParentNode;
                    while (parent != null)
                    {
                        if (parent is Interface.DocumentStructure.ISVGSVGElement)
                            return (Interface.DocumentStructure.ISVGSVGElement)parent;
                        parent = parent.ParentNode;
                    }
                    return null;
				}
			}
		}

        /// <summary>
        /// 对象是否处于激活状态
        /// 所谓激活状态就是已经被加入绘制序列
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// 获取节点的Xml:Lang属性
		/// </summary>
		public string XmlLang
		{
			get
			{
				string s = this.GetAttribute("xml:lang");
				if(s .Length ==0)
				{
					if(ParentNode is SVGElement)
					{
						SVGElement par = (SVGElement) ParentNode;
						s = par.XmlLang;
					}
					else s = "";
				}
				
				return s;
			}
		}

		/// <summary>
		/// 获取离当前节点最近的视图节点
		/// </summary>
		public virtual Interface.ISVGElement ViewPortElement
		{
			get
			{
				if(this.Equals(this.OwnerDocument.DocumentElement))
				{
					return null;
				}
				else
				{
					XmlNode parent = this.ParentNode;
					while(parent != null)
					{
						if(parent is Interface.DocumentStructure.ISVGSVGElement)
							return (Interface.ISVGElement) parent;
						parent = parent.ParentNode;
					}
					return null;
				}
			}
		}
		#endregion

		#region ..实现ISVGTests
		public Interface.DataType.ISVGStringList RequiredFeatures
		{
			get
			{
				string s = (HasAttribute("requiredFeatures")) ? GetAttribute("requiredFeatures") : null;
				return new DataType.SVGStringList();
			}
		}

		public Interface.DataType.ISVGStringList RequiredExtensions
		{
			get
			{
				string s = (HasAttribute("requiredExtensions")) ? GetAttribute("requiredExtensions") : null;
				return new DataType.SVGStringList(s,new char[]{',',' '});
			}
		}

		public Interface.DataType.ISVGStringList SystemLanguage
		{
			get
			{
				string s = (HasAttribute("systemLanguage")) ? GetAttribute("systemLanguage") : null;
				return new DataType.SVGStringList(s,new char[]{',',' '});
			}
		}


		public bool HasExtension(string extension)
		{
			DataType.SVGStringList list = (DataType.SVGStringList)  this.RequiredExtensions;
			return list.Contains(extension);
		}

		public bool SupportsAllTests
		{
			get
			{
				YP.SVG.DataType.SVGStringList list;
				if(this.RequiredFeatures.NumberOfItems>0)
				{
					list = (DataType.SVGStringList) this.RequiredFeatures;
					for(int i = 0;i<list.NumberOfItems;i++)
					{
						DataType.SVGString req = (DataType.SVGString)list.GetItem(i);
						if(!Document.SVGDocument.SupportedFeatures.Contains(req.Value)) 
							return false;
					}
				}

				if(this.RequiredExtensions.NumberOfItems>0)
				{
					list = (DataType.SVGStringList) this.RequiredExtensions;
					for(int i = 0;i<list.NumberOfItems;i++)
					{
						DataType.SVGString req = (DataType.SVGString)list.GetItem(i);
						if(!Document.SVGDocument.SupportedExtensions.Contains(req.Value))
							return false;
					}
				}
				if(this.SystemLanguage.NumberOfItems>0)
				{
					// TODO: or if one of the languages indicated by user preferences exactly equals a prefix of one of the languages given in the value of this parameter such that the first tag character following the prefix is "-".
					list = (DataType.SVGStringList) this.SystemLanguage;
					bool found = false;
					string currentLang = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

					for(int i = 0;i<list.NumberOfItems;i++)
					{
						DataType.SVGString req = (DataType.SVGString)list.GetItem(i);
						if(req.Value == currentLang)
							found = true;
					}
					if(!found) 
						return false;
				}
				return true;
			}
		}
		#endregion

		#region ..属性操作
        public void AddSVGAttribute(string attributeName, string attributeValue)
        {
    
            if (this.ownerDocument.IsActive && this.ParentNode != null)
                this.BeforeAttributeChanged(attributeName);
                
            this.SetSVGAttribute(attributeName, attributeValue);

            if (this.ownerDocument.IsActive && this.ParentNode != null)
                AfterAttributeChanged(attributeName);
        }

		/// <summary>
		/// 当属性发生修改活着添加时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
        public virtual void SetSVGAttribute(string attributeName, string attributeValue)
        {
            if (string.Compare(attributeName,"id")==0)
            {
                if (this.id != attributeValue)
                    this.OnIDChanged(this.id, attributeValue);
                this.id = attributeValue;
            }
        }
		#endregion

        #region ..BeforeAttributeChanged
        public virtual void BeforeAttributeChanged(string attributeName)
        {
            AttributeChangedResult result = this.AttributeChangeTest(attributeName);
            if ((result & AttributeChangedResult.GraphicsPathChanged) == AttributeChangedResult.GraphicsPathChanged | (result & AttributeChangedResult.TransformChanged) == AttributeChangedResult.TransformChanged)
               if(this.IsActive)
                   this.ownerDocument.RefreshOriginalElement(this);
        }
        #endregion

        #region ..AfterAttributeChanged
        public virtual void AfterAttributeChanged(string attributeName)
        {
            AttributeChangedResult result = this.AttributeChangeTest(attributeName);
            if ((result) != AttributeChangedResult.NoVisualChanged && this.IsActive)
            {
                this.ownerDocument.RefreshElement(this);
            }
        }
        #endregion

        #region ..AttributeChangeTest
        /// <summary>
        /// 判断属性改变的动作
        /// </summary>
        /// <returns></returns>
        public virtual AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            return AttributeChangedResult.NoVisualChanged;
        }
        #endregion

		#region ..Clone
		public override System.Xml.XmlNode CloneNode(bool deep)
		{
			this.OwnerDocument.isInImport = true;
            var clonenode = base.CloneNode(deep) as SVGElement;
			bool remove = this.RemoveID();
			if(this.ownerDocument.StartCopy && clonenode is YP.SVG.Interface.ISVGRenderer)
			{
				if(remove)
                    clonenode.InternalRemoveAttribute("id");
			}
			this.OwnerDocument.isInImport = false;
			return clonenode;
		}
		#endregion		

		#region IDisposable 成员
		public void Dispose()
		{
			// TODO:  添加 SVGElement.Dispose 实现
			GC.SuppressFinalize(this);
			GC.Collect(0);
		}
		#endregion

        #region ..RemoveID
        public virtual bool RemoveID()
		{
			return true;
		}
		#endregion

		#region ..OnIDChanged
		/// <summary>
		/// occurs when the id changed
		/// </summary>
		public virtual void OnIDChanged(string oldValue,string newValue)
		{
		}
		#endregion

		#region ..private fields for the ICustomTypeDescriptor
	    //public YP.SVG.Property.CustomPropertyCollection properties;
		#endregion

		#region ..UpdateElementAttribute
		public static bool UpdateElementAttribute(SVGElement element,string defaultvalue,string attributename,string attributeValue,ArrayList validNames)
		{
			bool change = false;
			string name = element.LocalName;

            //直接更新
			if(validNames.Contains(name))
			{
                string a = element.GetFinalAttribute(attributename);
				if(a.Length == 0)
					a = defaultvalue;
				if(a.ToLower() != attributeValue)
				{
                    element.SetFinalAttribute(attributename, attributeValue);
					
					if(element is Text.SVGTextElement &&(attributename=="font-size" || attributename=="font-family" || attributename=="font-weight" || attributename =="font-style" || attributename=="text-decoration"))
					{
						YP.SVG.Text.SVGTextElement text = element as YP.SVG.Text.SVGTextElement;

						#region ..刷新路径并更新属性
						element.OwnerDocument.BeginProcess();
						System.Xml.XmlNodeList childs = text.GetElementsByTagName("tspan");
						element.OwnerDocument.EndProcess();
						if(childs.Count > 0)
						{
							using(YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator(text.StyleContainer))
							{
                                float tempsize = text.TextRender.GetFontSize(sp, text);
                                FontFamily family = text.TextRender.GetFontFamily(sp);
                                StringFormat sf = text.TextRender.GetStringFormat(sp);
								//判断是否是竖直布局
								bool vert = (sf.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical;
								float lineheight = (float)family.GetLineSpacing(FontStyle.Regular) / (float)family.GetEmHeight(FontStyle.Regular) * tempsize;
								sp.BeginStyleContainer(text);
                                tempsize = text.TextRender.GetFontSize(sp, text);
                                family = text.TextRender.GetFontFamily(sp);
								float lineheight1 = (float)family.GetLineSpacing(FontStyle.Regular) / (float)family.GetEmHeight(FontStyle.Regular) * tempsize;

								#region ..如果行距发生改变，更新dx或dy以适应布局
								if(lineheight != lineheight1)
								{
									float scale = (float)lineheight1 / (float)lineheight;
									for(int j = 0;j<childs.Count;j++)
									{
										YP.SVG.Text.SVGTSpanElement tspan = childs[j] as YP.SVG.Text.SVGTSpanElement;
										if(tspan != null)
										{
											if(vert)
											{
												if(tspan.Dx.NumberOfItems > 0)
												{
													YP.SVG.DataType.SVGLengthList lengthlist = tspan.Dx as YP.SVG.DataType.SVGLengthList;
													YP.SVG.DataType.SVGLength[] lengths = lengthlist.GetSVGLengthes();
													for(int k =0;k<lengths.Length;k++)
													{
														float temp = lengths[k].Value;
														lengths[k] = new YP.SVG.DataType.SVGLength(temp * scale,lengths[k].ownerElement,lengths[k].direction);
													}
													string templength = new YP.SVG.DataType.SVGLengthList(lengths).ToString();
													tspan.SetSVGAttribute("dx",templength);
													tspan.InternalSetAttribute("dx",templength);
													templength = null;
												}
											}
											else
											{
												if(tspan.Dy.NumberOfItems > 0)
												{
													YP.SVG.DataType.SVGLengthList lengthlist = tspan.Dy as YP.SVG.DataType.SVGLengthList;
													YP.SVG.DataType.SVGLength[] lengths = lengthlist.GetSVGLengthes();
													for(int k =0;k<lengths.Length;k++)
													{
														float temp = lengths[k].Value;
														lengths[k] = new YP.SVG.DataType.SVGLength(temp * scale,lengths[k].ownerElement,lengths[k].direction);
													}
													string templength =  new YP.SVG.DataType.SVGLengthList(lengths).ToString();
													tspan.SetSVGAttribute("dy",templength);
													tspan.InternalSetAttribute("dy",templength);
													templength = null;
												}
											}
										}
									}
								}
								#endregion
							}
						}
						//(element as YP.SVGDom.Text.SVGTextElement).RefreshPath(g);
							
						#endregion

						change = true;
					}
					else 
						change = true;
				}
			}

            //更新子节点
            SVGTransformableElement group = element as SVGTransformableElement;
            if (group != null)
            {
                bool change1 = false;
                for (int i = 0; i < group.ChildElements.Count; i++)
                {
                    bool change3 = UpdateElementAttribute(group.ChildElements[i] as YP.SVG.SVGElement, defaultvalue, attributename, attributeValue, validNames);
                    change = change3 || change;
                    change1 = change1 || (change3 && group.ChildElements[i] is YP.SVG.Text.SVGTextElement);
                }
            }
            else 
			name = null;
			return change;
		}
		#endregion

		#region ..GetArrayString
		public string GetArrayString(float[] arrays)
		{
			if(arrays == null || arrays.Length == 0)
				return "none";
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<arrays.Length;i++)
				sb.Append(arrays[i].ToString()+" ");
			return sb.ToString().Trim();
		}
		#endregion

		#region ..RemoveChild
        /// <summary>
        /// 移除指定的子节点
        /// </summary>
        /// <param name="child">正在被移除的节点</param>
        /// <param name="recordOperation">如果产生撤消记录，设置为true，否则设置为false</param>
        /// <returns>已移除的节点</returns>
        public XmlNode RemoveChild(XmlNode child, bool recordOperation)
        {
            if (child == null)
                return child;
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            var child1 = this.InternalRemoveChild(child);
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
            return child1;
        }
		#endregion

        #region ..SetAttribute
        /// <summary>
        /// 设置具有指定名称的属性的值
        /// </summary>
        /// <param name="attributeName">要设置的属性名称</param>
        /// <param name="attributeValue">要设置的属性的值</param>
        /// <param name="recordOperation">如果产生撤消记录，设置为true，否则设置为false</param>
        public  void SetAttribute(string attributeName, string attributeValue, bool recordOperation)
        {
            if (attributeValue == null || attributeName == null)
                return;
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            this.InternalSetAttribute(attributeName, attributeValue);
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
        }
        #endregion

        #region ..RemoveAttribute
        /// <summary>
        /// 按名称移除属性
        /// </summary>
        /// <param name="attributeName">要移除的属性名称</param>
        /// <param name="recordOperation">如果产生撤消记录，设置为true，否则设置为false</param>
        public void RemoveAttribute(string attributeName, bool recordOperation)
        {
            if (attributeName == null)
                return;
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            this.InternalRemoveAttribute(attributeName);
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
        }
        #endregion

        #region ..RemoveAll
        /// <summary>
        /// 移除当前节点的所有指定属性和子级
        /// </summary>
        /// <param name="recordOperation">如果产生撤消记录，设置为true，否则设置为false</param>
        public void RemoveAll(bool recordOperation)
        {
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            this.InternalRemoveAll();
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
        }
        #endregion

        #region ..AppendChild
        public XmlElement AppendChild(XmlElement child, bool recordOperation)
        {
            if (child == null)
                return null;
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            XmlElement elm = child;
            if (child.OwnerDocument != this.OwnerDocument)
                elm = this.ownerDocument.ImportNode(child,true) as XmlElement;
            XmlElement result = this.InternalAppendChild(elm) as SVGElement;
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
            return result;
        }
        #endregion

        #region ..PrependChild
        public XmlElement PrependChild(XmlElement child, bool recordOperation)
        {
            if (child == null)
                return null;
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = recordOperation;
            XmlElement result = this.InternalPrependChild(child) as XmlElement;
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
            return result;
        }
        #endregion

        #region ..InDom
        /// <summary>
        /// 判断节点是否已经被加入到Dom树
        /// </summary>
        /// <returns></returns>
        public bool InDom()
        {
            if (ParentElement == null)
                return false;
            if (this == this.ownerDocument.RootElement)
                return true;
            return this.ParentElement.InDom();
        }
        #endregion

        #region ..GetFinalAttribute
        public virtual string GetFinalAttribute(string attributeName)
        {
            return this.GetAttribute(attributeName);
        }
        #endregion

        #region ..SetFinalAttribute
        public virtual void SetFinalAttribute(string attributeName,string attributeValue)
        {
            this.InternalSetAttribute(attributeName, attributeValue);
        }
        #endregion

        #region ..RemoveAllChild
        /// <summary>
        /// 清除当前节点的所有子级
        /// </summary>
        public void RemoveAllChild()
        {
            this.RemoveAllChild(false);
        }

        /// <summary>
        /// 清除当前节点的所有子级
        /// </summary>
        /// <param name="record">如果要记录撤消动作，设置为true，否则设置为false</param>
        public void RemoveAllChild(bool record)
        {
            bool old = this.ownerDocument.AcceptNodeChanged;
            this.ownerDocument.AcceptNodeChanged = record;
            while(this.HasChildNodes)
            {
                this.InternalRemoveChild(this.ChildNodes[0]);
            }
            this.ownerDocument.InvokeUndos();
            this.ownerDocument.AcceptNodeChanged = old;
        }
        #endregion

        #region ..XmlElement 操作节点、属性方法
        #region ..Internal 方法

        //以下方法用于调用XmlElement的基类方法而不触发改变动作，谨慎使用
        public XmlNode InternalRemoveChild(XmlNode oldChild)
        {
            return base.RemoveChild(oldChild);
        }

        public void InternalSetAttribute(string name, string value)
        {
            base.SetAttribute(name, value);
        }

        public XmlNode InternalInsertAfter(XmlNode newChild, XmlNode refChild)
        {
            return base.InsertAfter(newChild, refChild);
        }

        public XmlNode InternalInsertBefore(XmlNode newChild, XmlNode refChild)
        {
            return base.InsertBefore(newChild, refChild);
        }

        public XmlNode InternalPrependChild(XmlNode newChild)
        {
            return base.PrependChild(newChild);
        }

        public XmlNode InternalAppendChild(XmlNode newChild)
        {
            return base.AppendChild(newChild);
        }

        public void InternalRemoveAll()
        {
            base.RemoveAll();
        }

        public void InternalRemoveAllAttributes()
        {
            base.RemoveAllAttributes();
        }

        public void InternalRemoveAttribute(string localName, string namespaceURI)
        {
            base.RemoveAttribute(localName, namespaceURI);
        }

        public void InternalRemoveAttribute(string name)
        {
            base.RemoveAttribute(name);
        }

        public XmlNode InternalRemoveAttributeAt(int i)
        {
            return base.RemoveAttributeAt(i);
        }

        public XmlAttribute InternalRemoveAttributeNode(string localName, string namespaceURI)
        {
            return base.RemoveAttributeNode(localName, namespaceURI);
        }

        public XmlAttribute InternalRemoveAttributeNode(XmlAttribute oldAttr)
        {
            return base.RemoveAttributeNode(oldAttr);
        }

        public XmlNode InternalReplaceChild(XmlNode newChild, XmlNode oldChild)
        {
            return base.ReplaceChild(newChild, oldChild);
        }

        public string InternalSetAttribute(string localName, string namespaceURI, string value)
        {
            return base.SetAttribute(localName, namespaceURI, value);
        }

        public XmlAttribute InternalSetAttributeNode(string localName, string namespaceURI)
        {
            return base.SetAttributeNode(localName, namespaceURI);
        }

        public XmlAttribute InternalSetAttributeNode(XmlAttribute newAttr)
        {
            return base.SetAttributeNode(newAttr);
        }
        #endregion

        #region ..override
        public override XmlNode RemoveChild(XmlNode oldChild)
        {
            var result= base.RemoveChild(oldChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override void SetAttribute(string name, string value)
        {
            base.SetAttribute(name, value);
            this.ownerDocument.InvokeUndos();
        }

        public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
        {
            var result = base.InsertAfter(newChild, refChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
        {
            var result= base.InsertBefore(newChild, refChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlNode PrependChild(XmlNode newChild)
        {
            var result= base.PrependChild(newChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlNode AppendChild(XmlNode newChild)
        {
            var result= base.AppendChild(newChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
            this.ownerDocument.InvokeUndos();
        }

        public override void RemoveAllAttributes()
        {
            base.RemoveAllAttributes();
            this.ownerDocument.InvokeUndos();
        }

        public override void RemoveAttribute(string localName, string namespaceURI)
        {
            base.RemoveAttribute(localName, namespaceURI);
            this.ownerDocument.InvokeUndos();
        }

        public override void RemoveAttribute(string name)
        {
            base.RemoveAttribute(name);
        }

        public override XmlNode RemoveAttributeAt(int i)
        {
            var result= base.RemoveAttributeAt(i);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlAttribute RemoveAttributeNode(string localName, string namespaceURI)
        {
            var result= base.RemoveAttributeNode(localName, namespaceURI);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
        {
            var result= base.RemoveAttributeNode(oldAttr);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
        {
            var result= base.ReplaceChild(newChild, oldChild);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override string SetAttribute(string localName, string namespaceURI, string value)
        {
            var result= base.SetAttribute(localName, namespaceURI, value);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlAttribute SetAttributeNode(string localName, string namespaceURI)
        {
            var result= base.SetAttributeNode(localName, namespaceURI);
            this.ownerDocument.InvokeUndos();
            return result;
        }

        public override XmlAttribute SetAttributeNode(XmlAttribute newAttr)
        {
            var result= base.SetAttributeNode(newAttr);
            this.ownerDocument.InvokeUndos();
            return result;
        }
        #endregion
        #endregion

        #region ICustomTypeDescriptor 成员
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;//TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList props = new ArrayList();

            System.Collections.ArrayList customProperties = new ArrayList();
            if (ownerDocument.PropertyGenerator != null)
            {
                Property.IProperty[] properties = ownerDocument.PropertyGenerator.GeneratePropertiesForElement(this);
                if(properties != null)
                {
                    foreach(Property.IProperty pro in properties)
                    {
                        if (pro is Property.CustomPropertyItem)
                            customProperties.Add((pro as Property.CustomPropertyItem).ConvertToCustomProperty(this));
                    }
                }
            }

            foreach (Property.IProperty property in customProperties)
            {
                ArrayList attrs = new ArrayList();

                if (property.Category != null)
                    attrs.Add(new CategoryAttribute(property.Category));

                if (property.Description != null)
                    attrs.Add(new DescriptionAttribute(property.Description));

                attrs.Add(new ReadOnlyAttribute(property.IsReadOnly));

                if (property.EditorType != null)
                    attrs.Add(new EditorAttribute(property.EditorType.FullName, typeof(UITypeEditor)));

                if (property.ConverterTypeName != null)
                    attrs.Add(new TypeConverterAttribute(property.ConverterTypeName));

                Attribute[] attrArray = (Attribute[])attrs.ToArray(typeof(Attribute));

                Property.CustomPropertyDescriptor pd = new Property.CustomPropertyDescriptor(property, this, property.Name, attrArray);
                props.Add(pd);
            }

            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropertyDescriptor));
            return new PropertyDescriptorCollection(propArray);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

        #region ..操作Property
        #region ..getvalue for the custom property
        /// <summary>
        /// get the property value for the custom property
        /// </summary>
        /// <returns></returns>
        internal virtual object GetPropertyValue(string attributeName)
        {
            return this.GetAttribute(attributeName);
        }
        #endregion

        #region ..setvalue for the custom property
        /// <summary>
        /// set the value to the custom property
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        internal virtual void SetPropertyValue(string attributeName, object value)
        {
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = true;
            this.InternalSetAttribute(attributeName, value.ToString());
            this.OwnerDocument.AcceptNodeChanged = old;
            this.OwnerDocument.InvokeUndos();
        }
        #endregion
        #endregion
    }
}
