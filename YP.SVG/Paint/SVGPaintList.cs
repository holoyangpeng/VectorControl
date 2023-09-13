using System;

namespace YP.SVG.Paint
{
	/// <summary>
	/// ʵ��SVGPaint�б�
	/// </summary>
	public class SVGPaintList:DataType.SVGTypeList,Interface.Paint.ISVGPaintList
	{
		#region ..���켰����
		public SVGPaintList(string paintstr,char[] seperators,YP.SVG.SVGStyleable ownerElement,string defaultvalue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = DataType.SVGStringList.ParseListStr(paintstr,seperators);
			foreach(string str in s)
			{
				this.AppendItem(new SVGPaint(str,ownerElement,defaultvalue));
			}
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
			return svgType is Interface.Paint.ISVGPaint;
		}
		#endregion

	}
}
