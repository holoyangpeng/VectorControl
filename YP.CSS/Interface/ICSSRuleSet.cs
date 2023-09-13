using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ʵ��CSS��һ��������CSS�������һ����Ϊ
	/// </summary>
	public interface ICSSRuleSet
	{
		/// <summary>
		/// ��ȡCSS���������
		/// </summary>
		byte CSSType{get;}

		/// <summary>
		/// ����RuleSet
		/// </summary>
		Interface.ICSSRuleSet ParentRule{get;}

		/// <summary>
		/// ��ȡ������ı����
		/// </summary>
		string CSSText{get;}

		/// <summary>
		/// ��ȡ������StyleSheet
		/// </summary>
		Base.StyleSheets.IStyleSheet ParentStyleSheet{get;}

		/// <summary>
		/// ƥ��ָ���Ľڵ�
		/// </summary>
		/// <param name="element">���ͽڵ�</param>
		/// <param name="content">��������</param>
		void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content);
	}
}
