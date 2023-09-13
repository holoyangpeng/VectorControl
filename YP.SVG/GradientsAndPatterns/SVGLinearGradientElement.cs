using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现SVG中的线性渐变LinearGradient对象
	/// </summary>
	public class SVGLinearGradientElement:YP.SVG.GradientsAndPatterns.SVGGradientElement,Interface.GradientsAndPatterns.ISVGLinearGradientElement
	{
		#region ..构造及消除
		public SVGLinearGradientElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.x1 = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y1 = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.x2 = new DataType.SVGLength("100%",this,LengthDirection.Hori);//,"100%");
			this.y2  = new DataType.SVGLength("0",this,LengthDirection.Vect);
		}
		#endregion

		#region ..私有变量
		DataType.SVGLength x1;
		DataType.SVGLength x2;
		DataType.SVGLength y1;
		DataType.SVGLength y2;
		public PointF StartPoint = PointF.Empty,EndPoint = PointF.Empty;
		#endregion

		#region ..公共属性
		public Interface.DataType.ISVGLength X1
		{
			get
			{
				return this.x1;
			}
		}

		public Interface.DataType.ISVGLength Y1
		{
			get
			{
				return this.y1;
			}
		}

		public Interface.DataType.ISVGLength X2
		{
			get
			{
				return this.x2;
			}
		}

		public Interface.DataType.ISVGLength Y2
		{
			get
			{
				return this.y2;
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
                    case "x1":
                        this.x1 = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori);
                        break;
                    case "y1":
                        this.y1 = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        break;
                    case "y2":
                        this.y2 = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        break;
                    case "x2":
                        this.x2 = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "100%");//);
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
//				case "x1":
//					return this.x1;
//				case "x2":
//					return this.x2;
//				case "y1":
//					return this.y1;
//				case "y2":
//					return this.y2;
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
			float x1 = this.X1.Value;
			float y1 = this.Y1.Value;
			float x2 = this.X2.Value;
			float y2 = this.Y2.Value;			

			float x11 = x1;
			float y11 = y1;
			float x21 = x2;
			float y21 = y2;

			this.paintPath.Reset();
			this.paintPath .AddLine(new PointF(x11,y11),new PointF(x11,y11 + 1f));
			this.paintPath.StartFigure();
			this.paintPath.AddLine(new PointF(x21,y21),new PointF(x21,y21 + 1f));
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
			float x1 = this.X1.Value;
			float y1 = this.Y1.Value;
			float x2 = this.X2.Value;
			float y2 = this.Y2.Value;			

			float x11 = x1;
			float y11 = y1;
			float x21 = x2;
			float y21 = y2;
			this.controlPoints = new PointF[]{new PointF((x11+x21)/2f,(y11 + y21)/2f + 0.5f),new PointF(x21,y21+0.5f),new PointF(x21,y21),PointF.Empty,new PointF(x21,(y11 + y21) /2f + 1),PointF.Empty,PointF.Empty,PointF.Empty,PointF.Empty};
//			this.GetTransform(fillPath).TransformPoints(this.controlPoints);
			return this.controlPoints;
		}
		#endregion

		#region ..GetBrush
		/// <summary>
		/// Get the GDI lineargradient brush
		/// </summary>
		/// <param name="bounds">the bounds want to be filled</param>
		/// <param name="opacity">the opacity</param>
        public override Brush GetBrush(SVG.SVGTransformableElement ownerElement, Rectangle bounds, float opacity)
		{
			Rectangle temprect = bounds;
            bounds.Height = bounds.Height == 0 ? 1 : bounds.Height;
            bounds.Width = bounds.Width == 0 ? 1 : bounds.Width;
			if(this.CurrentTime != this.OwnerDocument.CurrentTime|| bounds != this.preBounds || opacity != this.preFloat)
			{
				SVG.SpreadMethod spread = (SVG.SpreadMethod)this.SpreadMethod;
				bool userSpaceOnUse = (SVG.SVGUnitType)this.GradientUnits == SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE ;
				float x1 = this.X1.Value;
				float y1 = this.Y1.Value;
				float x2 = this.X2.Value;
				float y2 = this.Y2.Value;

				float x11 = x1;
				float y11 = y1;
				float x21 = x2;
				float y21 = y2;
	
				RectangleF rect = RectangleF.Empty;
				PointF[] points = new PointF[]{new PointF(x1,y1),new PointF(x2,y2)};
				using(Matrix matrix = this.GradientTransform.FinalMatrix.GetGDIMatrix())
				{
					if(!matrix.IsIdentity)
					{
						matrix.TransformPoints(points);
					}
				}
				x1 = (float)Math.Round(points[0].X,5);
				x2 = (float)Math.Round(points[1].X,5);
				y1 = (float)Math.Round(points[0].Y,5);
				y2 = (float)Math.Round(points[1].Y,5);

				if(!userSpaceOnUse) 
				{
					x1 = bounds.X + x1 * bounds.Width;
					y1 = bounds.Y + y1 * bounds.Height;
					x2 = bounds.X + x2 * bounds.Width;
					y2 = bounds.Y + y2 * bounds.Height;
				}

				bool vert = Math.Abs(x1) == Math.Abs(x2);
				float k = 1;

				PointF startpoint = new PointF(x1,y1);
				PointF endpoint = new PointF(x2,y2);
				PointF start1 = startpoint;
				PointF end1 = endpoint;

				ColorBlend colorBlend = new ColorBlend(this.ChildElements.Count);

				ColorBlend tempblend = this.GetColors(opacity);
				float[] positions = tempblend.Positions;
				Color[] colors = tempblend.Colors;

				float[] temppos = (float[])positions.Clone();
				Color[] tempcolor = (Color[])colors.Clone();
				Array.Sort(temppos,tempcolor);
				if(temppos.Length != 0 || tempcolor.Length != 0)
				{
					Color start = tempcolor[0];
					Color end = tempcolor[tempcolor.Length-1];

					float startx = temppos[0];
					float endx = temppos[temppos.Length - 1];

					if(temppos[0] != 0)
					{
						float[] temppos1 = (float[])temppos.Clone();
						Color[] tempcolor1 = (Color[])tempcolor.Clone();
						temppos = new float[temppos.Length + 1];
						tempcolor = new Color[tempcolor.Length + 1];
						tempcolor1.CopyTo(tempcolor,1);
						temppos1.CopyTo(temppos,1);
						temppos[0] = 0f;
						tempcolor[0] = start;
					}

					if(temppos[temppos.Length -1] != 1f)
					{
						float[] temppos1 = (float[])temppos.Clone();
						Color[] tempcolor1 = (Color[])tempcolor.Clone();
						temppos = new float[temppos.Length +1];
						temppos1.CopyTo(temppos,0);
						temppos[temppos.Length -1] = 1f;
						tempcolor = new Color[tempcolor.Length + 1];
						tempcolor1.CopyTo(tempcolor,0);
						tempcolor[tempcolor.Length -1] = end;
					}

					PointF[] points1 = new PointF[1];

					PointF[] points2 = new PointF[4];
		
					if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_PAD ) 
					{
						if(vert)
						{
							startpoint = new PointF(startpoint.X,(float)Math.Min(y1,bounds.Y));
							endpoint = new PointF(endpoint.X,(float)Math.Max(y2,bounds.Bottom));
							float oldy = y1;
							float oldheight = y2 - y1;
							for(int i = 0;i<temppos.Length;i++)
							{
								temppos[i] = (float)Math.Max(0,Math.Min(1,(oldy + oldheight * temppos[i] - startpoint.Y)/ (endpoint.Y - startpoint.Y)));
							}
						}
						else
						{
							k =(y2 - y1) / (x2 - x1);
							float b =  (y1 - k * x1 ) / (1 + k * k);
							PointF p1 = bounds.Location;
							PointF p2 = new PointF(bounds.Right,bounds.Y);
							PointF p3 = new PointF(bounds.Right,bounds.Bottom);
							PointF p4 = new PointF(bounds.X,bounds.Bottom);
							points1 = new PointF[]{p1,p2,p3,p4};
							for(int i = 0;i<points1.Length;i++)
							{
								PointF point = points1[i];
								float y = (k * k * point.Y + k * point.X ) / ( 1 + k * k ) + b;
								float x = k* (point.Y - y ) + point.X;
								points2[i] = new PointF(x,y);
								if(x1 < x2)
								{
									if(x < startpoint.X)
										startpoint = new PointF(x,y);
									else if(x > endpoint.X)
										endpoint = new PointF(x,y);
								}
								else
								{
									if(x <endpoint.X)
										endpoint = new PointF(x,y);
									else if(x > startpoint.X )
										startpoint = new PointF(x,y);
								}
							}
							float oldx = x1;
							float oldwidth = x2 - x1;
							for(int i = 0;i<temppos.Length;i++)
							{
								temppos[i] = (oldx + oldwidth * temppos[i] - startpoint.X)/ (endpoint.X - startpoint.X);
							}
				
						}
						Array.Sort(temppos,tempcolor);
						if(temppos[0] != 0)
						{
							float[] temppos1 = (float[])temppos.Clone();
							Color[] tempcolor1 = (Color[])tempcolor.Clone();
							temppos = new float[temppos.Length + 1];
							tempcolor = new Color[tempcolor.Length + 1];
							tempcolor1.CopyTo(tempcolor,1);
							temppos1.CopyTo(temppos,1);
							temppos[0] = 0f;
							tempcolor[0] = tempcolor1[0];
							temppos1 = null;
							tempcolor1 = null;
						}

						if(temppos[temppos.Length -1] != 1f)
						{
							float[] temppos1 = (float[])temppos.Clone();
							Color[] tempcolor1 = (Color[])tempcolor.Clone();
							temppos = new float[temppos.Length +1];
							temppos1.CopyTo(temppos,0);
							temppos[temppos.Length -1] = 1f;
							tempcolor = new Color[tempcolor.Length + 1];
							tempcolor1.CopyTo(tempcolor,0);
							tempcolor[tempcolor.Length -1] = tempcolor1[tempcolor1.Length - 1];
							temppos1 = null;
							tempcolor1 = null;
						}
					}
			
					
					this.brush = new LinearGradientBrush(startpoint,endpoint,start,end);
					if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_REFLECT )
						((System.Drawing.Drawing2D.PathGradientBrush)brush).WrapMode = WrapMode.Tile;
					else if(spread == SVG.SpreadMethod.SVG_SPREADMETHOD_REPEAT  )
						((System.Drawing.Drawing2D.PathGradientBrush)brush).WrapMode = WrapMode.TileFlipXY;

					this.StartPoint = startpoint;
					this.EndPoint = endpoint;
					colorBlend.Colors = tempcolor;
					colorBlend.Positions = temppos;
					((LinearGradientBrush)brush).InterpolationColors = colorBlend;
				
					tempcolor = null;
					temppos = null;
					points1 = null;
					points = null;
				}
			}
			this.preBounds = temprect;
			this.CurrentTime = this.OwnerDocument.CurrentTime;
			this.preFloat = opacity;
            var brush1 = (this.brush as LinearGradientBrush).Clone() as LinearGradientBrush;
            brush1.InterpolationColors = (this.brush as LinearGradientBrush).InterpolationColors;
            return brush1;
		}
		#endregion
	}
}
