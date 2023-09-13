using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现变换列表的列表
	/// </summary>
	public class SVGTransformListList:DataType.SVGTypeList
	{
		#region ..构造及消除
		public SVGTransformListList(string transformstr,string type,char[] seperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(transformstr,seperators);
			foreach(string str in s)
			{
				if(str.Length > 0)
					this.AppendItem(new SVGTransformList(type + "(" + str + ")"));
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
			return svgType is Interface.CTS.ISVGTransformList;
		}
		#endregion
	}
}
