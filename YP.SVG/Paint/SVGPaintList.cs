using System;

namespace YP.SVG.Paint
{
	/// <summary>
	/// 实现SVGPaint列表
	/// </summary>
	public class SVGPaintList:DataType.SVGTypeList,Interface.Paint.ISVGPaintList
	{
		#region ..构造及消除
		public SVGPaintList(string paintstr,char[] seperators,YP.SVG.SVGStyleable ownerElement,string defaultvalue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = DataType.SVGStringList.ParseListStr(paintstr,seperators);
			foreach(string str in s)
			{
				this.AppendItem(new SVGPaint(str,ownerElement,defaultvalue));
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
			return svgType is Interface.Paint.ISVGPaint;
		}
		#endregion

	}
}
