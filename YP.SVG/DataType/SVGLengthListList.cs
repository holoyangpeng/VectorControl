using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现长度列表的列表
	/// </summary>
	public class SVGLengthListList:DataType.SVGTypeList
	{
		#region ..构造及消除
		/// <summary>
		/// 用指定的字符串初始化列表
		/// </summary>
		/// <param name="liststr">代表列表的字符串</param>
		/// <param name="topseperators">用于分割列表的字符集</param>
		/// <param name="childseperators">用于构造子级列表的字符集</param>
		public SVGLengthListList(string liststr,Interface.ISVGElement ownerElement,LengthDirection direction,char[] topseperators,char[] childseperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(liststr,topseperators);
			foreach(string str in s)
			{
				if(str.Trim().Length > 0)
					this.AppendItem(new DataType.SVGLengthList(str,ownerElement,direction,childseperators));
			}
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
			return svgType is Interface.DataType.ISVGLengthList;
		}
		#endregion
	}
}
