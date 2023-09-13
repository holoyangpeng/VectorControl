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
	/// 提供编辑文件属性的设计器
	/// </summary>
	public class FileEditor:ModalEditor
	{
		#region ..Constructor
		public FileEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..protected fields
		protected string filter = string.Empty;
		#endregion

		#region ..EditValue
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{

				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (edSvc != null) 
				{
					System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
					dlg.Filter = filter;
					if(value is string)
						dlg.FileName = value.ToString();
					if(dlg.ShowDialog() == DialogResult.OK)
						value = dlg.FileName;
				}
			}
			return value;
		}
		#endregion
	}
}
