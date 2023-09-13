using System;
using System.Collections;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Text;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现选择器
	/// </summary>
	public class Selector:Interface.ISelector
	{
		#region ..文字匹配
		static Regex reSelector = new Regex(CSSStyleRuleSet.sSelector);
		#endregion

		#region ..Constructor
		internal Selector(string selectorText):this(selectorText,new Hashtable())
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.selectorText = selectorText;
		}

		internal Selector(string selectortext,Hashtable nameTable)
		{
			this.NsTable = nameTable;
			this.selectorText = selectortext;
		}
		#endregion

		#region ..private fields
		private Hashtable NsTable;
		string selectorText = string.Empty;
		private string sXpath = null;
		int level = 0;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取选择器的层次，其用途在于表明规则的重要级别，对于具备属性的两项规则，取层次较高的规则属性为最终属性值
		/// </summary>
		public int Level
		{
			get
			{
				if(this.sXpath == null)
					this.GetXPath();
				return level;
			}
		}
		/// <summary>
		/// 获取对应的xpath
		/// </summary>
		public string XPath
		{
			get
			{
				if(this.sXpath == null)
				{
					this.GetXPath();
				}
				return this.sXpath;
			}
		}
		#endregion

		#region ..构建层次
		/// <summary>
		/// 添加层次级别
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		void AddLevel(int a,int b,int c)
		{
			this.level += a * 100 + b * 10 + c;
		}
		#endregion

		#region ..构建Xpath
		private string NsToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["ns"];

			if(g != null && g.Success)
			{
				string prefix = g.Value.TrimEnd(new char[]{'|'});

				if(prefix.Length == 0)
					r += "[namespace-uri()='']";
				else if(NsTable.ContainsKey(prefix))
					r += "[namespace-uri()='" + NsTable[prefix] + "']";
				else
					r += "[false]";
			}
			else if(NsTable.ContainsKey(String.Empty))
				r += "[namespace-uri()='" + NsTable[String.Empty] + "']";
			return r;
		}

		private string TypeToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["type"];
			string s = g.Value;
			if(!g.Success || s=="*") r = String.Empty;
			else
			{
				r = "[local-name()='" + s + "']";
				this.AddLevel(0, 0, 1);
			}

			return r;
		}

		private string ClassToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["class"];

			foreach(Capture c in g.Captures)
			{
				r += "[contains(concat(' ',@class,' '),' " + c.Value.Substring(1) + " ')]";
				AddLevel(1, 0, 0);
			}
			return r;
		}

		private string IdToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["id"];
			if(g.Success)
			{
				// r = "[id('" + g.Value.Substring(1) + "')]";
				r = "[@id='" + g.Value.Substring(1) + "']";
				AddLevel(0, 1, 0);
			}
			return r;
		}


		private string GetAttributeMatch(string attSelector)
		{
			string fullAttName = attSelector.Trim();
			int pipePos = fullAttName.IndexOf("|");
			string attMatch = String.Empty;

			if(pipePos == -1 || pipePos == 0)
			{
				string attName = fullAttName.Substring(pipePos+1);
				attMatch = "@" + attName;
			}
			else if(fullAttName.StartsWith("*|"))
				attMatch = "@*[local-name()='" + fullAttName.Substring(2) + "']";
			else
			{
				string ns = fullAttName.Substring(0, pipePos);
				string attName = fullAttName.Substring(pipePos+1);
				if(NsTable.ContainsKey(ns))
					attMatch = "@" + ns + ":" + attName;
				else
					attMatch = "false";
			}
			return attMatch;
		}
		private string PredicatesToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["attributecheck"];
			
			foreach(Capture c in g.Captures)
			{
				r += "[" + GetAttributeMatch(c.Value) + "]";
				AddLevel(0, 1, 0);
			}

			g = match.Groups["attributevaluecheck"];
			Regex reAttributeValueCheck = new Regex("^" + CSSStyleRuleSet.attributeValueCheck + "?$");
	

			foreach(Capture c in g.Captures)
			{
				Match valueCheckMatch = reAttributeValueCheck.Match(c.Value);
				
				string attName = valueCheckMatch.Groups["attname"].Value;
				string attMatch = GetAttributeMatch(attName);
				string eq = valueCheckMatch.Groups["eqtype"].Value;	// ~,^,$,*,|,nothing
				string attValue = valueCheckMatch.Groups["attvalue"].Value;

				switch(eq)
				{
					case "":
						r += "[" + attMatch + "='" + attValue + "']";
						break;
					case "~":
						r += "[contains(concat(' '," + attMatch + ",' '),' " + attValue + " ')]";
						break;
					case "^":
						r += "[starts-with(" + attMatch + ",'" + attValue + "')]";
						break;
					case "$":
						int a = attValue.Length - 1;

						r += "[substring(" + attMatch + ",string-length(" + attMatch + ")-" + a + ")='" + attValue + "']";
						break;
					case "*":
						r += "[contains(" + attMatch + ",'" + attValue + "')]";
						break;
					case "|":
						r += "[" + attMatch + "='" + attValue + "' or starts-with(" + attMatch + ",'" + attValue + "-')]";
						break;
				}
				AddLevel(0, 1, 0);
			}

			return r;
		}

		private string PseudoClassesToXPath(Match match)
		{
			int specificityA = 0;
			int specificityB = 1;
			int specificityC = 0;
			string r = String.Empty;
			Group g = match.Groups["pseudoclass"];

			foreach(Capture c in g.Captures)
			{
				Regex reLang = new Regex(@"^lang\(([A-Za-z\-]+)\)$");
				Regex reContains = new Regex("^contains\\((\"|\')?(?<stringvalue>.*?)(\"|\')?\\)$");

				string s = @"^(?<type>(nth-child)|(nth-last-child)|(nth-of-type)|(nth-last-of-type))\(\s*";
				s += @"(?<exp>(odd)|(even)|(((?<a>[\+-]?\d*)n)?(?<b>[\+-]?\d+)?))";
				s += @"\s*\)$";
				Regex reNth = new Regex(s);

				string p = c.Value.Substring(1);

				if(string.Compare(p,"root")==0)
				{
					r += "[not(parent::*)]";
				}
				else if(p.StartsWith("not"))
				{
					string expr = p.Substring(4, p.Length-5);
					Selector sel = new Selector(expr, NsTable);

					string xpath = sel.XPath;
					if(xpath != null && xpath.Length>3)
					{
						// remove *[ and ending ]
						xpath = xpath.Substring(2, xpath.Length-3);

						r += "[not(" + xpath + ")]";

						int specificity = sel.level;

		
						specificityA = (int)Math.Floor(specificity / 100f);
						specificity -= specificityA*100;
					
						specificityB = (int)Math.Floor((specificity) / 10f);

						specificity -= specificityB * 10;
		
						specificityC = specificity;
					}
				}
				else if(string.Compare(p,"first-child")==0)
					r += "[count(preceding-sibling::*)=0]";

				else if(string.Compare(p,"last-child")==0)
					r += "[count(following-sibling::*)=0]";
				else if(string.Compare(p,"only-child")==0)
					r += "[count(../*)=1]";
				else if(string.Compare(p,"only-of-type")==0)
					r += "[false]";

				else if(string.Compare(p,"empty")==0)
				    r += "[not(child::*) and not(text())]";
				else if(string.Compare(p,"target")==0)
					r += "[false]";
				else if(string.Compare(p,"first-of-type")==0)
					r += "[false]";
				else if(reLang.IsMatch(p))
					r += "[lang('" + reLang.Match(p).Groups[1].Value + "')]";
				else if(reContains.IsMatch(p))
					r += "[contains(string(.),'" + reContains.Match(p).Groups["stringvalue"].Value + "')]";
				else if(reNth.IsMatch(p))
				{
					Match m = reNth.Match(p);
					string type = m.Groups["type"].Value;
					string exp = m.Groups["exp"].Value;
					int a = 0;
					int b = 0;
					if(string.Compare(exp,"odd")==0)
					{
						a = 2;
						b = 1;
					}
					else if(string.Compare(exp,"even")==0)
					{
						a = 2;
						b = 0;
					}
					else
					{
						string v = m.Groups["a"].Value;

						if(v.Length == 0) a = 1;
						else if(v.Equals("-")) a = -1;
						else a = Int32.Parse(v);

						if(m.Groups["b"].Success) b = Int32.Parse(m.Groups["b"].Value);
					}


					if(type.Equals("nth-child") || type.Equals("nth-last-child"))
					{
						string axis;
						if(type.Equals("nth-child")) axis = "preceding-sibling";
						else axis = "following-sibling";

						if(a == 0)
							r += "[count(" + axis + "::*)+1=" + b + "]";
						else
							r += "[((count(" + axis + "::*)+1-" + b + ") mod " + a + "=0)and((count(" + axis + "::*)+1-" + b + ") div " + a + ">=0)]";
					}
				}
				AddLevel(specificityA, specificityB, specificityC);
			}
			return r;
		}

		private void SeperatorToXPath(Match match, StringBuilder xpath, string cur)
		{
			Group g = match.Groups["seperator"];
			if(g.Success)
			{
				string s = g.Value.Trim();
				if(s.Length == 0) cur += "//*";
				else if(string.Compare(s,">")==0) cur += "/*";
				else if(string.Compare(s,"+")==0 ||string.Compare(s,"~") ==0)
				{
					xpath.Append("[preceding-sibling::*");
					if(string.Compare(s,"+")==0)
					{
						xpath.Append("[position()=1]");
					}
					xpath.Append(cur);
					xpath.Append("]");
					cur = String.Empty;
				}
			}
			xpath.Append(cur);
		}
		#endregion

		#region ..分析XPath
		internal void GetXPath()
		{
			StringBuilder xpath = new StringBuilder("*");
			
			Match match = reSelector.Match(this.selectorText );
			while(match.Success)
			{
				if(match.Success && match.Value.Length > 0)
				{
					string x = String.Empty;
					x += NsToXPath(match);
					x += TypeToXPath(match);
					x += ClassToXPath(match);
					x += IdToXPath(match);
					x += PredicatesToXPath(match);
					x += PseudoClassesToXPath(match);
					SeperatorToXPath(match, xpath, x);
				}
				match = match.NextMatch();
			}
			sXpath = xpath.ToString();
		}

		#endregion
	}
}
