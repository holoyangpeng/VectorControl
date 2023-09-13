using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����һ��CSS StyleSheet��һ����Ϊ����һ��StyleSheet��ContentType ͨ������Ϊ��text/css"
	/// </summary>
	public interface ICSSStyleSheet:Base.StyleSheets.IStyleSheet
	{
		/// <summary>
		/// ��ȡ�Ӽ�CSS�鼯��
		/// </summary>
		Interface.ICSSRuleSetList ChildRuleSets{get;}

		/// <summary>
		/// ����һ��CSSRuleSet
		/// </summary>
		/// <param name="rulesetname">CSS����������</param>
		Interface.ICSSRuleSet CreateRuleSet(string rulesetname);


		/// <summary>
		/// ��ȡ������RuleSet�������StyleSheetͨ��@Import�����Ļ�
		/// </summary>
		Interface.ICSSRuleSet ParentRule{get;}
	}
}
