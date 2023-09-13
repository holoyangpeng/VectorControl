using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace YP.SVG
{
    /// <summary>
    /// SVG文档中的XmlNode
    /// </summary>
    public class SVGTextNode:XmlText
    {
        #region ..Constructor
        public SVGTextNode(string strData, SVG.Document.SVGDocument doc)
            : base(strData, doc)
        {
        }
        #endregion

        #region ..properties
        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (base.Value != value)
                {
                    if (this.ParentNode is SVG.Interface.Text.ITextElement)
                    {
                        SVGTransformableElement elm = this.ParentNode as SVGTransformableElement;
                        if (elm != null)
                            elm.OwnerDocument.RefreshOriginalElement(elm);
                    }
                    base.Value = value;
                    if (this.ParentNode is SVG.Interface.Text.ITextElement)
                    {
                        (this.ParentNode as SVG.Interface.Text.ITextElement).UpdateText();
                        SVGTransformableElement elm = this.ParentNode as SVGTransformableElement;
                        if (elm != null && elm.IsActive)
                        {
                            elm.OwnerDocument.InvokeAttributeChange(new AttributeChangedEventArgs(elm, "innertext"));
                            elm.OwnerDocument.RefreshElement(elm);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
