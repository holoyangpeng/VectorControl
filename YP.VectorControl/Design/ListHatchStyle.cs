using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// ListHatchStyle 的摘要说明。
	/// </summary>
	internal class ListHatchStyle:ListBox
	{
		#region ..构造及消除
		public ListHatchStyle()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.BorderStyle = BorderStyle.None;
			this.sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			this.sf.LineAlignment = StringAlignment.Center;
			System.Array array = System.Enum.GetValues(typeof(YP.SVG.HatchStyle));
			foreach(object a in array)
				this.Items.Add(a.ToString());
			this.ItemHeight = 18;
		}
		#endregion

		#region ..常量
		internal const int RectangleWidth = 30;
		internal const int ContentMargin = 6;
		System.Drawing.StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
		#endregion

		#region ..私有变量
		Color backColor = Color.White;
		Color forecolor = Color.Black;
		#endregion

		#region ..属性
		public Color HatchBackColor
		{
			set
			{
				if(this.backColor != value)
				{
					this.backColor = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.backColor;
			}
		}

		/// <summary>
		/// 获取或设置网纹的前景颜色
		/// </summary>
		public Color HatchForeColor
		{
			set
			{
				this.forecolor = value;
			}
			get
			{
				return this.forecolor;
			}
		}
		#endregion

		#region ..获取或设置填充图案的类型
		/// <summary>
		/// 获取或设置填充图案的类型
		/// </summary>
		public YP.SVG.HatchStyle SelectedStyle
		{
			set
			{
				int index = this.FindString(value.ToString());
				if(index >= 0)
					this.SelectedIndex = index;
			}
			get
			{
				string text = this.SelectedItem.ToString();
				YP.SVG.HatchStyle style = YP.SVG.HatchStyle.None;
				if(System.Enum.IsDefined(typeof(YP.SVG.HatchStyle),text))
					style = (YP.SVG.HatchStyle)System.Enum.Parse(typeof(YP.SVG.HatchStyle),text,false);
				return style;
			}
		}
		#endregion

		#region ..DrawItem
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem (e);
//			e.DrawBackground();
			if(e.Index >= 0)
			{
				YP.SVG.HatchStyle style = (YP.SVG.HatchStyle)System.Enum.Parse(typeof(YP.SVG.HatchStyle),this.Items[e.Index].ToString(),false);
				Color back = this.backColor;
				bool selected = e.State == DrawItemState.Selected ;
				Rectangle rect = e.Bounds;
				rect.Inflate(-1,-1);
				rect.Height --;
				
				if(e.State == DrawItemState.Selected)
				{
					e.Graphics.DrawRectangle(SystemPens.Highlight,rect);
//					back = e.BackColor;
				}
				if(style == YP.SVG.HatchStyle.None)
				{
					sf.Alignment = StringAlignment.Center;
					e.Graphics.DrawString("无",e.Font,Brushes.Black,e.Bounds,this.sf);
				}
				else if((int)style < 56)
				{
					sf.Alignment = StringAlignment.Near;
                    System.Drawing.Drawing2D.HatchStyle style1 = HatchStyle.BackwardDiagonal;
                    if (System.Enum.IsDefined(typeof(System.Drawing.Drawing2D.HatchStyle), style.ToString()))
                        style1 = (System.Drawing.Drawing2D.HatchStyle)System.Enum.Parse(typeof(System.Drawing.Drawing2D.HatchStyle),style.ToString(),false);
					using(System.Drawing.Brush brush = new HatchBrush(style1,this.forecolor,back))
					{
						rect.Width = RectangleWidth;
						e.Graphics.RenderingOrigin = rect.Location;
						e.Graphics.FillRectangle(brush,rect);
						e.Graphics.DrawRectangle(Pens.Black,rect);
						e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
					}
				}
				else
				{
					sf.Alignment = StringAlignment.Near;
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						switch(style)
						{
							case YP.SVG.HatchStyle.Center:
								
								path.AddEllipse(rect.X,rect.Y ,RectangleWidth,rect.Height);
								using(System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path))
								{
									rect.Width = RectangleWidth;
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor};
									brush.InterpolationColors = bl;
									brush.CenterPoint = new PointF(rect.X + rect.Width / 2f,rect.Y + rect.Height / 2f);
									e.Graphics.FillRectangle(new SolidBrush(this.backColor),rect);
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.VerticalCenter:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.Vertical))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,0.5f,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor,this.backColor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.HorizontalCenter:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,0.5f,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor,this.backColor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.DiagonalLeft:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,0.5f,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor,this.backColor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.DiagonalRight:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,0.5f,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor,this.backColor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.LeftRight:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
							case YP.SVG.HatchStyle.TopBottom:
								rect.Width = RectangleWidth;
								using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(rect,this.backColor,this.forecolor,System.Drawing.Drawing2D.LinearGradientMode.Vertical))
								{
									ColorBlend bl = new ColorBlend();
									bl.Positions = new float[]{0,1};
									bl.Colors = new Color[]{this.backColor,this.forecolor};
									brush.InterpolationColors = bl;
									e.Graphics.FillRectangle(brush,rect);
									e.Graphics.DrawRectangle(Pens.Black,rect);
									bl = null;

									e.Graphics.DrawString(style.ToString(),e.Font,Brushes.Black,new RectangleF(rect.Right + ContentMargin,rect.Y,e.Bounds.Width,rect.Height),this.sf);
								}
								break;
						}
					}
				}
			}
		}
		#endregion
	}
}
