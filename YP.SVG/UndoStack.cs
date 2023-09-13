using System;
using System.Collections;
using System.Collections.Specialized;

using YP.SVG.Interface;

namespace YP.SVG
{
    /// <summary>
    /// define the stack to store the undo operation
    /// </summary>
	public class UndoStack
	{
		#region ..私有变量
		Stack undostack = new Stack();
		Stack redostack = new Stack();
		bool acceptChanges = true;
		Document.SVGDocument doc = null;
		#endregion

		#region ..公共字段
		/// <summary>
		/// 控制是否记录操作过程
		/// </summary>
		public bool AcceptChanges
		{
			get
			{
				return acceptChanges;
			}
		}	
		#endregion

		#region ..构造及消除
		public UndoStack(Document.SVGDocument doc)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.doc = doc;
		}
		#endregion

		#region ..公共属性
		/// <summary>
		/// 确定当前是否可以执行撤销操作
		/// </summary>
		public bool CanUndo
		{
			get
			{
				return this.undostack.Count > 0;
			}
		}

		/// <summary>
		/// 确定当前是否可以执行重作操作
		/// </summary>
		public bool CanRedo
		{
			get
			{
				return this.redostack.Count > 0;
			}
		}
		#endregion

		#region ..公共方法
		/// <summary>
		/// 往堆栈中推进一个操作，执行此操作之后，所有的重作记录将被清除
		/// </summary>
		/// <param name="operation">操作</param>
		public virtual void Push(Interface.IUndoOperation[] operations) 
		{
			if (operations == null) 
			{
				throw new ArgumentNullException("错误的操作");
			}
		
			if (AcceptChanges) 
			{
				this.acceptChanges = false;
				UndoCollection undo = new UndoCollection(operations); 
				undostack.Push(undo);
				ClearRedoStack();
				this.acceptChanges = true;
			}
		}

		/// <summary>
		/// 清除重作记录
		/// </summary>
		void ClearRedoStack()
		{
			redostack.Clear();
		}
		
		/// <summary>
		/// 清除堆栈中t所有记录
		/// </summary>
		public void ClearAll()
		{
			undostack.Clear();
			redostack.Clear();
		}

		/// <summary>
		/// 撤销上次记录
		/// </summary>
		/// <returns>在撤销过程中发生改变的对象</returns>
		public virtual Interface.ISVGElement[] Undo()
		{
			if(this.CanUndo)
			{
				bool old = this.AcceptChanges;
				this.acceptChanges = false;
				UndoCollection operations = (UndoCollection)this.undostack.Pop();
				if(operations == null)
					return null;
				DateTime time1 = operations.OperateTime;
				ArrayList list = new ArrayList();
				for(int i = 0;i<operations.Count;i++)//each(IUndoOperation undo in operations)
				{
					Interface.IUndoOperation undo = operations[i] as Interface.IUndoOperation;
					if(undo != null)
					{
						if(undo is YP.SVG.Undo.NodeUndoOperation)
						{
							if(!list.Contains((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement)&& (undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement != null)
								list.Add((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement);
						}
						undo.Undo();
					}
				}
				if(list.Count > 0)
				{
					YP.SVG.SVGElement[] l1 = new SVGElement[list.Count];
					list.CopyTo(l1);
					this.doc.InvokeSelectionChanged(new CollectionChangedEventArgs(l1,CollectionChangeAction.Change));
				}
				this.redostack.Push(operations);
				this.acceptChanges = old;
			}
			return null;			
		}

		/// <summary>
		/// 重复上次记录
		/// </summary>
		/// <returns>在重作过程中发生改变的对象</returns>
		public virtual Interface.ISVGElement[] Redo()
		{
			if(this.CanRedo)
			{
				bool old = this.AcceptChanges;
				this.acceptChanges = false;
				UndoCollection operations = (UndoCollection)this.redostack.Pop();
				if(operations == null)
					return null;
				DateTime time1 = operations.OperateTime;
				ArrayList list = new ArrayList();
				for(int i = operations.Count - 1;i>=0;i--)
				{
					Interface.IUndoOperation undo = operations[i] as Interface.IUndoOperation;
					if(undo != null)
					{
						undo.Redo();
						if(undo is YP.SVG.Undo.NodeUndoOperation)
						{
							if(!list.Contains((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement) && (undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement != null)
								list.Add((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement);
						}
					}
				}
				if(list.Count > 0)
				{
					YP.SVG.SVGElement[] l1 = new SVGElement[list.Count];
					list.CopyTo(l1);
					this.doc.InvokeSelectionChanged(new CollectionChangedEventArgs(l1,CollectionChangeAction.Change));
				}
				this.undostack.Push(operations);
				this.acceptChanges = old;
			}
			return null;			
		}
		#endregion

		#region ..更新对象
		public void Update()
		{
			if(this.CanUndo)
			{
				UndoCollection undo = this.undostack.Peek() as UndoCollection;
			}
		}
		#endregion
	}
}
