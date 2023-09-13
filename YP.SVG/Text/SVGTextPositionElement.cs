using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.DataType;

namespace YP.SVG.Text
{
	/// <summary>
	/// 实现Position类节点的通用行为
	/// </summary>
	public abstract class SVGTextPositionElement:Text.SVGTextContentElement,Interface.Text.ISVGTextPositioningElement
	{
		#region ..构造及消除
		public SVGTextPositionElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.x = new DataType.SVGLengthList("",this,LengthDirection.Hori);
			this.y = new DataType.SVGLengthList("",this,LengthDirection.Vect);
			this.dx = new DataType.SVGLengthList("",this,LengthDirection.Hori);
			this.dy = new DataType.SVGLengthList("",this,LengthDirection.Vect);
			this.rotate = new DataType.SVGNumberList("",new char[]{',',' '},"0");//,this);
		}
		#endregion

		#region ..私有变量
		DataType.SVGLengthList x,y,dx,dy;
		DataType.SVGNumberList rotate;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取x属性
		/// </summary>
		public ISVGLengthList X
		{
			get
			{
				return this.x;
			}
		}

		/// <summary>
		/// 获取y属性
		/// </summary>
		public ISVGLengthList Y
		{
			get
			{
				return this.y;
			}
		}

		/// <summary>
		/// 获取dx属性
		/// </summary>
		public ISVGLengthList Dx
		{
			get
			{
				return this.dx;
			}
		}

		/// <summary>
		/// 获取dy属性
		/// </summary>
		public Interface.DataType.ISVGLengthList Dy
		{
			get
			{
				return this.dy;
			}
		}

		/// <summary>
		/// 获取rotate属性
		/// </summary>
		public Interface.DataType.ISVGNumberList Rotate
		{
			get
			{
				return this.rotate;
			}
		}
		#endregion

		#region ..获取节点相对于指定位置的坐标
//		/// <summary>
//		/// 获取节点相对于指定位置的坐标
//		/// </summary>
//		/// <param name="refPos">参考位置</param>
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

		#region ..属性操作
		/// <summary>
		/// 当属性发生修改时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
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

		#region ..获取可动画属性
		/// <summary>
		/// 获取可动画属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
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

		#region ..获取锚点集
		/// <summary>
		/// 获取锚点集
		/// </summary>
		public override PointF[] GetAnchors()
		{
			return new PointF[0];
		}
		#endregion

		#region ..更新属性
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
