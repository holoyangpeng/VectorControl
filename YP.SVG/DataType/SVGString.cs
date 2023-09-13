using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现SVG中的字符串
	/// </summary>
	public struct SVGString:Interface.DataType.ISVGString
	{
		#region ..构造及消除
		public SVGString(string str)
		{
			this.isEmpty = false;
			this.valuestr = str;
			this.defaultValue = string.Empty;
		}

		public SVGString(string str,string defaultValue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.defaultValue = defaultValue;
			if(str == null || str.Trim().Length ==0)
                this.valuestr = defaultValue;
			else
				this.valuestr = str;
			
			this.isEmpty = false;
		}
		#endregion

		#region ..静态变量
		static SVGString svgstring = new SVGString();

		public static SVGString Empty
		{
			get
			{
				svgstring.isEmpty = true;
				return svgstring;
			}
		}
		#endregion

		#region ..私有变量
		string valuestr;
		string defaultValue;
		bool isEmpty;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取字符串值
		/// </summary>
		public string Value
		{
			get
			{
				return this.valuestr;
			}
		}

		/// <summary>
		/// 判断对象是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// 获取对象的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		#endregion

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Value;
		}
		#endregion

        #region ..string 到SVGLength的隐式转换
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator SVGString(string x)
        {
            return new SVGString(x);
        }
        #endregion

        public static bool operator ==(SVGString str1, string str2)
        {
            return string.Compare(str1.valuestr, str2) == 0;
        }

        public static bool operator !=(SVGString str1, string str2)
        {
            return string.Compare(str1.valuestr, str2) != 0;
        }
	}
}
