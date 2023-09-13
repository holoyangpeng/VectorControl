using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ�ֹ��򼯺�
	/// </summary>
	public class CSSRuleSetList:Interface.ICSSRuleSetList
	{
		#region ..����ƥ��
		static Regex unkownregex = new Regex(@"^@[^;]+;");
		#endregion

		#region ..Constructor
		internal CSSRuleSetList(string rulestr,Interface.ICSSStyleSheet styleSheet)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string css = rulestr;
			this.list.AddRange(ParseRuleList(ref css,styleSheet).list);
			this.parentsheet = styleSheet;
		}

		internal CSSRuleSetList(Interface.ICSSStyleSheet styleSheet)
		{
			this.parentsheet = styleSheet;
		}
		#endregion

		#region ..private fields
		System.Collections.ArrayList list = new System.Collections.ArrayList();
		Interface.ICSSStyleSheet parentsheet = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ������Ŀ
		/// </summary>
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}
		#endregion

		#region ..��ȡָ����
		/// <summary>
		/// ��ȡָ����
		/// </summary>
		/// <param name="index">����</param>
		public Interface.ICSSRuleSet GetItem(int index)
		{
			if(index >= 0 && index < this.list.Count)
				return (Interface.ICSSRuleSet)this.list[index];
			return null;
		}
		#endregion

		#region ..�����ַ���
		/// <summary>
		/// �����ַ���
		/// </summary>
		/// <param name="css">CSS�ַ���</param>
		internal static CSSRuleSetList ParseRuleList(ref string css,Interface.ICSSStyleSheet styleSheet)
		{
			CSSRuleSetList list = null;
			try
			{
				bool withBrackets = false;
				css = css.Trim();
				if(css.StartsWith("{"))
				{
					withBrackets = true;
					css = css.Substring(1);
				}

				list = new CSSRuleSetList(styleSheet);

				while(true)
				{
					css = css.Trim();
					if(css.Length == 0)
					{
						if(withBrackets)
						{
							throw new Exception("����ƥ��");
						}
						break;
					}
					else if(css.StartsWith("}"))
					{
						// end of block;
						css = css.Substring(1);
						break;
					}
					else if(css.StartsWith("@"))
					{
						Interface.ICSSRuleSet rule = null;
						rule = CSS.CSSMediaRuleSet.Parse(ref css,styleSheet);
						if(rule == null)
						{
							rule = CSS.CSSImportRuleSet.Parse(ref css,styleSheet);
							if(rule == null)
							{
								rule = CSS.CSSPageRuleSet.Parse(ref css,styleSheet);
								if(rule == null)
								{
									rule = CSS.CSSFontFaceRuleSet .Parse(ref css,styleSheet);

									if(rule == null)
									{
										rule = CSS.CSSCharSetRuleSet.Parse(ref css,styleSheet);

										if(rule == null)
										{
											Match m = unkownregex.Match(css);
											if(m.Success)
												css = css.Substring(m.Length);
										}
									}
								}
							}
						}

						if(rule != null)
							list.list.Add(rule);
					}
					else
					{
						// must be a selector or error
						CSS.CSSStyleRuleSet rule = CSS.CSSStyleRuleSet.Parse(ref css,styleSheet);
						if(rule != null)
						{
							list.list.Add(rule);
						}
						else
						{
							// this is an unknown rule format, possibly a new kind of selector. Try to find the end of it to skip it
							int startBracket = css.IndexOf("{");
							int endBracket = css.IndexOf("}");
							int endSemiColon = css.IndexOf(";");
							int endRule;

							if(endSemiColon > 0 && endSemiColon < startBracket)
							{
								endRule = endSemiColon;
							}
							else
							{
								endRule = endBracket;
							}


							if(endRule > -1)
							{
								css = css.Substring(endRule+1);
							}
							else
							{
								throw new Exception("Can not parse the CSS file",null);
							}
						}

						//}
					}  
				}
			}
			catch(System.Exception e1)
			{
				System.Diagnostics.Debug.Assert(true,e1.Message);
			}
			return list;
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
			foreach(CSS.CSSRuleSet rule in this.list)
			{
				if(rule != null)
					rule.MatchStyleable(element,content);
			}
		}
		#endregion
	}
}
