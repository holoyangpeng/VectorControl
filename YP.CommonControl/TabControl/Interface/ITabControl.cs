using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// ����TabControl��һ����Ϊ
	/// </summary>
	public interface ITabControl
	{
		#region ..����
		/// <summary>
		/// ��ȡ������ѡ��������
		/// </summary>
		int SelectedIndex{set;get;}

		/// <summary>
		/// ��ȡ�����õ�ǰѡ���ѡ�
		/// </summary>
		ITabPage SelectedTab{set;get;}

		/// <summary>
		/// ��ȡ������ѡ��Ƿ���ʾ�ڶ���
		/// </summary>
		bool PositionTop{set;get;}

		/// <summary>
		/// ��ȡ��ǰ�Ƿ���ͨ��MDI�Ӵ�����ʽ��ʾ
		/// </summary>
		bool MDIFormMode{get;}

		/// <summary>
		/// �����Ƿ���ʾѡ�������ť
		/// </summary>
		bool ShowTabButton{set;get;}

		/// <summary>
		/// ��ȡ������ѡ��Ƿ������ȼ�����
		/// </summary>
		bool HotTrack{set;get;}

		/// <summary>
		/// ��ȡ������ѡ��ĳ��ȼ��㷽ʽ
		/// </summary>
		TabSizeMode SizeMode{set;get;}

		/// <summary>
		/// ��ȡ������ѡ���ѡ����������ɫ
		/// </summary>
		Color TabColor{set;get;}

		/// <summary>
		/// ��ѡ�����ά�ȷ�Χʱ���Ƿ���ʾ������ť
		/// </summary>
		bool ShowNavigateButton{set;get;}

		/// <summary>
		/// ѡ��Ƿ���ʾ���ư�ť
		/// </summary>
		bool ShowControlBox{set;get;}

		/// <summary>
		/// ��ȡTabControl��ѡ�����
		/// </summary>
		ITabPageCollection TabPages{get;}

		/// <summary>
		/// ����TabControl�Ƿ���ʾIDE���͵ı�
		/// </summary>
		bool IDEBorder{set;get;}
		#endregion

		#region ..�¼�
		/// <summary>
		/// ��ѡ����������ı�ʱ����
		/// </summary>
		event EventHandler SelectedIndexChanged;
		#endregion
	}
}
