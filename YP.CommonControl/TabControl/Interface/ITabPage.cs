using System;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// ����TabPage��һ����Ϊ
	/// </summary>
	public interface ITabPage
	{
		/// <summary>
		/// ���ƻ�����TabPage����ʾ�Ŀؼ�
		/// </summary>
		Control Control{set;get;}

		/// <summary>
		/// ��ȡ������TabPage�ɼ���
		/// </summary>
		bool Visible{set;get;}

		/// <summary>
		/// ��ȡ������TabPage��ͼ������
		/// </summary>
		System.Drawing.Image Image{set;get;}

		/// <summary>
		/// ��ȡ������TabPage�ı���
		/// </summary>
		string Text{set;get;}
	}
}
