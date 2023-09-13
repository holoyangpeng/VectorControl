using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YP.SVG;


namespace YP.SymbolDesigner.Helper
{
    public class MatlabHelper
    {
        /// <summary>
        /// 获取给定图元中用于设定连接点的主图元
        /// </summary>
        /// <param name="ownerElement"></param>
        /// <returns></returns>
        public static SVGTransformableElement GetMainElement(SVGTransformableElement ownerElement)
        {
            if (ownerElement is SVG.DocumentStructure.SVGUseElement)
                return ownerElement;
            if (ownerElement is SVG.DocumentStructure.SVGGElement)
            {
                var result = ownerElement;
                var g = ownerElement as SVG.DocumentStructure.SVGGElement;
                //直接添加时，将g节点上的连接点信息复制到主图元对象
                var rects = g.ChildElements;
                if (rects.Count > 0)
                    result = rects[0] as SVGTransformableElement;

                foreach (SVGElement elm in rects)
                {
                    //第一个有textBlock子对象的图元
                    if (elm.Name != "line" && elm.GetElementsByTagName("textBlock").Count > 0)
                    {
                        result = elm as SVGTransformableElement;
                        break;
                    }
                }

                return result;
            }

            return ownerElement;
        }
    }
}
