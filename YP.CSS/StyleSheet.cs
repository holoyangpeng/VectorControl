using System;
using System.Net;

namespace YP.Base.CSS
{
	/// <summary>
	/// StyleSheet 的摘要说明。
	/// </summary>
	public class StyleSheet:Base.StyleSheets.IStyleSheet
	{
		#region ..Constructor
		public StyleSheet(Base.Interface.IWebElement ownerElement)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ownerElement = ownerElement;
			if(this.ownerElement != null)
			{
                string strhref = this.ownerElement.GetAttribute("href").Trim();
                if (strhref.Length > 0)
                {
                    if (this.ownerElement.BaseURI != null && this.ownerElement.BaseURI.Length > 0)
                        this.href = new Uri(new Uri(this.ownerElement.BaseURI), strhref);
                    else
                        this.href = new Uri(strhref);
                }
                
				this.title = this.ownerElement.GetAttribute("title").Trim();
				this.type = this.ownerElement.GetAttribute("type").Trim();
			}
			this.sheetContent = ownerElement.InnerText;
		}

		public StyleSheet(Uri href,string type,string title)
		{
			this.href = href;
			this.type = type;
			this.title = title;
			WebRequest request = (WebRequest)WebRequest.Create(this.href);
			try
			{
				WebResponse response = (WebResponse)request.GetResponse();

				System.IO.StreamReader str = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.Default, true);
				this.sheetContent = str.ReadToEnd();
				str.Close();
			}
			catch
			{
			}
		}
		#endregion

		#region ..private fields
		Uri href = null;
		string type = string.Empty;
		string title = string.Empty;
		Base.Interface.IWebElement ownerElement;
		string sheetContent = string.Empty;
		#endregion

		#region ..继承属性
		protected string SheetContent
		{
			get
			{
				return this.sheetContent;
			}
		}
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取StyleSheet链接位置
		/// </summary>
		public Uri Href
		{
			get
			{
				return this.href;
			}
			set
			{
			}
		}

		/// <summary>
		/// 获取类型
		/// </summary>
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string Title
		{
			get
			{
				return this.title;
			}
		}

		/// <summary>
		/// 获取与之相联系的节点对象
		/// </summary>
		public Base.Interface.IWebElement OwnerElement
		{
			get
			{
				return this.ownerElement;
			}
		}
		#endregion

		#region ..匹配节点
		/// <summary>
		/// 匹配指定的节点
		/// </summary>
		/// <param name="element"></param>
		public virtual void MatchStyleable(Base.Interface.IStyleElement element)
		{

		}
		#endregion
	}
}
