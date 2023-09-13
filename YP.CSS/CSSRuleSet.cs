using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现CSS中单个的规则声明
	/// </summary>
	public abstract class CSSRuleSet:Interface.ICSSRuleSet
	{
		#region ..Constructor
		public CSSRuleSet(Base.StyleSheets.IStyleSheet stylesheet)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.parentStyleSheet = stylesheet;
			if(stylesheet is CSS.CSSStyleSheet)
			{
				if(((CSS.CSSStyleSheet)stylesheet).ReplacedStrings != null)
				{
					this.ReplacedStrings = (string[])((CSS.CSSStyleSheet)stylesheet).ReplacedStrings.Clone();
				}
			}
		}
		#endregion

		#region ..private fields
		Enum.CSSRuleType cssType = Enum.CSSRuleType.STYLE_RULE;
		CSSRuleSet parentRule = null;
		protected string[] ReplacedStrings = new string[0];
		Base.StyleSheets.IStyleSheet parentStyleSheet;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取CSS规则的类型
		/// </summary>
		public byte CSSType
		{
			get
			{
				return (byte)this.cssType;
			}
		}

		/// <summary>
		/// 父级RuleSet
		/// </summary>
		public Interface.ICSSRuleSet ParentRule
		{
			get
			{
				return this.parentRule;
			}
		}

		/// <summary>
		/// 获取所属的StyleSheet
		/// </summary>
		public Base.StyleSheets.IStyleSheet ParentStyleSheet
		{
			get
			{
				return this.parentStyleSheet;
			}
		}

		/// <summary>
		/// 获取规则的文本表达
		/// </summary>
		public abstract string CSSText{get;}
		#endregion

		#region ..恢复格式
		/// <summary>
		/// 恢复格式
		/// </summary>
		/// <param name="oristr"></param>
		/// <returns></returns>
		internal string ReFormatString(string oristr)
		{
			Regex re = new Regex(@"(?<quote>"")?<<<(?<number>[0-9]+)>>>""?");
			return re.Replace(oristr, new MatchEvaluator(StringReplaceEvaluator));
		}

		private string StringReplaceEvaluator(Match match)
		{
			int i = Convert.ToInt32(match.Groups["number"].Value);
			string r = ReplacedStrings[i];
			if(!match.Groups["quote"].Success) r = r.Trim(new char[2]{'\'', '"'});
			return r;
		}
		#endregion

		#region ..匹配节点
		/// <summary>
		/// 匹配节点
		/// </summary>
		/// <param name="element">类型节点</param>
		/// <param name="content">CSS规则内容</param>
		/// <returns></returns>
		public virtual void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			
		}
		#endregion
	}
}
