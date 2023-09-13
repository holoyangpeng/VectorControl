﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SVG.Interface.Text
{
    public interface ISVGTextBlockElement:
        ISVGElement,
        ISVGTests,
        ISVGLangSpace,
        ISVGExternalResourcesRequired,
        ISVGStylable,
        ISVGTransformable
    {
        /// <summary>
        /// 表示矩形对象的X属性
        /// </summary>
        SVG.DataType.SVGLength X { get; }

        /// <summary>
        /// 表示矩形对象的Y属性
        /// </summary>
        SVG.DataType.SVGLength Y { get; }

        /// <summary>
        /// 表示矩形对象的Width对象
        /// </summary>
        SVG.DataType.SVGLength Width { get; }

        /// <summary>
        /// 表示矩形对象的Height属性
        /// </summary>
        SVG.DataType.SVGLength Height { get; }
    }
}
