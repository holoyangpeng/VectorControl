using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ICSSStyleRule ��ժҪ˵����
	/// </summary>
	public interface ICSSStyleRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// ��ȡѡ�������ı����
		/// </summary>
		string SelectorText{get;set;}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		Interface.ICSSRuleSetContent RuleSetContent{get;}
	}
}
