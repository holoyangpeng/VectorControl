using System;

namespace YP.SVG
{
		#region ..��¼������Ϣ
		/// <summary>
		/// ��¼����Ķ�����Ϣ
		/// </summary>
		public struct ClickAction
		{
			ActionType type;
			string actionArgs;

			public ClickAction(ActionType type,string arg)
			{
				this.type = type;
				this.actionArgs = arg;
			}

			/// <summary>
			/// ��ȡ�����ö���ִ�еĶ�������
			/// </summary>
			public ActionType Type
			{
				set
				{
					this.type = value;
				}
				get
				{
					return this.type;
				}
			}

			/// <summary>
			/// ��ȡ�����ö���ִ�ж����Ĳ���
			/// </summary>
			public string ActionArgs
			{
				set
				{
					this.actionArgs = value;
				}
				get
				{
					return this.actionArgs;
				}
			}

			public static bool operator == (ClickAction action1,ClickAction action2)
			{
				return action1.type == action2.type && action1.actionArgs == action2.actionArgs;
			}

			public static bool operator != (ClickAction action1,ClickAction action2)
			{
				return action1.type != action2.type || action1.actionArgs != action2.actionArgs;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}

			public override bool Equals(object obj)
			{
				if(obj is ClickAction)
					return this == (ClickAction)obj;
				return false;
			}

			public override string ToString()
			{
				if(this.type == ActionType.None)
					return "none";
				else
					return this.type.ToString() + "(" + this.actionArgs + ")";
			}


			/// <summary>
			/// ��������
			/// </summary>
			/// <param name="actionstring"></param>
			/// <returns></returns>
			public static ClickAction Parse(string actionstring)
			{
				ActionType type = ActionType.None;
				string arg = string.Empty;
				int index = actionstring.IndexOf("(");
				if(index >= 0)
				{
					string a = actionstring.Substring(0,index);
					try
					{
                        if (System.Enum.IsDefined(typeof(ActionType), a))
						type = (ActionType)System.Enum.Parse(typeof(ActionType),a,true);
						int index1 = actionstring.LastIndexOf(")");
						if(index1 < 0)
							index1 = actionstring.Length - 1;
						a = actionstring.Substring(index + 1,index1 - index-1);
						arg = a;
						a = null;
					}
					catch{}
				}
				return new ClickAction(type,arg);
			}
		}
		#endregion
}
