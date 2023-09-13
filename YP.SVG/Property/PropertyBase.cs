using System;

namespace YP.SVG.Property
{
	/// <summary>
	/// ����������ȡֵ������ֵ���¼�
	/// </summary>
	public delegate void PropertyValueEventHandler(object sender, PropertyValueEventArgs e);

	/// <summary>
	/// ����������󶨵�ͼԪʱ���¼�
	/// </summary>
	public delegate void AttachingToElementEventHandler(object sender, AttachingToElementEventArgs e);

	/// <summary>
	/// �洢PerpertyValueEventHandler������
	/// </summary>
	public class PropertyValueEventArgs:System.EventArgs
	{
		#region ..Constructor
		/// <summary>
		/// create the getpropertyeventargs instance with the input property item
		/// </summary>
		/// <param name="ownerPropertyItem"></param>
		public PropertyValueEventArgs(Property.IProperty ownerPropertyItem)
		{
			this.ownerPropertyItem = ownerPropertyItem;
		}
		#endregion

		#region ..private fields
		Property.IProperty ownerPropertyItem = null;
		object propertyValue = string.Empty;
		bool needContinueDefaultSetValue = false;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ��������һ��ֵ��ָʾ������������Ϊ�Զ���ֵʱ���Ƿ���Ĭ�ϵ���������д��ֵ
		/// </summary>
		public bool ContinueDefaultAction
		{
			set
			{
				this.needContinueDefaultSetValue = value;
			}
			get
			{
				return this.needContinueDefaultSetValue;
			}
		}

		/// <summary>
		/// �����������
		/// </summary>
		public Property.IProperty OwnerPropertyItem
		{
			get
			{
				return this.ownerPropertyItem;
			}
		}

		/// <summary>
		/// ��ǰ�������ֵ
		/// </summary>
		public object PropertyValue
		{
			set
			{
				this.propertyValue = value;
			}
			get
			{
				return this.propertyValue;
			}
		}
		#endregion
	}

	/// <summary>
	/// �洢AttachToElement�¼�������
	/// </summary>
	public class AttachingToElementEventArgs:System.EventArgs
	{
		#region ..Constructor
		internal AttachingToElementEventArgs(SVGElement targetElement)
		{
			this.targetElement = targetElement;
		}
		#endregion

		#region ..private fields
		SVGElement targetElement = null;
		bool result = true;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ��Ҫ�󶨵�SVGͼԪ����
		/// </summary>
		public SVGElement TargetElement
		{
			get
			{
				return this.targetElement;
			}
		}

		/// <summary>
		/// ָ����δ������԰�ͼԪ�����ȷ�Ͻ�������󶨵�ͼԪ������Ϊtrue����������Ϊfalse
		/// </summary>
		public bool Result
		{
			set
			{
				this.result = value;
			}
			get
			{
				return this.result;
			}
		}
		#endregion
	}

	/// <summary>
	/// �Զ���������Ļ���
	/// </summary>
	public class CustomPropertyItem:Property.IProperty
	{
		#region ..private fields
		private string category;
		private object defaultValue;
		private string description;
		private string editor;
		private string name;
		private string type;
		private string typeConverter;
		private string attributeName;
		bool isReadOnly = false;
		PropertyValueEventHandler getPropertyValue;
		PropertyValueEventHandler setPropertyValue;
		#endregion

		#region ..Constructor
		/// <summary>
		/// ��ʼ���Զ���������
		/// </summary>
		/// <param name="name">��ʾ�����Կ��е�����.</param>
		/// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, string type,string attributeName) : this(name, type, null, null, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, Type type,string attributeName) :
			this(name, type.AssemblyQualifiedName, null, null, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
		/// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName) : this(name, type, category, null, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="getValue">��ȡ����ֵ�ķ���</param>
        /// <param name="setValue">��ȡ����ֵ�ķ���</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) : this(name, type, category, null, null,attributeName,false,getValue,setValue) { }

		/// <summary>
        /// ��ʼ���Զ���������
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="isReadOnly">ָ���������Ƿ�Ϊֻ������</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,bool isReadOnly) : this(name, type, category, null, null,attributeName,isReadOnly) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="isReadOnly">ָ���������Ƿ�Ϊֻ������</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="getValue">��ȡ����ֵ�ķ���</param>
        /// <param name="setValue">��ȡ����ֵ�ķ���</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,bool isReadOnly,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) : this(name, type, category, null, null,attributeName,isReadOnly,getValue,setValue) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="isReadOnly">ָ���������Ƿ�Ϊֻ������</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName, bool isReadOnly) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName,isReadOnly) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="getValue">��ȡ����ֵ�ķ���</param>
        /// <param name="setValue">��ȡ����ֵ�ķ���</param>
        /// <param name="isReadOnly">ָ���������Ƿ�Ϊֻ������</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName, bool isReadOnly,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName,isReadOnly,getValue,setValue) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
		public CustomPropertyItem(string name, string type, string category, string description,string attributeName) :
			this(name, type, category, description, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
		public CustomPropertyItem(string name, Type type, string category, string description,string attributeName):
			this(name, type.AssemblyQualifiedName, category, description, null,attributeName) { }

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string attributeName):this(name,type,category,description,defaultValue,attributeName,false)
		{
		}

		/// <summary>
        /// ��ʼ���Զ���������
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
        /// <param name="isReadOnly">ָ�������Ƿ�Ϊֻ������</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string attributeName,bool isReadOnly):this(name,type,category,description,defaultValue,attributeName,isReadOnly,null,null)
		{
		}

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
        /// <param name="isReadOnly">ָ�������Ƿ�Ϊֻ������</param>
        /// <param name="getValue">��ȡ����ֵ�ķ���</param>
        /// <param name="setValue">������ֵ�����ı�ʱ����������ֵ�ķ���</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string attributeName,bool isReadOnly,PropertyValueEventHandler getValue, PropertyValueEventHandler setValue)
		{
			this.name = name;
			this.type = type;
			this.category = category;
			this.description = description;
			this.defaultValue = defaultValue;
			this.attributeName = attributeName;
			this.isReadOnly = isReadOnly;
			this.getPropertyValue = getValue;
			this.setPropertyValue = setValue;
		}

		/// <summary>
        /// ��ʼ���Զ���������.
		/// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
		public CustomPropertyItem(string name, Type type, string category, string description, object defaultValue,string attributeName) :
			this(name, type.AssemblyQualifiedName, category, description, defaultValue,attributeName) { }

        /// <summary>
        /// ��ʼ���Զ���������.
        /// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
        /// <param name="editor">��ʾ�����Կ���ʱ���༭����ֵ����Ӧ�ı༭��</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string editor, string typeConverter,string attributeName) : this(name, type, category, description, defaultValue,attributeName)
		{
			this.editor = editor;
			this.typeConverter = typeConverter;
		}

        /// <summary>
        /// ��ʼ���Զ���������.
        /// </summary>
        /// <param name="name">��ʾ�����Կ��е�����.</param>
        /// <param name="type">��Ӧ�����Ե�����.</param>
        /// <param name="category">����ʾ�����Կ���ʱ���趨�����������������.</param>
        /// <param name="attributeName">��Ӧ�����Ե�����</param>
        /// <param name="description">���������������ֵ������������Կ��������</param>
        /// <param name="defaultValue">Ĭ��ֵ�����û��Ĭ��ֵ������Ϊnull</param>
        /// <param name="editor">��ʾ�����Կ���ʱ���༭����ֵ����Ӧ�ı༭��</param>
		public CustomPropertyItem(string name, Type type, string category, string description, object defaultValue, string editor, string typeConverter, string attributeName) : this(name, type.AssemblyQualifiedName, category, description, defaultValue, attributeName)
        {
            this.editor = editor;
            this.typeConverter = typeConverter;
        }



        #endregion

        #region ..public properties
        /// <summary>
        /// ���û��߻�ȡһ��ֵ��ָʾ��������λ�ȡ����ֵ
        /// </summary>
        public PropertyValueEventHandler GetValue
		{
			set
			{
				this.getPropertyValue = value;
			}
			get
			{
				return this.getPropertyValue;
			}
		}

		/// <summary>
		/// ������ֵ�����ı�ʱ��ָ������������������Ӧֵ
		/// </summary>
		public PropertyValueEventHandler SetValue
		{
			set
			{
				this.setPropertyValue = value;
			}
			get
			{
				return this.setPropertyValue;
			}
		}

		/// <summary>
		/// ���������������.
		/// </summary>
		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		/// <summary>
		/// ����������Ӧ��TypeConverter����
		/// </summary>
		public string ConverterTypeName
		{
			get { return typeConverter; }
			set { typeConverter = value; }
		}

		/// <summary>
		/// �������Ĭ��ֵ�������Ĭ��ֵ������Ϊnull.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		
		/// <summary>
		/// �������Ƿ�ֻ��
		/// </summary>
		public bool IsReadOnly
		{
			set
			{
				this.isReadOnly = value;
			}
			get
			{
				return this.isReadOnly;
			}
		}

		/// <summary>
		/// ��ʾ�����Կ��������е�������Ϣ
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// �������Ӧ��Editor����
		/// </summary>
		public Type EditorType
		{
			get 
			{
				if(editor != null)
					return Type.GetType(editor); 
				return null;
			}
			set 
            {
                if (value != null)
                    editor = value.FullName;
                else
                    editor = string.Empty;
            }
		}

		/// <summary>
		/// ��ʾ�����Կ��е�����
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// �������Ӧֵ������
		/// </summary>
		public Type PropertyType
		{
			get 
			{
				if(this.type != null)
					return Type.GetType(type); 
				return null;
			}
			set 
			{
				if (value != null)
					type = value.FullName;
				else
					type = string.Empty;
			}
		}

		/// <summary>
		/// ������ֵ��Ӧ��svgͼԪ�е���������
		/// </summary>
		public string AttributeName
		{
			get
			{
				return this.attributeName;
			}
		}
		#endregion

		#region ..GetCustomPropertyForElement
		internal SVG.Property.CustomProperty ConvertToCustomProperty(SVG.SVGElement ownerElement)
		{
			SVG.Property.CustomProperty property = new SVG.Property.CustomProperty(ownerElement,this.name,this.type,this.category,this.description,this.defaultValue,this.attributeName,this.isReadOnly);
			property.getPropertyValue = this.getPropertyValue;
			property.setPropertyValue = this.setPropertyValue;
			property.editor = this.editor;
			property.typeConverter = this.typeConverter;
			return property;
		}
		#endregion
	}
}
