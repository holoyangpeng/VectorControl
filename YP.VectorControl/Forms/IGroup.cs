using System;
using YP.SVG.Interface;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// IGroup ��ժҪ˵����
	/// </summary>
	internal interface IGroup
	{
		/// <summary>
		/// ��ȡ���������չ��״̬
		/// </summary>
		bool Expand{set;get;}

		string ID{get;set;}

		int Count{get;}

        int IndexOf(IOutlookBarPath path);
	}
}
