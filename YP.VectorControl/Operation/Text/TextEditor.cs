using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using YP.SVG.Text;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// 实现文本对象的编辑
	/// </summary>
	internal class TextEditor:DisposeBase
	{
		#region ..构造及消除
		internal TextEditor(YP.SVG.Text.SVGTextElement ownerTextElement,Canvas mousearea,TextOperation textoperation)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ownerTextElement = ownerTextElement;
			this.ownerTextElement.Render = false;
			this.selection = new Selection(this);
			this.mouseArea = mousearea;
			this.caret = new Caret(this);
			string mode = this.ownerTextElement.GetAttribute("writing-mode").Trim();
			this.vertical =string.Compare(mode,"tb") ==0 ||string.Compare(mode,"tb-rl") ==0;
			YP.SVG.StyleContainer.StyleOperator sp = this.ownerTextElement.OwnerDocument.CreateStyleOperator();
			sp.BeginStyleContainer(this.ownerTextElement);
            float fontsize = this.ownerTextElement.TextRender.GetFontSize(sp, this.ownerTextElement);
            FontFamily family = this.ownerTextElement.TextRender.GetFontFamily(sp);
			this.textFont = new Font(family,fontsize);
			
			this.caretthread = new Thread(new ThreadStart(this.CaretMethod));
			this.caretthread.IsBackground = true;
			this.caretthread.Start();
			this.caret.CaretChanged +=new CaretChangedEventHandler(caret_CaretChanged);// += new EventHandler(ChangeCaretOffset);
			this.selection.SelectionChanged += new Text.SelectionChangedEventHandler(AdaptSelection);
			this.AttachActions();
			this.mouseArea.KeyPress += new KeyPressEventHandler(DealKeyPress);
			this.mouseArea.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.mouseArea.ImeMode = ImeMode.On;
			this.mouseArea.validContent = true;
			sp.Dispose();
			this.textOperation = textoperation;
			if(this.ownerTextElement!= null && this.ownerTextElement.TextContentInfos.Count > 0)
			{
				this.ownerTextElement.InEdit = true;
				this.Caret.AdaptCaret(this.ownerTextElement.TextContentInfos[0] as TextContentInfo,0);
			}
		}

		public override void Dispose()
		{
			this.dispose = true;
			this.caretthread.Abort();
			this.caretthread = null;
			this.textFont.Dispose();
			this.mouseArea.validContent = true;
			if(this.mouseArea.InDragDrop)
			{
				Bitmap bmp = this.mouseArea.GetContent();
				using(Graphics g = Graphics.FromImage(bmp))
				{
					if(this.ownerTextElement != null)
					{
						this.ownerTextElement.InEdit = false;
						g.SmoothingMode = SmoothingMode.HighQuality;
						this.ownerTextElement.Render = true;
						using(YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator(this.ownerTextElement.StyleContainer))
						{
							sp.drawConnects = true;
                            //if (this.ownerTextElement.ParentElement is SVGDom.SVGTransformableElement)
                            //    this.ownerTextElement.TotalTransform = (this.ownerTextElement.ParentElement as SVGDom.SVGTransformableElement).TotalTransform.Clone();
							sp.connectElements = this.mouseArea.connectableElements;
							this.ownerTextElement.SVGRenderer.Draw(g,sp);
						}
					}
				}
			}
			this.mouseArea.KeyPress -= new KeyPressEventHandler(DealKeyPress);
			if(this.ownerTextElement != null)
			{
				this.ownerTextElement.Render = true;
                this.ownerTextElement.InEdit = false;
//				this.ownerTextElement.OwnerDocument.ChangeSelectElement(this.ownerTextElement); 
			}
			this.selection.SelectionChanged -= new Text.SelectionChangedEventHandler(AdaptSelection);
			
			this.caret.CaretChanged -= new CaretChangedEventHandler(caret_CaretChanged);
			this.ownerTextElement = null;
			this.selection = null;
			this.caret = null;
			
			
			base.Dispose();
			
		}
		#endregion

		#region ..静态变量
		internal static System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex("(?:\r\n)|\r|\n");
		#endregion

		#region ..私有变量
		YP.SVG.Text.SVGTextElement ownerTextElement;
		Canvas mouseArea = null;
		//		Matrix totalTransform = new Matrix();
		Caret caret = null;
		Selection selection;
		Thread caretthread;
		Font textFont = new Font("Arial",12);
		Hashtable actions = new Hashtable();
		bool mousedown = false;
		Ime ime = null;
		InfoPos startpos = new InfoPos(null,0);
		TextOperation textOperation = null;
		bool dispose = false;
		bool vertical = false;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取包含的文本对象
		/// </summary>
		public YP.SVG.Text.SVGTextElement OwnerTextElement
		{
			get
			{
				return this.ownerTextElement;
			}
		}
		#endregion

		#region ..保护属性
		/// <summary>
		/// 获取编辑器的插入符号
		/// </summary>
		internal Caret Caret
		{
			get
			{
				return this.caret;
			}
		}

		/// <summary>
		/// 判断对象是否已经消除
		/// </summary>
		internal bool Disposed
		{
			get
			{
				return this.dispose;
			}
		}

		/// <summary>
		/// gets the total transform
		/// </summary>
		Matrix TotalTransform
		{
			get
			{
				if(this.ownerTextElement != null &&this.ownerTextElement.OwnerDocument.ScaleStroke)
					return this.mouseArea.GetTotalTransformForElement(this.ownerTextElement);
				return null;
			}
		}

		/// <summary>
		/// 获取编辑器的选区
		/// </summary>
		internal Selection Selection
		{
			get
			{
				return this.selection;
			}
		}
		#endregion

		#region ..OnMouseDown
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		internal bool OnMouseDown(MouseEventArgs e)
		{
			try
			{
				if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
				{
					PointF[] ps = new PointF[]{new PointF(e.X,e.Y)};
                    using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.ownerTextElement))
					{
						matrix.Invert();
						matrix.TransformPoints(ps);
						PointF p = ps[0];
						TextContentInfo info = null;
						int offset = this.ownerTextElement.GetCharNumAtPosition(new YP.SVG.DataType.SVGPoint(p.X,p.Y),ref info);
						if(info != null)
						{
							this.startpos = new InfoPos(info,offset);
							this.selection.AdaptSelection(startpos,startpos);
							this.Caret.AdaptCaret(info,offset);
						}
						this.mousedown = true;
					}
				}
			}
			catch{}
			return true;
			
		}
		#endregion

		#region ..OnMouseMove
		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		internal bool OnMouseMove(MouseEventArgs e)
		{
			try
			{
				if(e.Button == MouseButtons.Left && this.mousedown)
				{
					PointF[] ps = new PointF[]{new PointF(e.X,e.Y)};
                    using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.ownerTextElement))
					{
						matrix.Invert();
						matrix.TransformPoints(ps);
						PointF p = ps[0];
						TextContentInfo info = null;
						int offset = this.ownerTextElement.GetCharNumAtPosition(new YP.SVG.DataType.SVGPoint(p.X,p.Y),ref info);
						if(info != null)
						{
							this.selection.AdaptSelection(this.startpos,new InfoPos(info,offset));
							this.Caret.AdaptCaret(info,offset);
						}
					}
				}
			}
			catch{}
			return true;
			
		}
		#endregion

		#region ..OnMouseUp
		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		internal bool OnMouseUp(MouseEventArgs e)
		{
			try
			{
				if(e.Button == MouseButtons.Left && this.mousedown)
				{
					PointF[] ps = new PointF[]{new PointF(e.X,e.Y)};
                    using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.ownerTextElement))
					{
						matrix.Invert();
						matrix.TransformPoints(ps);
						PointF p = ps[0];
						TextContentInfo info = null;
						int offset = this.ownerTextElement.GetCharNumAtPosition(new YP.SVG.DataType.SVGPoint(p.X,p.Y),ref info);
						if(info != null)
						{
							this.selection.AdaptSelection(this.startpos,new InfoPos(info,offset));
							this.Caret.AdaptCaret(info,offset);
						}
					}
				}
			}
			catch{}
			this.mousedown = false;

			return true;
			
		}
		#endregion

		#region ..OnPaint
		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			GraphicsContainer c = g.BeginContainer();
			g.SmoothingMode = SmoothingMode.HighQuality;
			try
			{
				if(this.dispose)
					return;
				if(this.ownerTextElement != null )
				{
					if(this.ownerTextElement.ParentNode == null)
					{
						this.textOperation.Reset();
						return;
					}

					this.mouseArea.validContent= false;
					
					this.ownerTextElement.Render = true;
					if(this.TotalTransform != null)
						g.Transform = this.TotalTransform.Clone();
					using(YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator(this.ownerTextElement.StyleContainer))
					{
                        sp.coordTransform = this.mouseArea.CoordTransform;
                        //if (this.ownerTextElement.ParentElement is SVGDom.SVGTransformableElement)
                        //    this.ownerTextElement.TotalTransform = (this.ownerTextElement.ParentElement as SVGDom.SVGTransformableElement).TotalTransform.Clone();
						//this.ownerTextElement.CreateTransform = false;
						this.ownerTextElement.SVGRenderer.Draw(g,sp);
						//this.ownerTextElement.CreateTransform = true;
					}
					this.PaintCaret(g);
					this.PaintSelection(g);
					this.ownerTextElement.Render = false;
				}
				
			}
			catch
			{
				
			}
			finally
			{
				g.EndContainer(c);
			}
		}
		#endregion

		#region ..Caret
		void PaintCaret(Graphics g)
		{
			if(this.caret.CaretVisible && this.selection.IsEmpty)
			{
				TextContentInfo info = this.Caret.Info;
				if(!this.ownerTextElement.TextContentInfos.Contains(info))
				{
					info = null;
					if(this.OwnerTextElement.TextContentInfos.Count > 0)
					{
						info = this.OwnerTextElement.TextContentInfos[0] as TextContentInfo;
						this.caret.AdaptCaret(info,0);
					}
				}
				PointF startp= info.GetStartPositionOfChar(this.Caret.Offset);
				float h = 100;
				if(info.TextFont != null)
					h = (float)info.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.textFont.Size;
				PointF[] ps = new PointF[]{new PointF(startp.X,startp.Y),new PointF(startp.X,startp.Y + h)};
				if(this.vertical)
					ps = new PointF[]{new PointF(startp.X,startp.Y),new PointF(startp.X + h,startp.Y )};
				if(!this.ownerTextElement.OwnerDocument.ScaleStroke)
                    this.mouseArea.GetTotalTransformForElement(this.ownerTextElement).TransformPoints(ps);
				g.DrawLine(Pens.Black,ps[0],ps[1]);
				ps = null;
			}
		}

		Point FindPointForOffset(int offset)
		{
			return FindPointForOffset(offset,true);
		}

		Point FindPointForOffset(int offset,bool total)
		{
			YP.SVG.DataType.SVGRect rect = (YP.SVG.DataType.SVGRect)this.ownerTextElement.GetExtentOfChar(offset);
			float x = rect.X;
			float y = rect.Y;
			float size = rect.Height;
			if(rect.GDIRect.Height == 0)
			{
				YP.SVG.Text.SVGTextElement pos = this.ownerTextElement;
				if(((YP.SVG.DataType.SVGLengthList)pos.X).NumberOfItems > 0)
					x = ((YP.SVG.DataType.SVGLength)pos.X.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dx).NumberOfItems > 0)
					x += ((YP.SVG.DataType.SVGLength)pos.Dx.GetItem(0)).Value;

				if(((YP.SVG.DataType.SVGLengthList)pos.Y).NumberOfItems > 0)
					y = ((YP.SVG.DataType.SVGLength)pos.Y.GetItem(0)).Value;
				if(((YP.SVG.DataType.SVGLengthList)pos.Dy).NumberOfItems > 0)
					y += ((YP.SVG.DataType.SVGLength)pos.Dy.GetItem(0)).Value;
				size = this.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / this.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * this.textFont.Size;
			}
			PointF[] p = new PointF[]{new PointF(x,y),new PointF(x,y + size)};
			YP.SVG.Text.TextContentInfo info = this.ownerTextElement.GetTextContentInfoOfChar(ref offset);
			if(info != null)
			{
				if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical ) == StringFormatFlags.DirectionVertical)
				{
					size = rect.Width;
					p = new PointF[]{new PointF(x,y),new PointF(x + size,y)};
				}
			}
			if(total)
                this.mouseArea.GetTotalTransformForElement(this.ownerTextElement).TransformPoints(p);
			PointF temp = p[0];
			p = null;
			return Point.Round(temp);
			
		}

		void CaretMethod()
		{
			if(this.ownerTextElement == null)
				return;
			while(true)
			{
				this.caret.CaretVisible = !this.caret.CaretVisible;

				this.Invalidate(this.Caret.Info,this.Caret.Offset);
				Thread.Sleep(400);
			}
		}

		#endregion

		#region ..PaintSelection
		/// <summary>
		/// 绘制选区
		/// </summary>
		/// <param name="g"></param>
		void PaintSelection(Graphics g)
		{
			if(this.ownerTextElement == null)
				return;

			if(!this.selection.IsEmpty)
			{
				InfoPos startpos = this.selection.StartPos;
				InfoPos endpos = this.selection.EndPos;
				List<TextContentInfo> list = this.ownerTextElement.TextContentInfos;
				int startindex = list.IndexOf(startpos.Info);
				int endindex = list.IndexOf(endpos.Info);
				System.IntPtr hdc = g.GetHdc();
				Win32.SetROP2(hdc,6);
                using (System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.ownerTextElement))
				{
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						for(int i = startindex;i<=endindex;i++)
						{
							if(i >= list.Count)
								continue;
							TextContentInfo info = list[i] as TextContentInfo;
							RectangleF bounds = info.Bounds;
							int offset1 = 0;
							int offset2 = info.TextContent.Length;
							if(i == startindex)
								offset1 = startpos.Offset;
							if(i == endindex)
								offset2 = endpos.Offset;
							PointF startp = info.GetStartPositionOfChar(offset1);
							PointF endp = info.GetStartPositionOfChar(offset2);
							path.Reset();
							if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical) != StringFormatFlags.DirectionVertical)
								path.AddRectangle(new RectangleF(startp.X,startp.Y,endp.X - startp.X,bounds.Height));
							else
								path.AddRectangle(new RectangleF(startp.X,startp.Y,bounds.Height,endp.Y - startp.Y));
							path.Transform(matrix);
							Win32.Win32PolyPolygon(hdc,path);
						}
						
					}
				}
				g.ReleaseHdc(hdc);
			}

			#region ..注释代码
			//			if(this.selection.Length > 0)
			//			{
			//				
			//				this.maxHeight = 0;
			//				int startoffset = this.selection.Offset;
			//				int endoffset = this.selection.Offset + this.selection.Length;
			//				TextContentInfo startinfo = this.ownerTextElement.GetTextContentInfoOfChar(ref startoffset);
			//				TextContentInfo endinfo = this.ownerTextElement.GetTextContentInfoOfChar(ref endoffset);
			//				int startindex = this.ownerTextElement.TextContentInfos.IndexOf(startinfo);
			//				int endindex = this.ownerTextElement.TextContentInfos.IndexOf(endinfo);
			//				for(int i = startindex;i<=endindex;i++)
			//				{
			//					TextContentInfo info = (TextContentInfo)this.ownerTextElement.TextContentInfos[i];
			//					int start = 0;
			//					int end = info.TextContent.Length;
			//					if(info == startinfo)
			//						start = startoffset;
			//					if(info == endinfo)
			//						end = endoffset;
			//
			//					PointF startp = info.GetStartPositionOfChar(start);
			//					PointF endp = info.GetStartPositionOfChar(end);
			//					float starth = (float)info.TextFont.FontFamily.GetLineSpacing(FontStyle.Regular ) / (float)info.TextFont.FontFamily.GetEmHeight(FontStyle.Regular) * (float)info.TextFont.Size;
			//					float left = (float)Math.Min(startp.X,endp.X);
			//					float right = (float)Math.Max(startp.X,endp.X);
			//					float top = (float)Math.Min(startp.Y,endp.Y);
			//					float bottom = (float)Math.Max(startp.Y,endp.Y);
			//					this.maxHeight = (float)Math.Max(this.maxHeight,starth);
			//					System.IntPtr hdc = g.GetHdc();
			//					Win32.SetROP2(hdc,6);
			////					using(Matrix matrix = this.mouseArea.CoordTransform.Clone())
			////					{
			////						matrix.Multiply(this.totalTransform);
			//						using(GraphicsPath path = new GraphicsPath())
			//						{
			//							if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical) != StringFormatFlags.DirectionVertical)
			//								path.AddRectangle(new RectangleF(left,startp.Y,right - left,starth));
			//							else
			//								path.AddRectangle(new RectangleF(startp.X,top,starth,bottom - top));
			//							path.FillMode = FillMode.Winding;
			//							path.Transform(this.ownerTextElement.TotalTransform.Clone());
			//							Win32.Win32PolyPolygon(hdc,path);
			//						}
			////					}
			//					g.ReleaseHdc(hdc);
			//				}
			//			}
			#endregion
		}

		

		void AdaptSelection(object sender,Text.SelectionChangedEventArgs e)
		{
			this.InvalidateRegion(e.OldStartPos,e.OldEndPos);
			this.InvalidateRegion(e.NewStartPos,e.NewEndPos);
		}
		#endregion

		#region ..Invalidate
		internal void Invalidate(TextContentInfo info,int offset)
		{
			try
			{
				if(info != null && this.ownerTextElement != null && this.ownerTextElement.TextContentInfos.Contains(info))
				{
					PointF p = info.GetStartPositionOfChar(offset);
					float h = 100;
					if(info.TextFont != null)
						h = (float)info.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.textFont.Size;

					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						if(this.vertical)
							path.AddRectangle(new RectangleF(p.X - 1,p.Y - 1,h + 4,3));
						else
							path.AddRectangle(new RectangleF(p.X - 1,p.Y - 1,3,h + 4));
                        path.Transform(this.mouseArea.GetTotalTransformForElement(this.ownerTextElement));
						using(System.Drawing.Region rg = new Region(path))
							InvidateMouseArea(rg);
					}
				}
			}
			catch(System.Exception e1)
			{
				System.Diagnostics.Debug.Assert(true,e1.Message);
			}
		}

		internal void InvalidateRegion(TextContentInfo info,int startoffset,int length)
		{
			try
			{
				if(this.ownerTextElement != null && info != null && this.ownerTextElement.TextContentInfos.Contains(info) && length > 0)
				{
					RectangleF bounds = info.Bounds;
					float h = 100;
					float w = 100;
					if(info.TextFont != null)
					{
						w = info.Bounds.Height * (length+1);
						h = info.Bounds.Height * 2f;
						if(w <= 3)
						{
							w = (float)info.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.textFont.Size;
							h = w * 2f;
							w = w * length;	
						}
					}
					using(GraphicsPath path = new GraphicsPath())
					{
						RectangleF rect = (this.ownerTextElement as SVG.Interface.ISVGPathable).GPath.GetBounds();
						if(rect.IsEmpty)
							rect = bounds;
						int offset = (int)Math.Max(0,startoffset);
						PointF p = info.GetStartPositionOfChar(offset);
						rect.Width = (float)Math.Max(bounds.Right,rect.Right) - p.X + w;
						rect.X = p.X;
						rect.Height = h;
						bounds.Width += w;
						if(this.vertical)
						{
							rect.Y = p.Y;
							rect.Width = w;
							rect.Height = (float)Math.Max(bounds.Bottom,rect.Bottom) - p.Y + h + 1;
							bounds.Height += h;
						}
						info.Bounds = bounds;
						path.AddRectangle(new RectangleF(p.X - this.mouseArea.grapSize,p.Y - this.mouseArea.grapSize,rect.Width + 2 * this.mouseArea.grapSize,rect.Height + 2 * this.mouseArea.grapSize));
                        path.Transform(this.mouseArea.GetTotalTransformForElement(this.ownerTextElement));
						using(System.Drawing.Region rg = new Region(path))
							InvidateMouseArea(rg);
					}
				}
			}
			catch(System.Exception e1)
			{
				Console.Write(e1.Message);
			}
		}

		internal void InvalidateRegion(InfoPos pos1,InfoPos pos2)
		{
			try
			{
				if(pos1 == pos2 || pos1.Info == null || pos2.Info == null)
					return;
                List<TextContentInfo> list = this.ownerTextElement.TextContentInfos;
				int startindex = list.IndexOf(pos1.Info);
				int endindex = list.IndexOf(pos2.Info);
				if(startindex > endindex)
				{
					int offset = startindex;
					startindex = endindex;
					endindex = offset;
				}
                using (System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.ownerTextElement))
				{
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						for(int i = startindex;i<=endindex;i++)
						{
							if(i < 0 || i >= list.Count)
								continue;
							TextContentInfo info = list[i] as TextContentInfo;
							RectangleF bounds = info.Bounds;
							int offset1 = 0;
							int offset2 = info.TextContent.Length;
							if(i == startindex)
								offset1 = pos1.Offset;
							if(i == endindex)
								offset2 = pos2.Offset;
							PointF startp = info.GetStartPositionOfChar(offset1);
							PointF endp = info.GetStartPositionOfChar(offset2);
							int r = this.mouseArea.grapSize / 2;
							if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical) != StringFormatFlags.DirectionVertical)
								path.AddRectangle(new RectangleF(startp.X - r,startp.Y - r,endp.X - startp.X + 2* r,bounds.Height + 2 * r));
							else
								path.AddRectangle(new RectangleF(startp.X - r,startp.Y - r,bounds.Width + r * 2,endp.Y - startp.Y + 2* r));
							path.Transform(matrix);
							using(System.Drawing.Region rg = new Region(path))
								InvidateMouseArea(rg);
						}
					
					}
				}
			}
			catch(System.Exception e1)
			{
				Console.Write(e1.Message);
			}
		}

		delegate void InvalidateMouseAreaCallBack(System.Drawing.Region rg);

		void InvidateMouseArea(System.Drawing.Region rg)
		{
			if(!this.mouseArea.InvokeRequired)
			{
				this.mouseArea.Invalidate(rg);
			}
			else
			{
				InvalidateMouseAreaCallBack bk = new InvalidateMouseAreaCallBack(InvidateMouseArea);
				this.mouseArea.Invoke(bk,new object[]{rg});
			}
		}

//		RectangleF GetRectangle(TextContentInfo info,int startoffset,int endoffset)
//		{
//			if(this.ownerTextElement != null && info != null && this.ownerTextElement.TextContentInfos.Contains(info) && startoffset != endoffset)
//			{
//				if(startoffset > endoffset)
//				{
//					int offset = startoffset;
//					startoffset = endoffset;
//					endoffset = offset;
//				}
//				RectangleF r1 = info.GetExtentOfChar(startoffset);
//				RectangleF r2 = info.GetExtentOfChar(endoffset);
//				int r = this.mouseArea.grapSize / 2;
//				if((info.TextStringFormat.FormatFlags & StringFormatFlags.DirectionVertical) != StringFormatFlags.DirectionVertical)
//					return new RectangleF(r1.X - r,r1.Y - r,r2.X - r1.X + 2 * r,r1.Height + r * 2);
//				else
//					return new RectangleF(r1.X - r,r2.Y - r,r1.Width + 2 * r,r2.Top -r1.Top + 2 * r);
//			}
//			return RectangleF.Empty;
//		}
		#endregion

		#region ..AttachActions
		void AttachActions()
		{
			this.actions[Keys.Left] = new Left();
			this.actions[Keys.Right] = new Right();
			this.actions[Keys.Home] = new Home();
			this.actions[Keys.End] = new End();
			this.actions[Keys.Up] = new Left();
			this.actions[Keys.Down] = new Right();
			this.actions[Keys.Shift | Keys.Left] = new ShiftLeft();
			this.actions[Keys.Shift | Keys.Right] = new ShiftRight();
			this.actions[Keys.Shift | Keys.Home] = new ShiftHome();
			this.actions[Keys.Shift | Keys.End] = new ShiftEnd();
			this.actions[Keys.Control | Keys.A ] = new CtrlA();
			this.actions[Keys.Delete] = new Delete();
			this.actions[Keys.Back] = new BackSpace();
			this.actions[Keys.Tab] = new Tab();
			this.actions[Keys.Escape] = new ESC();
			this.actions[Keys.Control | Keys.C] = new Copy();
			this.actions[Keys.Control | Keys.X] = new Cut();
			this.actions[Keys.Control | Keys.V] = new Paste();
			this.actions[Keys.Clear] = new Clear();
			this.actions[Keys.Enter] = new Enter();
		}
		#endregion

		#region ..ProcessDialogKey
		internal bool ProcessDialogKey(Keys keyData)
		{
			Action action = (Action)this.actions[keyData];
			if(action != null)
			{
				action.Execute(this);
				return true;
			}
			return false;
		}
		#endregion

		#region ..DealKeyPress
		void DealKeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
		{
			char ch = e.KeyChar;
			if (ch < ' ' )  
			{
				return;
			}
		//	MessageBox.Show(ch.ToString());
			this.Insert(ch.ToString());
			this.OwnerTextElement.OwnerDocument.InvokeUndos();
		}
		#endregion

		#region ..Insert
		/// <summary>
		/// 插入字符串
		/// </summary>
		/// <param name="str">要插入的字符串</param>
		internal void Insert(string str)
		{
			if(this.ownerTextElement != null && this.ownerTextElement.ParentNode != null)
			{
				bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
				this.mouseArea.SVGDocument.AcceptNodeChanged = true;
				this.ClearSelects();
				int offset = this.caret.Offset;
				int offset1 = offset;
				YP.SVG.Text.TextContentInfo info = this.Caret.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref offset);
				if(info != null)
				{
					string text = info.InsertString(offset,str);
					offset1 += str.Length;
					this.InvokeUndo();
					this.Caret.AdaptCaret(this.Caret.Info,offset1);
					this.InvalidateRegion(info,offset,str.Length);
				}
				this.mouseArea.SVGDocument.AcceptNodeChanged = old;
				
			}
		}
		#endregion

		#region ..ClearSelects
		/// <summary>
		/// 清除选区内容
		/// </summary>
		internal void ClearSelects()
		{			
			if(!this.selection.IsEmpty)
			{
				bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
				this.mouseArea.SVGDocument.AcceptNodeChanged = true;
				int startoffset = this.selection.StartPos.Offset;
				int endoffset = this.selection.EndPos.Offset;
				TextContentInfo startinfo = this.selection.StartPos.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref startoffset);
				TextContentInfo endinfo = this.selection.EndPos.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref endoffset);
				int startindex = this.ownerTextElement.TextContentInfos.IndexOf(startinfo);
				int endindex = this.ownerTextElement.TextContentInfos.IndexOf(endinfo);
                List<TextContentInfo> list = this.ownerTextElement.TextContentInfos;
				if(startindex == endindex)
					this.InvalidateRegion(this.selection.StartPos,new InfoPos(this.selection.EndPos.Info,this.selection.EndPos.Info.TextContent.Length));
				else
				{
					TextContentInfo info = list[list.Count - 1] as TextContentInfo;
					this.InvalidateRegion(this.selection.StartPos,new InfoPos(info,info.TextContent.Length ));
				}
				for(int i = startindex;i<endindex;i++)
				{
					if(i < 0 || i >= list.Count)
						continue;
					TextContentInfo info = (TextContentInfo)list[i];
					int start = 0;
					int end = info.TextContent.Length;
					if(i == startindex)
						start = startoffset;
					if(i == endindex)
						end = endoffset;

					string text= info.RemoveString(start,end - start);
					if(text.Length == 0 && i!= startindex)
					{
						if(info.OwnerTextNode != null && info.OwnerTextNode.ParentNode != null)
							(info.OwnerTextNode.ParentNode as SVG.SVGElement).InternalRemoveChild(info.OwnerTextNode);
						if(info.OwnerTextContentElement.ParentNode != null)
							(info.OwnerTextContentElement.ParentNode as SVG.SVGElement).InternalRemoveChild(info.OwnerTextContentElement);
						this.ownerTextElement.RemoveInfo(info);
						endindex --;
						i --;
					}
				}
				if(endindex > startindex)
				{
					string text = endinfo.TextContent.Substring(endoffset);
					if(text.Length > 0)
						startinfo.AppendString(text);
					if(endinfo.OwnerTextNode != null &&endinfo.OwnerTextNode.ParentNode != null)
						(endinfo.OwnerTextNode.ParentNode as SVG.SVGElement).InternalRemoveChild(endinfo.OwnerTextNode);
					if(endinfo.OwnerTextContentElement != null && endinfo.OwnerTextContentElement.ParentNode != null)
						(endinfo.OwnerTextContentElement.ParentNode as SVG.SVGElement).InternalRemoveChild(endinfo.OwnerTextContentElement);
					this.ownerTextElement.RemoveInfo(endinfo);
				}
				else
				{
					startinfo.RemoveString(startoffset,endoffset - startoffset);
				}
				this.Caret.AdaptCaret(startinfo,startoffset);
				this.selection.AdaptSelection(this.selection.StartPos,this.selection.StartPos);
				this.mouseArea.SVGDocument.AcceptNodeChanged = old;
			}
		}
		#endregion

		#region ..改变属性
		//		internal void AdaptAttribute(AdaptAttributeEventArgs e)
		//		{
		//			if(this.selection.Length > 0)
		//			{
		//				bool old = this.ownerTextElement.OwnerDocument.AcceptNodeChanged;
		//				this.ownerTextElement.OwnerDocument.AcceptNodeChanged = false;
		//				int startoffset = this.selection.Offset;
		//				int endoffset = this.selection.Offset + this.selection.Length;
		//				TextContentInfo startinfo = this.ownerTextElement.GetTextContentInfoOfChar(ref startoffset);
		//				TextContentInfo endinfo = this.ownerTextElement.GetTextContentInfoOfChar(ref endoffset);
		//				int startindex = this.ownerTextElement.TextContentInfos.IndexOf(startinfo);
		//				int endindex = this.ownerTextElement.TextContentInfos.IndexOf(endinfo);
		//				YP.SVGDom.Document.SvgDocument doc = this.mouseArea.SVGDocument;
		//				
		//				for(int i = startindex;i<=endindex;i++)
		//				{
		//					TextContentInfo info = (TextContentInfo)this.ownerTextElement.TextContentInfos[i];
		//					int start = 0;
		//					int end = info.TextContent.Length;
		//					if(info == startinfo)
		//						start = startoffset;
		//					if(info == endinfo)
		//						end = endoffset;
		//
		//					if(start == end)
		//						continue;
		//					this.InvalidateInfo(info);
		//					if(start == 0 && end == info.TextContent.Length)
		//					{
		//						System.Xml.XmlText text = info.OwnerTextNode;
		//						YP.SVGDom.Text.SVGTextContentElement textelement = null;
		//						if(info.OwnerTextContentElement.ChildNodes.Count == 1)
		//						{
		//							textelement = info.OwnerTextContentElement;
		//							this.ownerTextElement.OwnerDocument.AcceptNodeChanged = true;
		//							textelement.AddSVGAttribute(e.AttributeName,e.AttributeValue);
		//							this.ownerTextElement.OwnerDocument.AcceptNodeChanged = false;
		//							textelement.InternalSetAttribute(e.AttributeName,e.AttributeValue);
		//							
		////							this.mouseArea.SVGDocument.InvokeElementChanged(new Base.Interface.ElementChangedEventArgs(textelement,((YP.SVGDom.SVGElement)textelement).ParentElement,((YP.SVGDom.SVGElement)textelement).ParentElement,YP.Base.ElementChangedAction.Change));
		//							
		//						}
		//						else if(info.OwnerTextContentElement is YP.SVGDom.Text.SVGTSpanElement)
		//						{
		////							System.Windows.Forms.MessageBox.Show("未实现，位于SVGDrawArea:Operation:Text:TextEditor");
		//						}
		//					}
		//					else
		//					{
		//						string text1 = info.TextContent.Substring(0,start);
		//						string text2 = info.TextContent.Substring(start,end - start);
		//						string text3 = info.TextContent.Substring(end);
		//						System.Xml.XmlText textnode1 = null,textnode2 = null,textnode3 = null;
		//						if(text1.Length > 0)
		//							textnode1 = doc.CreateTextNode(text1);
		//						if(text2.Length > 0)
		//							textnode2 = doc.CreateTextNode(text2);
		//						if(text3.Length > 0)
		//							textnode3 = doc.CreateTextNode(text3);
		//						YP.SVGDom.Text.SVGTSpanElement tspan1 = null;
		//						System.Xml.XmlNode parent = null;
		//						System.Xml.XmlNode ownerNode = null;
		//						if(info.OwnerTextContentElement is YP.SVGDom.Text.SVGTSpanElement)
		//						{
		//							tspan1 = (YP.SVGDom.Text.SVGTSpanElement)info.OwnerTextContentElement;
		//							parent = info.OwnerTextContentElement.ParentNode;
		//							ownerNode = info.OwnerTextContentElement;
		//						}
		//						else
		//						{
		//							tspan1 = (YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
		//							parent = info.OwnerTextContentElement;
		//							ownerNode = info.OwnerTextNode;
		//						}
		//						
		//						#region ..修改SVGTextElement对象
		////						if(info.OwnerTextContentElement is YP.SVGDom.Text.SVGTextElement)
		////						{
		//							System.Xml.XmlNode next = ownerNode.NextSibling;
		//							parent.InternalRemoveChild(ownerNode);
		//							int index = this.ownerTextElement.TextContentInfos.IndexOf(info);
		//
		//						if(textnode1 != null)
		//						{
		//							YP.SVGDom.Text.SVGTSpanElement tspan = (YP.SVGDom.Text.SVGTSpanElement)tspan1.CloneNode(false);//(YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
		//							tspan.InternalAppendChild(textnode1);
		//							YP.SVGDom.Text.TextContentInfo content = new YP.SVGDom.Text.TextContentInfo(textnode1.Value,info.TextFont,info.TextStringFormat,tspan);
		//							content.OwnerTextNode = textnode1;
		//							if(next != null)
		//								parent.InternalInsertBefore(tspan,next);
		//							else
		//								parent.InternalAppendChild(tspan);
		//							this.ownerTextElement.InsertInfo(index,content);
		//							endindex ++;
		//							i++;
		//							index ++;
		//						}
		//						if(textnode2 != null)
		//						{
		//							YP.SVGDom.Text.SVGTSpanElement tspan = (YP.SVGDom.Text.SVGTSpanElement)tspan1.CloneNode(false);//(YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
		//							tspan.InternalAppendChild(textnode2);
		//							YP.SVGDom.Text.TextContentInfo content = new YP.SVGDom.Text.TextContentInfo(textnode2.Value,info.TextFont,info.TextStringFormat,tspan);
		//							content.OwnerTextNode = textnode2;
		//							this.ownerTextElement.InsertInfo(index,content);
		//							tspan.AddSVGAttribute(e.AttributeName,e.AttributeValue);
		//							tspan.InternalSetAttribute(e.AttributeName,e.AttributeValue);
		//							if(next != null)
		//								parent.InternalInsertBefore(tspan,next);
		//							else
		//								parent.InternalAppendChild(tspan);
		//							endindex ++;
		//							i++;
		//							index ++;
		//						}
		//
		//						if(textnode3 != null)
		//						{
		//							YP.SVGDom.Text.SVGTSpanElement tspan = (YP.SVGDom.Text.SVGTSpanElement)tspan1.CloneNode(false);//(YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
		//							tspan.InternalAppendChild(textnode3);
		//							YP.SVGDom.Text.TextContentInfo content = new YP.SVGDom.Text.TextContentInfo(textnode3.Value,info.TextFont,info.TextStringFormat,tspan);
		//							content.OwnerTextNode = textnode3;
		//							if(next != null)
		//								parent.InternalInsertBefore(tspan,next);
		//							else
		//								parent.InternalAppendChild(tspan);
		//							this.ownerTextElement.InsertInfo(index,content);
		//							endindex ++;
		//							i++;
		//							index ++;
		//						}
		//
		//							this.ownerTextElement.RemoveInfo(info);
		//							endindex --;
		//							i --;
		////						}
		//						#endregion
		//					}
		//				}
		//				if(e.AttributeName.Trim().StartsWith("font"))
		//					this.mouseArea.Invalidate();
		//				this.ownerTextElement.OwnerDocument.InvokeUndos();
		//				this.ownerTextElement.OwnerDocument.AcceptNodeChanged = old;
		//			}
		//		}
		#endregion

		#region ..ExecuteBehaviorPresent
		internal bool ExecuteBehaviorPresent(Behavior behavior)
		{
			bool a = true;
			switch(behavior)
			{
				case Behavior.AdjustLayer:
					a = false;
					break;
				case Behavior.AlignElements:
					a = false;
					break;
				case Behavior.Distriute:
					a = false;
					break;
				case Behavior.Group:
					a = false;
					break;
				case Behavior.UnGroup:
					a = false;
					break;
				case Behavior.AdjustSize:
					a = false;
					break;
				case Behavior.SelectNone:
					a = true;
					break;
				case Behavior.Copy:
				case Behavior.Cut:
					a = !this.selection.IsEmpty;
					break;
				case Behavior.Paste:
					IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
					a = data.GetDataPresent(DataFormats.Text);
					break;
				case Behavior.Undo:
					a = true;
					break;
				case Behavior.Redo:
					a = true;
					break;
				case Behavior.Delete:
					a = true;
					break;
				case Behavior.SelectAll:
					a = true;
					break;
				case Behavior.Transform:
					a = false;
					break;
			}
			return a;
		}
		#endregion

		#region ..Copy
		internal void Copy()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
				
			TextContentInfo startinfo = this.selection.StartPos.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref startoffset);
			TextContentInfo endinfo = this.selection.EndPos.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref endoffset);
			int startindex = this.ownerTextElement.TextContentInfos.IndexOf(startinfo);
			int endindex = this.ownerTextElement.TextContentInfos.IndexOf(endinfo);
			//judge whether the text is vertical
			
			for(int i = startindex;i<=endindex;i++)
			{
				TextContentInfo info = (TextContentInfo)this.ownerTextElement.TextContentInfos[i];
				int start = 0;
				int end = info.TextContent.Length;
				if(i == startindex)
					start = this.selection.StartPos.Offset;
				if(i == endindex)
					end = this.selection.EndPos.Offset;
				SVG.Text.SVGTSpanElement tspan = info.OwnerTextContentElement as SVG.Text.SVGTSpanElement;
				if(tspan != null && ((tspan.Dy.NumberOfItems > 0 && !vertical )|| (vertical && tspan.Dx.NumberOfItems > 0)) && i>startindex)
					sb.Append("\n");
				sb.Append(info.TextContent.Substring(start,end - start));
			}

			if(sb.Length > 0)
			{
				DataObject data = new DataObject(DataFormats.Text,sb.ToString());
				System.Windows.Forms.Clipboard.SetDataObject(data);
			}
			sb = null;
		}
		#endregion

		#region ..Clear
		internal void Clear()
		{
			int startindex = 0;
			int endindex = this.ownerTextElement.TextContentInfos.Count - 1;
			for(int i = startindex;i<=endindex;i++)
			{
				TextContentInfo info = (TextContentInfo)this.ownerTextElement.TextContentInfos[i];
				int start = 0;
				int end = info.TextContent.Length;
				string text= info.RemoveString(start,end - start);
				if(text.Length == 0)
				{
					(info.OwnerTextNode.ParentNode as SVG.SVGElement).InternalRemoveChild(info.OwnerTextNode);
					this.ownerTextElement.RemoveInfo(info);
					endindex --;
					i --;
				}
					
			}
			if(this.ownerTextElement.TextContentInfos.Count > 0)
			{
				TextContentInfo info = this.ownerTextElement.TextContentInfos[0] as TextContentInfo;
				InfoPos pos = new InfoPos(info,0);
				this.selection.AdaptSelection(pos,pos);
				this.Caret.AdaptCaret(info,0);
			}
			this.caret.CaretVisible = true;
		}
		#endregion

		#region ..SplitInOffset
		internal void SplitInOffset(InfoPos pos)
		{
			bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
			this.mouseArea.SVGDocument.AcceptNodeChanged = true;
           
			SVG.Text.TextContentInfo info = pos.Info;//this.ownerTextElement.GetTextContentInfoOfChar(ref offset);
			if(info != null)
			{
				int offset = pos.Offset;
				this.ClearSelects();
                bool old1 = this.ownerTextElement.ParseContent;
                this.ownerTextElement.ParseContent = false;
				string temp1 = info.TextContent.Substring(0,offset);
				string temp2 = info.TextContent.Substring(offset);
				System.Xml.XmlText node1 = this.mouseArea.SVGDocument.CreateTextNode(temp1);
				System.Xml.XmlText node2 = this.mouseArea.SVGDocument.CreateTextNode(temp2);
				SVG.Text.SVGTSpanElement tspan1 = null;
				System.Xml.XmlNode parent = null;
				System.Xml.XmlNode ownerNode = null;
				SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
				if(info.OwnerTextContentElement is SVG.Text.SVGTSpanElement)
				{
					tspan1 = (SVG.Text.SVGTSpanElement)info.OwnerTextContentElement;
					parent = info.OwnerTextContentElement.ParentNode;
					ownerNode = info.OwnerTextContentElement;
				}
				else
				{
					tspan1 = (SVG.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
					parent = info.OwnerTextContentElement;
					ownerNode = info.OwnerTextNode;
				}
				if(parent != null && ownerNode != null)
				{
					System.Xml.XmlNode next = ownerNode.NextSibling;

                    doc.AcceptNodeChanged = false;
                    //创建tspan node
					SVG.Text.SVGTSpanElement firstTspan = (SVG.Text.SVGTSpanElement)tspan1.CloneNode(false);//(Core.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
                    firstTspan.InternalAppendChild(node1);
                    
                    SVG.Text.SVGTSpanElement secondTspan = (SVG.Text.SVGTSpanElement)tspan1.CloneNode(false);//(
                    float height = (float)info.TextFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.TextFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.TextFont.Size;
                    string y = height.ToString();

                    if (!vertical)
                    {
                        secondTspan.InternalSetAttribute("x", this.ownerTextElement.HasAttribute("x") ? this.ownerTextElement.GetAttribute("x") : "0");
                        secondTspan.InternalSetAttribute("dy", y);
                    }
                    else
                    {
                        secondTspan.InternalSetAttribute("y", this.ownerTextElement.HasAttribute("y") ? this.ownerTextElement.GetAttribute("y") : "0");
                        secondTspan.InternalSetAttribute("dx", (-height).ToString());
                    }
                    secondTspan.InternalAppendChild(node2);
                    
                    doc.AcceptNodeChanged = old;

                    //添加节点和TextContentInfo
                    int index = this.ownerTextElement.TextContentInfos.IndexOf(info);
                    int index1 = index;
                    if (next != null)
                        firstTspan = (parent as SVG.SVGElement).InternalInsertBefore(firstTspan, next) as SVG.Text.SVGTSpanElement;
                    else
                        firstTspan = (parent as SVG.SVGElement).InternalAppendChild(firstTspan) as SVG.Text.SVGTSpanElement;
                    node1 = firstTspan.FirstChild as System.Xml.XmlText;
                    Font font = new Font(info.TextFont.FontFamily.Name, info.TextFont.Size, info.TextFont.Style);
                    StringFormat format = new StringFormat(info.TextStringFormat);
                    SVG.Text.TextContentInfo firstContent = new SVG.Text.TextContentInfo(node1.Value, font, format, firstTspan);
                    firstContent.OwnerTextNode = node1;

					this.ownerTextElement.InsertInfo(index,firstContent);
					index ++;

                    if (next != null)
                        secondTspan = (parent as SVG.SVGElement).InternalInsertBefore(secondTspan, next) as SVG.Text.SVGTSpanElement;
                    else
                        secondTspan = (parent as SVG.SVGElement).InternalAppendChild(secondTspan) as SVG.Text.SVGTSpanElement;
                    node2 = secondTspan.FirstChild as System.Xml.XmlText;
                    SVG.Text.TextContentInfo secondContent = new SVG.Text.TextContentInfo(node2.Value, font, format, secondTspan);
                    secondContent.OwnerTextNode = node2;
					this.ownerTextElement.InsertInfo(index,secondContent);
                    if (ownerNode.ParentNode is SVG.SVGElement)
                        (ownerNode.ParentNode as SVG.SVGElement).InternalRemoveChild(ownerNode);
                    this.ownerTextElement.RemoveInfo(info);
                    this.ownerTextElement.ParseContent = old1;
					this.InvokeUndo();
					this.Caret.AdaptCaret(secondContent,0);
					this.mouseArea.Invalidate();
				}
			}
			this.mouseArea.SVGDocument.AcceptNodeChanged = old;
		}
		#endregion

		#region ..caret_CaretChanged
		private void caret_CaretChanged(object sender, CaretChangedEventArgs e)
		{
			if (ime == null) 
				ime = new Ime(this.mouseArea.Handle, this.mouseArea.Font);
			else 
				ime.Font = this.mouseArea.Font;
			TextContentInfo info = this.Caret.Info;
			if(info != null && info.TextFont != null)
			{
				PointF startp= info.GetStartPositionOfChar(this.Caret.Offset);
				float h = 100;
				if(info.TextFont != null)
					h = (float)info.TextFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.TextFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.TextFont.Size;
				PointF[] ps = new PointF[]{new PointF(startp.X,startp.Y),new PointF(startp.X,startp.Y + h)};
				if(this.vertical)
					ps = new PointF[]{new PointF(startp.X,startp.Y),new PointF(startp.X + h,startp.Y )};
                this.mouseArea.GetTotalTransformForElement(this.ownerTextElement).TransformPoints(ps);
				System.Drawing.PointF pos = ps[0];
				ime.SetIMEWindowLocation((int)pos.X + 2,(int)pos.Y);
			}
			this.Invalidate(e.OldInfo,e.OldOffset);
			this.Invalidate(e.NewInfo,e.NewOffset);
		}
		#endregion

		#region ..GetStartInfo
		internal TextContentInfo GetStartInfo(TextContentInfo info)
		{
			if(this.ownerTextElement != null)
			{
				List<TextContentInfo> list = this.ownerTextElement.TextContentInfos;
				int index = list.IndexOf(info);
				TextContentInfo info1 = info;
				for(int i = index - 1;i>=0;i--)
				{
					TextContentInfo info2 = list[i] as TextContentInfo;
					if(!this.IsInLineInfo(info2))
						break;
					info1 = info2;
				}
				return info1;
			}
			return null;
		}

		internal TextContentInfo GetEndInfo(TextContentInfo info)
		{
			if(this.ownerTextElement != null)
			{
				List<TextContentInfo> list = this.ownerTextElement.TextContentInfos;
				int index = list.IndexOf(info);
				TextContentInfo info1 = info;
				for(int i = index + 1;i<list.Count;i++)
				{
					TextContentInfo info2 = list[i] as TextContentInfo;
					if(!this.IsInLineInfo(info2))
						break;
					info1 = info2;
				}
				return info1;
			}
			return null;
		}

		internal bool IsInLineInfo(TextContentInfo info)
		{
			YP.SVG.SVGElement element = info.OwnerTextContentElement;
			string x = element.GetAttribute("x");
			if(x.Length > 0)
				return false;
			x = element.GetAttribute("y");
			if(x.Length > 0)
				return false;
			x = element.GetAttribute("dx");
			if(x.Length > 0)
				return false;
			x = element.GetAttribute("dy");
			if(x.Length > 0)
				return false;
			return true;
		}
		#endregion

		#region ..InvokeUndo
		internal void InvokeUndo()
		{
			bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
			this.mouseArea.SVGDocument.AcceptNodeChanged = true;
			CaretUndo undo = new CaretUndo(this,new InfoPos(this.caret.Info,this.caret.Offset));
			this.mouseArea.SVGDocument.PushUndo(undo);
			this.mouseArea.SVGDocument.InvokeUndos();
			this.mouseArea.SVGDocument.AcceptNodeChanged = old;
		}
		#endregion

		#region ..Paste
		internal void Paste()
		{
			if(this.ownerTextElement == null)
				return;
			System.Windows.Forms.IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
			if(data.GetDataPresent(System.Windows.Forms.DataFormats.Text))
			{
				string text = data.GetData(System.Windows.Forms.DataFormats.Text).ToString();
				if(text.Length > 0)
				{
					bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
					this.mouseArea.SVGDocument.AcceptNodeChanged = true;
					string[] strings = rg.Split(text);
					
					
					try
					{
						if(strings.Length <= 1)
							this.Insert(text);
						else
						{
							this.ClearSelects();
							TextContentInfo info = this.caret.Info;
							
							if(!this.ownerTextElement.TextContentInfos.Contains(info))
								return;
							int offset = this.caret.Offset;
							offset = (int)Math.Max(0,Math.Min(offset,info.TextContent.Length));
							string temp1 = info.TextContent.Substring(0,offset) + strings[0];
							strings[strings.Length -1] = strings[strings.Length -1]+ info.TextContent.Substring(offset);
							System.Xml.XmlText node1 = this.mouseArea.SVGDocument.CreateTextNode(temp1);
							YP.SVG.Text.SVGTSpanElement tspan1 = null;
							System.Xml.XmlNode parent = null;
							System.Xml.XmlNode ownerNode = null;
							YP.SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
							if(info.OwnerTextContentElement is YP.SVG.Text.SVGTSpanElement)
							{
								tspan1 = (YP.SVG.Text.SVGTSpanElement)info.OwnerTextContentElement;
								parent = info.OwnerTextContentElement.ParentNode;
								ownerNode = info.OwnerTextContentElement;
							}
							else
							{
								tspan1 = (YP.SVG.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
								parent = info.OwnerTextContentElement;
								ownerNode = info.OwnerTextNode;
							}

							if(parent != null && ownerNode != null)
							{
								System.Xml.XmlNode next = ownerNode.NextSibling;
								if(ownerNode.ParentNode is SVG.SVGElement)
									(ownerNode.ParentNode as SVG.SVGElement).InternalRemoveChild(ownerNode);
								int index = this.ownerTextElement.TextContentInfos.IndexOf(info);
							
								//第一个TextContentInfo
								YP.SVG.Text.SVGTSpanElement tspan = (YP.SVG.Text.SVGTSpanElement)tspan1.CloneNode(false);//(YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
								tspan.InternalAppendChild(node1);
								Font font = new Font(info.textFont.FontFamily.Name,info.textFont.Size,info.textFont.Style);
								StringFormat format = new StringFormat(info.TextStringFormat);
								YP.SVG.Text.TextContentInfo content = new YP.SVG.Text.TextContentInfo(node1.Value,font,format,tspan);
								content.OwnerTextNode = node1;
                                if (next != null)
                                {
                                    (parent as SVG.SVGElement).InternalInsertBefore(tspan, next);
                                }
                                else
                                    (parent as SVG.SVGElement).InternalAppendChild(tspan);
								this.ownerTextElement.InsertInfo(index,content);
								index ++;
								string x = this.ownerTextElement.GetAttribute("x");
								float height = (float)info.textFont.FontFamily.GetLineSpacing(FontStyle.Regular) / (float)info.textFont.FontFamily.GetEmHeight(FontStyle.Regular) * info.textFont.Size;
								string y = height.ToString();

								for(int i = 1;i<strings.Length;i++)
								{
									System.Xml.XmlText node2 = this.mouseArea.SVGDocument.CreateTextNode(strings[i]);
									tspan = (YP.SVG.Text.SVGTSpanElement)tspan1.CloneNode(false);//(YP.SVGDom.Text.SVGTSpanElement)doc.CreateElement(doc.Prefix,"tspan",doc.NamespaceURI);
									tspan.InternalSetAttribute("x",x);
									tspan.InternalSetAttribute("dy",y);
									tspan.InternalAppendChild(node2);
									font = new Font(info.textFont.FontFamily.Name,info.textFont.Size,info.textFont.Style);
									format = new StringFormat(info.TextStringFormat);
									content = new YP.SVG.Text.TextContentInfo(node2.Value,font,format,tspan);
									content.OwnerTextNode = node2;
									if(next != null)
										(parent as SVG.SVGElement).InternalInsertBefore(tspan,next);
									else
										(parent as SVG.SVGElement).InternalAppendChild(tspan);
									this.ownerTextElement.InsertInfo(index,content);
									
									index ++;
								}	
								this.ownerTextElement.RemoveInfo(info);
								this.InvokeUndo();
								if(content != null )
									this.Caret.AdaptCaret(content,content.TextContent.Length);
								this.mouseArea.Invalidate();
							}
						}
						this.mouseArea.SVGDocument.AcceptNodeChanged = old;
					}
					catch(System.Exception e1)
					{
						Console.Write(e1.Message);
					}

				}
			}
		}
		#endregion
	}
}
