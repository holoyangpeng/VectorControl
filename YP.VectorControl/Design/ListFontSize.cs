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

namespace YP.VectorControl.Design
{
	/// <summary>
	/// ListFontSize 的摘要说明。
	/// </summary>
	internal class ListFontSize:DropDownEditor
	{
		#region ..构造及消除
		public ListFontSize()
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
					System.Windows.Forms.ListBox style = new ListBox();
					float[] a = new float[]{8,9,10,11,12,13,14,16,18,20,22,24, 26,28,36,48,72,80,88,96, 128,168};
					for(int j = 0;j<a.Length;j++)
					{
						style.Items.Add(a[j]);
					}
					a = null;
					style.BorderStyle = BorderStyle.None;
					style.Height = 150;
					style.Width = 80;
					style.SelectedIndex = style.FindString(value.ToString());
					style.SelectedIndexChanged += new EventHandler(arrow_SelectedIndexChanged);
					edSvc.DropDownControl(style);
					if(changed && style.SelectedItem != null)
					{
						try
						{
							value = float.Parse(style.SelectedItem.ToString());
						}
						catch{}
					}
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
	}
}
