using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG
{
	/// <summary>
	/// 实现SVGLocatableElement对象，这类对象都具有特定的边界
	/// </summary>
	public abstract class SVGLocatableElement:SVGStyleable,Interface.ISVGLocatable
    {
        #region ..构造及消除
        public SVGLocatableElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

        #region ..公共属性
        

        /// <summary>
        /// 指示当前对象是否被选中
        /// </summary>
        public bool Selected
        {
            set
            {
                if (value && !this.OwnerDocument.SelectCollection.Contains(this))
                    this.OwnerDocument.SelectCollection.Add(this);
                else if (!value && this.OwnerDocument.SelectCollection.Contains(this))
                    this.OwnerDocument.SelectCollection.Remove(this);
            }
            get
            {
                return this.OwnerDocument.SelectCollection.Contains(this);
            }
        }

		/// <summary>
		/// 获取最近一层的视图元素
		/// </summary>
		public Interface.ISVGElement NearestViewportElement
		{
			get
			{
				XmlNode parent = this.ParentNode;
				while(parent != null)
				{
					if(parent is Interface.DocumentStructure.ISVGSVGElement)
					{
						return ( Interface.DocumentStructure.ISVGSVGElement)parent;
					}
					parent = parent.ParentNode;
				}
				return null;
			}
		}

		/// <summary>
		/// 获取最远一层的视图元素
		/// </summary>
		public Interface.ISVGElement FarthestViewportElement
		{
			get
			{
				Document.SVGDocument doc = (Document.SVGDocument) this.OwnerDocument;
				if(doc.RootElement == this) 
					return null;
				else
					return (Interface.ISVGElement) doc.RootElement;
			}
		}

		/// <summary>
		/// 获取当前用户空间的边界
		/// </summary>
		/// <returns></returns>
		public virtual Interface.DataType.ISVGRect GetBBox()
		{
			if(this is Interface.ISVGPathable)
			{
				Interface.ISVGPathable p = (Interface.ISVGPathable) this;
				GraphicsPath gpath = (p as SVG.Interface.ISVGPathable).GPath;
				DataType.SVGMatrix svgMatrix = (DataType.SVGMatrix)this.GetScreenCTM();
				return new DataType.SVGRect(gpath.GetBounds(svgMatrix.GetGDIMatrix()));
			}
			else
			{
				RectangleF union = RectangleF.Empty;
				YP.SVG.SVGLocatableElement locChild;

				foreach(XmlNode childNode in ChildNodes)
				{
					if(childNode is YP.SVG.SVGLocatableElement)
					{
						locChild = (YP.SVG.SVGLocatableElement)childNode;
						DataType.SVGRect svgBBox = (DataType.SVGRect)locChild.GetBBox();
						RectangleF bbox = svgBBox.GDIRect;
						if(union.IsEmpty)
							union = bbox;
						else
							union = RectangleF.Union(union, bbox);
					}
				}
				
				return new DataType.SVGRect(union);
			}
		}

		/// <summary>
		/// 获取未经过变换的边界
		/// </summary>
		/// <returns></returns>
		public virtual Interface.DataType.ISVGRect GetOriBBox()
		{
			if(this is Interface.ISVGPathable)
			{
				Interface.ISVGPathable p = (Interface.ISVGPathable) this;
				GraphicsPath gpath = (p as SVG.Interface.ISVGPathable).GPath;
				return new DataType.SVGRect(gpath.GetBounds());
			}
			else
			{
				RectangleF union = RectangleF.Empty;
				YP.SVG.SVGLocatableElement locChild;

				foreach(XmlNode childNode in ChildNodes)
				{
					if(childNode is YP.SVG.SVGLocatableElement)
					{
						locChild = (YP.SVG.SVGLocatableElement) childNode;
						DataType.SVGRect svgBBox = (DataType.SVGRect)locChild.GetBBox();
						RectangleF bbox = svgBBox.GDIRect;
						if(union.IsEmpty)
							union = bbox;
						else
							union = RectangleF.Union(union, bbox);
					}
				}
				
				return new DataType.SVGRect(union);
			}
		}

		/// <summary>
		/// 获取从最近一层视图对象到这一层之间所产生的变换
		/// </summary>
		/// <returns></returns>
		public Interface.CTS.ISVGMatrix GetCTM()
		{
			Interface.ISVGElement nVE = this.NearestViewportElement;
			Interface.CTS.ISVGMatrix matrix = new DataType.SVGMatrix();
			if(nVE != null)
			{
				Interface.CTS.ISVGTransformList svgTList;
				Interface.ISVGLocatable par = this;// (SvgTransformableElement) ParentNode;
				while(par != nVE)
				{
					if(par is Interface.ISVGTransformable)
					{
						svgTList = ((Interface.ISVGTransformable)par).Transform;
						matrix = matrix.Multiply(svgTList.FinalMatrix);
					}
					par = (Interface.ISVGLocatable) ((System.Xml.XmlNode)par).ParentNode;
				}
			}
			return matrix;
		}

		/// <summary>
		/// 获取从上层用户空间到当前用户空间所经过的变换
		/// </summary>
		/// <returns></returns>
		public Interface.CTS.ISVGMatrix GetScreenCTM()
		{
			Interface.ISVGElement nVE = this.FarthestViewportElement;
			Interface.CTS.ISVGMatrix matrix = new DataType.SVGMatrix();
			if(nVE != null)
			{
				Interface.CTS.ISVGTransformList svgTList;
				Interface.ISVGLocatable par = this;// (SvgTransformableElement) ParentNode;
				while(par != nVE)
				{
					if(par is Interface.ISVGTransformable)
					{
						svgTList = ((Interface.ISVGTransformable)par).Transform;
						matrix = matrix.Multiply(svgTList.FinalMatrix);
						}
                    if (((XmlNode)par).ParentNode is Interface.ISVGLocatable)
                        par = (Interface.ISVGLocatable)((XmlNode)par).ParentNode;
                    else
                        break;
				}
			}
			return matrix;
		}
		#endregion

		#region ..公共方法
		public Interface.CTS.ISVGMatrix GetTransformToElement(Interface.ISVGElement element )
		{
			throw new NotImplementedException("getTransformToElement()");
		}
		#endregion
    }
}
