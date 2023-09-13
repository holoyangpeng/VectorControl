using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��CSS�е����Ĺ�������
	/// </summary>
	public abstract class CSSRuleSet:Interface.ICSSRuleSet
	{
		#region ..Constructor
		public CSSRuleSet(Base.StyleSheets.IStyleSheet stylesheet)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
		/// ��ȡCSS���������
		/// </summary>
		public byte CSSType
		{
			get
			{
				return (byte)this.cssType;
			}
		}

		/// <summary>
		/// ����RuleSet
		/// </summary>
		public Interface.ICSSRuleSet ParentRule
		{
			get
			{
				return this.parentRule;
			}
		}

		/// <summary>
		/// ��ȡ������StyleSheet
		/// </summary>
		public Base.StyleSheets.IStyleSheet ParentStyleSheet
		{
			get
			{
				return this.parentStyleSheet;
			}
		}

		/// <summary>
		/// ��ȡ������ı����
		/// </summary>
		public abstract string CSSText{get;}
		#endregion

		#region ..�ָ���ʽ
		/// <summary>
		/// �ָ���ʽ
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

		#region ..ƥ��ڵ�
		/// <summary>
		/// ƥ��ڵ�
		/// </summary>
		/// <param name="element">���ͽڵ�</param>
		/// <param name="content">CSS��������</param>
		/// <returns></returns>
		public virtual void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			
		}
		#endregion
	}
}
