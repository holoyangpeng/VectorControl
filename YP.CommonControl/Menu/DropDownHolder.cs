using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.CommonControl.Menu
{
	/// <summary>
	/// DropDownHolder 的摘要说明。
	/// </summary>
	internal class DropDownHolder : Common.BaseForm
	{
		#region ..Constructor
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;
		System.Windows.Forms.Panel panel1;
		public DropDownHolder()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			this.KeyPreview = true;
			this.StartPosition = FormStartPosition.Manual;
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(292, 273);
			this.panel1.TabIndex = 0;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// DropDownHolder
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DropDownHolder";
			this.ShowInTaskbar = false;
			this.Text = "DropDownHolder";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..private fields
		bool _closeDropDownCalled = false;
		bool _canceled = false;
		#endregion

		#region ..属性
		public bool Canceled
		{
			get
			{
				return this._canceled;
			}
		}
		#endregion

		#region ..CloseDropDown
		public void CloseDropDown()
		{
			this._closeDropDownCalled = true;
			this.Hide();
		}
		#endregion

		#region ..SetDrowDownControl
		public void SetDrowDownControl(Control c)
		{
			if(!this.panel1.Controls.Contains(c))
				this.panel1.Controls.Add(c);
			c.BringToFront();
		}
		#endregion

		#region ..OnKeyDown
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);
			if(e.Modifiers == 0 &&e.KeyCode == Keys.Escape)
				this.Hide();
		}
		#endregion

		#region ..OnDeactivate
		protected override void OnDeactivate(EventArgs e)
		{
			this.Owner = null;
			base.OnDeactivate (e);
			if(this._closeDropDownCalled)
				this._canceled = true;
			this.Hide();
		}
		#endregion

		#region override Property CreateParams
		protected override CreateParams CreateParams 
		{
			get
			{
				// Extend the CreateParams property 
				CreateParams cp = base.CreateParams;

				cp.Parent = IntPtr.Zero;
			
				// Appear as a top-level window
				cp.Style = unchecked((int)(uint)Win32.Enum.WindowStyles.WS_POPUP);
			
				// Set styles so that it does not have a caption bar and is above all other 
				// windows in the ZOrder, i.e. TOPMOST
				cp.ExStyle = (int)Win32.Enum.WindowExStyles.WS_EX_TOPMOST + 
					(int)Win32.Enum.WindowExStyles.WS_EX_TOOLWINDOW;
				return cp;
			}
		}
		#endregion

		#region ..Popup
		internal void Popup(Point screenPoint)
		{
			this.Location = screenPoint;
		}
		#endregion

		#region ..panel1_Paint
		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(SystemPens.ControlDark,0,0,this.Width - 1,this.Height - 1);
		}
		#endregion
	}
}
