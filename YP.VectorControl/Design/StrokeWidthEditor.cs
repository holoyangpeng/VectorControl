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
	/// 提供选择线条粗细的编辑器
	/// </summary>
	public class StrokeWidthEditor:DropDownEditor
	{
		#region ..构造及消除
		public StrokeWidthEditor()
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
					ListStrokeWidth style = new ListStrokeWidth();
					style.Height = 150;
					int index = style.FindString(value.ToString());
					style.SelectedIndex = index;
					style.SelectedIndexChanged += new EventHandler(style_SelectedIndexChanged);
					
					edSvc.DropDownControl(style);
					if(changed)
					{
						try
						{
							string a = style.SelectedItem.ToString();
							value = float.Parse(a);
							a = null;
						}
						catch{}
					}
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
			float a = 1f;
			try
			{
				a = float.Parse(e.Value.ToString());
			}
			catch{}
			using(System.Drawing.Pen pen = new Pen(Color.Black,a))
			{
				pen.Alignment = PenAlignment.Center;
				float top = e.Bounds.Y + (float)(e.Bounds.Height)/2f;
				e.Graphics.DrawLine(pen,e.Bounds.X + 1,top,e.Bounds.Right - 1,top);
			}
		}
		#endregion

	}
}
