using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����@Media�����һ����Ϊ
	/// </summary>
	public interface ICSSMediaRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// ��ȡ�Ӽ��Ĺ����б�
		/// </summary>
		Interface.ICSSRuleSetList ChildRuleSets{get;}
	}
}
