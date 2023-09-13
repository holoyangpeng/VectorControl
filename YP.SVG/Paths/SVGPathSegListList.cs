using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// SVGPathSegListList 的摘要说明。
	/// </summary>
	public class SVGPathSegListList:DataType.SVGTypeList
	{
		#region ..构造及消除
		public SVGPathSegListList(string pathstr,char[] seperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(pathstr,seperators);
			foreach(string str in s)
			{
				if(str.Trim().Length > 0)
					this.AppendItem(new SVGPathSegList(str));
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
			return svgType is Interface.Paths.ISVGPathSegList;
		}
		#endregion
	}
}
