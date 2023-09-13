using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ�ֵ�����CSS style����
	/// </summary>
	public class CSSStyleRuleSet:CSS.CSSRuleSet,Interface.ICSSStyleRuleSet
	{
		#region ..�ı�ƥ��
		internal static string nsPattern = string.Empty;
		internal static string attributeValueCheck = string.Empty;
		
		internal static string sSelector = string.Empty; 
			
		private static string sStyleRule = string.Empty;
		private static Regex regex = null;

        static CSSStyleRuleSet()
        {
            CSSStyleRuleSet.nsPattern = @"([A-Za-z\*][A-Za-z0-9]*)?\|";
            CSSStyleRuleSet.attributeValueCheck = "(?<attname>(" + nsPattern + ")?[a-zA-Z0-9]+)\\s*(?<eqtype>[\\~\\^\\$\\*\\|]?)=\\s*(\"|\')?(?<attvalue>.*?)(\"|\')?";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("(?<ns>" + nsPattern + ")?");
            sb.Append(@"(?<type>([A-Za-z\*][A-Za-z0-9]*))?" );
            sb.Append(@"((?<class>\.[A-Za-z][A-Za-z0-9]*)+)?" );
            sb.Append(@"(?<id>\#[A-Za-z][A-Za-z0-9]*)?" );
            sb.Append(@"((?<predicate>\[\s*(" );
            sb.Append(@"(?<attributecheck>(" + nsPattern + ")?[a-zA-Z0-9]+)" );
            sb.Append(@"|" );
            sb.Append("(?<attributevaluecheck>" + attributeValueCheck + ")" );
           sb.Append( @")\s*\])+)?" );
            sb.Append(@"((?<pseudoclass>\:[a-z\-]+(\([^\)]+\))?)+)?" );
            sb.Append(@"(?<pseudoelements>(\:\:[a-z\-]+)+)?" );
            sb.Append(@"(?<seperator>(\s*(\+|\>|\~)\s*)|(\s+))?");
            CSSStyleRuleSet.sSelector = sb.ToString();
            CSSStyleRuleSet.sStyleRule = "^((?<selector>(" + sSelector + @")+)(\s*,\s*)?)+";
            CSSStyleRuleSet.regex = new Regex(sStyleRule);
        }
		#endregion

		#region ..Constructor
		internal CSSStyleRuleSet(Interface.ICSSStyleSheet styleSheet):base(styleSheet)
		{
		}
		#endregion

		#region ..private fields
		string selectorText = string.Empty;
		CSS.CSSRuleSetContent ruleContent = null;
		Selector[] selectors = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡѡ�������ı����
		/// </summary>
		public string SelectorText
		{
			get
			{
				return this.selectorText;
			}
			set
			{
				this.selectorText = value;
			}
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public Interface.ICSSRuleSetContent RuleSetContent
		{
			get
			{
				return this.ruleContent;
			}
		}

		/// <summary>
		/// ��ȡCSS���ݱ��
		/// </summary>
		public override string CSSText
		{
			get
			{
				return this.selectorText + "{" +this.ruleContent.CSSText + "}";
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
		internal static CSS.CSSStyleRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			Match match = regex.Match(cssstr);
			if(match.Success && match.Length > 0)
			{
				CSS.CSSStyleRuleSet rule = new CSS.CSSStyleRuleSet(styleSheet);

				rule.selectorText = cssstr.Substring(0,match.Length);

				Group selectorMatches = match.Groups["selector"];

				int len = selectorMatches.Captures.Count;
				ArrayList sels = new ArrayList();
				for(int i = 0; i<len; i++)
				{
					string str = rule.ReFormatString(selectorMatches.Captures[i].Value.Trim());
					if(str.Length > 0)
					{
						sels.Add(new Selector(str));
					}
				}
				rule.selectors = (Selector[])sels.ToArray(typeof(Selector));
				cssstr = cssstr.Substring(match.Length);
				rule.ruleContent = CSS.CSSRuleSetContent.ParseRuleContent(ref cssstr);
				return rule;
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region ..ƥ��ڵ�
		/// <summary>
		/// ƥ��ڵ�
		/// </summary>
		/// <param name="element">���ͽڵ�</param>
		/// <param name="content">CSS��������</param>
		/// <returns></returns>
		public override void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			foreach(Selector sel in this.selectors)
			{
				if(sel != null)
				{
					bool match = element.MatchXPath(sel.XPath);

					if(match)
					{
						string[] s = this.ruleContent.PropertyNames;
						for(int i = 0;i<s.Length;i++)
						{
							string name = s[i];
							string valuestr = this.ruleContent.GetProperty(name);
							string priority = this.ruleContent.GetPriority(name);
							content.SetProperty(name,valuestr,priority,sel.Level);
						}
						break;
					}
				}
			}
		}
		#endregion
	}
}
