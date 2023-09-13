using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现@font-face
	/// </summary>
	public class CSSFontFaceRuleSet:CSS.CSSRuleSet ,Interface.ICSSFontFaceRule
	{
		private static Regex regex = new Regex(@"^@font-face");

		#region ..Constructor
		internal CSSFontFaceRuleSet(Interface.ICSSStyleSheet styleSheet):base(styleSheet)
		{
		//
		// TODO: 在此处添加构造函数逻辑
		//
		}
		#endregion

		#region ..private fields
		CSS.CSSRuleSetContent content = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取规则内容
		/// </summary>
		public Interface.ICSSRuleSetContent RuleSetContent
		{
			get
			{
				return this.content;
			}
		}

		public override string CSSText
		{
			get
			{
				return string.Empty;
			}
		}
		#endregion

		#region ..解析CSS Style
		/// <summary>
		/// 解析CSS Style 
		/// </summary>
		/// <param name="cssstr"></param>
		/// <param name="styleparse"></param>
		/// <param name="styleSheet"></param>
		/// <returns></returns>
		internal static CSS.CSSFontFaceRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			CSS.CSSFontFaceRuleSet page = null;
			Match m = regex.Match(cssstr);
			if(m.Success && m.Length > 0)
			{
				cssstr = cssstr.Substring(m.Length);
				page = new CSSFontFaceRuleSet(styleSheet);
				page.content = CSS.CSSRuleSetContent.ParseRuleContent(ref cssstr);
			}
			return page;
		}
		#endregion
	}
}
