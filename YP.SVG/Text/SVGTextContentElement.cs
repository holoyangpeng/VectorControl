using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing.Text;
using System.Collections.Generic;
using YP.SVG.Interface.DataType;
using YP.SVG.Interface.CTS;

namespace YP.SVG.Text
{
	/// <summary>
	/// 定义文本基类，实现SVGTextElement, SVGTSpanElement, SVGTRefElement, SVGAltGlyphElement and SVGTextPathElement等节点
	/// </summary>
	public abstract class SVGTextContentElement:SVGTransformableElement,
        Interface.Text.ISVGTextContentElement,
        Interface.Text.ITextElement
    {
        #region ..static
        static Regex textRegex = new Regex(@"^\d", RegexOptions.Compiled);
        #endregion

        #region ..构造及消除
        public SVGTextContentElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			//this.gp = new GraphicsPath();
		}
		#endregion

		#region ..公共属性
        public override string Label
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        /// <summary>
        /// 获取文本信息
        /// </summary>
        public List<TextContentInfo> TextContentInfos
        {
            get
            {
                if (this.textContentInfos.Count == 0)
                {
                    this.ParseTextContentInfo();
                }
                return this.textContentInfos;
            }
        }

		/// <summary>
		/// 获取长度调整类别
		/// </summary>
		public System.Enum LengthAdjust
		{
			get
			{
				LengthAdjustType lengthadjust = LengthAdjustType.LENGTHADJUST_SPACING;
				if(this.GetAttribute("lengthAdjust").Trim() == "spacingAndGlyphs")
					lengthadjust = LengthAdjustType.LENGTHADJUST_SPACINGANDGLYPHS;
				return lengthadjust;
			}
		}

		/// <summary>
		/// 获取TextLength属性
		/// </summary>
		public ISVGLength TextLength
		{
			get
			{
				return new DataType.SVGLength(this.GetAttribute("textLength"),this,LengthDirection.Viewport,"0");//);
			}
		}

		/// <summary>
		/// 获取最顶层的Text对象
		/// </summary>
		public YP.SVG.Text.SVGTextElement OwnerTextElement
		{
			get
			{
				YP.SVG.Text.SVGTextElement text = null;
				System.Xml.XmlNode parent = this;
				while(parent != null)
				{
					if(parent is YP.SVG.Text.SVGTextElement)
					{
						text = parent as SVGTextElement;
						break;
					}
					parent = parent.ParentNode;
				}
				return text;
			}
		}
		#endregion

		#region ..绘制相关
		/// <summary>
		/// 确定是否绘制
		/// </summary>
		public bool Render = true;
        
        public bool ParseContent = true;
		#endregion

		#region ..私有变量
        string preInnerXml = null;
        List<TextContentInfo> textContentInfos = new List<TextContentInfo>();
		System.Drawing.Drawing2D.GraphicsPath oriPath = new GraphicsPath();
		ArrayList poses =new ArrayList();
		Cache.TextCache textCache = new YP.SVG.Cache.TextCache();
		#endregion

        #region ..常量
        const string TabString = "    ";
		#endregion

        #region ..GetNumberOfChars
        /// <summary>
		/// 获取当前节点中将要绘制的总字符数目，包括由“tref”所指向的文本节点字符长度
		/// </summary>
		/// <returns></returns>
		public virtual int GetNumberOfChars()
		{
			int i = 0;
			foreach(Text.TextContentInfo info in this.textContentInfos)
			{
				i += info.TextContent.Length;
			}
			return i;
		}
		#endregion

        #region ..GetComputedTextLength
        /// <summary>
		/// 当绘制全部字符时，所需要的总的GDI长度
		/// </summary>
		/// <returns></returns>
		public virtual float  GetComputedTextLength ()
		{
			throw new NotImplementedException();
		}
		#endregion

        #region ..GetSubStringLength
        /// <summary>
		/// 绘制子字符串所需要的长度
		/// </summary>
		/// <param name="charnum">起始字符索引</param>
		/// <param name="nchars">子字符串长度</param>
		/// <returns></returns>
		public float  GetSubStringLength (int charnum,int nchars )
		{
			throw new NotImplementedException();
		}
		#endregion

        #region ..GetStartPositionOfChar
        /// <summary>
		/// 获取指定索引处字符的开始绘制位置
		/// </summary>
		public ISVGPoint GetStartPositionOfChar(int charnum)
		{
			TextContentInfo posinfo = this.GetTextContentInfoOfChar(ref charnum);
			if(posinfo != null)
			{
				PointF p = posinfo.GetStartPositionOfChar(charnum);
				return new YP.SVG.DataType.SVGPoint(p.X,p.Y);
			}
			PointF p1 = PointF.Empty;
			if(this is Text.SVGTextPositionElement)
			{
				YP.SVG.Text.SVGTextPositionElement pos = (YP.SVG.Text.SVGTextPositionElement)this;
				if(((YP.SVG.DataType.SVGLengthList)pos.X).NumberOfItems > 0)
					p1.X = ((YP.SVG.DataType.SVGLength)pos.X.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dx).NumberOfItems > 0)
					p1.X += ((YP.SVG.DataType.SVGLength)(pos.Dx.GetItem(0))).Value;

				if(((YP.SVG.DataType.SVGLengthList)pos.Y).NumberOfItems > 0)
					p1.Y = ((YP.SVG.DataType.SVGLength)pos.Y.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dy).NumberOfItems > 0)
					p1.Y += ((YP.SVG.DataType.SVGLength)(pos.Dy.GetItem(0))).Value;
			}
			return new YP.SVG.DataType.SVGPoint(p1.X,p1.Y);
		}
		#endregion

        #region ..GetEndPositionOfChar
        /// <summary>
		/// 获取指定索引处字符绘制结束时的位置
		/// </summary>
		public ISVGPoint GetEndPositionOfChar (int charnum )
		{
			TextContentInfo posinfo = this.GetTextContentInfoOfChar(ref charnum);
			if(posinfo != null)
			{
				PointF p = posinfo.GetEndPositionOfChar(charnum);
				return new YP.SVG.DataType.SVGPoint(p.X,p.Y);
			}
			PointF p1 = PointF.Empty;
			if(this is Text.SVGTextPositionElement)
			{
				YP.SVG.Text.SVGTextPositionElement pos = (YP.SVG.Text.SVGTextPositionElement)this;
				if(((YP.SVG.DataType.SVGLengthList)pos.X).NumberOfItems > 0)
					p1.X = ((YP.SVG.DataType.SVGLength)pos.X.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dx).NumberOfItems > 0)
					p1.X += ((YP.SVG.DataType.SVGLength)(pos.Dx.GetItem(0))).Value;

				if(((YP.SVG.DataType.SVGLengthList)pos.Y).NumberOfItems > 0)
					p1.Y = ((YP.SVG.DataType.SVGLength)pos.Y.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dy).NumberOfItems > 0)
					p1.Y += ((YP.SVG.DataType.SVGLength)(pos.Dy.GetItem(0))).Value;
			}
			return new YP.SVG.DataType.SVGPoint(p1.X,p1.Y);
		}
		#endregion

        #region ..GetExtentOfChar
        /// <summary>
		/// 获取指定索引处字符的绘制边界
		/// </summary>
		public ISVGRect  GetExtentOfChar (int charnum)
		{
			TextContentInfo posinfo = this.GetTextContentInfoOfChar(ref charnum);
			RectangleF rect = RectangleF.Empty;
			if(posinfo != null)
			{
				return new YP.SVG.DataType.SVGRect(posinfo.GetExtentOfChar(charnum));
			}

			return new YP.SVG.DataType.SVGRect(RectangleF.Empty);
		}
		#endregion

        #region ..GetRotationOfChar
        /// <summary>
		/// 获取指定索引处字符相对于当前用户空间的旋转角度
		/// </summary>
		/// <param name="charnum">字符索引</param>
		/// <returns></returns>
		public float GetRotationOfChar (int charnum )
		{
			throw new NotImplementedException();
		}
        #endregion

        #region ..GetCharNumAtPosition
        /// <summary>
        /// 获取指定位置处的字符索引
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int GetCharNumAtPosition(ISVGPoint point, ref TextContentInfo caretInfo)
		{
			PointF p = new PointF(point.X,point.Y);
			TextContentInfo posinfo = null;
			int index = 0;
			RectangleF pbounds = (this.OwnerTextElement as SVG.Interface.ISVGPathable).GPath.GetBounds();
			bool xlower = p.X < pbounds.X;
			bool ylower = p.Y < pbounds.Y;
			bool xupper = p.X > pbounds.Right;
			bool yupper = p.Y > pbounds.Bottom;
			TextContentInfo xminInfo = null;
			TextContentInfo yminInfo = null;
			float minx = float.MaxValue;
			float miny = float.MaxValue;
			for(int i =0;i<this.textContentInfos.Count;i++)//foreach(TextContentInfo info in this.textContentInfos)
			{
				TextContentInfo info = (TextContentInfo)this.textContentInfos[i];
				RectangleF bounds = info.Bounds;
				if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical) != StringFormatFlags.DirectionVertical)
				{
					if(bounds.Contains(p))
					{
						posinfo = info;
						break;
					}
					if(p.Y >= bounds.Top && p.Y < bounds.Bottom)
					{
						float x = bounds.X - p.X;
						if(x < minx)
						{
							minx = x;
							xminInfo = info;
						}
					}
					float y = bounds.Y - p.Y;
					if(Math.Abs(y) < Math.Abs(miny))
					{
						miny = y;
						yminInfo = info;
					}
				}
				else
				{
					if(bounds.Contains(p))
					{
						posinfo = info;
						break;
					}
				}
			}
			if(posinfo == null)
			{
				if(xminInfo != null)
					posinfo = xminInfo;
				else
					posinfo = yminInfo;
			}
			if(posinfo != null)
			{
				caretInfo = posinfo;
				index = posinfo.GetCharNumAtPosition(point);
				return index;
			}
			caretInfo = null;
			return 0;
		}
		#endregion

        #region ..SelectSubString
        /// <summary>
		/// 选择子字符串
		/// </summary>
		/// <param name="charnum">开始字符索引</param>
		/// <param name="nchars">字符串长度</param>
		public void SelectSubString (int charnum,int nchars )
		{
			throw new NotImplementedException();
		}
        #endregion

        #region ..GetGDIString
        /// <summary>
        /// 获取绘制文本
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <returns></returns>
        public string GetGDIString(string oritext)
		{
			if(this.XmlSpace == "preserve") 
				return oritext;
			else 
				return oritext.Trim();
		}
		#endregion

        #region ..SegGraphicsPath
        /// <summary>
		/// 设置GDI绘制路径
		/// </summary>
		/// <param name="path"></param>
        public void SegGraphicsPath(GraphicsPath path)
		{
			this.graphicsPath = path;
		}
		#endregion

        #region ..ParseTextContentInfo
        /// <summary>
		/// 解析文本信息
		/// </summary>
		public bool ParseTextContentInfo()
		{
            if (this.preInnerXml == this.InnerXml || !ParseContent)
            {
                bool b = this.preInnerXml != this.InnerXml;
                //this.preInnerXml = this.InnerXml;
                return b;
            }
            this.preInnerXml = this.InnerXml;
			this.textContentInfos.Clear();
			bool old = this.OwnerDocument.inLoadProcess;
			this.OwnerDocument.inLoadProcess = true;
			this.Normalize();
			this.OwnerDocument.inLoadProcess = old;
			float emSize = 12;
			//不提交，不同步
			old = this.OwnerDocument.AcceptNodeChanged;
			this.OwnerDocument.AcceptNodeChanged = false;
			System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
			using(FontFamily family = new FontFamily("Arial"))
			{
				using(System.Drawing.Font font = new Font(family,emSize,style))
				{
                    using (SVG.StyleContainer.StyleOperator sp = new SVG.StyleContainer.StyleOperator())
                    {
                        System.Xml.XmlElement element = this;
                        if (this is YP.SVG.Text.SVGTRefElement)
                            element = ((YP.SVG.Text.SVGTRefElement)this).RefElement;
                        if (element != null)
                        {
                            sp.BeginStyleContainer(element as SVGStyleable);
                            if (!(element.FirstChild is System.Xml.XmlText))
                            {
                                System.Xml.XmlText textnode = element.OwnerDocument.CreateTextNode("");
                                if (textnode != null)
                                    (element as SVGElement).InternalPrependChild(textnode);
                            }
                            StringFormat sf = SVG.Render.SVGTextRenderer.GetStringFormat(this,sp);
                            foreach (System.Xml.XmlNode child in element.ChildNodes)
                            {
                                if (child is System.Xml.XmlText)
                                {
                                    string text = this.GetGDIString(child.Value);
                                    if (text.Length > 0 || this.textContentInfos.Count == 0)
                                    {
                                        Text.TextContentInfo info = new TextContentInfo(text, font, sf, this);
                                        info.OwnerTextNode = (System.Xml.XmlText)child;
                                        this.textContentInfos.Add(info);
                                    }
                                }
                                else if (child is SVGTextContentElement)
                                {
                                    ((SVGTextContentElement)child).ParseTextContentInfo();
                                    this.textContentInfos.AddRange(((SVGTextContentElement)child).textContentInfos);
                                }
                            }

                            sp.EndContainer(element as SVGStyleable);
                        }
                    }
				}
			}
			this.OwnerDocument.AcceptNodeChanged = old;
            return true;
		}
		#endregion

		#region ..获取节点相对于指定位置的坐标
		/// <summary>
		/// 获取节点相对于指定位置的坐标
		/// </summary>
		/// <param name="refPos">参考位置</param>
		/// <returns></returns>
		public static float GetReferencedShiftBy(float textFontSize,string sBaselineShift)
		{
			float shiftBy = 0;
			if(string.Compare(sBaselineShift,"") != 0)
			{
				if(sBaselineShift.EndsWith("%"))
				{
					shiftBy = DataType.SVGNumber.ParseNumberStr(sBaselineShift.Substring(0, sBaselineShift.Length-1)) / 100 * textFontSize;
				}
				else if(string.Compare(sBaselineShift,"sub")==0)
				{
					shiftBy = -0.6F * textFontSize;
				}
				else if(string.Compare(sBaselineShift,"super")==0)
				{
					shiftBy = 0.6F * textFontSize;
				}
				else if(string.Compare(sBaselineShift,"baseline")==0)
				{
					shiftBy = 0;
				}
				else
				{
					shiftBy = DataType.SVGNumber.ParseNumberStr(sBaselineShift);
				}
			}

			return shiftBy;
		}
		#endregion

		#region ..获取指定索引处字符所在TextContentInfo
		/// <summary>
		/// 获取指定索引处字符所在TextContentInfo
		/// </summary>
		public TextContentInfo GetTextContentInfoOfChar(ref int charnum)
		{
			TextContentInfo posinfo = null;
			int i = 0;
			int index = 0;
			foreach(TextContentInfo info in this.textContentInfos)
			{
				
				if(i + info.TextContent.Length > charnum || index == this.textContentInfos.Count - 1)
				{
					charnum -= i;
					posinfo = info;
					break;
				}
				i += info.TextContent.Length;
				index ++;
			}
			return posinfo;
		}
		#endregion

		#region ..删除指定的文本信息
		/// <summary>
		/// 删除指定的文本信息
		/// </summary>
		/// <param name="info">要删除的文本信息</param>
        public void RemoveInfo(Text.TextContentInfo info)
		{
			if(this.textContentInfos.Contains(info))
			{
				if(this.textContentInfos.Count == 1)
				{
					System.Xml.XmlText textnode = this.OwnerDocument.CreateTextNode("");
					if(textnode != null)
						this.InternalAppendChild(textnode);
					info.OwnerTextNode = textnode;
				}
				else
				{
					int index = this.textContentInfos.IndexOf(info);
					InfoUndoOperation op = new InfoUndoOperation(this,index,info,InfoAction.Remove);
					this.textContentInfos.Remove(info);
					this.OwnerDocument.PushUndo(op);
//					info.Dispose();
//					info = null;
				}
			}
		}
		#endregion

		#region ..插入文本信息
		/// <summary>
		/// 插入文本信息
		/// </summary>
		/// <param name="index">插入索引</param>
		/// <param name="info">文本信息</param>
        public void InsertInfo(int index, YP.SVG.Text.TextContentInfo info)
		{
			if(index >= 0 && index < this.textContentInfos.Count)
			{
				InfoUndoOperation op = new InfoUndoOperation(this,index,info,InfoAction.Insert);
				this.textContentInfos.Insert(index,info);
				this.OwnerDocument.PushUndo(op);
			}
			else
			{
				index = this.textContentInfos.Count;
				InfoUndoOperation op = new InfoUndoOperation(this,index,info,InfoAction.Insert);
				this.textContentInfos.Add(info);
				this.OwnerDocument.PushUndo(op);
			}
		}
		#endregion

		#region ..更新文本
		/// <summary>
		/// 更新文本
		/// </summary>
        public void UpdateText()
        {
            if (!this.OwnerDocument.IsActive)
                return;
            if (this is SVGTextElement)
            {
                if (this.IsActive)
                {
                    this.OwnerDocument.RefreshOriginalElement(this);
                    this.UpdateElement(true);
                    if (this.ParseTextContentInfo())
                    {
                        this.UpdatePath();
                        this.OwnerDocument.RefreshElement(this);
                    }
                }
            }
            else if (this.ParentElement is SVGTextContentElement)
            {
                (this.ParentElement as SVGTextContentElement).ParseContent = this.ParseContent;
                (this.ParentElement as SVGTextContentElement).UpdateText();
            }
        }
		#endregion

        #region ..RefreshTextContent
        public void RefreshTextContent()
        {
            this.OwnerDocument.RefreshElement(this, true);
        }
        #endregion

        #region ..InternalRemoveChild
        public override System.Xml.XmlNode RemoveChild(System.Xml.XmlNode oldChild)
        {
            if (oldChild is System.Xml.XmlText)
                this.OwnerDocument.RefreshOriginalElement(this);
            System.Xml.XmlNode node = base.InternalRemoveChild(oldChild);
            if (oldChild is System.Xml.XmlText)
                this.UpdateText();

            return node;
        }
        #endregion

        #region ..InternalAppendChild
        public override System.Xml.XmlNode AppendChild(System.Xml.XmlNode newChild)
        {
            if (newChild is System.Xml.XmlText && this.IsActive)
                this.OwnerDocument.RefreshOriginalElement(this);
            System.Xml.XmlNode node = base.InternalAppendChild(newChild);
            if (newChild is System.Xml.XmlText)
                this.UpdateText();

            return node;
        }
        #endregion

        #region ..ITextElement
        void Interface.Text.ITextElement.UpdateText()
        {
            this.UpdateText();
        }
        #endregion
    }
}
