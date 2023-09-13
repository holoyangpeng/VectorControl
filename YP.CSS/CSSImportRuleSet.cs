using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现@import
	/// </summary>
	public class CSSImportRuleSet:CSS.CSSRuleSet,Interface.ICSSImportRuleSet
	{
		private static Regex regex = new Regex(@"^@import\s(url\()?""(?<importhref>[^""]+)""\)?(\s(?<importmedia>([a-z]+)(\s*,\s*)?)+)?;");

		#region ..Constructor
		internal CSSImportRuleSet(Base.StyleSheets.IStyleSheet styleSheet):base(styleSheet)
		{
		}
		#endregion

		#region ..private fields
		Interface.ICSSStyleSheet refsheet = null;
		string href = string.Empty;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取指向的StyleSheet
		/// </summary>
		public Interface.ICSSStyleSheet RefStyleSheet
		{
			get
			{
				return this.refsheet;
			}
		}

		/// <summary>
		/// 获取规则的字符串表示
		/// </summary>
		public override string CSSText
		{
			get
			{
				string text = "@import";
				if(this.href != null)
					text += "\"" + this.href + "\";" ;
				return text;
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
		internal static CSS.CSSImportRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			Match m = regex.Match(cssstr);
			CSS.CSSImportRuleSet import = null;
			if(m.Success)
			{
				string href = m.Groups["importhref"].Value;
				string meida = m.Groups["importmedia"].Value;
				cssstr = cssstr.Substring(m.Length);
				if(href.Length >0)
				{
					import = new CSSImportRuleSet(styleSheet);
					href = import.ReFormatString(href);
					Uri uri = null;
					try
					{
						if(styleSheet != null)
							uri = new Uri(styleSheet.Href,href);
						else
							uri = new Uri(href);
					}
					catch
					{
					}

					if(uri != null)
						import.refsheet = new CSS.CSSStyleSheet(uri,styleSheet == null?string.Empty:styleSheet.Type,styleSheet == null?string.Empty:styleSheet.Title);
				}
			}
			return import;
		}
		#endregion

		#region ..匹配节点
		/// <summary>
		/// 匹配指定的节点
		/// </summary>
		/// <param name="element">类型节点</param>
		/// <param name="content">CSS规则内容</param>
		public override void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content)
		{
			if(this.refsheet != null)
				((CSS.CSSStyleSheet)this.refsheet).MatchStyleable(element,content);
		}
		#endregion
	}
}
