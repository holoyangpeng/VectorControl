using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGTests 的一般行为，这一类对象都具备requiredFeatures, requiredExtensions 和 systemLanguage属性
	/// </summary>
	public interface ISVGTests:Interface.ISVGElement
	{
		Interface.DataType.ISVGStringList RequiredFeatures{get;}
		Interface.DataType.ISVGStringList RequiredExtensions{get;}
		Interface.DataType.ISVGStringList SystemLanguage{get;}

		bool HasExtension ( string extension );
	}
}
