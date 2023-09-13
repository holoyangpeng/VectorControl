using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义撤销/重作的一般定义和属性
	/// </summary>
	public interface IUndoOperation
	{
		/// <summary>
		/// 重复上一步操作
		/// </summary>
		void Redo();

		/// <summary>
		/// 撤销上一步操作
		/// </summary>
		void Undo();
	}
}
