using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现辐射渐变对象RadialGradient
	/// </summary>
	public class SVGRadialGradientElement:YP.SVG.GradientsAndPatterns.SVGGradientElement,Interface.GradientsAndPatterns.ISVGRadialGradientElement
	{
		#region ..构造及消除
		public SVGRadialGradientElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.cx = new DataType.SVGLength("50%",this,LengthDirection.Hori);//,);
			this.cy = new DataType.SVGLength("50%",this,LengthDirection.Vect);//,);
			this.r = new DataType.SVGLength("50%",this,LengthDirection.Viewport);//,"50%");
			this.fx = new DataType.SVGLength("50%",this,LengthDirection.Hori);//,"50%");
			this.fy = new DataType.SVGLength("50%",this,LengthDirection.Vect);//,"50%");
		}
		#endregion

		#region ..私有变量
		DataType.SVGLength cx,cy,r,fx,fy;
		bool setfx = false;
		bool setfy = false;
		#endregion

		#region ..公共属性
		public Interface.DataType.ISVGLength Cx
		{
			get
			{
				return this.cx;
			}
		}

		public Interface.DataType.ISVGLength Cy
		{
			get
			{
				return this.cy;
			}
		}

		public Interface.DataType.ISVGLength R
		{
			get
			{
				return this.r;
			}
		}

		public Interface.DataType.ISVGLength Fx
		{
			get
			{
				return this.fx;
			}
		}

		public Interface.DataType.ISVGLength Fy
		{
			get
			{
				return this.fy;
			}
		}
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
                    case "cx":
                        this.cx = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "50%");//);
                        if (this.setfx)
                            this.fx = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "50%");//);
                        break;
                    case "cy":
                        this.cy = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect, "50%");//);
                        if (this.setfy)
                            this.fy = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect, "50%");//);
                        break;
                    case "r":
                        this.r = new DataType.SVGLength(attributeValue, this, LengthDirection.Viewport, "50%");//);
                        break;
                    case "fx":
                        this.fx = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "50%");//);
                        this.setfx = true;
                        break;
                    case "fy":
                        this.fy = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);//,"50%");
                        this.setfy = true;
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (Exception e)
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
//				case "cx":
//					return this.cx;
//				case "cy":
//					return this.cy;
//				case "r":
//					return this.r;
//				case "fx":
//					return this.fx;
//				case "fy":
//					return this.fy;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..当对象填充指定路径时，获取其控制路径
		/// <summary>
		/// 当对象填充指定路径时，获取其控制路径
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public override GraphicsPath GetControlPath(GraphicsPath fillPath)
		{
			bool userSpaceOnUse = (YP.SVG.SVGUnitType)this.GradientUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
			float cx = this.Cx.Value;
			float cy = this.Cy.Value;
			float r = this.R.Value;
			float fx = this.Fx.Value;
			float fy = this.Fy.Value;

			this.paintPath.Reset();
			this.paintPath.AddEllipse(cx - r,cy - r,2 * r,2 * r);
//			this.paintPath.Transform(this.GetTransform(fillPath));
			return this.paintPath;
		}
		#endregion

		#region ..当对象填充指定路径时，获取其控制点集
		/// <summary>
		/// 当对象填充指定路径时，获取其控制点集
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public override PointF[] GetControlPoints(GraphicsPath fillPath)
		{
			bool userSpaceOnUse = (YP.SVG.SVGUnitType)this.GradientUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
			float cx = this.Cx.Value;
			float cy = this.Cy.Value;
			float r = this.R.Value;
			float fx = this.Fx.Value;
			float fy = this.Fy.Value;
			this.controlPoints = new PointF[]{new PointF(cx,cy),new PointF(cx + r * (float)Math.Sin(Math.PI /2 + Math.PI / 12),cy + r * (float)Math.Cos(Math.PI /2 + Math.PI / 12)),new PointF(cx + r * (float)Math.Sin(Math.PI /2 - Math.PI / 12),cy + r * (float)Math.Cos(Math.PI /2 - Math.PI / 12)),new PointF(cx,cy + r),PointF.Empty,PointF.Empty,PointF.Empty,new PointF(cx + r,cy)};
//			this.GetTransform(fillPath).TransformPoints(this.controlPoints);
			return this.controlPoints;
		}
		#endregion

		#region ..当对象填充指定路径时，绘制画笔
		/// <summary>
		/// 当对象填充指定路径时，获取绘制画笔
		/// </summary>
		/// <param name="bounds">填充路径边界</param>
        public override Brush GetBrush(SVG.SVGTransformableElement ownerElement, Rectangle bounds, float opacity)
		{
			Rectangle temprect = bounds;
            bounds.Height = bounds.Height == 0 ? 1 : bounds.Height;
            bounds.Width = bounds.Width == 0 ? 1 : bounds.Width;
			if(this.CurrentTime != this.OwnerDocument.CurrentTime|| bounds != this.preBounds || opacity != this.preFloat)
			{
				SVG.SpreadMethod spread = (SVG.SpreadMethod)this.SpreadMethod;
				bool userSpaceOnUse = (SVG.SVGUnitType)this.GradientUnits == SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE ;
				float cx = this.Cx.Value;
				float cy = this.Cy.Value;
				float rx = this.R.Value;
				float ry = this.R.Value;
				float fx = this.Fx.Value;
				float fy = this.Fy.Value;

				using(GraphicsPath path1 = new GraphicsPath())
				{
					path1.AddEllipse(cx - rx,cy - ry,2 * rx,2 * ry);
					using(Matrix coord = new Matrix())
					{
						RectangleF viewPort = new RectangleF(0,0,1,1);

						ColorBlend colorBlend = new ColorBlend(this.ChildElements.Count);
						ColorBlend tempblend = this.GetColors(opacity);
						float[] positions = tempblend.Positions;
						Color[] colors = tempblend.Colors;

						float[] temppos = (float[])positions.Clone();
						Color[] tempcolor = (Color[])colors.Clone();
						Array.Sort(temppos,tempcolor);
						
						if(temppos.Length > 0 && tempcolor.Length > 0)
						{
							Color start = tempcolor[0];
							Color end = tempcolor[tempcolor.Length-1];

							if(temppos.Length > 0 && temppos[0] != 0)
							{
								float[] temppos1 = (float[])temppos.Clone();
								Color[] tempcolor1 = (Color[])tempcolor.Clone();
								temppos = new float[temppos.Length + 1];
								tempcolor = new Color[tempcolor.Length + 1];
								tempcolor1.CopyTo(tempcolor,1);
								temppos1.CopyTo(temppos,1);
								temppos[0] = 0f;
								tempcolor[0] = start;
								temppos1 = null;
								tempcolor1 = null;
							}

							if(temppos.Length > 0 && temppos[temppos.Length -1] != 1f)
							{
								float[] temppos1 = (float[])temppos.Clone();
								Color[] tempcolor1 = (Color[])tempcolor.Clone();
								temppos = new float[temppos.Length +1];
								temppos1.CopyTo(temppos,0);
								temppos[temppos.Length -1] = 1f;
								tempcolor = new Color[tempcolor.Length + 1];
								tempcolor1.CopyTo(tempcolor,0);
								tempcolor[tempcolor.Length -1] = end;
								temppos1 = null;
								tempcolor1 = null;

							}
			
							if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_PAD )
							{
								float minx = (float)Math.Min(cx - rx,viewPort.X);
								float miny = (float)Math.Min(cy - ry,viewPort.Y);
								float maxr = (float)Math.Max(rx,viewPort.Width / 2f);

								float oldx = cx - rx;
								float oldr = rx;
								for(int i = 0;i<temppos.Length;i++)
								{
									temppos[i] = (oldx + oldr * temppos[i] - minx) / maxr;
								}

								if(temppos.Length > 0 && temppos[0] != 0)
								{
									float[] temppos1 = (float[])temppos.Clone();
									Color[] tempcolor1 = (Color[])tempcolor.Clone();
									temppos = new float[temppos.Length + 1];
									tempcolor = new Color[tempcolor.Length + 1];
									tempcolor1.CopyTo(tempcolor,1);
									temppos1.CopyTo(temppos,1);
									temppos[0] = 0f;
									tempcolor[0] = start;
									temppos1 = null;
									tempcolor1 = null;
								}

								if(temppos.Length > 0 && temppos[temppos.Length -1] != 1f)
								{
									float[] temppos1 = (float[])temppos.Clone();
									Color[] tempcolor1 = (Color[])tempcolor.Clone();
									temppos = new float[temppos.Length +1];
									temppos1.CopyTo(temppos,0);
									temppos[temppos.Length -1] = 1f;
									tempcolor = new Color[tempcolor.Length + 1];
									tempcolor1.CopyTo(tempcolor,0);
									tempcolor[tempcolor.Length -1] = end;
									temppos1 = null;
									tempcolor1 = null;
								}
							}

							Array.Reverse(tempcolor);
							Array.Reverse(temppos);
							for(int i = 0;i<temppos.Length;i++)
							{
								temppos[i] = 1- temppos[i];
							}

							using(Matrix matrix = new Matrix())//this.GradientTransform.FinalMatrix.GetGDIMatrix().Clone())
							{
								if(!userSpaceOnUse)
								{
									matrix.Translate(bounds.X,bounds.Y);
									matrix.Scale(bounds.Width,bounds.Height);
								}
								matrix.Multiply(this.GradientTransform.FinalMatrix.GetGDIMatrix());
								
								path1.Transform(matrix);
								
								this.brush = new PathGradientBrush(path1);
								colorBlend.Colors = tempcolor;
								colorBlend.Positions = temppos;
								((System.Drawing.Drawing2D.PathGradientBrush)brush).InterpolationColors = colorBlend;
						
								if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_REFLECT )
									((System.Drawing.Drawing2D.PathGradientBrush)brush).WrapMode = WrapMode.Tile;
								else if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_REPEAT  )
									((System.Drawing.Drawing2D.PathGradientBrush)brush).WrapMode = WrapMode.TileFlipXY;
								else
									((System.Drawing.Drawing2D.PathGradientBrush)brush).WrapMode = WrapMode.Clamp;

								PointF[] focus = new PointF[]{new PointF(fx,fy)};
								matrix.TransformPoints(focus);
								((System.Drawing.Drawing2D.PathGradientBrush)brush).CenterPoint = focus[0];
								focus = null;
								tempcolor = null;
								temppos = null;
								colorBlend = null;
								tempblend = null;
								positions = null;
								colors = null;
							}
						}
					}
				}
			}
			this.preBounds = temprect;
			this.CurrentTime = this.OwnerDocument.CurrentTime;
			this.preFloat = opacity;
			return (Brush)this.brush.Clone();
		}
		#endregion
	}
}
