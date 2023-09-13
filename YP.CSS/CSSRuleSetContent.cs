using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现CSS中单个Rule声明块的内容
	/// </summary>
	public class CSSRuleSetContent:Interface.ICSSRuleSetContent
	{
		#region ..分析匹配成员
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
		/// 获取父级的CSSRuleSet
		/// </summary>
		public Interface.ICSSRuleSet ParentRuleSet
		{
			get
			{
				return this.parentRuleSet;
			}
		}

		/// <summary>
		/// 获取所包括的属性名称
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
		/// 获取CSS声明的内容
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
		/// 获取声明中所指定的属性个数
		/// </summary>
		public int NumberOfItems
		{
			get
			{
				return this.properties.Count;
			}
		}
		#endregion

		#region ..分析内容
		/// <summary>
		/// 分析规则内容
		/// </summary>
		/// <param name="cssText">代表内容的字符串</param>
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
				throw new Exception("数据格式错误",null);
			}
			return content;
		}
		#endregion

		#region ..获取指定属性值
		/// <summary>
		/// 获取指定属性值
		/// </summary>
		/// <param name="propertyname">属性名称</param>
		/// <returns></returns>
		public string GetProperty(string propertyname)
		{
			if(this.properties.ContainsKey(propertyname))
				return ((CSS.CSSPropertyContent)this.properties[propertyname]).PropertyValue;
			return string.Empty;
		}
		#endregion

		#region ..获取优先级
		/// <summary>
		/// 获取指定属性值
		/// </summary>
		/// <param name="propertyname">属性名称</param>
		/// <returns></returns>
		public string GetPriority(string propertyname)
		{
			if(this.properties.ContainsKey(propertyname))
				return ((CSS.CSSPropertyContent)this.properties[propertyname]).Priority;
			return string.Empty;
		}
		#endregion

		#region ..设置指定属性值
		/// <summary>
		/// 设置指定属性的值
		/// </summary>
		/// <param name="propertyname">属性名称</param>
		/// <param name="propertyvalue">属性值</param>
		/// <param name="priority">附加说明属性的优先级，比如用"important”说明属性的重要性</param>
		/// <param name="level">属性层次</param>
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
        /// 是否包含指定项的定义
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
