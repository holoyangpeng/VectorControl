using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现数字列表的列表
	/// </summary>
	public class SVGNumberListList:DataType.SVGTypeList
	{
		#region ..构造及消除
		/// <summary>
		/// 用指定的字符串初始化列表
		/// </summary>
		/// <param name="liststr">代表列表的字符串</param>
		/// <param name="topseperators">用于分割列表的字符集</param>
		/// <param name="childseperators">用于构造子级列表的字符集</param>
		public SVGNumberListList(string liststr,char[] topseperators,char[] childseperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(liststr,topseperators);
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
					this.AppendItem(new DataType.SVGNumberList(temp,childseperators,"0"));
			}
			s = null;
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
			return svgType is Interface.DataType.ISVGNumberList;
		}
		#endregion
	}
}
