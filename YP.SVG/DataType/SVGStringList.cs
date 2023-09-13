using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ���ַ����б�
	/// </summary>
	public class SVGStringList:DataType.SVGTypeList,Interface.DataType.ISVGStringList
	{
		#region ..���켰����
		/// <summary>
		/// ͨ���ַ����ͷָ��������ַ����б�
		/// </summary>
		/// <param name="stringliststr"></param>
		/// <param name="seperator"></param>
		public SVGStringList(string stringliststr,char[] seperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = SVGStringList.ParseListStr(stringliststr,seperators);
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
				{
					this.stringlist.Add(temp);
					this.AppendItem(new DataType.SVGString(temp,string.Empty));
				}
				temp = null;
			}
			s = null;

		}

		public SVGStringList()
		{
		}
		#endregion

		#region ..˽�б���
		ArrayList stringlist = new ArrayList();
		#endregion

		#region ..����б�
		/// <summary>
		/// ����б�
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			this.stringlist.Clear();
		}
		#endregion

		#region ..����Ƿ�Ϊ��Ч������ֵ
		/// <summary>
		/// ����Ƿ�Ϊ��Ч������ֵ
		/// </summary>
		/// <param name="svgType">��������</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGString;
		}
		#endregion

		#region ..ȷ��ĳ��Ԫ���Ƿ����б���
		/// <summary>
		/// ȷ��ĳ��Ԫ���Ƿ����б���
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override bool Contains(object child)
		{
			return this.stringlist.Contains (child);
		}
		#endregion

		#region ..�����ַ���
		/// <summary>
		/// �����ַ���
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string[] ParseListStr(string str,char[] seperators)
		{
			if(str.Trim().EndsWith(";"))
				str = str.Trim().Substring(0,str.Trim().Length - 1);
			string xpath = @"\s?[";
			foreach(char ch in seperators)
			{
				if(ch == ' ')
					xpath += @"\s";
				else
					xpath += ch.ToString();
			}
			xpath += @"]+\s?";
			Regex re = new Regex(xpath);
			str = re.Replace(str, ";");
			return str.Split(new char[1]{';'});

//			if(str == null)
//			{
//				return new string[0];
//			}
//			else
//			{
//				string pattern = string.Empty;
//				if(seperators.Length == 2)
//					pattern = @"\s+,?\s*|,\s*";
//				else
//					pattern = ";";
//				Regex re = new Regex(pattern);
//				str = re.Replace(str, ",");
//				return str.Split(new char[1]{','});
//			}
		}
		#endregion
	}
}
