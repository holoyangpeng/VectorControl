using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace YP.SVG.Text
{
    /// <summary>
    /// 扩展类，实现TextBlock
    /// </summary>
    public class SVGTextBlockElement:SVGTransformableElement,
        Interface.ISVGPathable,
        Interface.Text.ITextElement
    {
        #region ..构造及消除
        public SVGTextBlockElement(string prefix, string localname, string ns, Document.SVGDocument doc)
            : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            this.x = new DataType.SVGLength("0", this, LengthDirection.Hori);
            this.y = new DataType.SVGLength("0", this, LengthDirection.Vect);
            this.width = new DataType.SVGLength("100%", this, LengthDirection.Hori);
            this.height = new DataType.SVGLength("100%", this, LengthDirection.Vect);
            render = new Render.SVGTextBlockRender(this);
		}
		#endregion

        #region ..private fields
        DataType.SVGLength x,y;
        DataType.SVGLength width , height ;
        Render.SVGTextBlockRender render;
        #endregion

        #region ..public properties
        public override Render.SVGBaseRenderer SVGRenderer
        {
            get
            {
                return render;
            }
        }
        /// <summary>
        /// 表示矩形对象的X属性
        /// </summary>
        public DataType.SVGLength X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.SetAttribute("x", value.ValueAsString, false);
            }
        }

        /// <summary>
        /// 表示矩形对象的Y属性
        /// </summary>
        public DataType.SVGLength Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.SetAttribute("y", value.ValueAsString, false);
            }
        }

        /// <summary>
        /// 表示矩形对象的Width对象
        /// </summary>
        public DataType.SVGLength Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.SetAttribute("width", value.ValueAsString, false);
            }
        }

        public override Interface.ISVGElement ViewPortElement
        {
            get
            {
                return FindViewportElement(this) as Interface.ISVGElement;
            }
        }

        public override string Label
        {
            get
            {
                return this.InnerText.Replace("\r\n", "\n");
            }
            set
            {
                this.InnerText = value;
            }
        }

        /// <summary>
        /// 表示矩形对象的Height属性
        /// </summary>
        public DataType.SVGLength Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.SetAttribute("height", value.ValueAsString, false);
            }
        }
        #endregion

        #region ..ISVGPathable成员
        public override GraphicsPath GraphicsPathIncludingTextBlock
        {
            get
            {
                return null;
            }
        }

        public System.Drawing.Drawing2D.GraphicsPath GPath
        {
            get 
            {
                if (this.graphicsPath == null)
                {
                    this.graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    this.render.GetPath(this,graphicsPath);
                }
                return graphicsPath;
            }
        }
        #endregion

        #region ..SetSVGAttribute
        public override void SetSVGAttribute(string attributeName, string attributeValue)
        {
            try
            {
                switch (attributeName)
                {
                    case "x":
                        this.x = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori,"0%");
                        break;
                    case "y":
                        this.y = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect,"0%");
                        break;
                    case "width":
                        this.width = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori,"100%");
                        break;
                    case "height":
                        this.height = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect,"100%");
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

        #region ..FindViewport
        Interface.ISVGTextBlockContainer FindViewportElement(SVGElement element)
        {
            if (element == null)
                return null;
            if (element.ParentElement is Interface.ISVGTextBlockContainer)
                return element.ParentElement as Interface.ISVGTextBlockContainer;

            return FindViewportElement(element.ParentElement);
        }
        #endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"x")==0 ||string.Compare(attributeName,"y") ==0 
                ||string.Compare(attributeName,"width") ==0 ||string.Compare(attributeName,"height") ==0 
                || attributeName=="text-anchor" ||string.Compare(attributeName,"vertical-align") ==0
                ||string.Compare(attributeName,"overflow") ==0 ||string.Compare(attributeName,"wrap") ==0)
                return AttributeChangedResult.GraphicsPathChanged;
            if (string.Compare(attributeName,"text-color")==0)
                return AttributeChangedResult.VisualChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..OnTransformChanged
        public override void OnTransformChanged()
        {
            this.UpdatePath(false);
            base.OnTransformChanged();
        }
        #endregion

        #region ..ITextElement
        void Interface.Text.ITextElement.UpdateText()
        {
            this.UpdatePath(false);
        }

        public override void UpdatePath(bool updateParent)
        {
            base.UpdatePath(updateParent);
            if (!updateParent && this.ParentElement is SVGTransformableElement)
                (this.ParentElement as SVGTransformableElement).UpdateTextBlockPath();
        }
        #endregion

        #region ..InternalAppendChild
        public override System.Xml.XmlNode AppendChild(System.Xml.XmlNode newChild)
        {
            if (newChild is System.Xml.XmlText&& this.IsActive)
            {
                this.OwnerDocument.RefreshOriginalElement(this);
                (this as Interface.Text.ITextElement).UpdateText();
            }
            System.Xml.XmlNode node = base.InternalAppendChild(newChild);
            if (newChild is System.Xml.XmlText && this.IsActive)
                this.OwnerDocument.RefreshElement(this);
            
            return node;
        }
        #endregion

        #region ..InternalRemoveChild
        public override System.Xml.XmlNode RemoveChild(System.Xml.XmlNode oldChild)
        {
            if (oldChild is System.Xml.XmlText && this.IsActive)
            {
                this.OwnerDocument.RefreshOriginalElement(this);
                (this as Interface.Text.ITextElement).UpdateText();
            }
            System.Xml.XmlNode node = base.InternalRemoveChild(oldChild);
            if (oldChild is System.Xml.XmlText && this.IsActive)
                this.OwnerDocument.RefreshElement(this);

            return node;
        }
        #endregion

        #region ..ExportSVGElement
        public SVGElement ExportSVGElement()
        {
            SVGElement elm = this.OwnerDocument.CreateElement("g") as SVGElement;

            foreach (System.Xml.XmlAttribute attr in this.Attributes)
                if(attr.Name != "text-anchor" && attr.Name != "transform")
                    elm.InternalSetAttribute(attr.Name, attr.Value);

            if (this.SVGRenderer is Render.SVGTextBlockRender)
                (this.SVGRenderer as Render.SVGTextBlockRender).ExportNativeSVG(elm);
            return elm;
        }
        #endregion

        public override System.Xml.XmlNode CloneNode(bool deep)
        {
            SVGTextBlockElement textBlock = base.CloneNode(deep) as SVGTextBlockElement;
            if (this.SVGRenderer != null && textBlock.SVGRenderer != null)
                this.SVGRenderer.CloneRender(textBlock.SVGRenderer);
            return textBlock;
        }
    }
}
