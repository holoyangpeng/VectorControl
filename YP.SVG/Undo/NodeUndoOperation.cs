using System;
using System.Xml;

namespace YP.SVG.Undo
{
	/// <summary>
	/// 实现节点的撤销重作功能
	/// </summary>
	public class NodeUndoOperation:Interface.IUndoOperation 
	{
		#region ..构造及消除
		public NodeUndoOperation(YP.SVG.Document.SVGDocument doc,System.Xml.XmlNode changeNode,System.Xml.XmlNodeChangedAction action,XmlNode oldParent,XmlNode newParent,YP.SVG.SVGElement changeSVGParent, string oldValue)
		{
			this.changeDocument = doc;
			this.changeNode = changeNode;
			this.action = action;
			this.changeSVGParent = changeSVGParent;
			this.oldParent = oldParent;
			this.newParent = newParent;
			if(this.changeNode is YP.SVG.SVGElement)
				this.nextSibling = (this.changeNode as YP.SVG.SVGElement).NextElement as YP.SVG.SVGElement;
			else
				this.nextSibling = this.changeNode.NextSibling;
			if(this.changeNode is YP.SVG.SVGElement)
				this.preSibling = (this.changeNode as YP.SVG.SVGElement).PreviousElement as YP.SVG.SVGElement;
			else
                this.preSibling = this.changeNode.PreviousSibling;
            this.oldValue = oldValue;
			this.newValue = this.changeNode.Value;
		}
		#endregion

		#region ..私有变量
		YP.SVG.Document.SVGDocument changeDocument = null;
		//记录发生改变的XML节点
		System.Xml.XmlNode changeNode = null;
		//如果更改节点不是SVG对象，记录其父级SVG对象
		YP.SVG.SVGElement changeSVGParent = null;
		System.Xml.XmlNodeChangedAction action = System.Xml.XmlNodeChangedAction.Change;
		System.Xml.XmlNode oldParent = null;
		System.Xml.XmlNode newParent = null;
		System.Xml.XmlNode preSibling;
		System.Xml.XmlNode nextSibling;
		string oldValue = string.Empty;
		string newValue = string.Empty;
		public System.DateTime EditedTime = System.DateTime.MaxValue;
//		Base.Struct.ElementPos oldStartPos = Base.Struct.ElementPos.Empty;
//		Base.Struct.ElementPos oldEndPos = Base.Struct.ElementPos.Empty;
//		Base.Struct.ElementPos newStartPos = Base.Struct.ElementPos.Empty;
//		Base.Struct.ElementPos newEndPos = Base.Struct.ElementPos.Empty;
		#endregion

		#region ..属性
		public YP.SVG.SVGElement ChangedElement
		{
			get
			{
				return this.changeSVGParent;
			}
		}
		#endregion

		#region ..重复上一步操作
		/// <summary>
		/// 重复上一步操作
		/// </summary>
		public void Redo()
		{
			string name = string.Empty;
			switch(this.action)
			{
					#region ..插入
				case System.Xml.XmlNodeChangedAction.Insert:
					if(this.changeNode is SVGElement)
					{
						bool next = this.nextSibling != null;
						if(this.nextSibling != null)
							next = next && this.nextSibling.ParentNode == this.newParent;
						bool pre = this.preSibling != null;
						if(this.preSibling != null)
							pre = pre && this.preSibling.ParentNode == this.newParent;
						if(next)
							(this.newParent as SVGElement).InternalInsertBefore(this.changeNode,this.nextSibling);
						else if(pre)
                            (this.newParent as SVGElement).InternalInsertAfter(this.changeNode, this.preSibling);
						else
                            (this.newParent as SVGElement).InternalAppendChild(this.changeNode);
					}
					else if(this.changeSVGParent != null)
					{
						if(this.newParent is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.newParent.Name, this.changeNode.Value);
							this.changeSVGParent.InternalSetAttribute(this.newParent.Name,this.changeNode.Value);
							name = this.newParent.Name;
						}
						else if(this.changeNode is System.Xml.XmlAttribute)
						{
							//this.changeSVGParent.AddSVGAttribute(this.changeNode.Name,this.changeNode.Value);
							this.changeSVGParent.InternalSetAttribute(this.changeNode.Name,this.changeNode.Value);
							name = this.changeNode.Name;
						}
					}
					break;
					#endregion

					#region ..改变
				case System.Xml.XmlNodeChangedAction.Change:
					if(this.changeSVGParent != null)
					{
						if(this.changeNode is System.Xml.XmlAttribute)
						{
							//this.changeSVGParent.AddSVGAttribute(this.changeNode.Name,this.newValue);
							this.changeSVGParent.InternalSetAttribute(this.changeNode.Name,this.newValue);
							name = this.changeNode.Name;
						}
						else if(this.changeNode is System.Xml.XmlText && this.newParent is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.newParent.Name, this.newValue);
							this.changeSVGParent.InternalSetAttribute(this.newParent.Name,this.newValue);
							name = this.newParent.Name;
						}
					}
					break;
					#endregion

					#region ..删除
				case System.Xml.XmlNodeChangedAction.Remove:
					if(this.changeNode is SVGElement)
					{
						if(this.changeNode.ParentNode == this.oldParent)
							(this.oldParent as SVGElement).InternalRemoveChild(this.changeNode);
					}
					else if(this.changeSVGParent != null)
					{
						if(this.oldParent is System.Xml.XmlAttribute)
						{
							//this.changeSVGParent.AddSVGAttribute(this.oldParent.Name,string.Empty);
							this.changeSVGParent.InternalRemoveAttribute(this.oldParent.Name);
							name = this.oldParent.Name;
						}
						else if(this.changeNode is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.changeNode.Name, string.Empty);
							this.changeSVGParent.InternalRemoveAttribute(this.changeNode.Name);
							name = this.changeNode.Name;
						}
					}
					break;
					#endregion
			}
		}
		#endregion

		#region ..撤销上一步操作
		/// <summary>
		/// 撤销上一步操作
		/// </summary>
		public void Undo()
		{
			string name = string.Empty;
			switch(this.action)
			{
					#region ..插入
				case System.Xml.XmlNodeChangedAction.Insert:
					if(this.changeNode is SVG.SVGElement)
					{
						if(this.changeNode.ParentNode is SVGElement)
							(this.changeNode.ParentNode as SVGElement).InternalRemoveChild(this.changeNode);
					}
					else if(this.changeSVGParent != null)
					{
						if(this.newParent is System.Xml.XmlAttribute)
						{
							//this.changeSVGParent.AddSVGAttribute(this.newParent.Name,string.Empty);
							this.changeSVGParent.InternalRemoveAttribute(this.newParent.Name);
							name = this.newParent.Name;
						}
						else if(this.changeNode is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.changeNode.Name, string.Empty);
							this.changeSVGParent.InternalRemoveAttribute(this.changeNode.Name);
							name = this.changeNode.Name;
						}
					}
					else if(this.changeNode != null && this.changeNode.ParentNode is SVGElement)
						(this.changeNode.ParentNode as SVGElement).InternalRemoveChild(this.changeNode);
					break;
					#endregion

					#region ..改变
				case System.Xml.XmlNodeChangedAction.Change:
					if(this.changeSVGParent != null)
					{
						if(this.changeNode is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.changeNode.Name, this.oldValue);
							this.changeSVGParent.InternalSetAttribute(this.changeNode.Name,this.oldValue);
							name = this.changeNode.Name;
							
						}
						else if(this.changeNode is System.Xml.XmlText && this.oldParent is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.oldParent.Name, this.oldValue);
							this.changeSVGParent.InternalSetAttribute(this.oldParent.Name,this.oldValue);
							name = this.oldParent.Name;
							
						}
					}
					break;
					#endregion

					#region ..删除
				case System.Xml.XmlNodeChangedAction.Remove:
					#region ..节点
					if(this.changeNode is SVGElement)
					{
						bool next = this.nextSibling != null;
						if(this.nextSibling != null)
							next = next && this.nextSibling.ParentNode == this.oldParent;
						bool pre = this.preSibling != null;
						if(this.preSibling != null)
							pre = pre && this.preSibling.ParentNode == this.oldParent;
						if(next)
							(this.oldParent as SVGElement).InternalInsertBefore(this.changeNode,this.nextSibling);
						else if(pre)
							(this.oldParent as SVGElement).InternalInsertAfter(this.changeNode,this.preSibling);
						else
							(this.oldParent as SVGElement).InternalAppendChild(this.changeNode);
					}
					#endregion

					#region ..属性
					else if(this.changeSVGParent != null)
					{
						if(this.oldParent is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.oldParent.Name, this.changeNode.Value);
							this.changeSVGParent.InternalSetAttribute(this.oldParent.Name,this.changeNode.Value);
							name = this.oldParent.Name;
						}
						else if(this.changeNode is System.Xml.XmlAttribute)
						{
                            //this.changeSVGParent.AddSVGAttribute(this.changeNode.Name, this.changeNode.Value);
							this.changeSVGParent.InternalSetAttribute(this.changeNode.Name,this.changeNode.Value);
							name = this.changeNode.Name;
						}
					}
					#endregion
					break;
					#endregion

			}
        }
		#endregion

		#region ..绑定动画
//		void AttachAnimate(YP.SVGDom.Animation.SVGAnimationElement anim)
//		{
//			YP.SVGDom.SVGStyleable target = (YP.SVGDom.SVGStyleable)anim.TargetElement;
//			string attributeName = anim.GetAttribute("attributeName").Trim();
//			if(anim is Animation.SVGAnimateMotionElement)
//				attributeName = "transform";
//			if(attributeName.Length > 0 && target != null)
//			{
//				YP.SVGDom.DataType.SVGAnimatedType animateType = (YP.SVGDom.DataType.SVGAnimatedType)target.GetAnimatedAttribute(attributeName);
//				target.AttachKeys(anim);
//				
//				animateType.InternalInsertBefore(anim,anim.NextAnimate);
//			}
//		}
		#endregion
	}
}
