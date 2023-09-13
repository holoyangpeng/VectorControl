using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>箭头选择器。</para>
	/// <para>箭头选择器为用户提供了一个可视化的箭头选择方式.</para>
	/// <para>可以将选择的箭头传给VectorControl的<see cref="VectorControl.StartArrow">StartArrow</see>或<see cref="VectorControl.EndArrow">EndArrow</see>属性，从而可以更改当前绘制对象获得绘制对象的箭头样式。</para>
	/// </summary>
	/// <example>
	/// <code>
	/// this.arrowSelector.EndArrow = true;
	/// this.vectorControl.EndArrow = this.arrowSelector.SelectedArrow;
	/// </code>
	/// </example>
	[ToolboxItem(false)]
	public sealed class ArrowSelector:ListBox
	{
		#region ..构造及消除
		public ArrowSelector()
		{
			 //
			 // TODO: 在此处添加构造函数逻辑
			 //
			try
			{
				base.Items.Add(ColorSelectorHelper.NoneColorStr);
                YP.SVG.Document.SVGDocument doc = Arrow.ArrowDocument;
				System.Xml.XmlNodeList list = doc.GetElementsByTagName("marker");
				for(int i = 0;i<list.Count;i++)
				{
					System.Xml.XmlElement element = list[i] as System.Xml.XmlElement;
					if(element == null)
						continue;
					base.Items.Add(new Arrow(element));
				}
			}
			catch(System.Exception e)
			{
				System.Diagnostics.Debug.Assert(true,e.Message);
			}
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.BorderStyle = BorderStyle.None;
			this.ItemHeight = ColorSelectorHelper.ItemHeight;
		 }
		#endregion

		#region ..私有变量
		bool endArrow = false;
		#endregion

		#region ..属性
		/// <summary>
		/// 指定选择器是否绘制结尾箭头
		/// </summary>
		public bool EndArrow
		{
			set
			{
				this.endArrow = value;
				this.Invalidate();
			}
			get
			{
				return this.endArrow;
			}
		}
		#endregion

		#region ..常量
		private const int RECTCOLOR_LEFT = 4;
		private const int RECTCOLOR_TOP = 2;
		private const int RECTTEXT_MARGIN = 10;
		#endregion

		#region ..override Items
		/// <summary>
		/// 请不要直接调用
		/// </summary>
		public new System.Windows.Forms.ListBox.ObjectCollection Items
		{
			set
			{
			}
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// 获取或设置控件的绘图模式
		/// </summary>
		public new DrawMode DrawMode
		{
			set
			{
				base.DrawMode = DrawMode.OwnerDrawFixed;
			}
			get
			{
				return DrawMode.OwnerDrawFixed;
			}
		}

		/// <summary>
		/// 获取当前选中箭头
		/// </summary>
		public Arrow SelectedArrow
		{
			set
			{
				this.SelectedIndex = base.Items.IndexOf(value);
			}
			get
			{
				if(this.SelectedIndex >= 0 && this.SelectedIndex < base.Items.Count)
					return base.Items[this.SelectedIndex] as Arrow;
				return null;
			}
		}
		#endregion

		#region ..绘制
        /// <summary>
        /// 引发ArrowSelector的DrawItem事件
        /// </summary>
        /// <param name="e"></param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.Graphics.SetClip(e.Bounds);
			if(e.State == DrawItemState.Selected || e.State == DrawItemState.None) 
				e.DrawBackground();
			System.Drawing.Drawing2D.GraphicsContainer c = e.Graphics.BeginContainer();
			if(e.Index >= 0 && e.Index < base.Items.Count)
			{
				Arrow arrow = base.Items[e.Index] as Arrow;
				if(arrow != null && arrow.MarkerElement is YP.SVG.ClipAndMask.SVGMarkerElement)
				{
					using(YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator(true))
					{
						using(System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
						{
							sp.DrawShadow = false;
							e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
							Rectangle rect = new Rectangle(e.Bounds.X + RECTCOLOR_LEFT,e.Bounds.Y + RECTCOLOR_TOP,e.Bounds.Width - 2 * RECTCOLOR_LEFT,e.Bounds.Height - 2 * RECTCOLOR_TOP);
                            //e.Graphics.FillRectangle(Brushes.White,rect);
                            //e.Graphics.DrawRectangle(Pens.Black,rect);
							
							if(this.endArrow)
							{
								path.AddLine(e.Bounds.X + RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2,e.Bounds.Right - 2*RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2);
								
								using(System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix())
								{
									matrix.Multiply(new Matrix(-1,0,0,1,0,0));
									matrix.Translate(2 * rect.Width,0,MatrixOrder.Append);
									path.Transform(matrix);
								}
								(arrow.MarkerElement as YP.SVG.ClipAndMask.SVGMarkerElement).MarkerEnd(e.Graphics,path,sp);
								path.Reset();
								path.AddLine(e.Bounds.X + RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2,e.Bounds.Right - 2*RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2);
								e.Graphics.DrawPath(Pens.Black,path);
							}
							else
							{
								path.AddLine(e.Bounds.X + 2 * RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2,e.Bounds.Right - RECTCOLOR_LEFT,e.Bounds.Y + e.Bounds.Height / 2);
								e.Graphics.DrawPath(Pens.Black,path);
								(arrow.MarkerElement as YP.SVG.ClipAndMask.SVGMarkerElement).MarkerStart(e.Graphics,path,sp);
							}
						}
					}
				}
				else
				{
					e.Graphics.DrawString(ColorSelectorHelper.NoneColorStr,e.Font,new SolidBrush(e.ForeColor),new Rectangle(e.Bounds .X + RECTCOLOR_LEFT,e.Bounds.Y + RECTCOLOR_TOP,e.Bounds.Width - 2 * RECTCOLOR_LEFT,e.Bounds.Height - 2 * RECTCOLOR_TOP));
				}
			}
			base.OnDrawItem (e);
			e.Graphics.ResetClip();
			e.Graphics.EndContainer(c);
		}
		#endregion
	}
}
