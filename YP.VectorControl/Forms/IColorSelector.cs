using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// IColorSelector ��ժҪ˵����
	/// </summary>
	internal interface IColorSelector
	{
		ColorCollecton Items{get;}

		int SelectedIndex{set;get;}

		int ItemHeight{set;get;}
	}
}
