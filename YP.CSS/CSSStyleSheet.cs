using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现CSSStyleSheet
	/// </summary>
	public class CSSStyleSheet:StyleSheet,Interface.ICSSStyleSheet
	{
		#region ..Constructor
		public CSSStyleSheet(Base.Interface.IWebElement ownerElement):base(ownerElement)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		public CSSStyleSheet(Uri href,string type,string title):base(href,type,title)
		{
		}
		#endregion

		#region ..private fields
		CSSRuleSetList childrules ;
		private ArrayList alReplacedStrings = new ArrayList();
		internal string[] ReplacedStrings;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取子级CSS块集合
		/// </summary>
		public Interface.ICSSRuleSetList ChildRuleSets
		{
			get
			{
				if(this.childrules == null)
				{
					string css = this.PreProcessContent();
					this.childrules = new CSSRuleSetList(css,this);
				}
				return this.childrules;
			}
		}

		/// <summary>
		/// 获取父级的RuleSet，如果该StyleSheet通过@Import建立的话
		/// </summary>
		public Interface.ICSSRuleSet ParentRule
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region ..创建一个CSSRuleSet
		/// <summary>
		/// 创建一个CSSRuleSet
		/// </summary>
		/// <param name="rulesetname">CSS规则块的名称</param>
		public Interface.ICSSRuleSet CreateRuleSet(string rulesetname)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ..格式化字符串
		private string StringReplaceEvaluator(Match match)
		{
			alReplacedStrings.Add(match.Value);
			return "\"<<<" + (alReplacedStrings.Count-1) + ">>>\"";
		}

		private string PreProcessContent()
		{
			if(SheetContent != null && SheetContent.Length > 0)
			{		
				Regex re = new Regex(@"(""(.|\n)*?[^\\]"")|('(.|\n)*?[^\\]')");
				string s = re.Replace(SheetContent, new MatchEvaluator(StringReplaceEvaluator));
			
				ReplacedStrings = (string[])alReplacedStrings.ToArray(s.GetType());
				alReplacedStrings.Clear();

				Regex reComment = new Regex(@"(//.*)|(/\*(.|\n)*?\*/)");
				s = reComment.Replace(s, String.Empty);
				return s;
			}
			else
			{
				return "";
			}
		}
		#endregion

		#region ..匹配节点
		/// <summary>
		/// 匹配指定的节点
		/// </summary>
		/// <param name="element">类型节点</param>
		/// <param name="content">CSS规则内容</param>
		public void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			this.ChildRuleSets.MatchStyleable(element,content);
		}
		#endregion
	}
}
