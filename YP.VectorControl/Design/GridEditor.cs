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
	/// 提供网格设置的UI界面
	/// </summary>
	[Serializable]
	internal class GridEditor:ModalEditor
	{
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
					GridSetupDialog grid = new GridSetupDialog();
					if(value is Grid)
						grid.Grid = (Grid)value;
					if(edSvc.ShowDialog(grid) == DialogResult.OK)
						value = grid.Grid;
					
				}
			}
			return value;
		}
		#endregion
	}
}
