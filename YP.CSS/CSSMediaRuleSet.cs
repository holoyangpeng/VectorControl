using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��@Media
	/// </summary>
	public class CSSMediaRuleSet:CSS.CSSRuleSet,Interface.ICSSMediaRuleSet
	{
		#region ..����ƥ��
		private static Regex regex = new Regex(@"^@media\s(?<medianames>([a-z]+(\s*,\s*)?)+)");
		#endregion

		#region ..Constructor
		internal CSSMediaRuleSet(Base.StyleSheets.IStyleSheet styleSheet):base(styleSheet)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..private fields
		CSS.CSSRuleSetList childrules = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ�Ӽ��Ĺ����б�
		/// </summary>
		public Interface.ICSSRuleSetList ChildRuleSets
		{
			get
			{
				return this.childrules;
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
		internal static CSS.CSSMediaRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			CSS.CSSMediaRuleSet media = null;
			Match m = regex.Match(cssstr);
			if(m.Success)
			{
				cssstr = cssstr.Substring(m.Length);
				media = new CSSMediaRuleSet(styleSheet);
				media.childrules = CSS.CSSRuleSetList.ParseRuleList(ref cssstr,styleSheet);
			}
			return media;
		}
		#endregion
	}
}
