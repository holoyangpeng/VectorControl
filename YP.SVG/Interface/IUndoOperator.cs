using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ���峷��/������һ�㶨�������
	/// </summary>
	public interface IUndoOperation
	{
		/// <summary>
		/// �ظ���һ������
		/// </summary>
		void Redo();

		/// <summary>
		/// ������һ������
		/// </summary>
		void Undo();
	}
}
