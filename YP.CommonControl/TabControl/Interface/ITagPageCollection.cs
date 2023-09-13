using System;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// ����ѡ�����
	/// </summary>
	public interface ITabPageCollection
	{
		/// <summary>
		/// ��ȡ��������������Ԫ����
		/// </summary>
		int Count{get;}

		/// <summary>
		/// ���TabPage
		/// </summary>
		/// <param name="newTab"></param>
		void Add(ITabPage newTab);

		/// <summary>
		/// ��ָ��λ�ò���ѡ�
		/// </summary>
		/// <param name="index">�����������</param>
		/// <param name="newTab"></param>
		void Insert(int index,ITabPage newTab);

		/// <summary>
		/// �Ƴ�TabPage
		/// </summary>
		/// <param name="tab"></param>
		void Remove(ITabPage tab);

		/// <summary>
		/// �Ƴ�ָ����������ѡ�
		/// </summary>
		/// <param name="index"></param>
		void RemoveAt(int index);

		/// <summary>
		/// ��ȡ������ָ����������TabPage
		/// </summary>
		ITabPage this[int index]{set;get;}

		/// <summary>
		/// �жϼ����Ƿ����ָ����TabPage
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		bool Contains(ITabPage tab);

		/// <summary>
		/// ��ȡָ��TabPage�ڼ����е�����
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		int IndexOf(ITabPage tab);
	}
}
