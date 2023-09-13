using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����CSS�����ļ��ϲ���
	/// </summary>
	public interface ICSSRuleSetList
	{
		/// <summary>
		/// ��ȡ������Ŀ
		/// </summary>
		int Count{get;}

		/// <summary>
		/// ��ȡָ����
		/// </summary>
		/// <param name="index">����</param>
		Interface.ICSSRuleSet GetItem(int index);

		/// <summary>
		/// ƥ��ָ���Ľڵ�
		/// </summary>
		/// <param name="element">���ͽڵ�</param>
		/// <param name="content">��������</param>
		void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content);
	}
}
