using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using YP.SVG;
using YP.SVG.DocumentStructure;

namespace YP.SymbolDesigner.Document
{
    public class SubSystemDocumentHelper
    {
        #region ..const 
        public const string SubSystemAttributeString = "subsystem";
        public const string SubSystemContentElementName = "subsystem";
        #endregion

        #region ..IsSubSystemElement
        /// <summary>
        /// 判断图元是否时子系统图元
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsSubSystemElement(SVGElement element)
        {
            if (element != null)
            {
                if (element.GetAttribute(SubSystemAttributeString) == "true")
                    return true;
                if (element is SVGUseElement && (element as SVGUseElement).RefElement != null && (element as SVGUseElement).RefElement.GetAttribute(SubSystemAttributeString) == "true")
                    return true; 
            }
            return false;
        }
        #endregion

        #region ..FindSubSystemContentelement
        /// <summary>
        /// 取得子系统图元中内容图元的组对象
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static SVGGElement FindSubSystemContentElement(SVGElement element)
        {
            if (IsSubSystemElement(element))
            {
                var elm = element.SelectSingleNode(string.Format("*[local-name()='{0}']", SubSystemContentElementName)) as SVGElement;
                if (elm != null && elm.FirstChildElement is SVGGElement)
                    return elm.FirstChildElement as SVGGElement;
            }
            return null;
        }
        #endregion

        #region ..CreateSubSystemContent
        /// <summary>
        /// 如果图元是子系统图元，为之创建子系统内容图元对象
        /// </summary>
        /// <param name="element"></param>
        /// <returns>内容图元的组对象</returns>
        public static SVGGElement CreateSubSystemContent(SVGElement element)
        {
            if (IsSubSystemElement(element))
            {
                var doc = element.OwnerDocument;
                var sub = doc.CreateElement(SubSystemContentElementName);
                var g = doc.CreateElement("g");
                sub = element.AppendChild(sub) as XmlElement;
                return sub.AppendChild(g) as SVGGElement;
            }
            return null;
        }
        #endregion

        #region ..FindParentSystem
        /// <summary>
        /// 寻找父一级的系统图元，如果没有，则返回文档的svg根节点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static SVGGElement FindParentSystem(SVGElement element)
        {
            if (element == null)
                return null;
            if (element == element.OwnerDocument.DocumentElement)
                return element as SVGGElement;
            if (IsSubSystemElement(element.ParentElement))
                return FindSubSystemContentElement(element.ParentElement as SVGTransformableElement);
            return FindParentSystem(element.ParentElement);

        }
        #endregion

        #region ..FindSubSystemViewElement
        /// <summary>
        /// 寻找指定图元所属的子系统图元
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static SVGElement FindSubSystemViewElement(SVGElement element)
        {
            if (element == null)
                return null;
            if (IsSubSystemElement(element))
                return element;
            return FindSubSystemViewElement(element.ParentElement);
        }
        #endregion
    }
}
