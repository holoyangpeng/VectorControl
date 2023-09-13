using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��@charset
	/// </summary>
	public class CSSCharSetRuleSet:CSS.CSSRuleSet,Interface.ICSSChartSetRule
	{
		private static Regex regex = new Regex(@"^@charset\s""(?<charsetencoding>[^""]+)"";");

		#region ..Constructor
		internal CSSCharSetRuleSet(Interface.ICSSStyleSheet styleSheet):base(styleSheet)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..private fields
		System.Text.Encoding encoding = null;
		#endregion
		
		#region ..public properties
		/// <summary>
		/// ��ȡ���������encoding��Ϣ
		/// </summary>
		public System.Text.Encoding Encoding
		{
			get
			{
				return this.encoding;
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
		internal static CSS.CSSCharSetRuleSet Parse(ref string cssstr,Interface.ICSSStyleSheet styleSheet)
		{
			CSS.CSSCharSetRuleSet page = null;
			Match m = regex.Match(cssstr);
			if(m.Success && m.Length > 0)
			{
				cssstr = cssstr.Substring(m.Length);
				page = new CSSCharSetRuleSet(styleSheet);
				page.encoding = System.Text.Encoding.GetEncoding(page.ReFormatString(m.Groups["charsetencoding"].Value.Trim()));
			}
			return page;
		}
		#endregion
	}
}
