using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Text
{
	/// <summary>
	/// ʵ��Position��ڵ��ͨ����Ϊ
	/// </summary>
	public abstract class SVGTextPositionElement:Text.SVGTextContentElement,Interface.Text.ISVGTextPositioningElement
	{
		#region ..���켰����
		public SVGTextPositionElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.x = new DataType.SVGLengthList("",this,LengthDirection.Hori);
			this.y = new DataType.SVGLengthList("",this,LengthDirection.Vect);
			this.dx = new DataType.SVGLengthList("",this,LengthDirection.Hori);
			this.dy = new DataType.SVGLengthList("",this,LengthDirection.Vect);
			this.rotate = new DataType.SVGNumberList("",new char[]{',',' '},"0");//,this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGLengthList x,y,dx,dy;
		DataType.SVGNumberList rotate;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡx����
		/// </summary>
		public ISVGLengthList X
		{
			get
			{
				return this.x;
			}
		}

		/// <summary>
		/// ��ȡy����
		/// </summary>
		public ISVGLengthList Y
		{
			get
			{
				return this.y;
			}
		}

		/// <summary>
		/// ��ȡdx����
		/// </summary>
		public ISVGLengthList Dx
		{
			get
			{
				return this.dx;
			}
		}

		/// <summary>
		/// ��ȡdy����
		/// </summary>
		public Interface.DataType.ISVGLengthList Dy
		{
			get
			{
				return this.dy;
			}
		}

		/// <summary>
		/// ��ȡrotate����
		/// </summary>
		public Interface.DataType.ISVGNumberList Rotate
		{
			get
			{
				return this.rotate;
			}
		}
		#endregion

		#region ..��ȡ�ڵ������ָ��λ�õ�����
//		/// <summary>
//		/// ��ȡ�ڵ������ָ��λ�õ�����
//		/// </summary>
//		/// <param name="refPos">�ο�λ��</param>
//		/// <returns></returns>
//		public override PointF GetReferencedPosition(PointF refPos,float textFontSize,string nBaselineShift)
//		{
//			if(this.X.NumberOfItems>0)
//			{
//				refPos.X = ((Interface.DataType.ISVGLength)this.X.GetItem(0)).Value;
//			}
//			if(this.Y.NumberOfItems>0)
//			{
//				refPos.Y = ((Interface.DataType.ISVGLength)this.Y.GetItem(0)).Value;
//			}
//			if(this.Dx.NumberOfItems>0)
//			{
//				float x = ((Interface.DataType.ISVGLength)this.Dx.GetItem(0)).Value;
//				refPos = new PointF(refPos.X + x,refPos.Y);
//			}
//			if(this.Dy.NumberOfItems>0)
//			{
//				refPos.Y += ((Interface.DataType.ISVGLength)this.Dy.GetItem(0)).Value;
//			}
//
//			return base.GetReferencedPosition(refPos,textFontSize,nBaselineShift);
//		}
		#endregion

		#region ..���Բ���
		/// <summary>
		/// �����Է����޸�ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
            try
            {
                switch (attributeName)
                {
                    case "x":
                        this.x = new DataType.SVGLengthList(attributeValue, this, LengthDirection.Hori);
                        break;
                    case "y":
                        this.y = new DataType.SVGLengthList(attributeValue, this, LengthDirection.Vect);
                        break;
                    case "dx":
                        this.dx = new DataType.SVGLengthList(attributeValue, this, LengthDirection.Hori);
                        break;
                    case "dy":
                        this.dy = new DataType.SVGLengthList(attributeValue, this, LengthDirection.Vect);
                        break;
                    case "rotate":
                        this.rotate = new DataType.SVGNumberList(attributeValue, new char[] { ',', ' ' }, "0");//,"auto",this);
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (System.Exception e)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e.Message }, ExceptionLevel.Normal));
            }
		}
		#endregion

		#region ..��ȡ�ɶ�������
		/// <summary>
		/// ��ȡ�ɶ�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//			switch(attributeName)
//			{
//				case "x":
//					return this.x;
//				case "y":
//					return this.y;
//				case "dx":
//					return this.dx;
//				case "dy":
//					return this.dy;
//				case "rotate":
//					return this.rotate;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..��ȡê�㼯
		/// <summary>
		/// ��ȡê�㼯
		/// </summary>
		public override PointF[] GetAnchors()
		{
			return new PointF[0];
		}
		#endregion

		#region ..��������
		public void UpdateDx(YP.SVG.DataType.SVGLengthList list)
		{
			this.dx = list;
		}

		public void UpdateDy(YP.SVG.DataType.SVGLengthList list)
		{
			this.dy = list;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"x")==0 ||string.Compare(attributeName,"font-size") ==0 ||string.Compare(attributeName,"font-family") ==0 
                ||string.Compare(attributeName,"font-weight") ==0 
                ||string.Compare(attributeName,"font-style") ==0 ||string.Compare(attributeName,"y") ==0 ||string.Compare(attributeName,"dx") ==0 
                ||string.Compare(attributeName,"dy") ==0 ||string.Compare(attributeName,"rotate") ==0)
                return AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion
    }
}
