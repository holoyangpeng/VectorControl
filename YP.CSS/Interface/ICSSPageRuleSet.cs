using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����@Page�����һ����Ϊ
	/// </summary>
	public interface ICSSPageRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		Interface.ICSSRuleSetContent RuleSetContent{get;}
	}
}
