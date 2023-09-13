using System;
using System.Collections;

using YP.SVG.Interface;

namespace YP.SVG
{
	/// <summary>
	/// UndoCollection ��ժҪ˵����
	/// </summary>
	public class UndoCollection:CollectionBase
	{
		#region ..���켰����
		public UndoCollection(IUndoOperation[] undos)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.AddRange(undos);
			this.operateTime = DateTime.Now;
		}
		#endregion

		#region ..˽�б���
		System.DateTime operateTime;
		#endregion

		#region ..��������
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
