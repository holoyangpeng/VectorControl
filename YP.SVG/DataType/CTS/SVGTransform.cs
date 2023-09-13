using System;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;

using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�еĶ�ά�任����
	/// </summary>
	public class SVGTransform:DataType.SVGType,Interface.CTS.ISVGTransform
	{
		#region ..���켰����
		public SVGTransform(string transformstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string str = transformstr;
			int start = str.IndexOf("(");
			if(start <0)
				throw new ApplicationException("��Ч�ı任����");
			string type = str.Substring(0, start);
			string valuesList = str.Substring(start+1, str.Length - start - 2);
			Regex re = new Regex("[\\s\\,]+");
			valuesList = re.Replace(valuesList, ",");
			string[] valuesStr = valuesList.Split(new char[]{','});
			int len = valuesStr.GetLength(0);
			float[] values = new float[len];

			#region ..����
			for(int i = 0; i<len; i++)
			{
				values.SetValue(SVGNumber.ParseNumberStr(valuesStr[i]), i);
			}

			if(string.Compare(type,"translate")==0)
			{
				if(len == 1) this.SetTranslate(values[0], 0);
				else if(len == 2) this.SetTranslate(values[0], values[1]);
				else throw new ApplicationException("Wrong number of arguments in translate transform");
			}
			else if(string.Compare(type,"rotate")==0)
			{
				if(len == 1) SetRotate(values[0],0,0);
				else if(len == 3) SetRotate(values[0], values[1], values[2]);
				else throw new ApplicationException("Wrong number of arguments in rotate transform");
			}
			else if(string.Compare(type,"scale")==0)
			{
				if(len == 1) SetScale(values[0], values[0]);
				else if(len == 2) SetScale(values[0], values[1]);
				else throw new ApplicationException("Wrong number of arguments in scale transform");
			}
			else if(string.Compare(type,"skewX")==0)
			{
				if(len == 1) SetSkewX(values[0]);
				else throw new ApplicationException("Wrong number of arguments in skewX transform");
			}
			else if(string.Compare(type,"skewY")==0)
			{
				if(len == 1) SetSkewY(values[0]);
				else throw new ApplicationException("Wrong number of arguments in skewY transform");
			}
			else if(string.Compare(type,"matrix")==0)
			{
				if(len == 6)
				{
					Matrix matrix = new Matrix(values[0], values[1], values[2], values[3], values[4], values[5]);
						SetMatrix(new SVGMatrix(matrix));
				}
				else throw new ApplicationException("Wrong number of arguments in matrix transform");
			}
			#endregion

			values = null;
			valuesStr = null;
			valuesList = null;
			re = null;
			type = null;
			str = null;
		}

		public SVGTransform(Interface.CTS.ISVGMatrix matrix)
		{
			this.SetMatrix(matrix);
		}
		#endregion

		#region ..˽�б���
		TransformType unitType = TransformType.SVG_TRANSFORM_MATRIX;
		Interface.CTS.ISVGMatrix matrix = new SVGMatrix();
		float angle = 0;
		DataType.SVGPoint centerPoint = SVGPoint.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���͵��ı����ʽ������Transform���ͺ�ֵ������
		/// </summary>
		public string TransformString
		{
			get
			{
				switch(this.unitType)
				{
					case TransformType.SVG_TRANSFORM_TRANSLATE:
						return "translate(" +this.matrix.E.ToString() + " " + this.matrix.F.ToString()+")";
					case TransformType.SVG_TRANSFORM_SCALE:
						return "scale(" + this.matrix.B.ToString() + " " + this.matrix.C.ToString()+")";
					case TransformType.SVG_TRANSFORM_SKEWX:
						return "skewX("+ this.matrix.A.ToString() +")";
					case TransformType.SVG_TRANSFORM_SKEWY:
						return "skewY(" + this.matrix.D.ToString() +")";
					case TransformType.SVG_TRANSFORM_ROTATE:
						return "rotate("+this.Angle.ToString() +")";
					case TransformType.SVG_TRANSFORM_MATRIX:
						return "matrix(" +this.matrix.A.ToString()+ " " + this.matrix.B.ToString()+" " + this.matrix.C.ToString()+" "+ this.matrix.D.ToString()+" "+this.matrix.E.ToString()+" " + this.matrix.F.ToString()+")";
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// ��ȡ�任���
		/// </summary>
		public TransformType TransformType
		{
			get
			{
				return this.unitType;
			}
		}

		/// <summary>
		/// ��ȡ��ʾ�˶�ά�任��SVGMatrix����
		/// </summary>
		public Interface.CTS.ISVGMatrix Matrix
		{
			get
			{
				return this.matrix;
			}
		}

		/// <summary>
		/// ��ȡ�˶�ά�任����ת��
		/// </summary>
		public DataType.SVGPoint RotatePoint
		{
			get
			{
				return this.centerPoint;
			}
		}

		/// <summary>
		/// ��ȡ�˶�ά�任����ת��
		/// </summary>
		public float Angle
		{
			get
			{
				return this.angle;
			}
		}
		#endregion

		#region ..��������
		public void SetMatrix(Interface.CTS.ISVGMatrix matrix)
		{
			this.unitType = TransformType.SVG_TRANSFORM_MATRIX;
			this.matrix = matrix;
		}

		public void SetTranslate(float tx, float ty)
		{
			this.unitType = TransformType.SVG_TRANSFORM_TRANSLATE;
			this.matrix.Translate(tx,ty);
		}

		public void SetScale(float sx, float sy)
		{
			this.unitType = TransformType.SVG_TRANSFORM_SCALE;
			this.matrix.ScaleNonUniform(sx,sy);
		}

		public void SetRotate(float angle, float cx, float cy)
		{
			this.unitType = TransformType.SVG_TRANSFORM_ROTATE;
			this.matrix.Translate(cx, cy);
			this.matrix.Rotate(angle);
			this.matrix.Translate(-cx, -cy);
			this.angle = angle;
			this.centerPoint = new DataType.SVGPoint(cx,cy);
		}

		public void SetSkewX(float angle)
		{
			this.unitType = TransformType.SVG_TRANSFORM_SKEWX;
			this.matrix.SkewX(angle);
			this.angle = angle;
		}

		public void SetSkewY(float angle)
		{
			this.unitType = TransformType.SVG_TRANSFORM_SKEWY;
			this.matrix.SkewY(angle);
			this.angle = angle;
		}
		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����,���ı���ﲻ����Transform���͵�����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			switch(this.unitType)
			{
				case TransformType.SVG_TRANSFORM_TRANSLATE:
					return "translate("+this.matrix.E.ToString() + " " + this.matrix.F.ToString()+")";
				case TransformType.SVG_TRANSFORM_SCALE:
					return "scale(" +this.matrix.B.ToString() + " " + this.matrix.C.ToString()+")";
				case TransformType.SVG_TRANSFORM_SKEWX:
					return "skewx(" +this.matrix.A.ToString()+")";
				case TransformType.SVG_TRANSFORM_SKEWY:
					return "skewy(" +this.matrix.D.ToString()+")";
				case TransformType.SVG_TRANSFORM_ROTATE:
					return "rotate("+this.Angle.ToString()+")";
				case TransformType.SVG_TRANSFORM_MATRIX:
					return "matrix("+this.matrix.A.ToString()+ " " + this.matrix.B.ToString()+" " + this.matrix.C.ToString()+" "+ this.matrix.D.ToString()+" "+this.matrix.E.ToString()+" " + this.matrix.F.ToString()+")";
			}
			return string.Empty;
		}
		#endregion
	}
}
