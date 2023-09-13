using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGPointListList 的摘要说明。
	/// </summary>
	public class SVGPointListList:DataType.SVGTypeList
	{
		#region ..构造及消除
		public SVGPointListList(string pointstr,char[] seperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(pointstr,seperators);
			foreach(string str in s)
			{
				if(str.Trim().Length > 0)
					this.AppendItem(new SVGPointList(str));
			}
			s = null;
			seperators = null;
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
			return svgType is Interface.CTS.ISVGPointList;
		}
		#endregion
	}
}
