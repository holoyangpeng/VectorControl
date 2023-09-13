using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 定义@page
	/// </summary>
	public class CSSPageRuleSet:CSS.CSSRuleSet,Interface.ICSSPageRuleSet
	{
		#region ..文字匹配
		private static Regex regex = new Regex(@"^@page");
		#endregion

		#region ..Constructor
		internal CSSPageRuleSet(Interface.ICSSStyleSheet styleSheet):base(styleSheet)
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
		internal static CSS.CSSPageRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			CSSPageRuleSet page = null;
			Match m = regex.Match(cssstr);
			if(m.Success && m.Length > 0)
			{
				cssstr = cssstr.Substring(m.Length);
				page = new CSSPageRuleSet(styleSheet);
				page.content = CSS.CSSRuleSetContent.ParseRuleContent(ref cssstr);
			}
			return page;
		}
		#endregion
	}
}
