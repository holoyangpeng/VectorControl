using System;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.CTS;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGPointList ��ժҪ˵����
	/// </summary>
	public class SVGPointList:DataType.SVGTypeList,Interface.CTS.ISVGPointList
	{
		#region ..���켰����
		public SVGPointList(string pointliststr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string listString = pointliststr.Trim();
			this.pointstr = listString;

			if ( listString.Length > 0 )
			{
				Regex delim = new Regex(@"\s+,?\s*|,\s*");
				String[] coords = delim.Split(listString);

				if ( coords.Length % 2 == 1 )
					throw new SVGException("��Ч�����ݸ�ʽ",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

				for ( int i = 0; i < coords.Length; i += 2 )
				{
					string x = coords[i];
					string y = coords[i+1];

					if ( x.Length == 0 || y.Length == 0 )
						throw new SVGException("��Ч�����ݸ�ʽ",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

					SVGPoint p =new SVGPoint(SVGNumber.ParseNumberStr(x), SVGNumber.ParseNumberStr(y));
					this.AppendItem(p);
				}
			}
		}

		public SVGPointList(SVGPoint[] ps)
		{
			this.list.AddRange(ps);
		}
		#endregion

		#region ..˽�б���
		string pointstr = string.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���ɵ㼯���ַ���
		/// </summary>
		public string PointStr
		{
			get
			{
				return this.pointstr;
			}
		}
		#endregion

		#region ..��ȡ�㼯
		/// <summary>
		/// ��ȡ��FDI��ṹPointF��ʾ�ĵ㼯
		/// </summary>
		/// <returns></returns>
		public PointF[] GetGDIPoints()
		{
			PointF[] ps = new PointF[this.list.Count];
			int i = 0;
			foreach(Interface.CTS.ISVGPoint point in this.list)
			{
				ps[i] = new PointF(point.X,point.Y);
				i ++;
			}
			return ps;
		}

		/// <summary>
		/// ��ȡ��SVGPoint�ṹ��ʾ�ĵ㼯
		/// </summary>
		/// <returns></returns>
		public SVGPoint[] GetPoints()
		{
			SVGPoint[] ps = new SVGPoint[this.list.Count];
			this.list.CopyTo(ps);
			return ps;
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
			return svgType is Interface.CTS.ISVGPoint;
		}
		#endregion

		#region ..��ȡ�б���ı����
		/// <summary>
		/// ��ȡ�б���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<this.NumberOfItems;i++)
			{
				YP.SVG.DataType.SVGPoint p = (YP.SVG.DataType.SVGPoint)this.GetItem(i);
				sb.Append( " "+p.X.ToString() + " "+p.Y.ToString());
			}
			return sb.ToString();
		}
		#endregion

		#region ..static Get Point String
		/// <summary>
		/// Get the string for the points array
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static string GetPointsString(PointF[] points)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<points.Length;i++)
			{
				PointF p = points[i];
				sb.Append( " "+p.X.ToString() + " "+p.Y.ToString());
			}
			return sb.ToString();
		}
		#endregion
	}
}
