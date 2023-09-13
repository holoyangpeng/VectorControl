using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl
{
	#region ..��������
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
	/// ʵ��ѡ�
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

		#region ..�¼�
		/// <summary>
		/// �������Ըı��¼�
		/// </summary>
		internal delegate void PropertyChangedEventHandler(object sender,PropertyStyle property,object oldValue);

		/// <summary>
		/// �����Ըı�ʱ����
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

		#region ..��������
		internal TabControl TabControl;
		#endregion

		#region ..public properties
		/// <summary>
		/// ���ƻ�����TabPage����ʾ�Ŀؼ�
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
		/// ��ȡ������TabPage��ͼ������
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
		/// ��ȡ������TabPage�ı���
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
		/// ��ȡ������TabPage�Ŀɼ���
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

		#region ..�����Է����ı�ʱ����
		void OnPropertyChanged(PropertyStyle property,object oldValue)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this,property,oldValue);
		}
		#endregion

		#region ..ת��ΪForm
		/// <summary>
		/// ���л�Ϊ������ʾģʽʱ����ȡ��Ӧ��Form
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

		#region ..����
		void Focusthis(object sender,EventArgs e)
		{
			if(this.TabControl != null)
				this.TabControl.SelectedTab = this;
		}
		#endregion
	}
}
