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
	/// �ṩ�༭���Ļ��࣬��Щ�༭�����Ի���UI���ı���ض����ֵ
	/// </summary>
	public abstract class BaseEditor:UITypeEditor
	{
		#region ..���켰����
		public BaseEditor()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		protected IWindowsFormsEditorService edSvc = null;
		#endregion


	}
}
