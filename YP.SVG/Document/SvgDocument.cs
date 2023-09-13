using System;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.Serialization;
using YP.SVG;

namespace YP.SVG.Document
{
	/// <summary>
	/// 实现SVG文档
	/// </summary>
	public class SVGDocument:System.Xml.XmlDocument,Document.ISVGDocument
	{
		#region ..静态变量
		public static string SvgNamespace = "http://www.w3.org/2000/svg";
        public static string XLinkNamespace = "http://www.w3.org/1999/xlink";
        public static string AudioNamespace = "http://ns.adobe.com/AdobeSVGViewerExtensions/3.0/";
        public static string PerfectSVGNamespace = "http://www.perfectsvg.com/perfectsvg";
        static public ArrayList SupportedFeatures = new ArrayList(new string[4]{
																				   "org.w3c.svg.static",
																				   "http://www.w3.org/TR/Svg11/feature#Shape",
																				   "http://www.w3.org/TR/Svg11/feature#BasicText",
																				   "http://www.w3.org/TR/Svg11/feature#OpacityAttribute"
																			   });
        static public ArrayList SupportedExtensions = new ArrayList();

		static Regex re = new Regex(@"(?<name>[a-z]+)=[""'](?<value>[^""']*)[""']");
		#endregion

		#region ..私有变量
		string url = string.Empty;
		Hashtable referencedFiles = new Hashtable();
		int currentTime1 = -1;
		/// <summary>
		/// 判断文档是否处于导入状态
		/// </summary>
		public bool inLoadProcess = false;
		SVGElementCollection styles = new SVGElementCollection();
		ArrayList externalStyles = new ArrayList();
		string baseUri = string.Empty;
		YP.SVG.SVGElementCollection viewChanges = new YP.SVG.SVGElementCollection();
		YP.SVG.SVGElement currentScene = null;
		System.Xml.XmlTextReader xmlReader = null;
		bool autoformat = true;
		Base.CSS.CSSStyleSheetList  cssStyleSheetList = new Base.CSS.CSSStyleSheetList();
		ArrayList undos = new ArrayList();
		bool inSelecting = false;
		bool isValid = true;
		System.Collections.Hashtable base64Source = new Hashtable();
		bool _stopRender = false;
		Hashtable refNodes = new Hashtable();

		bool styleChanged = false;
		YP.SVG.SVGElementCollection connects = new SVGElementCollection();
		bool inTransaction = false;
		/// <summary>
		/// stores the custom propertie
		/// </summary>
		Hashtable customProperties = new Hashtable();
		//Property.CustomPropertyCollection commonCustomProperties = new YP.SVG.Property.CustomPropertyCollection();
		//YP.Forms.PropertyCategory defaultProperties = YP.Forms.PropertyCategory.All;
		bool recordHistory = true;
		bool scaleStroke = false;
        bool enableCache = true;
        bool initializedConnectElement = false;
        SVGElementCollection redrawElements = new SVGElementCollection();
        List<SVGElement> tobeRefreshElements = new List<SVGElement>();
        List<SVGDocument> refDocuments = new List<SVGDocument>();
        Property.IPropertyGenerator propertyGenerator = null;
		#endregion

		#region ..保护字段
		/// <summary>
		/// 指定一个值,该值指示文档是否正在处于节点导入状态,如Clone或Import
		/// </summary>
		public bool isInImport = false;

		/// <summary>
		/// 指定一个值，该值指示文档当前是否正处于Use对象的构造
		/// </summary>
		public bool inUseCreate = false;

		//指示文档是否开始拷贝
		public bool StartCopy = false;

		//indicates whether has something changed
		public bool HasSomethingChanged = false;


		///// <summary>
		///// Define the operation users want to disable
		///// </summary>
		//public YP.Forms.Operator disabledOperator = YP.Forms.Operator.None;

		///// <summary>
		///// define the features users want to disable
		///// </summary>
		//public YP.Forms.DisabledFeatures disabledFeatures = YP.Forms.DisabledFeatures.None;
		#endregion

		#region ..场景属性
		/// <summary>
		/// 获取或设置当前编辑场景
		/// </summary>
        public YP.SVG.SVGElement CurrentScene
		{
			get
			{
				if(this.currentScene == null)
					this.currentScene = (YP.SVG.SVGElement)this.RootElement ;
				else if(this.currentScene.ParentNode == null)
					this.currentScene = (YP.SVG.SVGElement)this.RootElement ;
				return this.currentScene;
			}
			set
			{
                if (this.currentScene != value)
                {
                    this.currentScene = value;
                    this.OnCurrentSceneChanged();
                }
			}
		}
		#endregion

        #region ..事件
        /// <summary>
        /// 当对象被移出时发生
        /// </summary>
        public event SVGElementChangedEventHandler ElementRemoved;
        /// <summary>
        /// 当对象被插入文档时发生
        /// </summary>
        public event SVGElementChangedEventHandler ElementInserted;

        /// <summary>
        /// 当对象被改变时发生
        /// </summary>
        public event AttributeChangedEventHandler ElementChanged;
        /// <summary>
        /// 当连接发生变化时发生
        /// </summary>
        public event ConnectionChangedEventHandler ConnectionChanged;

		/// <summary>
		/// 当当前编辑场景发生改变时发生
		/// </summary>
        public event EventHandler CurrentSceneChanged;

		/// <summary>
		/// Occurs when the css style in the document changed
		/// </summary>
        public event EventHandler CSSChanged;
		#endregion

		#region ..公共字段
        public XmlNamespaceManager NamespaceManager;
        public XmlParserContext XmlParserContext = null;
		/// <summary>
		/// 记录当场景改变时，是否显示主场景
		/// </summary>
        public bool OnlyShowCurrentScene = false;

		/// <summary>
		/// 控制文档是否创建细节信息，包括文本，位置等。
		/// </summary>
		public bool createDetail = true;
		#endregion

		#region ..public properties
        /// <summary>
        /// gets or sets a value to enable the cache
        /// </summary>
        public bool EnableCache
        {
            get
            {
                return this.enableCache;
            }
            set
            {
                this.enableCache = value;
            }
        }

        /// <summary>
        /// set the custom property generator if need
        /// </summary>
        public Property.IPropertyGenerator PropertyGenerator
        {
            set
            {
                this.propertyGenerator = value;
            }
            internal get
            {
                return this.propertyGenerator;
            }
        }

		/// <summary>
		/// determine whether 
		/// </summary>
		public bool UseDefaultConnectPoint
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region ..构造及消除
        /// <summary>
        /// 从指定的文件中导入SVG文档
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SVGDocument Create(string filePath)
        {
            return SvgDocumentFactory.CreateDocumentFromFile(filePath);
        }

        /// <summary>
        /// 创建SVG文档，文档Root节点具备指定的Size
        /// </summary>
        /// <param name="size">定义SVGRoot节点的Size</param>
        /// <returns>新的SVGDocument对象</returns>
        public static SVG.Document.SVGDocument CreateDocumentWithSize(Size size, string template)
        {
            return CreateDocumentWithSize(new DataType.SVGLength(size.Width), new DataType.SVGLength(size.Height), template);
        }

        /// <summary>
        /// 创建SVG文档，文档Root节点具备指定的Size
        /// </summary>
        /// <param name="width">定义SVGRoot节点的Width</param>
        /// <param name="height">定义SVGRoot节点的Height</param>
        /// <returns>新的SVGDocument对象</returns>
        public static SVG.Document.SVGDocument CreateDocumentWithSize(DataType.SVGLength width, DataType.SVGLength height,String template)
        {
            template = template.Replace("{width}", width.ValueAsString).Replace("{height}", height.ValueAsString);
            return YP.SVG.Document.SvgDocumentFactory.CreateDocument(template);
        }

        /// <summary>
        /// 构造新的SVGDocument对象
        /// </summary>
		public SVGDocument()
		{
			this.NamespaceManager = new XmlNamespaceManager(this.NameTable);
			this.NamespaceManager.PushScope();
			this.NamespaceManager.AddNamespace(string.Empty, SVGDocument.SvgNamespace); 
			this.NamespaceManager.AddNamespace("xlink", SVGDocument.XLinkNamespace);
			this.NamespaceManager.AddNamespace("a",SVGDocument.AudioNamespace); 
			this.NamespaceManager.AddNamespace("perfectsvg",SVGDocument.PerfectSVGNamespace);
			this.NodeChanged += new System.Xml.XmlNodeChangedEventHandler(ChangeNode);
            this.NodeInserted += new XmlNodeChangedEventHandler(SVGDocument_NodeInserted);
			this.NodeRemoved += new System.Xml.XmlNodeChangedEventHandler(ChangeNode);
			this.NodeRemoving += new XmlNodeChangedEventHandler(SvgDocument_NodeRemoving);
			this.XmlParserContext = new XmlParserContext(this.NameTable,this.NamespaceManager,"","svg","-//W3C//DTD SVG 1.1//EN","http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd",this.BaseURI,"zh",XmlSpace.None,System.Text.Encoding.UTF8);
			this.selectCollection.CollectionChanged += new CollectionChangedEventHandler(ChangeSelect);
			this.selectCollection.NotifyEvent = true;
			this.PreserveWhitespace = false;
			this.baseUri = System.IO.Path.GetTempPath();
		}

       
		#endregion

		#region ..通用文档属性
		#region ..私有变量
		YP.SVG.SVGElementCollection selectCollection = new YP.SVG.SVGElementCollection();
		bool acceptNodeChanged = false;
		UndoStack undoStack ;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取文档的CSS列表
		/// </summary>
        public Base.StyleSheets.IStyleSheetList CSSStyleSheetList
		{
			get
			{
				return this.cssStyleSheetList;
			}
		}

        public string Title { set; get; }

		/// <summary>
		/// 获取或设置文档是否处理节点改变事件,将该事件计入Undo Stack
		/// </summary>
        public bool AcceptNodeChanged
		{
			set
			{
				this.acceptNodeChanged = value;
			}
			get
			{
				return this.acceptNodeChanged;
			}
        }

		/// <summary>
		/// 获取文档的选择元素集合。
		/// 在文档中，操作对象往往通过集合形式出现，通过SelectCollection来操作文档的选择元素。
		/// </summary>
		public SVGElementCollection SelectCollection
		{
			get
			{
				return selectCollection;
			}
        }

		/// <summary>
		/// 判断文档是否处于撤销重作状态之中
		/// </summary>
        public bool InRedoUndo
		{
			get
			{
				return !this.undoStack.AcceptChanges;
			}
		}
		#endregion

		#region ..事件
		/// <summary>
		/// 当文档出错时发生
		/// </summary>
		public event ExceptionOccuredEventHandler ExceptionOccured;

        /// <summary>
        /// 当文档的选集发生改变时，触发
        /// </summary>
        public event CollectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// 当文档在某个过程中有节点对象发生改变时发生
        /// </summary>
        public event CollectionChangedEventHandler ElementsChanged;

		/// <summary>
		/// 当文档导入时发生
		/// </summary>
		public event EventHandler Loaded;
		#endregion

		#region ..公共方法
		/// <summary>
		/// 取消关于文档的任何修改，将文档返回到刚创建时的状态
		/// </summary>
        public virtual void Revert()
		{
		}

        /// <summary>
        /// 当文档发生修改时，重复执行一次当前操作
        /// </summary>
        public virtual void Redo()
		{
			bool old = this.acceptNodeChanged;
			this.acceptNodeChanged = false;
			this.undoStack.Redo();
			this.acceptNodeChanged = true;
			this.undos.Clear();
            this.ForceRefreshAllElementToBeUpdated();
		}

        /// <summary>
        /// 取消文档上一步操作
        /// </summary>
        public virtual void Undo()
		{
			bool old = this.acceptNodeChanged;
			this.acceptNodeChanged = false;
			this.undoStack.Undo();
            this.acceptNodeChanged = old;
			this.undos.Clear();
            this.ForceRefreshAllElementToBeUpdated();
		}

        /// <summary>
        /// 清除所有重作记录
        /// </summary>
        public void ClearUndos()
		{
			bool old = this.acceptNodeChanged;
			this.acceptNodeChanged = false;
			this.undoStack.ClearAll();
			this.acceptNodeChanged = old;
            
		}
        #endregion

        #region ..清除选区
        /// <summary>
        /// 清除选区
        /// </summary>
        public void ClearSelects()
		{
			this.ChangeSelectElement(null);
		}
		#endregion

		#region ..改变选择
		void ChangeSelect(object sender,CollectionChangedEventArgs e)
		{
			if(this.SelectionChanged != null)
				this.SelectionChanged(this,e);
		}
        #endregion

        #region ..添加选择对象
        /// <summary>
        /// 选择指定对象
        /// </summary>
        /// <param name="element"></param>
        public void ChangeSelectElement(YP.SVG.SVGElement element)
		{
			this.selectCollection.NotifyEvent = element == null;
			this.selectCollection.Clear();
			this.selectCollection.NotifyEvent = true;
			if(element != null && !this.selectCollection.Contains(element))
				this.selectCollection.Add(element);
		}

        public void ChangeSelectElements(YP.SVG.SVGElementCollection elements)
		{
			this.selectCollection.NotifyEvent = elements == null;
			this.selectCollection.Clear();
			if(elements != null)
			{
				this.selectCollection.NotifyEvent = true;
				this.selectCollection.AddRange(elements);
			}
		}

        public void ChangeSelectElements(YP.SVG.SVGElement[] elements)
		{
			this.selectCollection.NotifyEvent = true;
			this.selectCollection.Clear();
			if(elements != null)
			{
				this.selectCollection.NotifyEvent = true;
				this.selectCollection.AddRange(elements);
			}
		}
        #endregion

        #region ..将当前动作压入堆栈
        /// <summary>
        /// 将重作动作压入堆栈
        /// </summary>
        /// <param name="undos">重作动作</param>
        public void PushUndo(Interface.IUndoOperation undo)	
		{
			if(this.recordHistory && this.acceptNodeChanged && this.undoStack.AcceptChanges)
				this.undos.Insert(0,undo);
		}
        #endregion

        #region ..将重作动作压入堆栈
        /// <summary>
        /// 将重作动作压入堆栈
        /// </summary>
        /// <param name="undos">重作动作</param>
        public void PushUndos(Interface.IUndoOperation[] undos)	
		{
			if(this.recordHistory && this.acceptNodeChanged && this.undoStack.AcceptChanges)
				this.undos.InsertRange(0,undos);
		}
		#endregion

		#region ..Load事件
		public virtual void OnLoaded()
		{
			if(this.Loaded != null)
				this.Loaded(this,EventArgs.Empty);
		}
		#endregion

		#region ..出错事件
		public virtual void OnExceptionOccured(ExceptionOccuredEventArgs e)
		{
			if(this.ExceptionOccured != null)
				this.ExceptionOccured(this,e);
		}
		#endregion

		#endregion

		#region ..属性
        bool notCauseEvent
        {
            get
            {
                //return (!this.doc.AcceptNodeChanged && !this._inUndoRedo) || this.doc.inLoadProcess || this.doc.isInImport || this.doc.inUseCreate;
                return this.inLoadProcess || this.isInImport || this.inUseCreate;
            }
        }

        public string DemoString
        {
            get
            {
                //YP.Base.License.License license = this.license as YP.Base.License.License;
                //if (license != null && license.Days > Base.License.License.ValidDays)
                //    return string.Empty;//
                return string.Empty;// "VectorControl.Net 3.0\n仅作为试用及演示\n版权所有(c)2005-2014 YP.Com";
            }
        }

        /// <summary>
        /// 判断文档是否处于被激活状态
        /// 所谓激活状态是指此时文档中的对象是否可以被加入绘制序列
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !this.inLoadProcess && !this.inUseCreate && !this.isInImport;
            }
        }

		/// <summary>
		/// 获取或设置当前时间（以毫秒为单位）
		/// </summary>
        public int CurrentTime
		{
			set
			{
				value = (int)Math.Max(0,value);
				if(this.currentTime1 != value)
				{
					this.currentTime1 = value;
				}
			}
			get
			{
				return this.currentTime1;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示绘制时线条是否随着Transform值进行变换
		/// </summary>
		public bool ScaleStroke
		{
			set
			{
				this.scaleStroke = value;
			}
			get
			{
				return this.scaleStroke;
			}
        }

		/// <summary>
		/// gets or set a value which indicates which category should be created automatically
		/// </summary>
  //      public YP.Forms.PropertyCategory DefaultProperties
		//{
		//	set
		//	{
		//		this.defaultProperties = value;
		//	}
		//	get
		//	{
		//		return this.defaultProperties;
		//	}
		//}

		/// <summary>
		/// gets a value indicates whether the document is in transaction
		/// </summary>
		public bool InTransaction
		{
			get
			{
				return this.inTransaction;
			}
		}

		/// <summary>
		/// Sets or get the style changed status of the documment
		/// </summary>
        public bool CSSStyleChanged
		{
			set
			{
				if(this.styleChanged != value)
				{
					this.styleChanged = value;
					this.OnCSSChanged();
				}
			}
			get
			{
				return this.styleChanged;
			}
		}

		/// <summary>
		/// 判断当前的绘制状态
		/// </summary>
		public bool IsStopRender
		{
			get
			{
				return this._stopRender;
			}
		}

		/// <summary>
		/// 判断当前文档是否有效
		/// </summary>
        public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示文档当前正在处理XPath选择事件，在此时，节点改变事件将被忽略
		/// </summary>
        public bool InXPathSelecting
		{
			get
			{
				return this.inSelecting;
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示文档在操作过程中，是否创建诸如位置等些界信息
		/// </summary>
        public bool CreateDetail
		{
			set
			{
				this.createDetail = value;
			}
			get
			{
				return this.createDetail;
			}
        }

		/// <summary>
		/// 获取指向本文档的文档地址
		/// </summary>
		public string Referrer
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// 获取文档的域
		/// </summary>
		public string Domain
		{
			get
			{
				Regex re = new Regex(@"[http|https]://(^/)+");
				string ret;
				if(re.IsMatch(Url))
				{
					Match match = re.Match(Url);
					ret = match.Groups[1].Value;
				}
				else ret = "";

				return ret;
			}
		}

		/// <summary>
		/// 获取文档的地址
		/// </summary>
		public string Url
		{
			get
			{
				return this.url;
			}
		}

		/// <summary>
		/// 获取SVG文档的根对象，一般为第一层的SVG对象
		/// </summary>
		public DocumentStructure.SVGSVGElement RootElement
		{
			get
			{
				System.Xml.XmlElement root = this.DocumentElement;
                if (root is DocumentStructure.SVGSVGElement)
                    return (DocumentStructure.SVGSVGElement)this.DocumentElement;
				return null;
			}
		}
		#endregion

		#region ..寻找和指定的ID号匹配的节点
		public System.Xml.XmlElement GetReferencedNode(string refid)
		{
			return this.GetReferencedNode(refid,null as string[], null);
		}

        public System.Xml.XmlElement GetReferencedNode(string refid, string[] tagnames)
        {
            return GetReferencedNode(refid, tagnames, null);
        }

		/// <summary>
		/// 寻找和指定的ID号匹配的节点
		/// </summary>
		/// <param name="id">需要匹配的ID号</param>
		/// <returns></returns>
        public System.Xml.XmlElement GetReferencedNode(string refid, string[] tagnames, SVGElement ownerElement)
		{
			string hash = refid;

			if(refid.StartsWith("#"))
			{
				hash = refid.Substring(1);
			}
            //if(this.refNodes.ContainsKey(hash))
            //{
            //    YP.SVG.SVGElement element = this.refNodes[hash] as YP.SVG.SVGElement;
            //    if((element != null && element.ID == hash && element.ParentNode != null))
            //        return element;
            //    else
            //        this.refNodes.Remove(hash);
            //}
			bool old = this.acceptNodeChanged;
			this.BeginProcess();
			XmlElement refElm = null;
            string pattern = string.Empty;
			if(tagnames == null || tagnames.Length == 0)
                pattern = "descendant::*[@id='" + hash + "']";
			else
			{
                System.Text.StringBuilder sb = new System.Text.StringBuilder("descendant::*[");
				if(tagnames.Length > 1)
					sb.Append("(");
				for(int i = 0;i<tagnames.Length;i++)
				{
					if(i > 0)
						sb.Append(" or ");
					sb.Append("local-name(.) ='" + tagnames[i] +"'");
				}
				if(tagnames.Length > 1)
					sb.Append(")");
				sb.Append (" and @id = '");
				sb.Append(hash);
				sb.Append("']");
                pattern = sb.ToString();
				sb = null;
			}
            XmlElement root = ownerElement;
            if (root == null)
                root = this.DocumentElement;
            refElm = root.SelectSingleNode(pattern) as System.Xml.XmlElement;
			this.EndProcess();
			this.acceptNodeChanged = old;
            this.refNodes[hash] = refElm;

            //if (refElm == null)
            //{
            //    foreach (var doc in this.refDocuments)
            //    {
            //        refElm = doc.GetReferencedNode(refid);
            //        if (refElm != null)
            //            break;
            //    }
            //}
			hash = null;
			return refElm;
		}

		/// <summary>
		/// 寻找和指定的ID号匹配的节点，并且该节点以指定节点位父级节点
		/// </summary>
		/// <param name="refid">需要匹配的ID号</param>
		/// <param name="element">父级对象</param>
		/// <returns></returns>
        public System.Xml.XmlNode GetReferencedNode(string refid, YP.SVG.SVGElement element)
		{
			refid = refid.Trim();
			while(refid.EndsWith(";"))
				refid = refid.Substring(0,refid.Length - 1);
			string id = refid;
			if(id.Trim().StartsWith("url"))
			{
				int start = id.Trim().IndexOf("#",0,id.Trim().Length);
				int end = id.Trim().IndexOf(")",0,id.Trim().Length);
				id= id.Trim().Substring(start+1,end - start - 1);
			}
			if(id.Trim().StartsWith("#"))
				id = id.Trim().Substring(1,id.Trim().Length - 1);
			if(element.ID == id)
				return element;
			string xpath = "//" + element.Name + "[@id=\""+id+"\"]";
			bool old = element.OwnerDocument.acceptNodeChanged;
			element.OwnerDocument.acceptNodeChanged = false;
			this.BeginProcess();
			System.Xml.XmlNode node = element.SelectSingleNode(xpath);
			this.EndProcess();
			element.OwnerDocument.acceptNodeChanged = old;
			return node;
		}
		#endregion

		#region ..寻找和指定路径匹配的文档
		/// <summary>
		/// 寻找和指定路径匹配的文档
		/// </summary>
		public System.IO.Stream GetReferencedFile(Uri uri)
		{
			string absuri = StreamHelper.GetAbsolutPath(uri);//GetAbsolutPath(uri);
			if(referencedFiles.Contains(absuri))
			{
				return (System.IO.Stream)referencedFiles[absuri];
			}
			else
			{
				System.IO.Stream stream = StreamHelper.GetStream(uri);
				referencedFiles[absuri] = stream;
				return stream;
			}
		}
		#endregion

		#region ..获取资源流
		public System.IO.Stream GetSourceStream(string base64SourceStr)
		{
			try
			{
				if(!this.base64Source.ContainsKey(base64SourceStr))
					this.base64Source[base64SourceStr] = new System.IO.MemoryStream(System.Convert.FromBase64String(base64SourceStr));
			}
			catch{}
			return this.base64Source[base64SourceStr] as System.IO.Stream;
		}
		#endregion

		#region ..override CreateProcessingInstruction
		public override System.Xml.XmlProcessingInstruction CreateProcessingInstruction(string target,string data)
		{
			System.Xml.XmlProcessingInstruction pi = base.CreateProcessingInstruction(target,data);

			#region ..处理样式
			if(Regex.IsMatch(pi.Data,"type=[\"']text\\/css[\"']"))
			{
				Regex re = new Regex(@"(?<name>[a-z]+)=[""'](?<value>[^""']*)[""']");
				Match match = re.Match(pi.Data);
				Uri uri = null;
				string type = string.Empty;
				string title = string.Empty;
				while(match.Success)
				{
					string name = match.Groups["name"].Value;
					string val = match.Groups["value"].Value;

					try
					{
						switch(name)
						{
							case "href":
								Uri ruri = new Uri(this.BaseURI);
								uri = new Uri(ruri,val);
								break;
							case "type":
								type = val;
								break;
							case "title":
								title = val;
								break;
							case "media":
								break;
						}
					}
					catch 
					{
						match = match.NextMatch();
						continue;
					}
					match = match.NextMatch();
				}

				if(uri != null)
					this.cssStyleSheetList.Add(uri,type,title);
			}
			#endregion

			return pi;
		}
		#endregion

		#region ..override CreateElement
        /// <summary>
        /// 创建具有指定文本的 System.Xml.XmlText
        /// </summary>
        /// <param name="text">Text 节点的文本</param>
        /// <returns>新的 XmlText 节点</returns>
        public override XmlText CreateTextNode(string text)
        {
            return new SVGTextNode(text, this);
        }

        /// <summary>
        /// 创建具有指定 System.Xml.XmlNode.Prefix、System.Xml.XmlDocument.LocalName 和 System.Xml.XmlNode.NamespaceURI的元素。
        /// </summary>
        /// <param name="prefix">新元素的前缀（如果有的话）。String.Empty 与 null 等效</param>
        /// <param name="localName">新元素的本地名称</param>
        /// <param name="ns">新元素的命名空间 URI（如果有的话）。String.Empty 与 null 等效</param>
        /// <returns>新的 System.Xml.XmlElement</returns>
		public override XmlElement CreateElement(string prefix, string localName, string ns)
		{
			SVGElement elm = null;
			switch(localName)
			{
				case "clipPath":
					elm = new ClipAndMask.SVGClipPathElement(prefix, localName, ns, this);
					break;
				case "rect":
						elm = new BasicShapes.SVGRectElement(prefix, localName, ns, this);
					break;
				case  "path":
					elm = new Paths.SVGPathElement(prefix, localName, ns, this);
					break;
				case "polyline":
					elm = new BasicShapes.SVGPolylineElement(prefix, localName, ns, this);
					break;
				case  "polygon":
					elm = new BasicShapes.SVGPolygonElement(prefix, localName, ns, this);
					break;
				case "circle":
					elm = new BasicShapes.SVGCircleElement(prefix, localName, ns, this);
					break;
				case "ellipse":
					elm = new BasicShapes.SVGEllipseElement (prefix, localName, ns, this);
					break;
				case "marker":
					elm = new ClipAndMask.SVGMarkerElement(prefix,localName,ns,this);
					break;
				case "line":
					elm = new BasicShapes.SVGLineElement(prefix, localName, ns, this);
					break;
				case "g":
					elm = new DocumentStructure.SVGGElement(prefix, localName, ns, this);
					break;
				case "svg":
					elm = new DocumentStructure.SVGSVGElement(prefix, localName, ns, this);
					break;
				case "text":
					elm = new Text.SVGTextElement(prefix, localName, ns, this);
					break;
                case"textBlock":
                    elm = new Text.SVGTextBlockElement(prefix, localName, ns, this);
                    break;
				case "tspan":
					elm = new Text.SVGTSpanElement(prefix, localName, ns, this);
					break;
				case "tref":
					elm = new Text.SVGTRefElement(prefix, localName, ns, this);
					break;
				case "linearGradient":
					elm = new GradientsAndPatterns.SVGLinearGradientElement(prefix, localName, ns, this);
					break;
				case "radialGradient":
					elm = new GradientsAndPatterns.SVGRadialGradientElement(prefix, localName, ns, this);
					break;
				case "stop":
					elm = new GradientsAndPatterns.SVGStopElement(prefix, localName, ns, this);
					break;
				case "symbol":
					elm = new DocumentStructure.SVGSymbolElement (prefix,localName,ns,this);
					break;
				case "image":
					elm = new DocumentStructure.SVGImageElement(prefix, localName, ns, this);
					break;
				case "a":
					elm = new DocumentStructure.SVGGElement(prefix, localName, ns, this);
					break;
				case "use":
					elm = new DocumentStructure.SVGUseElement(prefix, localName, ns, this);
					break;
//				case "desc":
//					elm = new DocumentStructure.SVGDescElement(prefix,localName,ns,this);
//					break;
//				case "title":
//					elm = new DocumentStructure.SVGTitleElement(prefix,localName,ns,this);
//					break;
//				case "switch":
//					elm = new DocumentStructure.SVGSwitchElement(prefix,localName,ns,this);
//					break;
//				case "mpath":
//					elm = new Animation.SVGMPathElement(prefix,localName,ns,this);
//					break;
//				case "animate":
//					elm = new Animation.SVGAnimateElement(prefix, localName, ns, this);
//					break;
//				case "set":
//					elm = new Animation.SVGSetElement (prefix, localName, ns, this);
//					break;
//				case "animateColor":
//					elm = new Animation.SVGAnimateColorElement(prefix, localName, ns, this);
//					break;
//				case "animateMotion":
//					elm = new Animation.SVGAnimateMotionElement(prefix, localName, ns, this);
//					break;
//				case "animateTransform":
//					elm = new Animation.SVGAnimateTransformElement(prefix, localName, ns, this);
//					break;
				case "pattern":
					elm = new GradientsAndPatterns.SVGPatternElement(prefix,localName,ns,this);
					break;	
				case "style":
					elm = new DocumentStructure.SVGStyleElement(prefix,localName,ns,this);
					break;
				case "connect":
                case "connection":
					elm = new BasicShapes.SVGConnectionElement(prefix,localName,ns,this);
					break;
                case "branch":
                    elm = new BasicShapes.SVGBranchElement(prefix, localName, ns, this);
                    break;
				default:
					elm = new SVGElement(prefix,localName,ns,this);
					break;
			}
			return elm;
		}
		#endregion

		#region ..override CreateAttribute
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new SVGAttribute(prefix,localName,namespaceURI,this);
		}
 
		#endregion

		#region ..OnCurrentSceneChanged
        public virtual void OnCurrentSceneChanged()
        {
            this.selectCollection.Clear();
            if (this.CurrentSceneChanged != null)
                this.CurrentSceneChanged(this, EventArgs.Empty);
        }
		#endregion

		#region ..导入
        #if PERF
        public DateTime FinishLoadTime = DateTime.Now;
#endif
		public override void Load(System.Xml.XmlReader reader)
		{
            DateTime dt = DateTime.Now;
			this.ChangeSelectElements(null as SVGElementCollection);
			this.acceptNodeChanged = false;
            this.styles.Clear();
			this.externalStyles.Clear();
			this.currentScene= null;
			this.XmlResolver = null;
			this.PreserveWhitespace = false;
			this.inLoadProcess = true;
			this.isValid = true;
			this.refNodes.Clear();
			if(reader is System.Xml.XmlTextReader)
			{
				this.xmlReader = (System.Xml.XmlTextReader)reader;
			}
			try
			{
				base.Load(reader);
				this.inLoadProcess = false;
				this.undoStack = null;
				this.undoStack = new UndoStack(this);
				this.acceptNodeChanged = true; 

                //初始化Root
                if (this.RootElement != null)
                    this.RootElement.InitializeGPath(true);
				this.OnLoaded();

#if PERF
                this.FinishLoadTime = DateTime.Now;
                TimeSpan ts = this.FinishLoadTime - dt;
                Console.WriteLine("Load Spend:" + ts.TotalMilliseconds.ToString());
#endif
			}
			catch(Exception e)
			{
				this.isValid = false;
				this.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[]{e.Message},ExceptionLevel.High));
				throw e;
			}
		}

		public override void LoadXml(string svgfragment)
		{
			System.Xml.XmlTextReader reader = new XmlTextReader(svgfragment,XmlNodeType.Document,this.XmlParserContext);
			reader.XmlResolver = null;
			this.Load(reader);
			reader.Close();
		}

		public override void Load(string filename)
		{
			//正常化filename
			
			System.IO.Stream stream = null;
			System.Xml.XmlTextReader xmlreader = null;
			try
			{
				Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
				uri = new Uri(uri,filename);
				stream = StreamHelper.GetStream(uri);// System.IO.File.Open(filename,System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite,System.IO.FileShare.ReadWrite);//GZipXmlTextReader.GetUnzippedStream(filename);
                xmlreader = new System.Xml.XmlTextReader(stream);//,XmlNodeType.Document,this.XmlParserContext);//new YP.SVGDom.Document.GZipXmlTextReader(filename);
				xmlreader.XmlResolver = null;
				this.baseUri = filename;
				this.Load(xmlreader);
			}
//			catch{}
			finally
			{
				if(stream != null)
					stream.Close();
				if(xmlreader != null)
					xmlreader.Close();
			}
		}
		#endregion

        #region ..SVGDocument_NodeInserted
        void SVGDocument_NodeInserted(object sender, XmlNodeChangedEventArgs e)
        {
            if (this.disposing || this.InXPathSelecting || this.inExporting)
                return;

            SVGElement oldParent = null;
            SVGElement newParent = null;
            if (e.OldParent is SVGElement)
                oldParent = (SVGElement)e.OldParent;
            if (e.NewParent is SVGElement)
                newParent = (SVGElement)e.NewParent;

            this.HasSomethingChanged = true;

            #region ..改变属性
            if (e.Node is YP.SVG.SVGAttribute)
            {
                System.Xml.XmlAttribute attr = (System.Xml.XmlAttribute)e.Node;
                if (newParent != null)
                    newParent.AddSVGAttribute(attr.Name, attr.Value);
            }
            else if (e.Node is XmlText && (e.NewParent is SVGAttribute || e.OldParent is SVGAttribute))
            {
                SVGElement elm = (e.NewParent as SVGAttribute).OwnerElement as SVGElement;
                if (elm != null)
                    elm.AddSVGAttribute(e.NewParent.Name, e.Node.Value);
            }
            #endregion

            #region ..动画
            //				else if(e.Node is Animation.SVGAnimationElement && (this.inLoadProcess || this.attachAnimate))
            //					if(e.Action == System.Xml.XmlNodeChangedAction.Insert )
            //						this.animateList.Add((YP.SVGDom.SVGElement)e.Node);
            #endregion

            if (this.inSelecting || this.InXPathSelecting)
            {
                return;
            }

            #region ..Style节点
            if (e.Node is YP.SVG.DocumentStructure.SVGStyleElement)
            {
                this.styles.Add((YP.SVG.SVGElement)e.Node);
            }
            #endregion

            #region ..SVGElement
            if (e.Node is YP.SVG.SVGElement)
                    if (e.NewParent is Interface.ISVGContainer)
                        YP.SVG.SVGContainerElement.AddChildElement((Interface.ISVGContainer)e.NewParent, (Interface.ISVGElement)e.Node);

            if (this.isInImport)
            {
                return;
            }

            #endregion

            #region ..Connect
            if (e.Node is SVG.BasicShapes.SVGConnectionElement)
                this.initializedConnectElement = false;
            #endregion


            #region ..撤销重作，同步代码
            if (!this.inLoadProcess && this.createDetail)
            {
                System.Xml.XmlNode changeNode = e.Node;
                XmlNode newParent1 = e.NewParent;
                XmlNode oldParent1 = e.OldParent;
                if (!this.acceptNodeChanged)
                {
                    this.ParseNodeChangeInformation(e);
                    return;
                }
                //删除的动作记录放在Removing中处理，以记录层次关系

                System.Xml.XmlNodeChangedAction action = e.Action;
                YP.SVG.SVGElement changeparent = null;
                int count = this.undos.Count;
                bool isAttribute = false;
                if (e.Node is System.Xml.XmlAttribute)
                {
                    changeparent = (e.Node as XmlAttribute).OwnerElement as SVGElement;
                    isAttribute = true;
                }
                else if (e.Node is XmlText && e.NewParent is XmlAttribute)
                {
                    changeparent = (e.NewParent as XmlAttribute).OwnerElement as SVGElement;
                    isAttribute = true;
                }
                //撤销重做，当编辑Text对象时，不记录
                if (!(e.Node is XmlText && e.NewParent is YP.SVG.Text.SVGTextElement) && !isAttribute || changeparent != null)
                {
                    YP.SVG.Undo.NodeUndoOperation undo = new YP.SVG.Undo.NodeUndoOperation(this, changeNode, action, oldParent1, newParent1, changeparent, e.OldValue);
                    this.PushUndo(undo);
                }
            }

            #endregion

            ParseNodeChangeInformation(e);
        }
        #endregion

        #region ..ChangeNode
        void ChangeNode(object sender,System.Xml.XmlNodeChangedEventArgs e)
		{
            if (this.disposing || this.InXPathSelecting || this.inExporting)
				return;

			SVGElement oldParent = null;
            SVGElement newParent = null;
            if (e.OldParent is SVGElement)
                oldParent = (SVGElement)e.OldParent;
            if (e.NewParent is SVGElement)
                newParent = (SVGElement)e.NewParent;
			
			this.HasSomethingChanged = true;
//			if(!this.isInImport)
//			{
				#region ..改变属性
				if(e.Node is YP.SVG.SVGAttribute)
				{
					System.Xml.XmlAttribute attr = (System.Xml.XmlAttribute)e.Node;
					if((e.Action == XmlNodeChangedAction.Insert || e.Action == XmlNodeChangedAction.Change) && newParent != null)
                        newParent.AddSVGAttribute(attr.Name, attr.Value);
					if(e.Action == XmlNodeChangedAction.Remove && oldParent != null)
                        oldParent.AddSVGAttribute(attr.Name, string.Empty);
				}
                else if (e.Node is XmlText && (e.NewParent is SVGAttribute || e.OldParent is SVGAttribute))
                {
                    if (e.Action == XmlNodeChangedAction.Insert || e.Action == XmlNodeChangedAction.Change)
                    {
                        SVGElement elm = (e.NewParent as SVGAttribute).OwnerElement as SVGElement;
                        if (elm != null)
                            elm.AddSVGAttribute(e.NewParent.Name, e.Node.Value);
                    }
                    else if(e.Action == XmlNodeChangedAction.Remove)
                    {
                        SVGElement elm = (e.OldParent as SVGAttribute).OwnerElement as SVGElement;
                        if (elm != null)
                            elm.AddSVGAttribute(e.OldParent.Name, string.Empty);
                    }
                }
				#endregion

					#region ..动画
//				else if(e.Node is Animation.SVGAnimationElement && (this.inLoadProcess || this.attachAnimate))
//					if(e.Action == System.Xml.XmlNodeChangedAction.Insert )
//						this.animateList.Add((YP.SVGDom.SVGElement)e.Node);
				#endregion
//			}

			if(this.inSelecting || this.InXPathSelecting)
			{
				return;
			}

            #region ..Style节点
            if (e.Node is YP.SVG.DocumentStructure.SVGStyleElement)
            {
                if (e.Action == System.Xml.XmlNodeChangedAction.Insert)
                {
                    this.styles.Add((YP.SVG.SVGElement)e.Node);
                }
                else if (e.Action == System.Xml.XmlNodeChangedAction.Remove)
                {
                    this.styles.Remove((YP.SVG.SVGElement)e.Node);
                }
            }
            #endregion

			#region ..SVGElement
			if(e.Node is YP.SVG.SVGElement)
			{
				if(e.Action == System.Xml.XmlNodeChangedAction.Insert)
				{
					if(e.NewParent is Interface.ISVGContainer)
						YP.SVG.SVGContainerElement.AddChildElement((Interface.ISVGContainer)e.NewParent,(Interface.ISVGElement)e.Node);
				}
				else if(e.Action == System.Xml.XmlNodeChangedAction.Remove )
				{
					if(this.selectCollection.Contains((YP.SVG.SVGElement)e.Node))
						this.selectCollection.Remove((YP.SVG.SVGElement)e.Node);
					if(e.OldParent is Interface.ISVGContainer)
					{
						if(((Interface.ISVGContainer)e.OldParent).ChildElements.Contains((Interface.ISVGElement)e.Node))
							((Interface.ISVGContainer)e.OldParent).ChildElements.Remove((Interface.ISVGElement)e.Node);
					}
				}
			}

			if(this.isInImport)
			{
				return;
			}

			#endregion

            #region ..Connect
            if (e.Node is SVG.BasicShapes.SVGConnectionElement)
                this.initializedConnectElement = false;
            #endregion

			#region ..撤销重作，同步代码
            if (!this.inLoadProcess && this.createDetail)
            {
                System.Xml.XmlNode changeNode = e.Node;
                XmlNode newParent1 = e.NewParent;
                XmlNode oldParent1 = e.OldParent;
                if (!this.acceptNodeChanged)
                {
                    this.ParseNodeChangeInformation(e);
                    return;
                }
                //删除的动作记录放在Removing中处理，以记录层次关系
                if (e.Action != XmlNodeChangedAction.Remove)
                {
                    System.Xml.XmlNodeChangedAction action = e.Action;
                    YP.SVG.SVGElement changeparent = null;
                    int count = this.undos.Count;
                    bool isAttribute = false;
                    if (e.Node is System.Xml.XmlAttribute)
                    {
                        changeparent = (e.Node as XmlAttribute).OwnerElement as SVGElement;
                        isAttribute = true;
                    }
                    else if (e.Node is XmlText && e.NewParent is XmlAttribute)
                    {
                        changeparent = (e.NewParent as XmlAttribute).OwnerElement as SVGElement;
                        isAttribute = true;
                    }
                    //撤销重做，当编辑Text对象时，不记录
                    if (!(e.Node is XmlText && e.NewParent is YP.SVG.Text.SVGTextElement) && !isAttribute || changeparent != null)
                    {
                        YP.SVG.Undo.NodeUndoOperation undo = new YP.SVG.Undo.NodeUndoOperation(this, changeNode, action, oldParent1, newParent1, changeparent, e.OldValue);
                        this.PushUndo(undo);
                    }
                }
            }
            
			#endregion

            ParseNodeChangeInformation(e);
		}
		#endregion

		#region ..引入节点
		public override XmlNode ImportNode(XmlNode node, bool deep)
		{
            if (node.OwnerDocument == this)
                return node;
			bool old = this.isInImport;
			this.isInImport = true;
			XmlNode node1 = base.ImportNode (node, deep);
			
			this.isInImport = old;
			return node1;
		}
		#endregion

		#region ..注册动画
		/// <summary>
		/// 注册动画，将文档中所有动画对象和所联系的属性相联系
		/// </summary>
//		public void AttachAnimates()
//		{
//			if(!this.inLoadProcess)
//				return;
//			foreach(YP.SVGDom.Animation.SVGAnimationElement animate in this.animateList)
//				this.AttachAnimate(animate);
//			this.animateList.Clear();
//		}
//
//		public void AttachAnimate(YP.SVGDom.Animation.SVGAnimationElement animate)
//		{
//			YP.SVGDom.SVGStyleable target = (YP.SVGDom.SVGStyleable)animate.TargetElement;
//			string attributeName = animate.GetAttribute("attributeName").Trim();
//			if(animate is Animation.SVGAnimateMotionElement)
//				attributeName = "transform";
//			if(attributeName.Length > 0 && target != null)
//			{
//				YP.SVGDom.DataType.SVGAnimatedType animateType = (YP.SVGDom.DataType.SVGAnimatedType)target.GetAnimatedAttribute(attributeName);
//				
//				if(animateType != null)
//				{
//					target.AttachKeys(animate);
//					animateType.AttachAnimates(animate);
//				}
//			}
//		}
		#endregion

		#region ..创建类型操作面板StyleOperator
		/// <summary>
		/// 创建类型操作面板StyleOperator
		/// </summary>
		/// <returns></returns>
		public StyleContainer.StyleOperator CreateStyleOperator()
		{
			var sp = new StyleContainer.StyleOperator();
            sp.ClipRegion.MakeEmpty();
            return sp;
		}
		#endregion

		#region ..匹配节点CSS
		/// <summary>
		/// 匹配节点CSS
		/// </summary>
		/// <param name="svgStyle">类型节点</param>
		/// <param name="content">CSS规则内容</param>
		public void MatchStyleable(YP.SVG.SVGStyleable svgStyle,Base.CSS.CSSRuleSetContent content)
		{
			foreach(Base.CSS.CSSStyleSheet sheet in this.cssStyleSheetList)
				sheet.MatchStyleable(svgStyle,content);
            foreach (YP.SVG.DocumentStructure.SVGStyleElement style in styles)
				style.MatchStyleable(svgStyle,content);
		}
		#endregion

		#region ..添加预定义对象
		/// <summary>
		/// 添加预定义对象
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
        public System.Xml.XmlNode AddDefsElement(YP.SVG.SVGElement element)
		{
			bool old = this.acceptNodeChanged;
			this.acceptNodeChanged = false;
			if(element.OwnerDocument != this)
				element = (YP.SVG.SVGElement)this.ImportNode(element,true);
			if(this.DocumentElement == null)
				return null;
			this.BeginProcess();
			var node = this.DocumentElement.SelectSingleNode("defs") as SVGElement;
			this.EndProcess();
			this.acceptNodeChanged = old;
			if(!(node is YP.SVG.SVGElement))
			{
				this.acceptNodeChanged = false;
				node = this.CreateElement(this.Prefix,"defs",this.NamespaceURI) as SVGElement;
				this.acceptNodeChanged = true;
                node = this.RootElement.InternalPrependChild(node) as SVGElement;
				this.RootElement.InternalPrependChild(this.CreateTextNode("\n"));
                this.RootElement.InternalInsertAfter(this.CreateTextNode("\n"), node);
				node.InternalAppendChild(this.CreateTextNode("\n"));
			}
			this.acceptNodeChanged = old;
			old = this.autoformat;
			this.autoformat = true;
			if(node is YP.SVG.SVGElement)
			{
				System.Xml.XmlNode node1 = ((YP.SVG.SVGElement)node).InternalAppendChild(element);
				node.InternalAppendChild(this.CreateTextNode("\n"));
				this.autoformat = old;
				return node1;
			}
			return null;
		}
		#endregion

		#region ..提交事件
		/// <summary>
		/// 提交选择改变事件
		/// </summary>
		/// <param name="e"></param>
        public void InvokeSelectionChanged(CollectionChangedEventArgs e)
		{
			if(this.SelectionChanged != null && !this.inTransaction )
				this.SelectionChanged(this,e);
		}
		#endregion

        #region ..InvokeElementsChanged
        public void InvokeElementsChanged(CollectionChangedEventArgs e)
        {
            if (this.ElementsChanged != null && !this.inTransaction)
            {
                foreach (SVGElement elm in e.ChangeElements)
                    this.RequireRedraw(elm , true);

                this.ElementsChanged(this, e);
            }
        }
        #endregion

        #region ..根据指定对象创建唯一的ID号
        public string CreateID(System.Xml.XmlNode child,YP.SVG.SVGElement parent)
		{
			return this.CreateID(child,parent,true);
		}

        public string CreateID(System.Xml.XmlNode child, YP.SVG.SVGElement parent, bool update)
		{
			if(child is YP.SVG.SVGStyleable)
			{
				string id = child.Name;
				if(((YP.SVG.SVGElement)child).GetAttribute("id").Length > 0)
					id = ((YP.SVG.SVGElement)child).GetAttribute("id").Trim();
				bool old = this.AcceptNodeChanged;
				this.AcceptNodeChanged = false;
				id = YP.SVG.Document.SvgDocumentFactory.CreateString(parent,id,(YP.SVG.SVGElement)child);
				if(update)
				{
					((YP.SVG.SVGElement)child).InternalSetAttribute("id",id);
				}
//				if(child.ParentNode != null)
//					this.InvokeElementChanged(new Document.ElementChangedEventArgs((YP.SVGDom.SVGElement)child,parent,parent,ElementChangedAction.Change));
				this.AcceptNodeChanged = old;
				return id;
			}
			return string.Empty;
		}
		#endregion

		#region ..撤销重作
		/// <summary>
		/// 提交撤销重作记录
		/// </summary>
		public void InvokeUndos()
		{
			if(this.inTransaction)
				return;
            //提交所有修改对象
            this.ForceRefreshAllElementToBeUpdated();
			int count = this.undos.Count;
			if(count > 0)
			{
				Interface.IUndoOperation[] operations = new Interface.IUndoOperation[this.undos.Count];
				this.undos.CopyTo(operations);
				
				this.undoStack.Push(operations);
			}
			this.undos.Clear();
		}
		#endregion

		#region ..开始内部操作
		/// <summary>
		/// 开始用XPath选择节点
		/// </summary>
		public void BeginProcess()
		{
			this.inSelecting = true;
		}
		#endregion

		#region ..结束选择
		/// <summary>
		/// 结束选择
		/// </summary>
        public void EndProcess()
		{
			this.inSelecting = false;
		}
		#endregion

		#region ..在删除之前，记录对象的原来层次
		private void SvgDocument_NodeRemoving(object sender, XmlNodeChangedEventArgs e)
		{
            if (this.disposing || this.InXPathSelecting || this.inExporting)
				return;
			//删除的动作记录放在Removing中处理，以记录层次关系
			System.Xml.XmlNode changeNode = e.Node;
			XmlNode newParent1 = e.NewParent ;
			XmlNode oldParent1 = e.OldParent;
			
			if(e.Node is System.Xml.XmlAttribute && e.Action == System.Xml.XmlNodeChangedAction.Insert)
				return;
				
			System.Xml.XmlNodeChangedAction action = e.Action;
			YP.SVG.SVGElement changeparent = null;
            bool isAttribute = false;
			int count = this.undos.Count;
            if (e.Node is System.Xml.XmlAttribute)
            {
                changeparent = (e.Node as XmlAttribute).OwnerElement as SVGElement;
                isAttribute = true;
            }
            if (e.Node is XmlText && e.NewParent is XmlAttribute)
            {
                changeparent = (e.NewParent as XmlAttribute).OwnerElement as SVGElement;
                isAttribute = true;
            }
			//撤销重做，当编辑Text对象时，不记录
			if(!(e.Node is XmlText && e.NewParent is YP.SVG.Text.SVGTextContentElement) && (!isAttribute || changeparent != null))
			{
				YP.SVG.Undo.NodeUndoOperation undo = new YP.SVG.Undo.NodeUndoOperation(this,changeNode,action,oldParent1,newParent1,changeparent,e.OldValue);
				this.PushUndo(undo);
			}
		}
		#endregion

		#region ..将文档绘制在指定的画布上
		/// <summary>
		/// 将文档绘制在指定的画布上
		/// </summary>
		/// <param name="g">画布</param>
        public void RenderToGraphics(System.Drawing.Graphics g)
		{
            var root = this.CurrentScene as SVGTransformableElement;
            if (root != null && root.SVGRenderer != null)
                root.SVGRenderer.Draw(g, this.CreateStyleOperator());
		}
		#endregion

		#region IDocument 成员

        public bool CanUndo
		{
			get
			{
				// TODO:  添加 SvgDocument.CanUndo getter 实现
				return this.undoStack.CanUndo;
			}
		}

        public bool CanRedo
		{
			get
			{
				// TODO:  添加 SvgDocument.CanRedo getter 实现
				return this.undoStack.CanRedo;
			}
		}

		#endregion

		#region ..开始绘制
		/// <summary>
		/// 开始绘制当前内容
		/// </summary>
        public void BeginRender()
		{
			this._stopRender = false;
		}
		#endregion

		#region ..停止绘制
		/// <summary>
		/// 停止绘制当前内容
		/// </summary>
        public void StopRender()
		{
			this._stopRender = true;
		}
		#endregion

		#region IDisposable 成员
		bool disposing = false;
		public void Dispose()
		{
			this.disposing = true;
			this.Loaded = null;
			this.SelectionChanged = null;
			this.base64Source = null;
			this.referencedFiles = null;
			// TODO:  添加 SvgDocument.Dispose 实现
			GC.SuppressFinalize(this);
			GC.ReRegisterForFinalize(this);
			GC.Collect();
		}
		#endregion

		#region ..override nodeChanged
		
		#endregion

		#region ISerializable 成员

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    // TODO:  添加 SvgDocument.GetObjectData 实现
        //    Type type = Type.GetType("YP.SVGDom.Document.SVGDocument");
        //    info.SetType(type);
        //}

		#endregion

		#region ..ClearCache
		/// <summary>
		/// clear cache so that all the content are repaint
		/// </summary>
		public void ClearCache()
		{
			this.currentTime1 = (this.currentTime1 + 1) % 2;
		}
		#endregion

		#region ..OnCSSChanged
		public virtual void OnCSSChanged()
		{
			if(this.CSSChanged != null)
				this.CSSChanged(this,EventArgs.Empty);
		}
		#endregion

		#region ..BeginUpdate
		/// <summary>
		/// begin to update, when you begin update, control won't update until you call the endupdate
		/// </summary>
		public void BeginUpdate()
		{
			this.inTransaction = true;
		}
		#endregion

		#region ..EndUpdate
		/// <summary>
		/// end to update, when you begin update, control won't update until you call the endupdate
		/// </summary>
		public void EndUpdate()
		{
			this.inTransaction = false;
			this.InvokeUndos();
		}
		#endregion

        #region ..DrawElementWithCache
        /// <summary>
        /// judge whether
        /// </summary>
        /// <returns></returns>
        public bool DrawElementWithCache(SVGStyleable element)
        {
            return this.enableCache 
                && element.StyleContainer != null 
                && element.CurrentTime == this.CurrentTime 
                && !element.NeedUpdateCSSStyle
                && element.IsActive;
        }
        #endregion

        #region ..InitializeConnectElement
        /// <summary>
        /// Initialize the connect element
        /// </summary>
        public void InitializeConnectElement()
        {
            if (initializedConnectElement)
                return;
            this.BeginProcess();
            XmlNodeList list = this.GetElementsByTagName("connect");
            bool result = false;
            foreach (XmlNode node in list)
            {
                if (node is SVG.BasicShapes.SVGConnectionElement)
                {
                    SVG.BasicShapes.SVGConnectionElement cnn = node as SVG.BasicShapes.SVGConnectionElement;
                    result=  cnn.StartElement == cnn.EndElement;
                }
            }
            this.EndProcess();
            initializedConnectElement = true;
        }
        #endregion

        #region ..ReDraw Element
        void RequireRedraw(SVGElement render)
        {
            if (render == null)
                return;
            if (!this.redrawElements.Contains(render))
                this.redrawElements.Add(render);
        }

        void RequireRedraw(SVGElement render, bool addParent)
        {
            if(render == null)
                return;

            this.RequireRedraw(render);

            if (addParent)
            {
                SVGElement elm = ((SVGElement)render).ParentElement;
                this.RequireRedraw(elm, true);
            }
        }

        public bool NeedReDraw(SVGElement render)
        {
            if (render == null)
                return false;

            return this.redrawElements.Contains(render);
        }

        public void FinishReDraw()
        {
            this.redrawElements.Clear();
            this.elementNewStates.Clear();
            this.elementOriginalStates.Clear();
        }
        #endregion

        #region ..RefreshElement
        List<SVGElement> elementOriginalStates = new List<SVGElement>();
        public void RefreshOriginalElement(SVGElement element)
        {
            SVGElement element1 = element;
            //处理text对象
            if (element is Text.SVGTextContentElement)
                element1 = (element as Text.SVGTextContentElement).OwnerTextElement;
            if (!elementOriginalStates.Contains(element1))
            {
                elementOriginalStates.Add(element1);
                this.InvokeElementsChanged(new CollectionChangedEventArgs(element1, CollectionChangeAction.Change));
            }
        }

        List<SVGElement> elementNewStates = new List<SVGElement>();
        public void RefreshElement(SVGElement element)
        {
            this.RefreshElement(element, false);
        }

        public void RefreshElement(SVGElement element, bool forceRefresh)
        {
           
            SVGElement element1 = element;
            //处理text对象
            if (element is Text.SVGTextContentElement)
                element1 = (element as Text.SVGTextContentElement).OwnerTextElement;
            if(forceRefresh)
                this.InvokeElementsChanged(new CollectionChangedEventArgs(element1, CollectionChangeAction.Change));
            else if (!elementNewStates.Contains(element1))
                elementNewStates.Add(element1);
        }
        #endregion

        #region ..ForceRefreshElement
        public void ForceRefreshAllElementToBeUpdated()
        {
            ForceRefreshAllElementToBeUpdated(true);
        }

        public void ForceRefreshAllElementToBeUpdated(bool needToRedraw)
        {
            if (this.elementNewStates.Count > 0 && needToRedraw)
                this.InvokeElementsChanged(new CollectionChangedEventArgs(this.elementNewStates.ToArray(), CollectionChangeAction.Change));
            this.elementOriginalStates.Clear();
            this.elementNewStates.Clear();
        }
        #endregion

        #region ..DoAction
        /// <summary>
        /// 以原子的方式执行一段代码，该段代码将产生一个撤销和重做操作
        /// </summary>
        /// <param name="action">需要执行的方法</param>
        public void DoAction(Action action)
        {
            this.DoAction(action, true);
        }

        /// <summary>
        /// 以原子的方式执行一段代码，该段代码将产生一个撤销和重做操作
        /// </summary>
        /// <param name="action">需要执行的方法</param>
        /// <param name="record">是否产生撤消记录</param>
        public void DoAction(Action action, bool record)
        {
            if (action == null)
                return;
            bool old = this.AcceptNodeChanged;
            this.AcceptNodeChanged = record;
            this.BeginUpdate();
            action.Invoke();
            this.EndUpdate();
            this.AcceptNodeChanged = old;
        }
        #endregion

        #region ..导出标准SVG
        bool exportBounds = false;
        bool inExporting = false;
        #region ..ExportSVG
        /// <summary>
        /// <para>导出SVG文件.</para>
        /// <para>VectorControl控件提供了导出SVG文件的功能，这个与属性XmlCode的差别在于：</para>
        /// <list type="bullet"><item><description>在VectorControl控件操作种，为了满足实际需要，增加了一些扩展要素（如连接线等）
        /// 采用的渲染方式也和SVG渲染有一定的差别，如果直接用XmlCode代码预览SVG文件，您可能会看到和编辑环境不一样的结果。
        /// <para>通过导出功能，VectorControl可以将编辑环境中的扩展要素转换为标准的SVG对象，使得导出的SVG文件能够完全被SVG浏览器解析并保持和编辑环境一样的效果。</para></description></item>
        /// <item><description>ExportSVG提供了一个参数，可以控制是否将编辑环境中的外部资源（如栅格图像）转换为XML文件的base64资源，
        /// 通过这种转换，SVG文件可以脱离你所插入的外部资源而独立使用。</description></item>
        /// </list>
        /// </summary>
        /// <param name="convertSourceToBase64">是否将SVG文档中的外部资源转换为base64资源</param>
        public string ExportSVG(bool convertSourceToBase64)
        {
            string code = string.Empty;
            SVGElement svg = this.ExportNativeSVG(convertSourceToBase64);
            if (svg != null)
            {
                code = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                code += svg.OuterXml.Replace("xmlns=\"\"", string.Empty);
                svg = null;

            }
            return code;
        }

        public SVGElement ExportNativeSVG(bool convertSourceToBase64)
        {
            string code = string.Empty;
            if (this.IsValid && this.DocumentElement != null)
            {
                using (System.Windows.Forms.Button btn = new System.Windows.Forms.Button())
                {
                    btn.Size = new Size(2, 2);
                    using (System.Drawing.Graphics g = btn.CreateGraphics())
                    {
                        this.RenderToGraphics(g);
                        ArrayList list = new ArrayList();

                        YP.SVG.DocumentStructure.SVGSVGElement svg = this.DocumentElement.Clone() as YP.SVG.DocumentStructure.SVGSVGElement;

                        if (svg != null)
                        {
                            this.BeginProcess();
                            bool old = this.AcceptNodeChanged;
                            this.AcceptNodeChanged = false;
                            g.ResetTransform();
                            inExporting = true;
                            try
                            {
                                this.ExportSVGElement(svg, convertSourceToBase64, list, g, svg.TotalTransform.Clone());
                            }
                            finally
                            {
                                inExporting = false;
                            }
                            if (list.Count > 0)
                            {
                                YP.SVG.SVGElement element = this.CreateElement(this.Prefix, "defs", this.NamespaceURI) as YP.SVG.SVGElement;
                                for (int i = 0; i < list.Count; i++)
                                    element.InternalPrependChild(list[i] as YP.SVG.SVGElement);
                                svg.InternalPrependChild(element);
                            }
                            list = null;
                            //add the demo string
                            //if (this.DemoString.Length > 0)
                            //{
                            //    System.Xml.XmlElement element = this.CreateElement("text");
                            //    Random rd = new Random();
                            //    element.InternalSetAttribute("x", rd.Next(0, 500).ToString());
                            //    element.InternalSetAttribute("y", rd.Next(0, 500).ToString());
                            //    element.InnerText = this.vectorControl.DemoString;
                            //    svg.InternalAppendChild(element);
                            //}
                            this.AcceptNodeChanged = old;
                            this.EndProcess();
                            return svg;
                        }
                    }
                }

            }
            return null;
        }
        #endregion

        #region ..ExportSVGElement
        void ExportSVGElement(YP.SVG.SVGElement element, bool base64, ArrayList defs, Graphics g, Matrix coordTransform)
        {
            Random random = new Random();
            using (System.Drawing.Drawing2D.Matrix total = new System.Drawing.Drawing2D.Matrix())
            {
                #region ..阴影
                if (element is YP.SVG.SVGTransformableElement)
                {
                    YP.SVG.SVGTransformableElement render = element as YP.SVG.SVGTransformableElement;
                    if (render.StyleContainer != null)
                    {
                        YP.SVG.StyleContainer.ShadowStyle shadow = render.StyleContainer.ShadowStyle;
                        if (shadow.DrawShadow)
                        {
                            YP.SVG.SVGElement shadowElement = this.GetShadowElement(render, coordTransform);
                            WriteShadowProperty(shadowElement, shadow, render.FillShadow);
                            render.ParentElement.InternalInsertBefore(shadowElement, render);

                            //if render support marker
                            if (render.SupportMarker)
                            {
                                YP.SVG.ClipAndMask.SVGMarkerElement marker = render.MarkerStart;
                                shadowElement = this.GetShadowElement(marker, coordTransform);
                                if (shadowElement != null)
                                {
                                    WriteShadowProperty(shadowElement, shadow, marker.FillShadow);
                                    render.ParentElement.InternalInsertBefore(shadowElement, render);
                                }

                                marker = render.MarkerEnd;
                                shadowElement = this.GetShadowElement(marker, coordTransform);
                                if (shadowElement != null)
                                {
                                    WriteShadowProperty(shadowElement, shadow, marker.FillShadow);
                                    render.ParentElement.InternalInsertBefore(shadowElement, render);
                                }
                            }

                        }
                    }
                }
                #endregion

                #region ..PointsElement
                if (element is YP.SVG.BasicShapes.SVGPointsElement)
                {
                    string transform = element.GetAttribute("transform");
                    bool valid = false;
                    if (transform.Trim().Length > 0)
                    {
                        YP.SVG.DataType.SVGTransformList ts = new YP.SVG.DataType.SVGTransformList(transform);
                        using (System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix())
                        {
                            if (!matrix.IsIdentity && (matrix.Elements[0] > 1 || matrix.Elements[3] > 1))
                            {
                                PointF[] ps = ((element as YP.SVG.BasicShapes.SVGPointsElement).Points as YP.SVG.DataType.SVGPointList).GetGDIPoints();
                                if (ps != null && ps.Length > 0)
                                {
                                    matrix.TransformPoints(ps);
                                    element.InternalSetAttribute("points", this.GetPointString(ps));
                                    valid = true;
                                }
                                ps = null;
                            }
                        }
                        if (valid)
                            element.InternalRemoveAttribute("transform");
                    }
                }
                #endregion

                #region ..直线
                else if (element is YP.SVG.BasicShapes.SVGLineElement)
                {
                    YP.SVG.BasicShapes.SVGLineElement line = element as YP.SVG.BasicShapes.SVGLineElement;
                    string transform = element.GetAttribute("transform");
                    bool valid = false;
                    if (transform.Trim().Length > 0)
                    {
                        YP.SVG.DataType.SVGTransformList ts = new YP.SVG.DataType.SVGTransformList(transform);
                        using (System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix())
                        {
                            if (!matrix.IsIdentity && (matrix.Elements[0] > 1 || matrix.Elements[3] > 1))
                            {
                                PointF[] ps = new PointF[] { new PointF(line.X1.Value, line.Y1.Value), new PointF(line.X2.Value, line.Y2.Value) };
                                matrix.TransformPoints(ps);
                                line.InternalSetAttribute("x1", ps[0].X.ToString());
                                line.InternalSetAttribute("y1", ps[0].Y.ToString());
                                line.InternalSetAttribute("x2", ps[1].X.ToString());
                                line.InternalSetAttribute("y2", ps[1].Y.ToString());
                                ps = null;
                                valid = true;
                            }
                        }
                    }
                    transform = null;
                    if (valid)
                        element.InternalRemoveAttribute("transform");
                }
                #endregion

                #region ..连接线
                else if (element is YP.SVG.BasicShapes.SVGConnectionElement)
                {
                    YP.SVG.BasicShapes.SVGConnectionElement connect = element as YP.SVG.BasicShapes.SVGConnectionElement;
                    YP.SVG.SVGElement temp = (connect as SVG.Interface.BasicShapes.ISVGBasicShape).ConvertToPath() as YP.SVG.SVGElement;
                    if (temp != null && connect.ParentElement != null)
                    {
                        connect.ParentElement.InternalInsertBefore(temp, connect);
                        temp.InternalSetAttribute("fill", "none");
                        temp.InternalRemoveAttribute("x1");
                        temp.InternalRemoveAttribute("x2");
                        temp.InternalRemoveAttribute("y1");
                        temp.InternalRemoveAttribute("y2");
                        temp.InternalRemoveAttribute("start");
                        temp.InternalRemoveAttribute("end");
                        temp.InternalRemoveAttribute("type");
                        temp.InternalRemoveAttribute("transform");
                    }
                }
                #endregion

                #region ..曲线
                else if (element is YP.SVG.Paths.SVGPathElement)
                {
                    YP.SVG.Paths.SVGPathElement path = element as YP.SVG.Paths.SVGPathElement;
                    string transform = element.GetAttribute("transform");
                    bool valid = false;
                    if (transform.Trim().Length > 0)
                    {
                        YP.SVG.DataType.SVGTransformList ts = new YP.SVG.DataType.SVGTransformList(transform);
                        using (System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix())
                        {
                            if (!matrix.IsIdentity && (matrix.Elements[0] > 1 || matrix.Elements[3] > 1))
                            {
                                path.TransformSegs(matrix);
                                path.InternalSetAttribute("d", path.PathSegList.PathString);
                                valid = true;
                            }
                        }
                    }
                    if (valid)
                        element.InternalRemoveAttribute("transform");
                }
                #endregion

                #region ..TextBlock
                else if (element is Text.SVGTextBlockElement)
                {
                    SVGElement elm = (element as Text.SVGTextBlockElement).ExportSVGElement();
                    if (element.ParentElement is DocumentStructure.SVGGElement)
                        element.ParentElement.InternalInsertBefore(elm, element);
                    else if (element.ParentElement != null && element.ParentElement.ParentElement != null)
                        element.ParentElement.ParentElement.InternalInsertAfter(elm, element.ParentElement);
                }
                #endregion

                #region ..栅格图像
                else if (element is YP.SVG.DocumentStructure.SVGImageElement)
                {
                    if (base64)
                    {
                        YP.SVG.DocumentStructure.SVGImageElement image = element as YP.SVG.DocumentStructure.SVGImageElement;
                        image.ConvertImageTo64();
                    }
                }
                #endregion

                #region ..其他直接绘制对象
                else if(element != null && element.Name != "branch")
                {
                    string transform = element.GetAttribute("transform");
                    if (transform.Trim().Length > 0)
                    {
                        using (System.Drawing.Drawing2D.Matrix matrix = (element as YP.SVG.SVGTransformableElement).Transform.FinalMatrix.GetGDIMatrix())
                        {
                            if (matrix.Elements[0] > 1 || matrix.Elements[3] > 1)
                            {
                                float a = 1;
                                a = (element as YP.SVG.SVGTransformableElement).StrokeStyle.strokewidth.Value;
                                if (a == 0)
                                    a = 1;
                                float scale = (float)Math.Max(matrix.Elements[0], matrix.Elements[3]);
                                a = a / scale;
                                a = (float)Math.Max(0.03f, a);
                                element.InternalSetAttribute("stroke-width", a.ToString());

                            }
                            total.Multiply(matrix);
                        }
                    }

                    //文本
                    //if (element is YP.SVGDom.Text.SVGTextElement)
                    //{
                    //    (element as YP.SVGDom.Text.SVGTextElement).UpdateText();
                    //}
                }
                #endregion

                #region ..图案
                if (element is YP.SVG.SVGTransformableElement && !(element is YP.SVG.DocumentStructure.SVGImageElement) && !(element is YP.SVG.BasicShapes.SVGLineElement))
                {
                    YP.SVG.SVGTransformableElement render = element as YP.SVG.SVGTransformableElement;
                    string temp = render.StyleContainer.FillStyle.HatchStyle.Value;
                    YP.SVG.HatchStyle hatch = YP.SVG.HatchStyle.None;
                    if (temp != null)
                    {
                        if (System.Enum.IsDefined(typeof(YP.SVG.HatchStyle), temp))
                            hatch = (YP.SVG.HatchStyle)System.Enum.Parse(typeof(YP.SVG.HatchStyle), temp, false);
                    }
                    if (hatch != YP.SVG.HatchStyle.None)
                    {
                        Color c = render.StyleContainer.FillStyle.HatchColor.GDIColor;
                        Color color = Color.White;
                        if (render.FillStyle.svgPaint.PaintType == (ulong)YP.SVG.PaintType.SVG_PAINTTYPE_RGBCOLOR)
                            color = ((YP.SVG.DataType.RGBColor)render.StyleContainer.FillStyle.svgPaint.RgbColor).GDIColor;

                        #region ..图案
                        //图案
                        if ((int)hatch < 56)
                        {
                            float width = 8, height = 8;
                            PointF[] ps = new PointF[] { new PointF(0, 0), new PointF(width, height) };
                            temp = string.Empty;
                            if (total.IsInvertible && !total.IsIdentity)
                            {
                                total.Invert();
                                temp = "matrix(" + total.Elements[0].ToString() + "," + total.Elements[1].ToString() + "," + total.Elements[2].ToString() + "," + total.Elements[3].ToString() + ",0,0)";// + total.Elements[4].ToString()+"," + total.Elements[5].ToString()+")";
                            }
                            ps = null;
                            using (System.Drawing.Bitmap bmp = new Bitmap((int)width, (int)height))
                            {
                                System.Drawing.Drawing2D.HatchStyle style1 = System.Drawing.Drawing2D.HatchStyle.Cross;

                                if (System.Enum.IsDefined(typeof(System.Drawing.Drawing2D.HatchStyle), hatch.ToString()))
                                    style1 = (System.Drawing.Drawing2D.HatchStyle)System.Enum.Parse(typeof(System.Drawing.Drawing2D.HatchStyle), hatch.ToString(), false);
                                using (HatchBrush brush = new HatchBrush(style1, c, color))
                                {
                                    using (System.Drawing.Graphics g1 = Graphics.FromImage(bmp))
                                    {
                                        g1.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                                    }
                                }
                                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                string source = YP.SVG.DocumentStructure.SVGImageElement.ConvertToBase64(ms);
                                YP.SVG.SVGElement pattern = element.OwnerDocument.CreateElement(element.Prefix, "pattern", element.NamespaceURI) as YP.SVG.SVGElement;
                                YP.SVG.SVGElement image = element.OwnerDocument.CreateElement(element.Prefix, "image", element.NamespaceURI) as YP.SVG.SVGElement;
                                bool old = element.OwnerDocument.AcceptNodeChanged;
                                element.OwnerDocument.AcceptNodeChanged = false;
                                image.InternalSetAttribute("width", width.ToString());
                                image.InternalSetAttribute("height", height.ToString());
                                image.InternalSetAttribute("href", YP.SVG.Document.SVGDocument.XLinkNamespace, source);
                                pattern.InternalAppendChild(image);
                                int a = random.Next(1000, 9999);
                                string id = "pattern-" + hatch.ToString() + "-" + a.ToString();
                                pattern.InternalSetAttribute("id", id);
                                pattern.InternalSetAttribute("width", width.ToString());
                                pattern.InternalSetAttribute("height", height.ToString());
                                pattern.InternalSetAttribute("viewBox", "0 0 " + width.ToString() + " " + height.ToString());
                                pattern.InternalSetAttribute("patternUnits", "userSpaceOnUse");
                                if (temp.Length > 0)
                                    pattern.InternalSetAttribute("patternTransform", temp);
                                element.InternalSetAttribute("fill", "url(#" + id + ")");
                                defs.Add(pattern);
                                element.OwnerDocument.AcceptNodeChanged = old;
                            }
                        }
                        #endregion

                        #region ..渐变
                        else
                        {
                            bool old = element.OwnerDocument.AcceptNodeChanged;
                            element.OwnerDocument.AcceptNodeChanged = false;
                            YP.SVG.SVGElement gradient = null;
                            YP.SVG.SVGElement stop = null;
                            switch (hatch)
                            {
                                case YP.SVG.HatchStyle.Center:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "radialGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    break;
                                case YP.SVG.HatchStyle.VerticalCenter:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "0.5");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    gradient.InternalSetAttribute("gradientTransform", "rotate(90)");
                                    break;
                                case YP.SVG.HatchStyle.HorizontalCenter:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "0.5");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    break;
                                case YP.SVG.HatchStyle.DiagonalLeft:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "0.5");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    gradient.InternalSetAttribute("gradientTransform", "rotate(45 0.5 0.5)");
                                    break;
                                case YP.SVG.HatchStyle.DiagonalRight:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "0.5");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    gradient.InternalSetAttribute("gradientTransform", "rotate(-45 0.5 0.5)");
                                    break;
                                case YP.SVG.HatchStyle.LeftRight:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    break;
                                case YP.SVG.HatchStyle.TopBottom:
                                    gradient = element.OwnerDocument.CreateElement(element.Prefix, "linearGradient", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(color));
                                    stop.InternalSetAttribute("offset", "0");
                                    gradient.InternalAppendChild(stop);
                                    stop = element.OwnerDocument.CreateElement(element.Prefix, "stop", element.NamespaceURI) as YP.SVG.SVGElement;
                                    stop.InternalSetAttribute("stop-color", YP.SVG.ColorHelper.GetColorStringInHex(c));
                                    stop.InternalSetAttribute("offset", "1");
                                    gradient.InternalAppendChild(stop);
                                    gradient.InternalSetAttribute("gradientTransform", "rotate(90)");
                                    break;
                            }
                            defs.Add(gradient);
                            int a = random.Next(1000, 9999);
                            string id = "gradient-" + hatch.ToString() + "-" + a.ToString();
                            gradient.InternalSetAttribute("id", id);
                            element.InternalSetAttribute("fill", "url(#" + id + ")");
                            element.OwnerDocument.AcceptNodeChanged = old;
                        }
                        #endregion
                    }
                }
                #endregion

                #region ..子对象
                //子对象
                if (element is SVGTransformableElement)
                {
                    string transform = element.GetAttribute("transform");
                    YP.SVG.SVGElementCollection list = (element as SVGTransformableElement).ChildElements;
                    bool isGroup = element is DocumentStructure.SVGGElement;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (isGroup)
                        {
                            if (transform.Trim().Length > 0)
                            {
                                string transform1 = transform + " " + (list[i] as YP.SVG.SVGElement).GetAttribute("transform");
                                (list[i] as YP.SVG.SVGElement).InternalSetAttribute("transform", transform1);
                                transform1 = null;
                            }
                        }
                        this.ExportSVGElement(list[i] as YP.SVG.SVGElement, base64, defs, g, coordTransform);
                    }
                    if(isGroup)
                        element.InternalRemoveAttribute("transform");
                }
                #endregion
            }

            #region ..Export the bound point
            //export the bound point
            if (exportBounds && element is SVG.SVGTransformableElement)
            {
                PointF[] ps = (element as SVG.SVGTransformableElement).ConnectionPoints;
                if (ps != null)
                {
                    element.InternalSetAttribute("boundPoints", SVG.DataType.SVGPointList.GetPointsString(ps));
                }
            }
            #endregion
        }
        #endregion

        #region ..GetShadowElement
        YP.SVG.SVGElement GetShadowElement(YP.SVG.SVGTransformableElement render, Matrix coordTransform)
        {
            if (render == null)
                return null;
            YP.SVG.SVGElement shadowElement = null;
            if (render is YP.SVG.Interface.BasicShapes.ISVGBasicShape)
                shadowElement = (render as YP.SVG.Interface.BasicShapes.ISVGBasicShape).ConvertToPath() as YP.SVG.SVGElement;
            else if (render is YP.SVG.DocumentStructure.SVGImageElement || render is YP.SVG.DocumentStructure.SVGUseElement)
            {
                shadowElement = render.OwnerDocument.CreateElement("path") as YP.SVG.SVGElement;
                foreach (System.Xml.XmlAttribute attribute in render.Attributes)
                {
                    string name = attribute.Name;
                    string valuestr = attribute.Value;
                    if (string.Compare(name,"x") != 0 &&string.Compare(name,"y") != 0 &&string.Compare(name,"width") != 0 &&string.Compare(name,"height") != 0 &&string.Compare(name,"id") != 0)
                        shadowElement.InternalSetAttribute(name, valuestr);
                    name = null;
                    valuestr = null;
                }
            }
            else
            {
                shadowElement = render.Clone() as YP.SVG.SVGElement;
                shadowElement.InternalRemoveAttribute("fill");
                shadowElement.InternalRemoveAttribute("stroke");
                shadowElement.InternalRemoveAttribute("opacity");
            }
            if (render is YP.SVG.BasicShapes.SVGConnectionElement)
                shadowElement.InternalRemoveAttribute("transform");
            shadowElement.InternalRemoveAttribute("shadow");
            shadowElement.InternalRemoveAttribute("shadowColor");
            shadowElement.InternalRemoveAttribute("xOffset");
            shadowElement.InternalRemoveAttribute("yOffset");
            shadowElement.InternalRemoveAttribute("shadowOpacity");
            shadowElement.InternalRemoveAttribute("marker-start");
            shadowElement.InternalRemoveAttribute("marker-end");
            shadowElement.InternalRemoveAttribute("marker-mid");
            return shadowElement;
        }

        YP.SVG.SVGElement GetShadowElement(YP.SVG.ClipAndMask.SVGMarkerElement marker, Matrix coordTransform)
        {
            if (marker == null)
                return null;
            YP.SVG.SVGElement shadowElement = null;
            shadowElement = marker.OwnerDocument.CreateElement("path") as YP.SVG.SVGElement;
            using (GraphicsPath path = marker.MarkerPath.Clone() as GraphicsPath)
            {
                using (Matrix matrix = coordTransform.Clone())
                {
                    path.Transform(marker.MarkerTransform);
                    matrix.Invert();
                    path.Transform(matrix);
                    shadowElement.InternalSetAttribute("d", YP.SVG.Paths.SVGPathElement.GetPathString(path));
                }

            }

            return shadowElement;
        }
        #endregion

        #region ..WriteShadowProperty
        void WriteShadowProperty(YP.SVG.SVGElement shadowElement, YP.SVG.StyleContainer.ShadowStyle shadow, bool fillShadow)
        {
            string transform = shadowElement.GetAttribute("transform");
            transform = "translate(" + shadow.XOffset.ToString() + " " + shadow.YOffset.ToString() + ") " + transform;
            shadowElement.InternalSetAttribute("transform", transform);
            string style = shadowElement.GetAttribute("style");
            style = style + ";stroke:" + YP.SVG.ColorHelper.GetColorStringInHex(shadow.ShadowColor) + ";opacity:" + shadow.Opacity.ToString();
            style = style + ";fill:" + (fillShadow ? YP.SVG.ColorHelper.GetColorStringInHex(shadow.ShadowColor) : "none");
            shadowElement.InternalSetAttribute("style", style);
        }
        #endregion

        #region ..GetPointString
        string GetPointString(PointF[] ps)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
            for (int i = 0; i < ps.Length; i++)
            {
                sb.Append(" " + ps[i].X.ToString() + " " + ps[i].Y.ToString());
            }
            return sb.ToString();
        }
        #endregion
        #endregion

        #region ..ExportImage
        /// <summary>
        /// 导出图片，图片尺寸自适应SVG内容
        /// </summary>
        /// <param name="imageName"></param>
        public void ExportWholeImage(string imageName)
        {
            if (this.DocumentElement is SVG.DocumentStructure.SVGSVGElement)
            {
                SVG.DocumentStructure.SVGSVGElement svg = this.RootElement as SVG.DocumentStructure.SVGSVGElement;
                //Draw a tempory image to get the bounds
                System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
                btn.Size = new Size(1, 1);
                using (System.Drawing.Graphics g = btn.CreateGraphics())
                {
                    this.RenderToGraphics(g);
                }
                btn.Dispose();
                using (System.Drawing.Drawing2D.GraphicsPath path = (svg as SVG.Interface.ISVGPathable).GPath.Clone() as System.Drawing.Drawing2D.GraphicsPath)
                {
                    path.Transform(svg.Transform.FinalMatrix.GetGDIMatrix());
                    RectangleF rect = path.GetBounds();
                    Rectangle rect1 = Rectangle.Ceiling(rect);
                    rect1 = new Rectangle(rect1.X - 1, rect1.Y - 1, rect1.Width + 3, rect1.Height + 3);

                    this.ExportImage(imageName, rect1.Size, rect1.Location);
                }
            }
        }

        /// <summary>
        /// 将SVG内容导出为图片
        /// </summary>
        /// <param name="imageName">将要导出的图片路径，</param>
        public void ExportContentAsImage(string imageName)
        {
            if (this.RootElement != null)
            {
                using (GraphicsPath path = (this.RootElement as Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                {
                    path.Transform(this.RootElement.TotalTransform);
                    RectangleF rect = path.GetBounds();
                    float width = rect.Width;
                    float height = rect.Height;
                    if ((int)width != width)
                        width = (int)width + 1;
                    if ((int)height != height)
                        height = (int)height + 1;
                    width = width < 1 ? 1 : width;
                    height = height < 1 ? 1 : height;
                    ExportImage(imageName, Size.Ceiling(new SizeF(width + 1, height+1 )), Point.Truncate(rect.Location));
                }
            }
        }

        void ExportImage(string imageName, System.Drawing.Size canvasSize, PointF left)
        {
            float width = canvasSize.Width;
            string filename = imageName;
            float height = canvasSize.Height;
            if (width > 0 && height > 0)
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                System.IO.FileInfo info = new System.IO.FileInfo(filename);
                ImageCodecInfo myImageCodecInfo = null;
                Encoder myEncoder;
                
                // for the Quality parameter category.
                myEncoder = Encoder.Quality;
                Color color = Color.White;
                switch (info.Extension.ToLower())
                {
                    case ".jpg":
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/jpeg");
                        break;
                    case ".gif":
                        format = System.Drawing.Imaging.ImageFormat.Gif;
                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/gif");
                        break;
                    case ".tiff":
                        format = System.Drawing.Imaging.ImageFormat.Tiff;
                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/tiff");
                        break;
                    case ".png":
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        color = Color.Transparent;
                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/png");
                        break;
                    case ".emf":
                        format = System.Drawing.Imaging.ImageFormat.Emf;
                        break;
                    case ".wmf":
                        format = System.Drawing.Imaging.ImageFormat.Wmf;
                        break;
                    case ".bmp":
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/bmp");
                        break;
                    case ".ico":
                        format = System.Drawing.Imaging.ImageFormat.Icon;
                        myImageCodecInfo = Common.ImageHelper.GetEncoderInfo("image/ico");
                        break;
                }
                using (Bitmap bmp = new Bitmap((int)width, (int)height))
                {
                    if (format != System.Drawing.Imaging.ImageFormat.Emf && format != System.Drawing.Imaging.ImageFormat.Wmf)
                    {
                        using (System.Drawing.Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(color);
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                            g.TranslateTransform(-left.X, -left.Y);
                            this.RenderToGraphics(g);
                        }
                        
                        EncoderParameter myEncoderParameter;
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bmp.Save(filename, myImageCodecInfo, myEncoderParameters);
                    }

                        //save to emf
                    else
                    {
                        try
                        {
                            using (Graphics gs = Graphics.FromImage(bmp))
                            {
                                using (System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(filename, gs.GetHdc()))
                                {
                                    using (Graphics g = Graphics.FromImage(mf))
                                    {
                                        g.Clear(Color.White);
                                        g.TranslateTransform(-left.X, -left.Y);
                                        this.RenderToGraphics(g);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion

        #region ..InvokeConnectChanged
        /// <summary>
        /// 触发ConnectChanged事件
        /// </summary>
        /// <param name="e"></param>
        public void InvokeConnectChanged(ConnectionChangedEventArgs e)
        {
            if (this.ConnectionChanged != null)
                this.ConnectionChanged(this, e);
        }
        #endregion

        #region ..DrawDemoString
        public bool drawDemoString = true;
        public void DrawDemoString(Graphics g)
        {
            Random rd = new Random((int)DateTime.Now.Ticks);
            Size size = new Size(100,100);
            int w = (int)Math.Max(size.Width, 400);
            int h = (int)Math.Max(size.Height, 400);
            w = rd.Next(0, w);
            h = rd.Next(0, h);
            this.DrawDemoString(g, new Point(w, h));
        }

        public void DrawDemoString(Graphics g, PointF p)
        {
            if (!this.drawDemoString)
                return;
            g.DrawString(this.DemoString, new Font("SimSun", 12), Brushes.Black, p, StringFormat.GenericTypographic);
        
        }
        #endregion

        #region ..public virtual on..
        public virtual void OnElementChanged(AttributeChangedEventArgs e)
        {
            if (this.ElementChanged != null && !this.notCauseEvent)
                this.ElementChanged(this, e);
        }

        public void InvokeAttributeChange(AttributeChangedEventArgs e)
        {
            this.OnElementChanged(e);
        }

        /// <summary>
        /// Occurs when an element is removed
        /// </summary>
        /// <param name="e">the data of the event</param>
        public virtual void OnElementRemoved(SVGElementChangedEventArgs e)
        {
            //check the connects before remove

            //if the removed element is connect
            if (e.Element is SVG.BasicShapes.SVGConnectionElement)
            {
                SVG.BasicShapes.SVGConnectionElement cnn = e.Element as SVG.BasicShapes.SVGConnectionElement;
                SVGTransformableElement startElement = cnn.StartElement as SVGTransformableElement;
                SVGTransformableElement endElement = cnn.EndElement;
                if (startElement != null)
                {
                    //cnn.ResetConnectElement(true);
                    this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, startElement, null, ConnectionChangedType.StartElement));
                }
                if (endElement != null)
                {
                    //cnn.ResetConnectElement(false);
                    this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, endElement, null, ConnectionChangedType.StartElement));
                }
            }
            //if not,
            else if(e.Element is SVGTransformableElement)
            {
                SVG.BasicShapes.SVGBranchElement[] cnns = (e.Element as SVGTransformableElement).Connections;
                if (cnns != null && cnns.Length > 0)
                {
                    for (int i = 0; i < cnns.Length; i++)
                    {
                        SVG.BasicShapes.SVGBranchElement cnn = cnns[i] as SVG.BasicShapes.SVGBranchElement;
                        ConnectionChangedType type = ConnectionChangedType.StartElement;
                        bool start = true;
                        if (cnn.EndElement == e.Element)
                        {
                            type = ConnectionChangedType.EndElement;
                            start = false;
                        }
                        //reset the connect element
                        
                        cnn.ResetConnectElement(start);
                        this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, e.Element, null, type));
                    }
                }
            }

            if (this.ElementRemoved != null)
                this.ElementRemoved(this, e);
        }

        /// <summary>
        /// Occurs when an element is added
        /// </summary>
        /// <param name="e">the data of the event</param>
        public virtual void OnElementInserted(SVGElementChangedEventArgs e)
        {
            //check the connect before insert
            //if is connect
            if (e.Element is SVG.BasicShapes.SVGConnectionElement)
            {
                SVG.BasicShapes.SVGConnectionElement cnn = e.Element as SVG.BasicShapes.SVGConnectionElement;
                SVGTransformableElement startElement = cnn.StartElement as SVGTransformableElement;
                SVGTransformableElement endElement = cnn.EndElement;
                if (startElement != null)
                    this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, null, startElement, ConnectionChangedType.StartElement));

                if (endElement != null)
                    this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, null, endElement, ConnectionChangedType.EndElement));
            }
            //if not
            else if(e.Element is SVGTransformableElement)
            {
                SVG.BasicShapes.SVGBranchElement[] cnns = (e.Element as SVGTransformableElement).Connections;
                if (cnns != null && cnns.Length > 0)
                {
                    for (int i = 0; i < cnns.Length; i++)
                    {
                        SVG.BasicShapes.SVGBranchElement cnn = cnns[i] as SVG.BasicShapes.SVGBranchElement;
                        ConnectionChangedType type = ConnectionChangedType.StartElement;
                        if (cnn.EndElement == e.Element)
                        {
                            type = ConnectionChangedType.EndElement;
                        }
                        //reset the connect element
                        
                        this.InvokeConnectChanged(new ConnectionChangedEventArgs(cnn, null, e.Element, type));
                    }
                }
            }
            if (this.ElementInserted != null)
                this.ElementInserted(this, e);
        }

        /// <summary>
        /// Occurs when the one connect is changed
        /// </summary>
        /// <param name="e">store the data of conect changed event</param>
        public virtual void OnConnectChanged(ConnectionChangedEventArgs e)
        {
            if (notCauseEvent)
                return;
            if (this.ConnectionChanged != null)
                this.ConnectionChanged(this, e);
        }
        #endregion

        #region ..ParseNodeChangeInformation
        void ParseNodeChangeInformation(System.Xml.XmlNodeChangedEventArgs e)
        {
            if (notCauseEvent)
                return;
            if (e.Node is YP.SVG.SVGTransformableElement || e.Node is YP.SVG.DocumentStructure.SVGGElement)
            {
                SVGElement element = (e.Node as YP.SVG.SVGElement);
                SVGElement oldParent = null;
                SVGElement newParent = null;
                if (e.OldParent is YP.SVG.SVGElement)
                    oldParent = (e.OldParent as YP.SVG.SVGElement);
                if (e.NewParent is YP.SVG.SVGElement)
                    newParent = (e.NewParent as YP.SVG.SVGElement);

                SVGElementChangedAction action = SVGElementChangedAction.Insert;
                if (e.Action == System.Xml.XmlNodeChangedAction.Remove)
                {
                    if (!this.NodeInDocument(e.OldParent))
                        return;
                    action = SVGElementChangedAction.Remove;
                }
                else if (!this.NodeInDocument(e.NewParent))
                    return;

                SVGElementChangedEventArgs e1 = new SVGElementChangedEventArgs(element, oldParent, newParent, action);

                if (e.Action == System.Xml.XmlNodeChangedAction.Remove)
                    this.OnElementRemoved(e1);
                else if (e.Action == System.Xml.XmlNodeChangedAction.Insert)
                    this.OnElementInserted(e1);
            }

            if (e.Node is YP.SVG.SVGAttribute)
            {
                System.Xml.XmlAttribute attr = (System.Xml.XmlAttribute)e.Node;
                if ((e.Action == XmlNodeChangedAction.Insert || e.Action == XmlNodeChangedAction.Change) && e.NewParent is SVGElement)
                    this.OnElementChanged(new AttributeChangedEventArgs(e.NewParent as SVGElement, attr.Name));
                if (e.Action == XmlNodeChangedAction.Remove && e.OldParent is SVGElement)
                    this.OnElementChanged(new AttributeChangedEventArgs(e.OldParent as SVGElement, attr.Name));
            }
            else if (e.Node is XmlText && (e.NewParent is SVGAttribute || e.OldParent is SVGAttribute))
            {
                if (e.Action == XmlNodeChangedAction.Insert || e.Action == XmlNodeChangedAction.Change)
                {
                    SVGElement elm = (e.NewParent as SVGAttribute).OwnerElement as SVGElement;
                    if (elm != null)
                        this.OnElementChanged(new AttributeChangedEventArgs(elm as SVGElement, e.NewParent.Name));
                }
                else if (e.Action == XmlNodeChangedAction.Remove)
                {
                    SVGElement elm = (e.OldParent as SVGAttribute).OwnerElement as SVGElement;
                    if (elm != null)
                        this.OnElementChanged(new AttributeChangedEventArgs(elm as SVGElement, e.OldParent.Name));
                }
            }
        }
        #endregion

        #region ..NodeInDocument
        /// <summary>
        /// judge whether the node exist in the current dom of the document
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool NodeInDocument(System.Xml.XmlNode node)
        {
            if (node == null)
                return false;
            if (node.ParentNode is System.Xml.XmlDocument)
                return true;
            if (node.ParentNode == null)
                return false;
            return this.NodeInDocument(node.ParentNode);
        }
        #endregion

        #region ..CreateRect, CreateCircle, CreateConnection
        /// <summary>
        /// 创建一个SVGRectElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGRectElement CreateSVGRectElement()
        {
            return this.CreateElement("rect") as BasicShapes.SVGRectElement;
        }

        /// <summary>
        /// 创建一个SVGCircleElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGCircleElement CreateSVGCircleElement()
        {
            return this.CreateElement("circle") as BasicShapes.SVGCircleElement;
        }

        /// <summary>
        /// 创建一个SVGEllipseElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGEllipseElement CreateSVGEllipseElement()
        {
            return this.CreateElement("ellipse") as BasicShapes.SVGEllipseElement;
        }

        /// <summary>
        /// 创建一个SVGPolylineElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGPolylineElement CreateSVGPolylineElement()
        {
            return this.CreateElement("polyline") as BasicShapes.SVGPolylineElement;
        }

        /// <summary>
        /// 创建一个SVGPolygonElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGPolygonElement CreateSVGPolygonElement()
        {
            return this.CreateElement("polygon") as BasicShapes.SVGPolygonElement;
        }

        /// <summary>
        /// 创建一个SVGConnectionElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGConnectionElement CreateSVGConnectionElement()
        {
            return this.CreateElement("connect") as BasicShapes.SVGConnectionElement;
        }

        /// <summary>
        /// 创建一个SVGLineElement对象
        /// </summary>
        /// <returns></returns>
        public BasicShapes.SVGLineElement CreateSVGLineElement()
        {
            return this.CreateElement("line") as BasicShapes.SVGLineElement;
        }

        /// <summary>
        /// 创建一个SVGUseElement对象
        /// </summary>
        /// <returns></returns>
        public SVG.DocumentStructure.SVGUseElement CreateSVGUseElement()
        {
            return this.CreateElement("use") as DocumentStructure.SVGUseElement;
        }

        /// <summary>
        /// 创建一个SVGImageElement对象
        /// </summary>
        /// <returns></returns>
        public SVG.DocumentStructure.SVGImageElement CreateSVGImageElement()
        {
            return this.CreateElement("image") as DocumentStructure.SVGImageElement;
        }

        /// <summary>
        /// 创建一个SVGTextElement对象
        /// </summary>
        /// <returns></returns>
        public SVG.Text.SVGTextElement CreateSVGTextElement()
        {
            return this.CreateElement("text") as Text.SVGTextElement;
        }

        /// <summary>
        /// 创建一个SVGTextBlockElement对象
        /// </summary>
        /// <returns></returns>
        public SVG.Text.SVGTextBlockElement CreateSVGTextBlockElement()
        {
            return this.CreateElement("textBlock") as Text.SVGTextBlockElement;
        }
        #endregion

        #region ..GetElementByID
        public new SVGElement GetElementById(string id)
        {
            return this.GetReferencedNode(id) as SVGElement;
        }
        #endregion

        #region ..AddRefDocument
        /// <summary>
        /// 添加引用文档
        /// 引用文档中的图元可以被本文档直接引用
        /// </summary>
        /// <param name="doc"></param>
        //public void AddRefDocument(SVGDocument doc)
        //{
        //    if (!this.refDocuments.Contains(doc))
        //        this.refDocuments.Add(doc);
        //}
        //#endregion

        //#region ..ClearRefDocuments
        ///// <summary>
        ///// 取消所有引用文档
        ///// </summary>
        //public void ClearRefDocuments()
        //{
        //    this.refDocuments.Clear();
        //}
        #endregion
    }
}
