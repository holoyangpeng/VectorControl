using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现字符串列表
	/// </summary>
	public class SVGStringList:DataType.SVGTypeList,Interface.DataType.ISVGStringList
	{
		#region ..构造及消除
		/// <summary>
		/// 通过字符串和分隔符构造字符串列表
		/// </summary>
		/// <param name="stringliststr"></param>
		/// <param name="seperator"></param>
		public SVGStringList(string stringliststr,char[] seperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
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

		#region ..私有变量
		ArrayList stringlist = new ArrayList();
		#endregion

		#region ..清空列表
		/// <summary>
		/// 清空列表
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			this.stringlist.Clear();
		}
		#endregion

		#region ..检测是否为有效的数据值
		/// <summary>
		/// 检测是否为有效的数据值
		/// </summary>
		/// <param name="svgType">检测的数组</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGString;
		}
		#endregion

		#region ..确定某个元素是否在列表中
		/// <summary>
		/// 确定某个元素是否在列表中
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override bool Contains(object child)
		{
			return this.stringlist.Contains (child);
		}
		#endregion

		#region ..解析字符串
		/// <summary>
		/// 解析字符串
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
