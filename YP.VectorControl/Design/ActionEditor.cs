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
	/// ActionEditor 的摘要说明。
	/// </summary>
	internal class ActionEditor:ModalEditor
	{
		#region ..构造及消除
		public ActionEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
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
					ActionSetupDialog grid = new ActionSetupDialog();
					if(value is YP.SVG.ClickAction)
						grid.Action = (YP.SVG.ClickAction)value;
					if(edSvc.ShowDialog(grid) == DialogResult.OK)
						value = grid.Action;
					
				}
			}
			return value;
		}
		#endregion
	} 
}
