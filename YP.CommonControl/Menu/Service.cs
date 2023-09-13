using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace YP.CommonControl.Menu
{
	/// <summary>
	/// Service ��ժҪ˵����
	/// </summary>
	public class DropDownService:IServiceProvider,IWindowsFormsEditorService	
	{
		#region ..MsgWaitForMultipleObjects
		[System.Runtime.InteropServices.DllImport("User32", SetLastError=true)]
			static extern int MsgWaitForMultipleObjects(int count,IntPtr pHandles,int bWaitAll,int dwMilliseconds,int dwWakeMask);
		#endregion

		#region ..Constructor
		public DropDownService(Control parent)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this._parent = parent;
		}
		#endregion

		#region ..private fields
		DropDownHolder holder = null;
		Control _parent = null;
		bool _canceled = false;
		bool doevents = true;
		#endregion

		#region ..public properties
		public bool Canceled
		{
			get
			{
				if(this.holder != null)
					return this.holder.Canceled;
				return true;
			}
		}

		public bool DoEvents
		{
			set
			{
				this.doevents = value;
			}
			get
			{
				return this.doevents;
			}
		}

		public bool Focused
		{
			get
			{
				return this.holder != null && this.holder.ContainsFocus;
			}
		}

		public bool InDropDown
		{
			get
			{
				return this.holder != null &&this.holder.Visible;
			}
		}
		#endregion

		#region IServiceProvider ��Ա
		public object GetService(Type serviceType)
		{
			if(serviceType.Equals(Type.GetType("System.ComponentModel.IWindowsFormsEditorService")))
				return this;
			// TODO:  ��� Service.GetService ʵ��
			return null;
		}
		#endregion

		#region IWindowsFormsEditorService ��Ա
		public void DropDownControl(Control control)
		{
			this._canceled = false;
			if(this.holder == null)
			{
				this.holder = new DropDownHolder();
				if(this._parent != null)
					this.holder.Location = this._parent.PointToScreen(new Point(0,0));
			}
			this.control_SizeChanged(control,EventArgs.Empty);
			this.holder.SetDrowDownControl(control);
			control.SizeChanged += new EventHandler(control_SizeChanged);
			this.PositionDropDownHolder();
			this.holder.Visible = true;
			if(this._parent != null)
				this.holder.Owner = this._parent.FindForm();
			this.DoModalLoop();
			this._canceled = this.holder.Canceled;
		}

		public void CloseDropDown()
		{
			// TODO:  ��� Service.CloseDropDown ʵ��
			if(this.holder != null &&this.holder.Visible)
				this.holder.CloseDropDown();
		}

		public System.Windows.Forms.DialogResult ShowDialog(Form dialog)
		{
			// TODO:  ��� Service.ShowDialog ʵ��
			throw new NotSupportedException(); 
		}
		#endregion

		#region ..PositionDropDownHolder
		void PositionDropDownHolder()
		{
			if(this._parent != null)
			{
				Point loc = this._parent.PointToScreen(new Point(0,0));
				Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
				if(loc.X < screenRect.X)
					loc.X = screenRect.X;
				else if(loc.X + this.holder.Width > screenRect.Right)
					loc.X = screenRect.Right - this.holder.Width - 2;

				if(loc.Y + this._parent.Height + this.holder.Height > screenRect.Bottom)
					loc.Offset(0,-this.holder.Height + 1);
				else
					loc.Offset(0,this._parent.Height - 1);
				this.holder.Popup(loc);
			}
		}
		#endregion

		#region ..DoModalLoop
		void DoModalLoop()
		{
			if(this.doevents)
			{
				System.Diagnostics.Debug.Assert(this.holder != null);
				while(this.holder .Visible)
				{
					Application.DoEvents();
					MsgWaitForMultipleObjects(1, IntPtr.Zero, 1, 5, 255);
				}
			}
		}
		#endregion

		#region ..SetDropSize
		public void SetDropSize(Size size)
		{
			if(this.holder != null)
			{
				this.holder.Size = new Size(size.Width + 2,size.Height + 2);
				this.PositionDropDownHolder();
			}
		}
		#endregion

		#region ..control_SizeChanged
		private void control_SizeChanged(object sender, EventArgs e)
		{
			Control control = sender as Control;
			this.holder.Size = new Size(control.Width + 1,control.Height + 1);
		}
		#endregion
	}
}
