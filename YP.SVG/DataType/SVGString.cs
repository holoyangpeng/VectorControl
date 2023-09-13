using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�е��ַ���
	/// </summary>
	public struct SVGString:Interface.DataType.ISVGString
	{
		#region ..���켰����
		public SVGString(string str)
		{
			this.isEmpty = false;
			this.valuestr = str;
			this.defaultValue = string.Empty;
		}

		public SVGString(string str,string defaultValue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.defaultValue = defaultValue;
			if(str == null || str.Trim().Length ==0)
                this.valuestr = defaultValue;
			else
				this.valuestr = str;
			
			this.isEmpty = false;
		}
		#endregion

		#region ..��̬����
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

		#region ..˽�б���
		string valuestr;
		string defaultValue;
		bool isEmpty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�ַ���ֵ
		/// </summary>
		public string Value
		{
			get
			{
				return this.valuestr;
			}
		}

		/// <summary>
		/// �ж϶����Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// ��ȡ�����Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Value;
		}
		#endregion

        #region ..string ��SVGLength����ʽת��
        /// <summary>
        /// ��ʽת��
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
