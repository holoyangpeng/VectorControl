using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����ISVGTests ��һ����Ϊ����һ����󶼾߱�requiredFeatures, requiredExtensions �� systemLanguage����
	/// </summary>
	public interface ISVGTests:Interface.ISVGElement
	{
		Interface.DataType.ISVGStringList RequiredFeatures{get;}
		Interface.DataType.ISVGStringList RequiredExtensions{get;}
		Interface.DataType.ISVGStringList SystemLanguage{get;}

		bool HasExtension ( string extension );
	}
}
