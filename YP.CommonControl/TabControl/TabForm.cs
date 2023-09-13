using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl
{
	/// <summary>
	/// ������Formģʽ����TabPage���Ӧ��Form
	/// </summary>
	internal class TabForm : Common.BaseForm
	{
		#region ..Constructor
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TabForm(TabPage page)
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();
			this.TabPage = page;

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
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
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
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
