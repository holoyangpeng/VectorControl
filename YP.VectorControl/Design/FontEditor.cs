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
	/// FontEditor 的摘要说明。
	/// </summary>
	internal class FontEditor:DropDownEditor
	{
		#region ..构造及消除
		public FontEditor()
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
					FontFamily[] families = System.Drawing.FontFamily.Families ;
					for(int j = 0;j<families.Length;j++)
					{
						style.Items.Add(families[j].Name);
					}
					families = null;
					style.BorderStyle = BorderStyle.None;
					style.Height = 150;
					style.SelectedIndex = style.FindString(value.ToString());
					style.SelectedIndexChanged += new EventHandler(arrow_SelectedIndexChanged);
					edSvc.DropDownControl(style);
					if(changed && style.SelectedItem != null)
						value = style.SelectedItem.ToString();
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
