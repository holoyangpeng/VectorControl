using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

using YP.SVG.Interface.DataType;
using YP.SVG.Interface.CTS;
using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现路径单元列表
	/// </summary>
	public class SVGPathSegList:DataType.SVGTypeList,Interface.Paths.ISVGPathSegList
	{
		private static Regex rePathCmd = new Regex(@"(?=[AQTCLHVZSMaqtclhvzsm])");
		private static Regex coordSplit = new Regex(@"(\s*,\s*)|(\s+)|((?<=[0-9])(?=-))", RegexOptions.ExplicitCapture);

		#region ..构造及消除
		private SVGPathSegList()
		{
		}

		public SVGPathSegList(string d)
		{
			Interface.Paths.ISVGPathSeg seg;
			string[] segs = rePathCmd.Split(d);

			foreach(string s in segs)
			{
				string segment = s.Trim();
				if(segment.Length > 0)
				{
					char cmd = (char) segment.ToCharArray(0,1)[0];
					float[] coords = GetCoords(segment);
					int length = coords.Length;
					switch(cmd)
					{
					#region moveto
						case 'M':
							for(int i = 0; i+1<length; i+=2)
							{
								if(i == 0)
								{
									seg = new SVGPathSegMovetoAbs(
										coords[i],
										coords[i+1]
										);
								}
								else
								{
									seg = new SVGPathSegMovetoAbs(
										coords[i],
										coords[i+1]
										);
								}
								AppendItem(seg);
							}
							break;
						case 'm':
							for(int i = 0; i+1<length; i+=2)
							{
								if(i == 0)
								{
									seg = new SVGPathSegMovetoRel(
										coords[i],
										coords[i+1]
										);
								}
								else
								{
									seg = new SVGPathSegMovetoRel(
										coords[i],
										coords[i+1]
										);
								}
								AppendItem(seg);
							}
							break;
					#endregion
					#region lineto
						case 'L':
							for(int i = 0; i+1<length; i+=2)
							{
								seg = new SVGPathSegLinetoAbs(
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 'l':
							for(int i = 0; i+1<length; i+=2)
							{
								seg = new SVGPathSegLinetoRel(
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 'H':
							for(int i = 0; i<length; i++)
							{
								seg = new SVGPathSegLinetoHorizontalAbs(
									coords[i]
									);
								AppendItem(seg);
							}
							break;
						case 'h':
							for(int i = 0; i<length; i++)
							{
								seg = new SVGPathSegLinetoHorizontalRel(
									coords[i]
									);
								AppendItem(seg);
							}
							break;
						case 'V':
							for(int i = 0; i<length; i++)
							{
								seg = new SVGPathSegLinetoVerticalAbs(
									coords[i]
									);
								AppendItem(seg);
							}
							break;
						case'v':
							for(int i = 0; i<length; i++)
							{
								seg = new SVGPathSegLinetoVerticalRel(
									coords[i]
									);
								AppendItem(seg);
							}
							break;
					#endregion
					#region beziers
						case 'C':
							for(int i = 0; i+5<length; i+=6)
							{
								seg = new SVGPathSegCurvetoCubicAbs(
									coords[i+4],
									coords[i+5],
									coords[i],
									coords[i+1],
									coords[i+2],
									coords[i+3]
									);
								AppendItem(seg);
							}
							break;
						case 'c':
							for(int i = 0; i+5<length; i+=6)
							{
								seg = new SVGPathSegCurvetoCubicRel(
									coords[i+4],
									coords[i+5],
									coords[i],
									coords[i+1],
									coords[i+2],
									coords[i+3]
									);

								AppendItem(seg);
							}
							break;
						case 'S':
							for(int i = 0; i<length; i+=4)
							{
								seg = new SVGPathSegCurvetoCubicSmoothAbs(
									coords[i+2],
									coords[i+3],
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case's':
							for(int i = 0; i<length; i+=4)
							{
								seg = new SVGPathSegCurvetoCubicSmoothRel(
									coords[i+2],
									coords[i+3],
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 'Q':
							for(int i = 0; i<length; i+=4)
							{
								seg = new SVGPathSegCurvetoQuadraticAbs(
									coords[i+2],
									coords[i+3],
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 'q':
							for(int i = 0; i<length; i+=4)
							{
								seg = new SVGPathSegCurvetoQuadraticRel(
									coords[i+2],
									coords[i+3],
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 'T':
							for(int i = 0; i<length; i+=2)
							{
								seg = new SVGPathSegCurvetoQuadraticSmoothRel(
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
						case 't':
							for(int i = 0; i<length; i+=2)
							{
								seg = new SVGPathSegCurvetoQuadraticSmoothRel(
									coords[i],
									coords[i+1]
									);
								AppendItem(seg);
							}
							break;
					#endregion
					#region arcs
						case 'A':
						case 'a':
							for(int i = 0; i+6<length; i+=7)
							{
								if(cmd=='A')
								{
									seg = new SVGPathSegArcAbs(
										coords[i+5],
										coords[i+6],
										coords[i],
										coords[i+1],
										coords[i+2],
										(coords[i+3]!=0),
										(coords[i+4]!=0)
										);
								}
								else
								{
									seg = new SVGPathSegArcRel(
										coords[i+5],
										coords[i+6],
										coords[i],
										coords[i+1],
										coords[i+2],
										(coords[i+3]!=0),
										(coords[i+4]!=0)
										);
								}
								AppendItem(seg);
							}
							break;
					#endregion
					#region close
						case 'z':
						case 'Z':
							seg = new SVGPathSegClosePath();
							AppendItem(seg);
							break;
					#endregion
					#region Unknown path command
						default:
							throw new ApplicationException("Unknown path command");
					#endregion
					}
				}
			}
		}

		public SVGPathSegList(ISVGPathSeg[] svgPathSegs)
		{
			if(svgPathSegs != null)
			{
				foreach(SVGPathSeg seg in svgPathSegs)
					this.AppendItem(seg);
			}
			if(this.list.Count > 0)
			{
				SVGPathSeg seg = (SVGPathSeg)this.list[this.list.Count - 1];
				PointF p1 = seg.GetRelativePreControl(this);
				PointF p2 = seg.GetLastPoint(this);
				seg.relativeNextControl = new PointF(2 * p2.X - p1.X,2 * p2.Y - p1.Y);

				seg = (SVGPathSeg)this.list[0];
				if(seg is SVGPathSegMove)
				{
					p1 = seg.GetRelativeNextControl(this);
					p2 = seg.GetLastPoint(this);
					((SVGPathSegMove)seg).SetRelativePreControl(new PointF(2 * p2.X - p1.X,2 * p2.Y - p1.Y));
				}
			}
		}
		#endregion

		#region ..私有变量
		ArrayList anchors = new ArrayList();
		SVGPathSegList normalSVGPathSegList ;
		ArrayList pathSegTypeLetters = new ArrayList();
		string pathSegTypeAsString = string.Empty;
		float totallength = -1;
		/// <summary>
		/// 记录列表中除掉M 和Z的单元
		/// </summary>
		public ArrayList ChildSegListExceptMove = new ArrayList();

		System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
		string pathString = string.Empty;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取列表中所有单元命令链接而成的字符串
		/// </summary>
		public string PathSegTypeAsString
		{
			get
			{
				if(this.pathSegTypeAsString.Length == 0)
				{
					char[] cs = new char[this.pathSegTypeLetters.Count];
					this.pathSegTypeLetters.CopyTo(cs);
					this.pathSegTypeAsString = new string(cs);
				}
				return this.pathSegTypeAsString;
			}
		}

		/// <summary>
		/// 获取路径的文本表达
		/// </summary>
		public string PathString
		{
			get
			{
				this.pathString = string.Empty;
				for(int i = 0;i<this.NumberOfItems;i++)
				{
					SVGPathSeg svgPathSeg = (SVGPathSeg)this.GetItem(i);
					this.pathString += svgPathSeg.PathString;
				}
				return this.pathString;
			}
		}

		/// <summary>
		/// 获取转换为正常路径结构的列表
		/// </summary>
		public Interface.Paths.ISVGPathSegList NormalSVGPathSegList
		{
			get
			{
				if(this.normalSVGPathSegList == null)
				{
					this.normalSVGPathSegList = new SVGPathSegList();
					for(int i = 0;i<this.NumberOfItems;i++)
					{
						SVGPathSeg svgPathSeg = (SVGPathSeg)this.GetItem(i);
						this.normalSVGPathSegList.AppendItem(svgPathSeg.ConvertToNormal(this));
					}
				}
				return this.normalSVGPathSegList;
			}
		}
		#endregion

		#region ..公共方法
		/// <summary>
		/// 清空列表项
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			this.pathSegTypeLetters.Clear();
			this.anchors.Clear();
			this.path.Reset();
			this.pathString = string.Empty;
		}

		/// <summary>
		/// 清空当前列表项，并用指定的ISVGPathSeg初始化列表
		/// </summary>
		public override Interface.DataType.ISVGType Initialize(Interface.DataType.ISVGType newItem)
		{
			Interface.DataType.ISVGType item = base.Initialize(newItem);
			if(item is Interface.Paths.ISVGPathSeg)
			{
				this.pathString = ((Interface.Paths.ISVGPathSeg)item).PathString;
				this.pathSegTypeLetters.Add(((Interface.Paths.ISVGPathSeg)item).PathSegTypeAsLetter[0]);
			}
			return item;
		}

		/// <summary>
		/// 在指定的索引处插入ISVGPathSeg项
		/// </summary>
		public override Interface.DataType.ISVGType InsertItemBefore(Interface.DataType.ISVGType newItem, int index)
		{
			PointF next = PointF.Empty;
			Interface.DataType.ISVGType item = base.InsertItemBefore(newItem,index);
			if(item is Interface.Paths.ISVGPathSeg)
			{
				if((int)index < this.NumberOfItems)
				{
					this.pathSegTypeLetters.Insert(index,((Interface.Paths.ISVGPathSeg)item).PathSegTypeAsLetter[0]);
				}
				else
				{
					this.pathSegTypeLetters.Add(((Interface.Paths.ISVGPathSeg)item).PathSegTypeAsLetter[0]);
				}
			}
			return item;
		}

		/// <summary>
		/// 用指定的ISVGPathSeg替换指定索引处的项
		/// </summary>
		public override Interface.DataType.ISVGType ReplaceItem(Interface.DataType.ISVGType newItem, int index)
		{
			PointF next = PointF.Empty;
			if(index >= 0 && index < this.list.Count && this.ValidType(newItem))
			{
				next = ((YP.SVG.Paths.SVGPathSeg)this.list[index]).relativeNextControl;
			}
			Interface.DataType.ISVGType item = base.ReplaceItem(newItem,index);
			if(item is Interface.Paths.ISVGPathSeg)
			{
				this.pathSegTypeLetters.RemoveAt(index);
				this.pathSegTypeLetters.Insert(index,((Interface.Paths.ISVGPathSeg)item).PathSegTypeAsLetter[0]);
				((YP.SVG.Paths.SVGPathSeg)item).relativeNextControl = next;
			}
			return item;
		}

		/// <summary>
		/// 移除指定索引处的项
		/// </summary>
		public override Interface.DataType.ISVGType RemoveItem(int index)
		{
			if(index - 1 >= 0 && index < this.list.Count)
			{
				YP.SVG.Paths.SVGPathSeg pre = (YP.SVG.Paths.SVGPathSeg)this.list[index - 1];
				YP.SVG.Paths.SVGPathSeg seg = (YP.SVG.Paths.SVGPathSeg)this.list[index];
			}
			Interface.DataType.ISVGType item = base.RemoveItem(index);
			if(item is Interface.Paths.ISVGPathSeg)
			{
				this.pathSegTypeLetters.RemoveAt(index);
			}
			return item;
		}

		/// <summary>
		/// 在列表末尾添加ISVGPathSeg项
		/// </summary>
		public override Interface.DataType.ISVGType AppendItem(Interface.DataType.ISVGType newItem)
		{
			Interface.DataType.ISVGType item = base.AppendItem(newItem);
			if(item is Interface.Paths.ISVGPathSeg)
			{
				this.pathString += ((Interface.Paths.ISVGPathSeg)item).PathString;
				string l = ((Interface.Paths.ISVGPathSeg)item).PathSegTypeAsLetter;
				this.pathSegTypeLetters.Add(l[0]);
			}
			return item;
		}
		#endregion

		#region ..GetCoords
		private float[] GetCoords(String segment)
		{
			float[] coords = new float[0];
		
			segment = segment.Substring(1);
			segment = segment.Trim();
			segment = segment.Trim(new char[]{','});

			if(segment.Length > 0)
			{
				string[] sCoords = coordSplit.Split(segment);

				coords = new float[sCoords.Length];
				for(int i = 0; i<sCoords.Length; i++)
				{
					coords[i] = DataType.SVGNumber.ParseNumberStr(sCoords[i]);
				}
			}
			return coords;
		}		
		#endregion

		#region ..获取指定项的前一项
		/// <summary>
		/// 获取指定项的前一项
		/// </summary>
		/// <param name="svgPathSeg">指定的路径单元</param>
		/// <returns></returns>
		public Interface.Paths.ISVGPathSeg PreviousSibling(Interface.Paths.ISVGPathSeg svgPathSeg)
		{
			int index = this.IndexOf(svgPathSeg);
			if(index - 1>= 0 && index - 1 < this.NumberOfItems)
				return (ISVGPathSeg)this.GetItem(index-1);
			return null;
		}
		#endregion

		#region ..获取指定项的后一项
		/// <summary>
		/// 获取指定项的后一项
		/// </summary>
		/// <param name="svgPathSeg"></param>
		/// <returns></returns>
		public Interface.Paths.ISVGPathSeg NextSibling(Interface.Paths.ISVGPathSeg svgPathSeg)
		{
			int index = this.IndexOf(svgPathSeg);
			if(index + 1 > 0 && index + 1 < this.NumberOfItems)
			{
				Interface.Paths.ISVGPathSeg next = (ISVGPathSeg)this.GetItem(index + 1);
				return next;
			}
			return null;
		}
		#endregion

		#region ..Clone
		/// <summary>
		/// 创建对象的副本
		/// </summary>
		/// <returns></returns>
		public SVGPathSegList Clone()
		{
			YP.SVG.Paths.SVGPathSeg[] segs = new YP.SVG.Paths.SVGPathSeg[this.list.Count];
			this.list.CopyTo(segs);
			SVGPathSegList list1 = new SVGPathSegList(segs);
			list1.pathSegTypeLetters = (ArrayList)this.pathSegTypeLetters.Clone();
			list1.anchors = (ArrayList)this.anchors.Clone();
			return list1;
		}
		#endregion

		#region ..获取路径单元数组
		/// <summary>
		/// 获取路径单元数组
		/// </summary>
		/// <returns></returns>
		public Paths.SVGPathSeg[] GetSVGPathSegs()
		{
			Paths.SVGPathSeg[] ps = new Paths.SVGPathSeg[this.NumberOfItems];
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
			return svgType is Interface.Paths.ISVGPathSeg;// && !(svgType is Interface.Paths.ISVGPathSegClosePath);
		}
		#endregion

		#region ..长度
		/// <summary>
		/// 获取总长度
		/// </summary>
		public float TotalLength
		{
			get
			{
				if(this.totallength == -1)
				{
					this.ChildSegListExceptMove.Clear();
					this.totallength = 0;
					foreach(SVGPathSeg seg in this.list)
					{
						if(!(seg is SVGPathSegMove) && !(seg is SVGPathSegClosePath))
						{
							this.totallength += seg.GetPathLength(this);
							this.ChildSegListExceptMove.Add(seg);
						}
					}
				}
				return totallength;
			}
		}
		#endregion

		#region ..获取其锚点集
		/// <summary>
		/// 获取其锚点集
		/// </summary>
		/// <returns></returns>
		public PointF[] GetAnchors()
		{
			PointF[] ps = new PointF[this.anchors.Count];
			this.anchors.CopyTo(ps);
			return ps;
		}
		#endregion

		#region ..获取路径
		/// <summary>
		/// 获取路径
		/// </summary>
		/// <returns></returns>
		public GraphicsPath GetGDIPath()
		{
			GraphicsPath path = new GraphicsPath();
			PointF lastPoint = PointF.Empty;
			PointF initPoint = PointF.Empty;
			Paths.SVGPathSegList list = this;
			this.anchors.Clear();
			for(int i = 0;i<list.NumberOfItems;i++)
			{
				SVGPathSeg seg = (SVGPathSeg)list.GetItem(i);
				if(seg == null)
					continue;
				seg.AppendGraphicsPath(path,ref lastPoint,list);
				if(seg is Paths.SVGPathSegMove)
					initPoint = lastPoint;
				else if(seg is YP.SVG.Paths.SVGPathSegClosePath)
					lastPoint = initPoint;

				this.anchors.Add(lastPoint);
			}
			return path;
		}
		#endregion

		#region ..获取指定单元相关联的SVGPathSegMove对象
		/// <summary>
		/// 获取指定单元相关联的SVGPathSegMove对象
		/// </summary>
		/// <param name="seg">参考路径单元</param>
		/// <returns></returns>
		public YP.SVG.Paths.SVGPathSegMove GetRelativeStartPathSeg(Interface.Paths.ISVGPathSeg seg)
		{
			if(this.Contains(seg))
			{
				int index = this.list.IndexOf(seg);
				for(int i = index;i>= 0;i--)
				{
					YP.SVG.Paths.SVGPathSeg seg1 = (YP.SVG.Paths.SVGPathSeg)this.list[i];
					if(seg1 is YP.SVG.Paths.SVGPathSegMove)
						return (YP.SVG.Paths.SVGPathSegMove)seg1;
					if(seg1 is YP.SVG.Paths.SVGPathSegClosePath && i < index)
						return null;
				}
			}
			return null;
		}
		#endregion

		#region ..获取指定单元相关联的封闭路径对象
		/// <summary>
		/// 获取指定单元相关联的SVGPathSegMove对象
		/// </summary>
		/// <param name="seg">参考路径单元</param>
		/// <returns></returns>
		public YP.SVG.Paths.SVGPathSegClosePath GetRelativeClosePathSeg(Interface.Paths.ISVGPathSeg seg)
		{
			if(this.Contains(seg))
			{
				int index = this.list.IndexOf(seg);
				for(int i = index;i<this.list.Count;i++)
				{
					YP.SVG.Paths.SVGPathSeg seg1 = (YP.SVG.Paths.SVGPathSeg)this.list[i];
					if(seg1 is YP.SVG.Paths.SVGPathSegClosePath)
						return ((YP.SVG.Paths.SVGPathSegClosePath)seg1 );
					if(seg1 is YP.SVG.Paths.SVGPathSegMove && i > index)
						return null;
				}
			}
			return null;
		}
		#endregion

		#region ..获取列表的字符串表示
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.PathString;
		}

		#endregion

		#region ..用指定的变换更新列表
		public SVGPathSegList TransformSegs(System.Drawing.Drawing2D.Matrix matrix)
		{
			PointF[] ps = new PointF[3];
			SVGPathSegList list = new SVGPathSegList();
			for(int i = 0;i<this.NumberOfItems;i++)
			{
				SVGPathSeg seg = this.GetItem(i) as SVGPathSeg;
				if(seg is SVGPathSegMove)
				{
					ps[0] = seg.GetLastPoint(this);
					ps[1] = ps[2] = PointF.Empty;
					matrix.TransformPoints(ps);
					SVGPathSegMove line = new SVGPathSegMovetoAbs(ps[0].X,ps[0].Y);
					list.AppendItem(line);
				}
				else if(seg is SVGPathSegLine)
				{
					ps[0] = seg.GetLastPoint(this);
					ps[1] = ps[2] = PointF.Empty;
					matrix.TransformPoints(ps);
					SVGPathSegLine line = new SVGPathSegLinetoAbs(ps[0].X,ps[0].Y);
					list.AppendItem(line);
				}
				else if(seg is SVGPathSegCurve)
				{
					SVGPathSegCurve curve = seg as SVGPathSegCurve;
					ps[0] = curve.GetLastPoint(this);
					ps[1] = curve.GetFirstControl(this);
					ps[2] = curve.GetSecondControl(this);
					matrix.TransformPoints(ps);
					SVGPathSeg c = new SVGPathSegCurvetoCubicAbs(ps[0].X,ps[0].Y,ps[1].X,ps[1].Y,ps[2].X,ps[2].Y);
					list.AppendItem(c);
				}
				else if(seg is SVGPathSegArc)
				{
					using(System.Drawing.Drawing2D.GraphicsPath path = seg.GetGDIPath(this))
					{
						path.Transform(matrix);
						PointF[] p = path.PathPoints;
						SVGPathSeg c = new SVGPathSegCurvetoCubicAbs(p[1].X,p[1].Y,p[2].X,p[2].Y,p[3].X,p[3].Y);
						list.AppendItem(c);
					}
				}
				else
					list.AppendItem(seg);
			}
			
			ps = null;
			return list;
		}
		#endregion
	}
}
