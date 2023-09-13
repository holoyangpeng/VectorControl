using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.CTS;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现二维变换列表
	/// </summary>
	public class SVGTransformList:DataType.SVGTypeList,Interface.CTS.ISVGTransformList
	{
        static Regex re = new Regex("([A-Za-z]+)\\s*\\(([eE\\-0-9\\.\\,\\s]+)\\)", RegexOptions.Compiled);

		#region ..构造及消除
		public SVGTransformList(string transformstr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			Match match = re.Match(transformstr);

			SVGTransform transform;
			while(match.Success)
			{
				transform = new SVGTransform(match.Value);
				this.AppendItem(transform);
				match = match.NextMatch();
			}
			transform = null;
			match = null;
		}

		public SVGTransformList(SVGTransform[] transforms)
		{
			this.list.AddRange(transforms);
		}
		#endregion

		#region ..私有变量
		SVGMatrix finalmatrix = new SVGMatrix();
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取最终的扭曲
		/// </summary>
		public Interface.CTS.ISVGMatrix FinalMatrix
		{
			get
			{
//				if(this.finalmatrix == null)
//				{
					IEnumerator en = list.GetEnumerator();
//					if(this.finalmatrix != null)
//						this.finalmatrix.Dispose();
//					this.finalmatrix = null;
					this.finalmatrix = new SVGMatrix(new System.Drawing.Drawing2D.Matrix());
					Interface.CTS.ISVGTransform transform;
					while(en.MoveNext())
					{
						transform = (Interface.CTS.ISVGTransform) en.Current;
						finalmatrix.Multiply(transform.Matrix);
					}
//				}
				return this.finalmatrix;
			}
		}

		/// <summary>
		/// 获取类型的文本表达式，包含Transform类型和值的描述
		/// </summary>
		public string TransformString
		{
			get
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
				foreach(SVGTransform sf in this.list)
					sb.Append(sf.TransformString + " ");
				return sb.ToString();
			}
		}
		#endregion

		#region ..获取变形数组
		/// <summary>
		/// 获取变形数组
		/// </summary>
		/// <returns></returns>
		public SVGTransform[] GetTransforms()
		{
			SVGTransform[] transf = new SVGTransform[this.list.Count];
			this.list.CopyTo(transf);
			return transf;
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
			return svgType is Interface.CTS.ISVGTransform;
		}
		#endregion
 
		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达，该文本不包含变换的Transform类型描述
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			foreach(SVGTransform sf in this.list)
			{
				sb.Append(sf.ToString()+" ");
			}
			return sb.ToString();
		}
		#endregion
	}
}
