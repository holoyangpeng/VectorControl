using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl
{
	#region ..属性类型
	internal enum PropertyStyle
	{
		Title,
		Image,
		Control,
		Focused,
		Closed,
		MaximumSize,
		Visible
	}
	#endregion

	/// <summary>
	/// 实现选项卡
	/// </summary>
	public class TabPage:Common.BaseControl ,Interface.ITabPage
	{
		#region ..Constructor
		public TabPage(string title):this(title,null)
		{
			
	
		}

		public TabPage(string title,Control c):this(title,c,null)
		{
		}

		public TabPage(string title,Control c,Image image)
		{
			this.text = title;
			this.control  = c;
			this.image = image;
			this.tabForm = new TabForm(this);
			this.Font = SystemInformation.MenuFont;
		}
		#endregion

		#region ..事件
		/// <summary>
		/// 处理属性改变事件
		/// </summary>
		internal delegate void PropertyChangedEventHandler(object sender,PropertyStyle property,object oldValue);

		/// <summary>
		/// 当属性改变时发生
		/// </summary>
		internal event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region ..private fields
		Control control = null;
		string text = string.Empty;
		Form tabForm = null;
		System.Drawing.Image image = null;
		bool visible = true;
       // object _tag = null;
		#endregion

		#region ..保护变量
		internal TabControl TabControl;
		#endregion

		#region ..public properties
		/// <summary>
		/// 控制或设置TabPage所显示的控件
		/// </summary>
		public Control Control
		{
			set
			{
				if(this.control != value)
				{
					Control old = this.control;
					this.control = value;
					this.OnPropertyChanged(PropertyStyle.Control,old);
				}
			}
			get
			{
				return this.control;
			}
		}

		/// <summary>
		/// 获取或设置TabPage的图像索引
		/// </summary>
		public System.Drawing.Image Image
		{
			set
			{
				if(this.image != value)
				{
					System.Drawing.Image old = this.image;
					this.image = value;
					this.OnPropertyChanged(PropertyStyle.Image,old);
				}
			}
			get
			{
				return this.image;
			}
		}

		/// <summary>
		/// 获取或设置TabPage的标题
		/// </summary>
		public override string Text
		{
			set
			{
				if(this.text != value)
				{
					string old = this.text;
					this.text = value;
					this.TabForm.Text = text;
					this.OnPropertyChanged(PropertyStyle.Title,old);
				}
			}
			get
			{
				return this.text;
			}
		}
		
		/// <summary>
		/// 获取或设置TabPage的可见性
		/// </summary>
		public new bool Visible
		{
			set
			{
				if(this.visible != value)
				{
					bool old = this.visible;
					this.visible = value;
					this.OnPropertyChanged(PropertyStyle.Visible,old);
				}
			}
			get
			{
				return this.visible;
			}
		}
		#endregion

		#region ..当属性发生改变时发生
		void OnPropertyChanged(PropertyStyle property,object oldValue)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this,property,oldValue);
		}
		#endregion

		#region ..转换为Form
		/// <summary>
		/// 当切换为窗体显示模式时，获取相应的Form
		/// </summary>
		internal Form TabForm
		{
			get
			{
				if(tabForm == null)
				{
					this.tabForm = new TabForm(this);
					tabForm.Size = new System.Drawing.Size(500,400);
					this.tabForm.Closed += new EventHandler(Close);
					this.tabForm.SizeChanged += new EventHandler(ChangeSize);
					this.tabForm.GotFocus += new EventHandler(Focusthis);
					this.tabForm.Disposed += new EventHandler(DisposeForm);
				}
				return this.tabForm;
			}
		}
		#endregion

		#region ..Close
		void Close(object sender,EventArgs e)
		{
			this.OnPropertyChanged(PropertyStyle.Closed,true);
		}
		#endregion

		#region ..ChangeSize
		void ChangeSize(object sender,EventArgs e)
		{
			if(this.tabForm.WindowState == FormWindowState.Maximized)
				this.OnPropertyChanged(PropertyStyle.MaximumSize,this.tabForm.Size);
		}
		#endregion

		#region ..DisposeForm
		void DisposeForm(object sender,EventArgs e)
		{
			this.tabForm = null;
		}
		#endregion

		#region ..激活
		void Focusthis(object sender,EventArgs e)
		{
			if(this.TabControl != null)
				this.TabControl.SelectedTab = this;
		}
		#endregion
	}
}
