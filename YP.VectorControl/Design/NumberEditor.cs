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
	/// NumberEditor ��ժҪ˵����
	/// </summary>
	internal class NumberEditor:DropDownEditor
	{
		#region ..���켰����
		public NumberEditor()
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
					System.Windows.Forms.NumericUpDown number = new NumericUpDown();
					number.Height = 40;
					number.BorderStyle = BorderStyle.None;
					try
					{
						number.Value = Decimal.Parse(value.ToString());
					}
					catch{}

					number.Minimum = 0;
					number.Maximum = 1;
					number.DecimalPlaces = 2;
					number.Increment = 0.1M;
					edSvc.DropDownControl(number);
					value = (float)number.Value;
				}
			}
			return value;
		}
		#endregion
	}
}
