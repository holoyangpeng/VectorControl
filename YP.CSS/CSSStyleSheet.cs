using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��CSSStyleSheet
	/// </summary>
	public class CSSStyleSheet:StyleSheet,Interface.ICSSStyleSheet
	{
		#region ..Constructor
		public CSSStyleSheet(Base.Interface.IWebElement ownerElement):base(ownerElement)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
		/// ��ȡ�Ӽ�CSS�鼯��
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
		/// ��ȡ������RuleSet�������StyleSheetͨ��@Import�����Ļ�
		/// </summary>
		public Interface.ICSSRuleSet ParentRule
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region ..����һ��CSSRuleSet
		/// <summary>
		/// ����һ��CSSRuleSet
		/// </summary>
		/// <param name="rulesetname">CSS����������</param>
		public Interface.ICSSRuleSet CreateRuleSet(string rulesetname)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ..��ʽ���ַ���
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

		#region ..ƥ��ڵ�
		/// <summary>
		/// ƥ��ָ���Ľڵ�
		/// </summary>
		/// <param name="element">���ͽڵ�</param>
		/// <param name="content">CSS��������</param>
		public void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			this.ChildRuleSets.MatchStyleable(element,content);
		}
		#endregion
	}
}
