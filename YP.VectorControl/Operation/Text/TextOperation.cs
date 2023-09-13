using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// 实现文本操作
	/// </summary>
	internal class TextOperation:Operation//,Interface.ITextOperation
	{
		#region ..构造及消除
		internal TextOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mouseArea = mousearea;
		}

		internal TextOperation(Canvas mousearea,YP.SVG.Text.SVGTextElement element):base(mousearea)
		{
			if(this.currentEditor != null)
				this.currentEditor.Dispose();
			this.preText = element;
			this.preText.Render = false;
//			this.mouseArea.SVGDocument.ChangeSelectElement(null);
			this.currentEditor = new Text.TextEditor(this.preText,this.mouseArea,this);
			Point p = this.mouseArea.PointToClient(Control.MousePosition);
			this.currentEditor.OnMouseDown(new MouseEventArgs(MouseButtons.Left,0,p.X,p.Y,4));
		}

		public override void Dispose()
		{
			if(this.currentEditor != null)
				this.currentEditor.Dispose();
			base.Dispose();
		}
		#endregion

		#region ..私有变量
		YP.SVG.Text.SVGTextElement preText = null;
		TextEditor currentEditor = null;
		internal bool first = true;
		#endregion

		#region ..保护属性
		/// <summary>
		/// 判断当前是否正在编辑文本
		/// </summary>
		internal bool Editing
		{
			get
			{
				return this.currentEditor != null;
			}
		}

        /// <summary>
        /// gets the current editing text element
        /// </summary>
        internal SVG.Text.SVGTextElement TextElementInEditiing
        {
            get
            {
                if (this.currentEditor != null)
                    return this.currentEditor.OwnerTextElement;
                return this.preText;
            }
        }

        /// <summary>
        /// gets a value indicates whether current is editing the text
        /// </summary>
		internal override bool EditText
		{
			get
			{
				return Editing;
			}
		}
		#endregion

		#region ..OnMouseDown
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(this.currentEditor != null)
				{
					if(this.currentEditor.OnMouseDown(e))
						return;
				}

				if(e.Button == MouseButtons.Left)
				{
					if(this.preText != null)
					{
						if(this.currentEditor != null)
							this.currentEditor.Dispose();
						this.preText.Render = false;
						this.mouseArea.SVGDocument.ChangeSelectElement(this.preText);
						//					this.mouseArea.InvalidateSeleciton();
						this.currentEditor = new Text.TextEditor(this.preText,this.mouseArea,this);
						this.currentEditor.OnMouseDown(e);
					}
					else if(this.currentEditor == null)
					{
						YP.SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
						YP.SVG.Text.SVGTextElement text = (YP.SVG.Text.SVGTextElement)doc.CreateElement(doc.Prefix,"text",doc.NamespaceURI);
						bool old = doc.AcceptNodeChanged;
						doc.AcceptNodeChanged = false;
						PointF p = this.mouseArea.PointToVirtualView(new PointF(e.X,e.Y));
						text.InternalSetAttribute("x",p.X.ToString());
						text.InternalSetAttribute("y",p.Y.ToString());
						text.InternalSetAttribute("xml:space","preserve");
						this.SetTextFont(text);
						text.Render = false;
						doc.AcceptNodeChanged = true;
						text = (YP.SVG.Text.SVGTextElement)this.mouseArea.AddElement(text);
						doc.AcceptNodeChanged = old;
						//this.mouseArea.Update();
						//text.TotalTransform = this.mouseArea.CoordTransform.Clone();
						if(this.currentEditor != null)
							this.currentEditor.Dispose();
						this.currentEditor = new Text.TextEditor(text,this.mouseArea,this);
						this.mouseArea.Invalidate();
						this.currentEditor.OnMouseDown(e);
					}
				}
			}
			catch{}
		}
		#endregion

		#region ..OnMouseMove
		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(this.currentEditor != null)
				{
					if(this.currentEditor.OnMouseMove(e))
						return;
				}

				this.preText = null;
			
				PointF p = new PointF(e.X,e.Y);
				YP.SVG.SVGElementCollection list = (YP.SVG.SVGElementCollection)this.mouseArea.SVGDocument.SelectCollection;
				foreach(YP.SVG.SVGElement element in list)
				{
					if(element is YP.SVG.Text.SVGTextElement)
					{
						YP.SVG.Text.SVGTextElement text = (YP.SVG.Text.SVGTextElement)element;
						GraphicsPath path = (GraphicsPath)(text as SVG.Interface.ISVGPathable).GPath.Clone();
                        Matrix matrix = this.mouseArea.GetTotalTransformForElement(text);//.TotalTransform.Clone();
						matrix.Invert();
						PointF[] ps = new PointF[]{p};
						matrix.TransformPoints(ps);
						p = ps[0];
						RectangleF rect = path.GetBounds();
						if(rect.IntersectsWith(new RectangleF(p.X - 2,p.Y - 2,4,4)) || rect.Contains(p))
						{
							this.preText = text;
							path.Dispose();
							matrix.Dispose();
							this.mouseArea.Cursor = Forms.Cursors.MoveControl;
							return;
						}
						path.Dispose();
						matrix.Dispose();
					}
				}

				System.Xml.XmlNodeList list1 = ((YP.SVG.SVGElement)this.mouseArea.SVGDocument.CurrentScene).ChildNodes;
				foreach(System.Xml.XmlNode node in list1)
				{
					if(node is YP.SVG.Text.SVGTextElement)
					{
						YP.SVG.Text.SVGTextElement text = (YP.SVG.Text.SVGTextElement)node;
						GraphicsPath path = (GraphicsPath)(text as SVG.Interface.ISVGPathable).GPath.Clone();
                        Matrix matrix = this.mouseArea.GetTotalTransformForElement(text); //.TotalTransform.Clone();
						matrix.Invert();
						PointF[] ps = new PointF[]{p};
						matrix.TransformPoints(ps);
						p = ps[0];
						RectangleF rect = path.GetBounds();
						if(rect.IntersectsWith(new RectangleF(p.X - 2,p.Y - 2,4,4)) || rect.Contains(p))
						{
							path.Dispose();
							this.preText = text;
							this.mouseArea.Cursor = Forms.Cursors.MoveControl;
							matrix.Dispose();
							return;
						}
						path.Dispose();
						matrix.Dispose();
					}
				}
				this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
			}
			catch{}
		}
		#endregion

		#region ..OnMouseUp
		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(object sender,MouseEventArgs e)
		{
			if(!this.IsValidDocument)
				return;
			if(this.currentEditor != null)
			{
				if(this.currentEditor.OnMouseUp(e))
					return;
			}
		}
		#endregion

		#region ..OnPaint
		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnPaint(object sender,PaintEventArgs e)
		{
			if(!this.IsValidDocument)
				return;
			if(this.currentEditor != null)
				this.currentEditor.OnPaint(e);
		}
		#endregion

		#region ..改变属性事件
		protected override void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e)
		{
//			if(this.currentEditor != null)
//			{
//				this.currentEditor.AdaptAttribute(e);
//			}
		}
		#endregion

		#region ..键盘事件
		public override bool ProcessDialogKey(Keys keyData)
		{
			if(this.currentEditor != null)
				return this.currentEditor.ProcessDialogKey(keyData);
			return false;
		}
		#endregion

        #region ..Reset
        /// <summary>
		/// 重置
		/// </summary>
		internal override void Reset()
		{
			if(this.currentEditor != null)
				this.currentEditor.Dispose();
			this.currentEditor = null;
			base.Reset();
		}
		#endregion

        #region ..SetTextFont
        /// <summary>
		/// 添加文本属性
		/// </summary>
		/// <param name="element"></param>
		void SetTextFont(YP.SVG.Text.SVGTextElement element)
		{
			if(element == null)
				return;
			string name = this.mouseArea.TextStyle.FontName;
			element.InternalSetAttribute("font-family",name);
			name = this.mouseArea.TextStyle.Size.ToString();
			element.InternalSetAttribute("font-size",name);
			if(this.mouseArea.TextStyle.Italic)
			{
				element.InternalSetAttribute("font-style","italic");
			}
			

			if(this.mouseArea.TextStyle.Bold)
			{
				element.InternalSetAttribute("font-weight","bold");
			}

			if(this.mouseArea.TextStyle.Underline)
			{
				element.InternalSetAttribute("text-decoration","underline");
			}

//			if(this.mouseArea.TextStyle.StrikeOut)
//			{
//				element.AddSVGAttribute("text-decoration","strike-out");
//				element.InternalSetAttribute("text-decoration","strike-out");
//			}
			name = null;
		}
		#endregion

		#region ..Invalidate
		internal override void Invalidate()
		{
			this.Reset();
		}
		#endregion

        #region ..ExecuteBehaviorPresent
        internal override bool ExecuteBehaviorPresent(Behavior behavior)
		{
			if(this.currentEditor != null)
				return this.currentEditor.ExecuteBehaviorPresent(behavior);
			return true;
		}
		#endregion
	}
}
