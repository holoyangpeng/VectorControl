using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.CTS;

namespace YP.SVG.Text
{
	/// <summary>
	/// 记录文本信息
	/// </summary>
	public class TextContentInfo:IDisposable
	{
		#region ..构造及消除
		public TextContentInfo()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		/// <summary>
		/// 通过指定的字符串，字体和布局信息构造TextContentInfo
		/// </summary>
		/// <param name="textContent"></param>
		/// <param name="textFont"></param>
		/// <param name="stringFormat"></param>
		public TextContentInfo(string textContent,Font textFont,StringFormat stringFormat,SVGTextContentElement ownerTextContentElement)
		{
			this.textContent = textContent;
			this.textFont = textFont;
			this.stringFormat = stringFormat;
			this.ownerTextContentElement = ownerTextContentElement;
		}

		public void Dispose()
		{
			this.textFont.Dispose();
			this.textFont = null;
			this.stringFormat.Dispose();
			this.stringFormat = null;
            GC.SuppressFinalize(this);
            GC.Collect(0);
		}

		#endregion   

		#region ..私有变量
		string textContent = string.Empty;
		public System.Drawing.Font textFont = new Font("Arial",12);
		StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
		SVGTextContentElement ownerTextContentElement = null;
		Hashtable startposes = new Hashtable();
		Hashtable endposes = new Hashtable();
		RectangleF bounds = RectangleF.Empty;
		System.Xml.XmlText ownerTextNode = null;
		Hashtable letterHeights = new Hashtable();
		string tabstring = " ";
		#endregion

		#region ..保护变量
		public YP.SVG.DataType.SVGLengthList X;
		public YP.SVG.DataType.SVGLengthList Y;
		public YP.SVG.DataType.SVGLengthList Dx;
		public YP.SVG.DataType.SVGLengthList Dy;
		#endregion

		#region ..保护属性
		/// <summary>
		/// 获取绘制边界
		/// </summary>
		public RectangleF Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
			}
		}
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置文本内容
		/// </summary>
		public string TextContent
		{
			get
			{
				return this.textContent;
			}
		}

		/// <summary>
		/// 获取或设置所关联的文本节点
		/// </summary>
		public System.Xml.XmlText OwnerTextNode
		{
			set
			{
				this.ownerTextNode = value;
			}
			get
			{
				return this.ownerTextNode;
			}
		}

		/// <summary>
		/// 获取或设置文本字体
		/// </summary>
		public System.Drawing.Font TextFont
		{
			set
			{
				this.textFont = value;
			}
			get
			{
				return this.textFont;
			}
		}

		/// <summary>
		/// 获取或设置文本布局信息
		/// </summary>
		public StringFormat TextStringFormat
		{
			set
			{
				this.stringFormat = value;
			}
			get
			{
				return this.stringFormat;
			}
		}

		/// <summary>
		/// 获取或设置其所属的文本节点
		/// </summary>
		public SVGTextContentElement OwnerTextContentElement
		{
			set
			{
				this.ownerTextContentElement = value;
			}
			get
			{
				return this.ownerTextContentElement;
			}
		}
		#endregion

		#region ..获取文本路径
		/// <summary>
		/// 获取文本路径
		/// </summary>
		/// <param name="refpos">参考位置</param>
		/// <param name="ownerTextElements">记录解析过的文本节点</param>
		public GraphicsPath GetGraphicsPath(Graphics g,float shift,ref PointF refpos,ref int xindex,ref int yindex,ref int dxindex,ref int dyindex,YP.SVG.StyleContainer.StyleOperator sp)
		{
			PointF start = new PointF(refpos.X,refpos.Y);
			this.startposes.Clear();
			this.endposes.Clear();
			this.letterHeights.Clear();
			GraphicsPath gp2 = new GraphicsPath();
			gp2.StartFigure();
			string text = this.textContent;
			FontFamily family = this.TextFont.FontFamily;
			FontStyle style = this.textFont.Style;
			float emSize = this.textFont.Size;
			StringFormat sf = this.stringFormat;

			float xCorrection1 = 0;
			try
			{
                if (sp.TextStyle.letter_spacing.Value.Length > 0 && sp.TextStyle.letter_spacing != "auto" && sp.TextStyle.letter_spacing != "inherit")
				{
                    YP.SVG.DataType.SVGLength length = new YP.SVG.DataType.SVGLength(sp.TextStyle.letter_spacing.Value, this.ownerTextContentElement, YP.SVG.LengthDirection.Viewport);
					xCorrection1 = length.Value;
				}
				if(sp.TextStyle.kerning.Value.Length > 0 && sp.TextStyle.kerning != "auto" && sp.TextStyle.kerning != "inherit")
				{
                    YP.SVG.DataType.SVGLength length = new YP.SVG.DataType.SVGLength(sp.TextStyle.kerning.Value, this.ownerTextContentElement, YP.SVG.LengthDirection.Viewport);
					xCorrection1 += length.Value;
				}
			}
			catch
			{
			}

            float yCorrection = (float)(family.GetCellAscent(FontStyle.Regular)) / (float)(family.GetEmHeight(FontStyle.Regular)) * emSize;

			bool vert = (sf.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical;

			float lineheight = (float)this.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)this.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * this.textFont.Size;
			if(this.textContent.Length > 0)
			{
				if((sf.FormatFlags & StringFormatFlags.DirectionRightToLeft) == StringFormatFlags.DirectionRightToLeft)
				{
					for(int i = this.textContent.Length-1;i>=0;i--)
					{
                        //if (text[i] == '\n')// || text[i] == ' ' || text[i] == '\t')
                        //    continue;
						if(xindex < this.X.NumberOfItems)
							refpos.X = ((YP.SVG.DataType.SVGLength)this.X.GetItem(xindex)).Value;
						if(yindex < this.Y.NumberOfItems)
							refpos.Y = ((YP.SVG.DataType.SVGLength)this.Y.GetItem(yindex)).Value;
						if(dxindex < this.Dx.NumberOfItems)
							refpos.X += ((YP.SVG.DataType.SVGLength)this.Dx.GetItem(dxindex)).Value;
						if(dyindex < this.Dy.NumberOfItems)
							refpos.Y += ((YP.SVG.DataType.SVGLength)this.Dy.GetItem(dyindex)).Value;
						PointF p = new PointF(refpos.X-(!vert?0:(yCorrection + shift)), refpos.Y - (vert?0:(yCorrection + shift)));
						string a = text[i].ToString();
                        if (text[i] == '\t')
                            a = this.tabstring;
						gp2.AddString(a, family, (int)style, emSize, p, sf);
						xindex ++;
						yindex ++;
						dxindex ++;
						dyindex ++;
						this.startposes.Add(i,p);
						SizeF size = g.MeasureString(a,this.textFont,280000,sf);
						refpos.X -= size.Width * 0.75f + xCorrection1;
						this.endposes.Add(i,new PointF(refpos.X,p.Y));
						this.letterHeights.Add(i,size.Height);
						a = null;
					}
				}
				else
				{
                    if (vert)
                        yCorrection *= 0.55f;
					for(int i = 0;i<this.textContent.Length;i++)
					{
                        //if (text[i] == '\n' )//|| text[i] == ' ' || text[i] == '\t')
                        //    continue;
						if(xindex < this.X.NumberOfItems)
							refpos.X = ((YP.SVG.DataType.SVGLength)this.X.GetItem(xindex)).Value;
						if(yindex < this.Y.NumberOfItems)
							refpos.Y = ((YP.SVG.DataType.SVGLength)this.Y.GetItem(yindex)).Value;
						if(dxindex < this.Dx.NumberOfItems)
							refpos.X += ((YP.SVG.DataType.SVGLength)this.Dx.GetItem(dxindex)).Value;
						if(dyindex < this.Dy.NumberOfItems)
							refpos.Y += ((YP.SVG.DataType.SVGLength)this.Dy.GetItem(dyindex)).Value;
						PointF p = new PointF(refpos.X-(!vert?0:(yCorrection + shift)), refpos.Y - (vert?0:(yCorrection + shift)));

						string a = text[i].ToString();
                        if (text[i] == '\t')
                            a = this.tabstring;
						gp2.AddString(a, family, (int)style, emSize, p, sf);

						xindex ++;
						yindex ++;
						dxindex ++;
						dyindex ++;
						this.startposes.Add(i,p);
						if((sf.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
						{
							SizeF size = g.MeasureString(a,this.textFont,280000,sf);
							float width = size .Height * 0.75f + xCorrection1;
							refpos.Y += width;
							p.Y += width;
							this.letterHeights.Add(i,(yCorrection + shift));
						}
						else
						{
							SizeF size = g.MeasureString(a,this.textFont,280000,sf);
							float width = size.Width * 0.75f + xCorrection1;
							refpos.X += width;
							p.X += width;
							this.letterHeights.Add(i,(yCorrection + shift));
						}
						a = null;
						this.endposes.Add(i,p);
					}
				}
			}
			else
			{
				if(xindex < this.X.NumberOfItems)
					refpos.X = ((YP.SVG.DataType.SVGLength)this.X.GetItem(xindex)).Value;
				if(yindex < this.Y.NumberOfItems)
					refpos.Y = ((YP.SVG.DataType.SVGLength)this.Y.GetItem(yindex)).Value;
				if(dxindex < this.Dx.NumberOfItems)
					refpos.X += ((YP.SVG.DataType.SVGLength)this.Dx.GetItem(dxindex)).Value;
				if(dyindex < this.Dy.NumberOfItems)
					refpos.Y += ((YP.SVG.DataType.SVGLength)this.Dy.GetItem(dyindex)).Value;
			}
			this.bounds = gp2.GetBounds();

            if (!vert)
                this.bounds.Height = lineheight;
            else
                this.bounds.Width = lineheight;
            if (this.bounds.IsEmpty)
            {
                if (!vert)
                {
                    float width = (float)Math.Max(2, bounds.Width);
                    float height = bounds.Height;
                    if (bounds.Height == 0)
                        height = (float)this.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)this.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * this.textFont.Size * 0.75f;
                    this.bounds = new RectangleF(new PointF(refpos.X, refpos.Y - yCorrection - shift), new SizeF(width, height));
                }
                else
                {
                    float height = (float)Math.Max(2, bounds.Height);
                    float width = bounds.Width;
                    if (bounds.Width == 0)
                        width = (float)this.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)this.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * this.textFont.Size * 0.75f;
                    this.bounds = new RectangleF(new PointF(refpos.X - yCorrection - shift, refpos.Y), new SizeF(width, height));
                }
            }
			return gp2;
		}
		#endregion

		#region ..获取指定索引处字符的开始绘制位置
		/// <summary>
		/// 获取指定索引处字符的开始绘制位置
		/// </summary>
		public PointF GetStartPositionOfChar(int charnum)
		{
			if(this.startposes.ContainsKey(charnum))
				return (PointF)this.startposes[charnum];
			if(charnum >= this.textContent.Length && this.endposes.ContainsKey(this.textContent.Length - 1))
				return (PointF)this.endposes[this.textContent.Length -1];
			return this.bounds.Location;
		}
		#endregion

		#region ..获取指定索引处字符绘制结束时的位置
		/// <summary>
		/// 获取指定索引处字符绘制结束时的位置
		/// </summary>
		public PointF GetEndPositionOfChar (int charnum )
		{
			if(this.endposes.ContainsKey(charnum))
				return (PointF)this.endposes[charnum];
			if(charnum >= this.textContent.Length && this.endposes.ContainsKey(this.textContent.Length -1))
				return (PointF)this.endposes[this.textContent.Length -1];
			return this.bounds.Location;
		}
		#endregion

		#region ..获取指定索引处字符的绘制边界
		/// <summary>
		/// 获取指定索引处字符的绘制边界
		/// </summary>
		public RectangleF GetExtentOfChar(int charnum)
		{
			if(this.textFont !=null && this.stringFormat != null)
			{
				PointF p = this.GetStartPositionOfChar(charnum);
				PointF p1 = this.GetEndPositionOfChar(charnum);
				float height = (float)this.textFont.FontFamily.GetLineSpacing(FontStyle.Regular )/(float)(this.textFont.FontFamily.GetEmHeight(FontStyle.Regular)) * this.textFont.Size;
				if((this.stringFormat.FormatFlags & StringFormatFlags.DirectionVertical)!= StringFormatFlags.DirectionVertical)
					return new RectangleF(p.X,p.Y,p1.X - p.X,height);
				else
					return new RectangleF(p.X,p.Y,height,(float)Math.Max(1,p1.Y - p.Y));
			}
			else
				return RectangleF.Empty;
		}
		#endregion

		#region ..获取指定位置处的字符索引
		/// <summary>
		/// 获取指定位置处的字符索引
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public int GetCharNumAtPosition (ISVGPoint point)
		{
			PointF p = new PointF(point.X,point.Y);
			for(int i = 0;i<this.textContent.Length;i++)
			{
				PointF startpos = (PointF)this.startposes[i];
				if((this.stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
				{
					if(startpos.Y >= p.Y)
						return i;
				}
				else
				{
					if(startpos.X >= p.X)
						return i;
				}
			}
			return this.textContent.Length;
		}
		#endregion

		#region ..插入字符串
		/// <summary>
		/// 插入字符串
		/// </summary>
		/// <param name="index">插入索引</param>
		/// <param name="str">要插入的字符串</param>
		/// <returns>操作以后的文本</returns>
		public string InsertString(int index,string str)
		{
			if(index >= 0 && index < this.textContent.Length)
			{
                bool old = this.ownerTextContentElement.ParseContent;
                try
                {
                    TextUndoOperation undo = new TextUndoOperation(this, index, 0, string.Empty, str);
                    this.textContent = this.textContent.Insert(index, str);
                    this.ownerTextContentElement.ParseContent = false;
                    if (this.ownerTextContentElement.Value != this.textContent)
                        this.ownerTextNode.Value = this.textContent;
                    this.ownerTextContentElement.OwnerDocument.PushUndo(undo);
                }
                finally
                {
                    this.ownerTextContentElement.ParseContent = old;
                }
			}
			else
				this.AppendString(str);
			
			str = null;
			return this.textContent;
		}
		#endregion

		#region ..添加字符串
		/// <summary>
		/// 将指定的字符串添加到原有字符串的末尾
		/// </summary>
		/// <param name="str"></param>
		public string AppendString(string str)
		{
//			int offset = this.textContent.Length;
//			PointF start = this.bounds.Location;
//			if(this.startposes.ContainsKey(offset -1))
//				start = (PointF)this.startposes[offset -1];
//			PointF end = this.bounds.Location;
//			if(this.endposes.ContainsKey(offset -1))
//				end = (PointF)this.endposes[offset - 1];
            bool old = this.ownerTextContentElement.ParseContent;
            try
            {
                TextUndoOperation undo = new TextUndoOperation(this, this.TextContent.Length, 0, string.Empty, str);
                this.textContent += str;
                this.ownerTextContentElement.ParseContent = false;
                if (this.ownerTextContentElement.Value != this.textContent)
                    this.ownerTextNode.Value = this.textContent;
                this.OwnerTextContentElement.OwnerDocument.PushUndo(undo);
                //			for(int i = offset;i<this.textContent.Length;i++)
                //			{
                //				this.startposes[i] = start;
                //				this.endposes[i] = end;
                //			}
                str = null;
            }
            finally
            {
                this.ownerTextContentElement.ParseContent = old;
            }
			return this.textContent;
		}
		#endregion

		#region ..删除字符串
		/// <summary>
		/// 删除字符串
		/// </summary>
		/// <param name="offset">指定开始删除的字符索引</param>
		/// <param name="length">指定删除的长度</param>
		/// <returns></returns>
		public string RemoveString(int offset,int length)
		{
            bool old = this.ownerTextContentElement.ParseContent;
            try
            {
                if (offset >= 0 && offset < this.textContent.Length && offset + length <= this.textContent.Length)
                {
                    TextUndoOperation undo = new TextUndoOperation(this, offset, length, this.textContent.Substring(offset, length), string.Empty);
                    this.textContent = this.textContent.Substring(0, offset) + this.textContent.Substring(offset + length);
                    this.ownerTextContentElement.OwnerDocument.PushUndo(undo);
                }
                this.ownerTextContentElement.ParseContent = false;
                this.ownerTextNode.Value = this.textContent;
            }
            finally
            {
                this.ownerTextContentElement.ParseContent = old;
            }
			return this.textContent;
		}
		#endregion

		#region ..替换字符串
		public void ReplaceString(int offset,int length,string text)
		{
            bool old = this.ownerTextContentElement.ParseContent;
            try
            {
                if (this.ownerTextContentElement != null)
                {
                    this.ownerTextContentElement.RefreshTextContent();
                    this.ownerTextContentElement.UpdatePath();
                    this.ownerTextContentElement.UpdateElement();
                }
                offset = (int)Math.Max(0, Math.Min(offset, this.textContent.Length));
                length = (int)Math.Min(this.textContent.Length - offset, length);
                if (offset < this.textContent.Length)
                {
                    this.textContent = this.textContent.Remove(offset, length);
                    this.textContent = this.textContent.Insert(offset, text);
                }
                else
                    this.textContent += text;
                this.ownerTextContentElement.ParseContent = false;
                if (this.ownerTextNode != null)
                    this.ownerTextNode.Value = this.textContent;
            }
            finally
            {
                this.ownerTextContentElement.ParseContent = old;
            }
            if (this.ownerTextContentElement != null)
                this.ownerTextContentElement.RefreshTextContent();
		}
		#endregion

		#region ..将所属的文本字符转换为单个的Text对象
		/// <summary>
		/// 将所属的文本字符转换为单个的Text对象
		/// </summary>
		/// <returns></returns>
		public YP.SVG.SVGElementCollection Break()
		{
			YP.SVG.SVGElementCollection list = new SVGElementCollection();
			YP.SVG.Text.SVGTextElement textElement = this.GetWholeAttributeTextElement();
			if(textElement != null)
			{
				YP.SVG.Document.SVGDocument doc = this.ownerTextContentElement.OwnerDocument;
				bool old = doc.AcceptNodeChanged;
				doc.AcceptNodeChanged = false;
				bool old1 = doc.inUseCreate;
				doc.inUseCreate = true;
				string text = this.textContent;
				for(int i = 0;i<this.textContent.Length;i++)
				{
					if(Char.IsWhiteSpace(this.textContent,i))
						continue;
					YP.SVG.Text.SVGTextElement temp = textElement.CloneNode(false) as SVGTextElement;
					temp.InnerText = this.textContent[i].ToString();
					if(this.startposes.ContainsKey(i))
					{
						PointF p = (PointF)this.startposes[i];
						float height = 0;
						if(this.letterHeights.ContainsKey(i))
							height = (float)this.letterHeights[i];
						temp.InternalSetAttribute("x",p.X.ToString());
						temp.InternalSetAttribute("y",(p.Y + height).ToString());
					}
					list.Add(temp);
				}
				text = null;
				doc.AcceptNodeChanged = old;
				doc.inUseCreate = old1;
			}
			return list;
		}

		/// <summary>
		/// 从最顶层的SCGTextElement开始，直到所属的TextNode，将每层的TextElement对象的属性合并，并最终生成一个SVGTextElement
		/// </summary>
		/// <returns></returns>
		YP.SVG.Text.SVGTextElement GetWholeAttributeTextElement()
		{
			if(this.ownerTextContentElement != null && this.ownerTextContentElement.OwnerDocument != null)
			{
				YP.SVG.Document.SVGDocument doc = this.ownerTextContentElement.OwnerDocument;
				bool old = doc.AcceptNodeChanged;
				bool old1 = doc.inUseCreate;
				doc.inUseCreate = true;
				doc.AcceptNodeChanged = false;
				YP.SVG.Text.SVGTextElement textElement = (YP.SVG.Text.SVGTextElement)doc.CreateElement(doc.Prefix,"text",doc.NamespaceURI);
				YP.SVG.Text.SVGTextContentElement content = this.ownerTextContentElement;
				while(content != null)
				{
					foreach(System.Xml.XmlAttribute attri in content.Attributes)
					{
						if(string.Compare(attri.Name , "x")==0
                            || string.Compare(attri.Name ,"y")==0
                            || string.Compare(attri.Name,"dx" ) ==0 
                            || string.Compare(attri.Name , "dy") ==0)
							continue;
						if(!textElement.HasAttribute(attri.LocalName,attri.NamespaceURI))
							textElement.InternalSetAttribute(attri.LocalName,attri.NamespaceURI,attri.Value);
					}
					//达到最顶层Text对象，跳出循环
					if(content is SVGTextElement)
						break;
					content = content.ParentElement as SVGTextContentElement;
				}
				doc.AcceptNodeChanged = old;
				doc.inUseCreate = old1;
				return textElement;
			}
			return null;
		}
		#endregion
	}
}
