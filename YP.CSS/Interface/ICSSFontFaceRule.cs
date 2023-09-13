using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义@font-face
	/// </summary>
	public interface ICSSFontFaceRule
	{
		/// <summary>
		/// 获取规则内容
		/// </summary>
		Interface.ICSSRuleSetContent RuleSetContent{get;}
	}
}
