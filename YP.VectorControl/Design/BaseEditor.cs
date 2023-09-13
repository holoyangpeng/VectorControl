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
	/// 提供编辑器的基类，这些编辑器可以基于UI，改变相关对象的值
	/// </summary>
	public abstract class BaseEditor:UITypeEditor
	{
		#region ..构造及消除
		public BaseEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		protected IWindowsFormsEditorService edSvc = null;
		#endregion


	}
}
