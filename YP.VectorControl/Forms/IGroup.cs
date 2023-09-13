using System;
using YP.SVG.Interface;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// IGroup 的摘要说明。
	/// </summary>
	internal interface IGroup
	{
		/// <summary>
		/// 获取或设置组的展开状态
		/// </summary>
		bool Expand{set;get;}

		string ID{get;set;}

		int Count{get;}

        int IndexOf(IOutlookBarPath path);
	}
}
