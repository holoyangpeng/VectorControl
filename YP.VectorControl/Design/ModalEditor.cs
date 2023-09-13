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
	/// �ṩһ������������������ͨ���򿪶Ի���ʵ�����Եı༭
	/// </summary>
    public abstract class ModalEditor : BaseEditor
	{
		#region ..���켰����
		public ModalEditor()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..GetEditStyle
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		#endregion
	}
}
