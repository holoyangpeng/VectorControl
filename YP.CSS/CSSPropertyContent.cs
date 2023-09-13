using System;

namespace YP.Base.CSS
{
	/// <summary>
	/// 存储规则中属性数据，内部使用
	/// </summary>
	public class CSSPropertyContent
	{
		#region ..构造及消除
		internal CSSPropertyContent(string propertyname,string propertyvalue,string priority,int level)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.Priority = priority;
			this.PropertyName = propertyname;
			this.PropertyValue = propertyvalue;
			this.Level = level;
		}
		#endregion

		#region ..保护字段
		/// <summary>
		/// 记录属性名称
		/// </summary>
		internal string PropertyName = string.Empty;

		/// <summary>
		/// 记录属性值
		/// </summary>
		internal string PropertyValue = string.Empty;

		/// <summary>
		/// 记录表示属性优先级的字符串
		/// </summary>
		internal string Priority  = string.Empty;

		/// <summary>
		/// 定义属性的级别，对于具备同样名称的属性，取较高层次者为最终属性
		/// </summary>
		internal int Level = 0;
		#endregion

		#region ..保护属性
		/// <summary>
		/// 获取数据的文本表达
		/// </summary>
		internal string CSSText
		{
			get
			{
				string ret = PropertyName + ":" + PropertyValue;
				if(Priority != null && Priority.Length > 0)
				{
					ret += " !" + Priority;
				}
				return ret;
			}
		}
		#endregion
	}
}
