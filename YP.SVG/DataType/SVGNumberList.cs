using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGNumberList 的摘要说明。
	/// </summary>
	public class SVGNumberList:DataType.SVGTypeList,Interface.DataType.ISVGNumberList
	{
		#region ..构造及消除
		public SVGNumberList(string numberliststr,char[] seperator,string defaultValue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = SVGStringList.ParseListStr(numberliststr,seperator);
			
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
					AppendItem(new SVGNumber(temp,defaultValue));
				temp = null;
			}
			s = null;
			seperator = null;
		}

		public SVGNumberList(SVGNumber[] numbers)
		{
			this.list.AddRange(numbers);
		}
		#endregion

		#region ..私有变量
		#endregion

		#region ..检测是否为有效的数据值
		/// <summary>
		/// 检测是否为有效的数据值
		/// </summary>
		/// <param name="svgType">检测的数组</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGNumber;
		}
		#endregion

		#region ..获取长度数组
		/// <summary>
		/// 获取长度数组
		/// </summary>
		/// <returns></returns>
		public SVGNumber[] GetSVGNumbers()
		{
			SVGNumber[] ns = new SVGNumber[this.list.Count];
			this.list.CopyTo(ns);
			return ns;
		}
		#endregion

		#region ..获取对象的文本表达
		/// <summary>
		/// 获取对象的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<this.list.Count;i++)
				sb.Append(((SVGNumber)list[i]).OriValueStr + " ");
			return sb.ToString();
		}
		#endregion
	}
}
