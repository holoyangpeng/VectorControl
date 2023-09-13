using System;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.CTS;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGPointList 的摘要说明。
	/// </summary>
	public class SVGPointList:DataType.SVGTypeList,Interface.CTS.ISVGPointList
	{
		#region ..构造及消除
		public SVGPointList(string pointliststr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string listString = pointliststr.Trim();
			this.pointstr = listString;

			if ( listString.Length > 0 )
			{
				Regex delim = new Regex(@"\s+,?\s*|,\s*");
				String[] coords = delim.Split(listString);

				if ( coords.Length % 2 == 1 )
					throw new SVGException("无效的数据格式",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

				for ( int i = 0; i < coords.Length; i += 2 )
				{
					string x = coords[i];
					string y = coords[i+1];

					if ( x.Length == 0 || y.Length == 0 )
						throw new SVGException("无效的数据格式",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

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

		#region ..私有变量
		string pointstr = string.Empty;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取构成点集的字符串
		/// </summary>
		public string PointStr
		{
			get
			{
				return this.pointstr;
			}
		}
		#endregion

		#region ..获取点集
		/// <summary>
		/// 获取用FDI点结构PointF表示的点集
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
		/// 获取用SVGPoint结构表示的点集
		/// </summary>
		/// <returns></returns>
		public SVGPoint[] GetPoints()
		{
			SVGPoint[] ps = new SVGPoint[this.list.Count];
			this.list.CopyTo(ps);
			return ps;
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
			return svgType is Interface.CTS.ISVGPoint;
		}
		#endregion

		#region ..获取列表的文本表达
		/// <summary>
		/// 获取列表的文本表达
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
