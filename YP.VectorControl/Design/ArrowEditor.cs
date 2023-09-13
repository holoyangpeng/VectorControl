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
using YP.VectorControl;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// ArrowEditor 的摘要说明。
	/// </summary>
	internal class ArrowEditor:DropDownEditor
	{
		#region ..构造及消除
		public ArrowEditor()
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
					ArrowSelector arrow = new ArrowSelector();
					if(value is ArrowProperty)
					{
						string id  = ((ArrowProperty)value).ID;
						if(id.StartsWith("start"))
							id = id.Substring(5);
						else if(id.StartsWith("end"))
							id = id.Substring(3);
						int index = arrow.FindString(id);
						arrow.SelectedIndex = index;
						arrow.EndArrow = ((ArrowProperty)value).EndArrow;
					}
					arrow.Height = 150;
					arrow.SelectedIndexChanged += new EventHandler(arrow_SelectedIndexChanged);
					edSvc.DropDownControl(arrow);
					if(changed)
						value = new ArrowProperty(arrow.SelectedArrow,null,arrow.EndArrow,string.Empty);
					changed = false;
				}
			}
			return value;
		}
		#endregion

		#region ..改变选择
		private void arrow_SelectedIndexChanged(object sender, EventArgs e)
		{
			changed = true;
			this.edSvc.CloseDropDown();
		}
		#endregion

		#region ..GetPaintValueSupport
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		#endregion

		#region ..PaintValue
		public override void PaintValue(PaintValueEventArgs e)
		{
			if(e.Value is ArrowProperty)
			{
				ArrowProperty arrow = (ArrowProperty)e.Value;
				if(arrow.Marker != null)
				{
					YP.SVG.ClipAndMask.SVGMarkerElement marker = arrow.Marker;
					using(YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator())
					{
						using(System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
						{
													
							Rectangle rect = e.Bounds;
							System.Drawing.Drawing2D.GraphicsContainer c = e.Graphics.BeginContainer();
							e.Graphics.SetClip(rect);
							e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;	
							if(arrow.EndArrow)
							{
								path.AddLine(rect.X,rect.Y + rect.Height / 2f,rect.Right,rect.Y + rect.Height/2f);
								e.Graphics.DrawPath(Pens.Black,path);
								marker.MarkerEnd(e.Graphics,path,sp);
							}
							else
							{
								path.AddLine(rect.X,rect.Y + rect.Height / 2f,rect.Right,rect.Y + rect.Height/2f);
								e.Graphics.DrawPath(Pens.Black,path);
								marker.MarkerStart(e.Graphics,path,sp);
							}
							e.Graphics.EndContainer(c);
						}
					}
				}
			}
		}

		#endregion
	}
}
