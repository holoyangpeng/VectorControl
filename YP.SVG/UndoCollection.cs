using System;
using System.Collections;

using YP.SVG.Interface;

namespace YP.SVG
{
	/// <summary>
	/// UndoCollection 的摘要说明。
	/// </summary>
	public class UndoCollection:CollectionBase
	{
		#region ..构造及消除
		public UndoCollection(IUndoOperation[] undos)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.AddRange(undos);
			this.operateTime = DateTime.Now;
		}
		#endregion

		#region ..私有变量
		System.DateTime operateTime;
		#endregion

		#region ..公共属性
		public IUndoOperation this[int index]
		{
			set
			{
				this.List[index] = value;
			}
			get
			{
				return this.List[index] as IUndoOperation;
			}
		}

		public DateTime OperateTime
		{
			get
			{
				return this.operateTime;
			}
			set
			{
				this.operateTime = value;
			}
		}
		#endregion

		#region ..Add
		public void Add(IUndoOperation undo)
		{
			if(!this.List.Contains(undo))
				this.List.Add(undo);
		}

		public void AddRange(IUndoOperation[] undos)
		{
			for(int i = 0;i<undos.Length;i++)
			{
				this.Add(undos[i]);
			}
		}
		#endregion
	}
}
