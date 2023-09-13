using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// IColorSelector 的摘要说明。
	/// </summary>
	internal interface IColorSelector
	{
		ColorCollecton Items{get;}

		int SelectedIndex{set;get;}

		int ItemHeight{set;get;}
	}
}
