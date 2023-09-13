using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ����@page
	/// </summary>
	public class CSSPageRuleSet:CSS.CSSRuleSet,Interface.ICSSPageRuleSet
	{
		#region ..����ƥ��
		private static Regex regex = new Regex(@"^@page");
		#endregion

		#region ..Constructor
		internal CSSPageRuleSet(Interface.ICSSStyleSheet styleSheet):base(styleSheet)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..private fields
		CSS.CSSRuleSetContent content = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ��������
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

		#region ..����CSS Style
		/// <summary>
		/// ����CSS Style 
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
