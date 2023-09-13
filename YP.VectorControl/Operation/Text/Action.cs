using System;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// 实现编辑器动作
	/// </summary>
	internal abstract class Action
	{
		#region ..构造及消除
		internal Action()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		/// <summary>
		/// 执行动作
		/// </summary>
		/// <param name="eidtor"></param>
		internal abstract void Execute(TextEditor eidtor);
	}
}
