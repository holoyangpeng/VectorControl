using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��CSS�е���Rule�����������
	/// </summary>
	public class CSSRuleSetContent:Interface.ICSSRuleSetContent
	{
		#region ..����ƥ���Ա
		private static Regex styleRegex = new Regex(@"^(?<name>[A-Za-z\-0-9]+)\s*:(?<value>[^;\}!]+)(!\s?(?<priority>important))?;?");
		#endregion

		#region ..Constructor
		public CSSRuleSetContent()
		{

		}

		public CSSRuleSetContent(string contentstr)
		{
			CSS.CSSRuleSetContent content = CSSRuleSetContent.ParseRuleContent(ref contentstr);
			if(content != null)
				this.properties = (Hashtable)content.properties.Clone();
		}
		#endregion

		#region ..private fields
		Hashtable properties = new Hashtable();
		Interface.ICSSRuleSet parentRuleSet = null;
		string[] names;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ������CSSRuleSet
		/// </summary>
		public Interface.ICSSRuleSet ParentRuleSet
		{
			get
			{
				return this.parentRuleSet;
			}
		}

		/// <summary>
		/// ��ȡ����������������
		/// </summary>
		public string[] PropertyNames
		{
			get
			{
				if(this.names == null)
				{
					this.names = new string[this.properties.Count];
					this.properties.Keys.CopyTo(this.names,0);
				}
				return this.names;
			}
		}

		/// <summary>
		/// ��ȡCSS����������
		/// </summary>
		public string CSSText
		{
			get
			{
				string ret = String.Empty;
				
				IDictionaryEnumerator enu = this.properties.GetEnumerator();
				while(enu.MoveNext())
				{
					CSS.CSSPropertyContent style = (CSS.CSSPropertyContent)enu.Value;
					ret += style.CSSText + ";";
				}
				return ret;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// ��ȡ��������ָ�������Ը���
		/// </summary>
		public int NumberOfItems
		{
			get
			{
				return this.properties.Count;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="cssText">�������ݵ��ַ���</param>
		internal static CSS.CSSRuleSetContent ParseRuleContent(ref string cssText)
		{
			bool startedWithABracket = false;

			cssText = cssText.Trim();
			if(cssText.StartsWith("{"))
			{
				cssText = cssText.Substring(1).Trim();
				startedWithABracket = true;
			}
			
			CSS.CSSRuleSetContent content = null;
			content = new CSSRuleSetContent();

			Match match = styleRegex.Match(cssText);
			while(match.Success)
			{

				string name = match.Groups["name"].Value;
				string value = match.Groups["value"].Value;
				//				if(this.parentRuleSet != null)
				//				{
				//					value = ().DeReplaceStrings(value);
				//				}
				string prio = match.Groups["priority"].Value;

				CSSPropertyContent style = new CSSPropertyContent(name,value,prio,0);

				bool addStyle = false;
				if(content.properties.ContainsKey(name))
				{
					string existingPrio = ((CSSPropertyContent)content.properties[name]).Priority;
				
					if(string.Compare(existingPrio,"important") != 0 || 
						string.Compare(prio,"important")==0)
					{
						content.properties.Remove(name);
						addStyle = true;
					}
				}
				else
				{
					addStyle = true;
				
				}

				if(addStyle)
				{
					content.properties.Add(name, style);
				}
            
				cssText = cssText.Substring(match.Length).Trim();
				match = styleRegex.Match(cssText);
			}
            
			cssText = cssText.Trim();
			if(cssText.StartsWith("}"))
			{
				cssText = cssText.Substring(1);
			}
			else if(startedWithABracket)
			{
				throw new Exception("���ݸ�ʽ����",null);
			}
			return content;
		}
		#endregion

		#region ..��ȡָ������ֵ
		/// <summary>
		/// ��ȡָ������ֵ
		/// </summary>
		/// <param name="propertyname">��������</param>
		/// <returns></returns>
		public string GetProperty(string propertyname)
		{
			if(this.properties.ContainsKey(propertyname))
				return ((CSS.CSSPropertyContent)this.properties[propertyname]).PropertyValue;
			return string.Empty;
		}
		#endregion

		#region ..��ȡ���ȼ�
		/// <summary>
		/// ��ȡָ������ֵ
		/// </summary>
		/// <param name="propertyname">��������</param>
		/// <returns></returns>
		public string GetPriority(string propertyname)
		{
			if(this.properties.ContainsKey(propertyname))
				return ((CSS.CSSPropertyContent)this.properties[propertyname]).Priority;
			return string.Empty;
		}
		#endregion

		#region ..����ָ������ֵ
		/// <summary>
		/// ����ָ�����Ե�ֵ
		/// </summary>
		/// <param name="propertyname">��������</param>
		/// <param name="propertyvalue">����ֵ</param>
		/// <param name="priority">����˵�����Ե����ȼ���������"important��˵�����Ե���Ҫ��</param>
		/// <param name="level">���Բ��</param>
		public void SetProperty(string propertyname,string propertyvalue,string priority,int level)
		{
			if(!this.properties.ContainsKey(propertyname))
				this.properties[propertyname] = new CSSPropertyContent(propertyname,propertyvalue,priority,level);
			else
			{
				CSS.CSSPropertyContent p = (CSS.CSSPropertyContent)this.properties[propertyname];
				bool higher = true;
				if(p.Priority == "important" &&string.Compare(priority,"important") != 0)
					higher = false;
				else
					higher = level >= p.Level;

				if(higher)
					this.properties[propertyname] = new CSSPropertyContent(propertyname,propertyvalue,priority,level);
			}
		}
		#endregion

        #region ..ContainsProperty
        /// <summary>
        /// �Ƿ����ָ����Ķ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsProperty(string name)
        {
            return this.properties.ContainsKey(name);
        }
        #endregion
    }
}
