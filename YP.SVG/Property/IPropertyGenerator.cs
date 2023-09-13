using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Property
{
    /// <summary>
    /// 定义提供自定义Property的外部接口
    /// </summary>
    public interface IPropertyGenerator
    {
        IProperty[] GeneratePropertiesForElement(SVGElement element);
    }
}
