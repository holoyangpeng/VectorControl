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
	/// 提供选择线条样式的编辑器
	/// </summary>
	public class StrokeStyleEditor:DropDownEditor
	{
		#region ..私有变量
		public StrokeStyleEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		bool changed = false;
		static System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"\s+");
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
					ListStrokeStyle style = new ListStrokeStyle();
					style.Height = 150;
					string temp =value.ToString().Replace(","," ");
					temp = rg.Replace(temp," ");
					int index = style.FindString(temp);
					temp = null;
					style.SelectedIndex = index;
					style.SelectedIndexChanged += new EventHandler(style_SelectedIndexChanged);
					edSvc.DropDownControl(style);
					if(changed)
					{
						if(style.SelectedItem != null)
							value = style.SelectedItem.ToString();
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
            if (e.Value != null)
            {
                string a = e.Value.ToString();
                a = a.Replace(",", " ");
                a = rg.Replace(a, " ");
                string[] strings = a.Split(new char[] { ' ' });

                bool valid = false;
                float[] temp = null;
                if (strings.Length > 1)
                {
                    valid = true;
                    temp = new float[strings.Length];
                    try
                    {
                        for (int i = 0; i < temp.Length; i++)
                            temp[i] = float.Parse(strings[i]);
                    }
                    catch
                    {
                        valid = false;
                    }
                }
                using (System.Drawing.Pen pen = new Pen(Color.Black, 1))
                {
                    pen.Alignment = PenAlignment.Center;
                    if (valid)
                        pen.DashPattern = temp;
                    float top = e.Bounds.Y + (float)(e.Bounds.Height) / 2f;
                    e.Graphics.DrawLine(pen, e.Bounds.X + 1, top, e.Bounds.Right - 1, top);
                }
            }
		}

		#endregion
	}
}
