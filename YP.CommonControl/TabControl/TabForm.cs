using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl
{
	/// <summary>
	/// 定义在Form模式下与TabPage相对应的Form
	/// </summary>
	internal class TabForm : Common.BaseForm
	{
		#region ..Constructor
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TabForm(TabPage page)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			this.TabPage = page;

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "TabForm";
		}
		#endregion
		#endregion

		#region ..private fields
		internal TabPage TabPage = null;
		#endregion
	}
}
