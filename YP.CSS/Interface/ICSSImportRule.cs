using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����@import���͹����һ����Ϊ
	/// </summary>
	public interface ICSSImportRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// ��ȡָ���StyleSheet
		/// </summary>
		Interface.ICSSStyleSheet RefStyleSheet{get;}
	}
}
