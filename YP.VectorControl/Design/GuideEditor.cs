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
using YP.VectorControl.Forms;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// GuideEditor 的摘要说明。
	/// </summary>
	[Serializable]
    internal class GuideEditor : ModalEditor
	{
		#region ..私有变量
		public GuideEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
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
					GuideSetupDialog grid = new GuideSetupDialog();
					if(value is Guide)
						grid.Guide = (Guide)value;
					if(edSvc.ShowDialog(grid) == DialogResult.OK)
						value = grid.Guide;
					
				}
			}
			return value;
		}
		#endregion
	}
}
