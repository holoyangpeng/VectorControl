using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>�������塣</para>
	/// <para>ͨ��PopupWindow�������Խ��Զ���Ŀؼ��Ե����˵��ķ�ʽ�ṩ���û���</para>
	/// <para>ע��,����Ҫ���ص�������,�����<see cref="Selector.PopupWindow.HidePopup">HidePopup()</see>��������ò�Ҫֱ�ӵ���Close().</para>
	/// </summary>
	public class PopouWindow : System.Windows.Forms.Form
	{
		#region ..���켰����
		protected Control m_Parent     = null;

		protected  int margin = 1;

		Control control = null;
		
		/// <summary>
		/// Designer support.
		/// </summary>
		internal PopouWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			this.Width = 150;
			InitializeComponent();

			this.ShowInTaskbar = false;

//			this.ControlRemoved += new ControlEventHandler(PopouWindow_ControlRemoved);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			SetStyle(ControlStyles.ResizeRedraw,true);
		}

		/// <summary>
		/// ����һ����������,�ô����ڲ�����popupControl�ؼ�����������parent�ؼ�
		/// </summary>
		/// <param name="parent">�������������Ŀؼ������ÿؼ�ʧȥ����ʱ��������������</param>
		/// <param name="popupControl">�������������ؼ�,�ÿؼ��ڴ����ڲ���������������</param>
		public PopouWindow(Control parent,Control popupControl):this()
		{
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			m_Parent = parent;
			this.ShowInTaskbar = false;
			this.PopupControl = popupControl;
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ���õ����ؼ������ô˿ؼ���ʹ����ֻ����һ���ӿؼ�
		/// </summary>
		Control PopupControl
		{
			set
			{
				this.Controls.Clear();
				if(value != control)
				{
					this.control = value;
				}
				if(this.control != null)
				{
					this.DockPadding.All = this.margin;
					control.Location = new Point(margin,margin);
					control.Dock = DockStyle.Fill;
					this.Controls.Add(control);
					this.MainControl = control;
				}
			}
		}

		/// <summary>
		/// ��ȡ�������ڵ���λ��ʱ��Ҫ�����ĳߴ�
		/// </summary>
		public Size AdjustSize
		{
			set
			{
				this.adjustSize = value;
			}
			get
			{
				return this.adjustSize;
			}
		}

		/// <summary>
		/// ��ȡ������һ��ֵ������ָʾ�ؼ��Ƿ������뽹��
		/// </summary>
		public override bool Focused
		{
			get
			{
				if(this.mainControl != null)
					return this.mainControl.Focused;
				return base.Focused;
			}
		}

		/// <summary>
		/// ��ȡ�����е���Ҫ���ÿؼ�
		/// </summary>
		Control MainControl
		{
			get
			{
				return this.mainControl;
			}
			set
			{
				if(this.mainControl != value)
				{
					this.mainControl = value;
//					this.mainControl.Focus();
//					this.mainControl.LostFocus += new EventHandler(mainControl_LostFocus);
				}
			}
		}
		#endregion

		#region ..˽�б���
		Control mainControl;
		Size adjustSize = Size.Empty;
		#endregion

		#region function Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{

			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// WPopUpFormBase
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(200, 168);
			this.ControlBox = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WPopUpFormBase";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
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
				cp.Style = unchecked((int)(uint)WindowStyles.WS_POPUP);
			
				// Set styles so that it does not have a caption bar and is above all other 
				// windows in the ZOrder, i.e. TOPMOST
				cp.ExStyle = (int)WindowExStyles.WS_EX_TOPMOST + 
					(int)WindowExStyles.WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		#endregion

		#region ..����
		bool _exitLoop = false;
		/// <summary>
		/// ����������
		/// </summary>
		/// <param name="screenPoint">��Ļ����</param>
		public virtual void Popup(Point screenPoint)
		{
			try
			{
				Point pt = screenPoint;
				this.ShowInTaskbar = false;
				//			this.Location = pt;
			
				pt = screenPoint;
				Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
				if(screenRect.Bottom < pt.Y + this.Height)
				{
					pt.Y = pt.Y - this.Height - 1 - this.adjustSize.Height;
				}

				if(screenRect.Right < pt.X + this.Width)
				{
					pt.X = screenRect.Right - this.Width - 2;
				}
				pt.X = (int)Math.Max(0,pt.X);
				pt.Y = (int)Math.Min(Math.Max(0,pt.Y),SystemInformation.WorkingArea.Height - this.Height - 1);
				this.Location = pt;

				Win32.ShowWindow(this.Handle, (short)ShowWindowStyles.SW_SHOWNOACTIVATE);

				if(this.mainControl != null)
					this.mainControl.Focus();
				this._exitLoop = false;
				// Create an object for storing windows message information
				Win32.MSG msg = new Win32.MSG();

				bool leaveMsg = false;

				// Process messages until exit condition recognised
				while(!_exitLoop)
				{
					// Suspend thread until a windows message has arrived
					if (Win32.WaitMessage())
					{
						// Take a peek at the message details without removing from queue
						while(!_exitLoop && Win32.PeekMessage(ref msg, 0, 0, 0, (int)PeekMessageFlags.PM_NOREMOVE))
						{
							//						Console.WriteLine("Track {0} {1}", this.Handle, ((Msgs)msg.message).ToString());

							bool eatMessage = false;

							// Mouse was pressed in a window of this application
							if ((msg.message == (int)Msgs.WM_LBUTTONUP) ||
								(msg.message == (int)Msgs.WM_MBUTTONUP) ||
								(msg.message == (int)Msgs.WM_RBUTTONUP) ||
								(msg.message == (int)Msgs.WM_XBUTTONUP) ||
								(msg.message == (int)Msgs.WM_NCLBUTTONUP) ||
								(msg.message == (int)Msgs.WM_NCMBUTTONUP) ||
								(msg.message == (int)Msgs.WM_NCRBUTTONUP) ||
								(msg.message == (int)Msgs.WM_NCXBUTTONUP)
								)
							{
								Point p = MousePosition;

								Rectangle rect = this.RectangleToScreen(new Rectangle(0,0,this.Width,this.Height));
								if(!rect.Contains(p))
								{
									if(this.m_Parent != null)
									{
										rect = this.m_Parent.RectangleToScreen(new Rectangle(0,0,this.m_Parent.Width,this.m_Parent.Height));
										if(!rect.Contains(p))
											this._exitLoop = true;
									}
								}
								//							else if(this.mainControl != null && this.mainControl.Focused)
								//								this._exitLoop = true;
							}

							if((msg.message == (int)Msgs.WM_LBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_RBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_NCLBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_NCMBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_NCXBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_XBUTTONDOWN) ||
								(msg.message == (int)Msgs.WM_MBUTTONDOWN))
							{
								Point p = MousePosition;

								Rectangle rect = this.RectangleToScreen(new Rectangle(0,0,this.Width,this.Height));
								if(!rect.Contains(p))
								{
									if(!(this.mainControl != null && this.mainControl.Capture))
										this._exitLoop = true;
								}
							}
						

							// Mouse move occured
							if (msg.message == (int)Msgs.WM_MOUSEMOVE) 
							{
								// Eat the message to prevent the intended destination getting it
								eatMessage = true;								
							}

							if (msg.message == (int)Msgs.WM_SETCURSOR) 
							{
								// Eat the message to prevent the intended destination getting it
								eatMessage = true;								
							}

							// We consume all keyboard input
							if ((msg.message == (int)Msgs.WM_KEYDOWN) ||
								(msg.message == (int)Msgs.WM_KEYUP) ||
								(msg.message == (int)Msgs.WM_SYSKEYDOWN) ||
								(msg.message == (int)Msgs.WM_SYSKEYUP))					
							{
								// Eat the message to prevent the intended destination getting it
								eatMessage = true;								
							}						

							// Should the message be eaten to prevent intended destination getting it?
							if (eatMessage)
							{
								Win32.MSG eat = new Win32.MSG();
								Win32.GetMessage(ref eat, 0, 0, 0);
							}
							else
							{	
								// Should the message we pulled from the queue?
								if (!leaveMsg)
								{
									if (Win32.GetMessage(ref msg, 0, 0, 0))
									{
										Win32.TranslateMessage(ref msg);
										Win32.DispatchMessage(ref msg);
									}
								}
								else
									leaveMsg = false;
							}
						}
					}
				}

				// Hide the window from view before killing it, as sometimes there is a
				// short delay between killing it and it disappearing because of the time
				// it takes for the destroy messages to get processed
				HideMenuWindow();

				// Commit suicide
				DestroyHandle();
			}
			catch{}
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(Pens.Black,0,0,this.Width - 1,this.Height - 1);
			base.OnPaint (e);
		}
		#endregion

		#region ..WndProc
		protected readonly int WM_DISMISS = (int)Msgs.WM_USER + 1;
		protected override void WndProc(ref Message m)
		{		
			try
			{
				if (m.Msg == WM_DISMISS)
					OnWM_DISMISS();
				else if(m.Msg == (int)Msgs.WM_MOUSEACTIVATE)
				{
					// Do not allow then mouse down to activate the window, but eat 
					// the message as we still want the mouse down for processing
					m.Result = (IntPtr)MouseActivateFlags.MA_NOACTIVATE;
					return;
				}

				base.WndProc(ref m);
			}
			catch{}
		}

		protected void OnWM_DISMISS()
		{	
			try
			{
				this._exitLoop = true;
				// Hide ourself
				HideMenuWindow();

				// Kill ourself
				DestroyHandle();
			}
			catch{}
		}
		#endregion

		#region ..HidePopop
		/// <summary>
		/// ���ص�������
		/// </summary>
		public void HidePopup()
		{
			this.OnWM_DISMISS();
		}
		#endregion

		#region ..����
		protected void HideMenuWindow()
		{
			Win32.ShowWindow(this.Handle, (short)ShowWindowStyles.SW_HIDE);
		}
		#endregion
	}
}
