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
	/// LabelTextEditor ��ժҪ˵����
	/// </summary>
	internal class LabelTextEditor:ModalEditor
	{
		#region ..���켰����
		public LabelTextEditor()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
					InputDialog input = new InputDialog();
					input.Content = value.ToString();
					if(edSvc.ShowDialog(input) == DialogResult.OK)
						value = input.Content;
				}
			}
			return value;
		}
		#endregion
	}
}
