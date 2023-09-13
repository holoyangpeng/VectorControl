using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;
using YP.SVG;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// 选择图案样式的编辑器
	/// </summary>
	public class HatchEditor:DropDownEditor
	{
		#region ..构造及消除
		public HatchEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		bool changed = false;
		#endregion

		#region ..EditValue
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{

			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{

				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (edSvc != null) 
				{
					ListHatchStyle style = new ListHatchStyle();
					if(value is Hatch)
					{
						style.HatchBackColor = ((Hatch)value).BackColor;
						style.SelectedStyle = ((Hatch)value).Style;
						style.HatchForeColor = ((Hatch)value).ForeColor;
//						string temp = ((HatchStyle)value).Style.ToString();
//						int index = style.FindString(temp);
//						style.SelectedIndex = index;
					}
					style.Height = 150;
					style.SelectedIndexChanged += new EventHandler(style_SelectedIndexChanged);
					edSvc.DropDownControl(style);
					if(changed)
						value = new Hatch(style.HatchBackColor,style.SelectedStyle,style.HatchForeColor);			
					changed = false;
				}
			}
			return value;
		}
		#endregion

		#region ..选择样式
		private void style_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.changed = true;
			this.edSvc.CloseDropDown();
		}
		#endregion

		#region ..GetPaintValueSupported
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		#endregion

		#region ..PaintValue
		public override void PaintValue(PaintValueEventArgs e)
		{
			if(e.Value is Hatch)
			{
				Hatch hatch = (Hatch)e.Value;
				using(System.Drawing.Brush brush = this.GetBrush(hatch,e.Bounds))
				{
					if(brush != null)
					{
						e.Graphics.RenderingOrigin = new Point(0,0);
						e.Graphics.FillRectangle(brush,e.Bounds);
					}
				}
			}
			else
				base.PaintValue(e);
				
		}
		#endregion

		#region ..GetBrush
		Brush GetBrush(Hatch hatch,Rectangle rect)
		{
			if(hatch.Style != YP.SVG.HatchStyle.None)
			{
				if((int)hatch.Style < 56)
				{
                    System.Drawing.Drawing2D.HatchStyle style1 = System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal;
                    if (System.Enum.IsDefined(typeof(System.Drawing.Drawing2D.HatchStyle), hatch.Style.ToString()))
                    style1 = (System.Drawing.Drawing2D.HatchStyle)System.Enum.Parse(typeof(System.Drawing.Drawing2D.HatchStyle), hatch.Style.ToString(), false);
					return new HatchBrush(style1,hatch.ForeColor,hatch.BackColor);
				}
				else
				{
					Brush brush = null;
					ColorBlend bl = new ColorBlend();
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						switch(hatch.Style)
						{
							case YP.SVG.HatchStyle.Center:
								path.AddEllipse(rect.X,rect.Y ,ListHatchStyle.RectangleWidth,rect.Height);
								brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
								bl.Positions = new float[]{0,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor};
								((PathGradientBrush)brush).InterpolationColors = bl;
								((PathGradientBrush)brush).CenterPoint = new PointF(rect.X + rect.Width / 2f,rect.Y + rect.Height / 2f);
								break;
							case YP.SVG.HatchStyle.VerticalCenter:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.Vertical);
							
								bl.Positions = new float[]{0,0.5f,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor,hatch.BackColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;

								break;
							case YP.SVG.HatchStyle.HorizontalCenter:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.Vertical);
								
								
								bl.Positions = new float[]{0,0.5f,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor,hatch.BackColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;
								break;
							case YP.SVG.HatchStyle.DiagonalLeft:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
								
								bl.Positions = new float[]{0,0.5f,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor,hatch.BackColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;
									
								break;
							case YP.SVG.HatchStyle.DiagonalRight:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
		
								bl.Positions = new float[]{0,0.5f,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor,hatch.BackColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;
									
								break;
							case YP.SVG.HatchStyle.LeftRight:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
								
								bl.Positions = new float[]{0,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;
									
								break;
							case YP.SVG.HatchStyle.TopBottom:
								
								brush = new LinearGradientBrush(rect,hatch.BackColor,hatch.ForeColor,System.Drawing.Drawing2D.LinearGradientMode.Vertical);
								
								bl.Positions = new float[]{0,1};
								bl.Colors = new Color[]{hatch.BackColor,hatch.ForeColor};
								((LinearGradientBrush)brush).InterpolationColors = bl;
								break;
						}
						bl = null;
						return brush;
					}
				}
			}
			return null;
		}
		#endregion

	}
}
