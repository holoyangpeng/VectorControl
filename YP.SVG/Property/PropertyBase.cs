using System;

namespace YP.SVG.Property
{
	/// <summary>
	/// 处理属性项取值或者设值的事件
	/// </summary>
	public delegate void PropertyValueEventHandler(object sender, PropertyValueEventArgs e);

	/// <summary>
	/// 处理属性项绑定到图元时的事件
	/// </summary>
	public delegate void AttachingToElementEventHandler(object sender, AttachingToElementEventArgs e);

	/// <summary>
	/// 存储PerpertyValueEventHandler的数据
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
		/// 获取或者设置一个值，指示当属性项设置为自定义值时，是否往默认的属性项中写入值
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
		/// 处理的属性项
		/// </summary>
		public Property.IProperty OwnerPropertyItem
		{
			get
			{
				return this.ownerPropertyItem;
			}
		}

		/// <summary>
		/// 当前属性项的值
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
	/// 存储AttachToElement事件的数据
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
		/// 获取将要绑定的SVG图元对象
		/// </summary>
		public SVGElement TargetElement
		{
			get
			{
				return this.targetElement;
			}
		}

		/// <summary>
		/// 指定如何处理属性绑定图元，如果确认将属性项绑定到图元，设置为true，否则设置为false
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
	/// 自定义属性项的基类
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
		/// 初始化自定义属性项
		/// </summary>
		/// <param name="name">显示在属性框中的名字.</param>
		/// <param name="type">对应的属性的类型.</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, string type,string attributeName) : this(name, type, null, null, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, Type type,string attributeName) :
			this(name, type.AssemblyQualifiedName, null, null, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
		/// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName) : this(name, type, category, null, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="getValue">获取属性值的方法</param>
        /// <param name="setValue">获取属性值的方法</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) : this(name, type, category, null, null,attributeName,false,getValue,setValue) { }

		/// <summary>
        /// 初始化自定义属性项
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="isReadOnly">指定该属性是否为只读属性</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,bool isReadOnly) : this(name, type, category, null, null,attributeName,isReadOnly) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="isReadOnly">指定该属性是否为只读属性</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="getValue">获取属性值的方法</param>
        /// <param name="setValue">获取属性值的方法</param>
		public CustomPropertyItem(string name, string type, string category,string attributeName,bool isReadOnly,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) : this(name, type, category, null, null,attributeName,isReadOnly,getValue,setValue) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="isReadOnly">指定该属性是否为只读属性</param>
        /// <param name="attributeName">对应的属性的名称</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName, bool isReadOnly) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName,isReadOnly) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="getValue">获取属性值的方法</param>
        /// <param name="setValue">获取属性值的方法</param>
        /// <param name="isReadOnly">指定该属性是否为只读属性</param>
		public CustomPropertyItem(string name, Type type, string category,string attributeName, bool isReadOnly,PropertyValueEventHandler getValue,PropertyValueEventHandler setValue) :
			this(name, type.AssemblyQualifiedName, category, null, null,attributeName,isReadOnly,getValue,setValue) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
		public CustomPropertyItem(string name, string type, string category, string description,string attributeName) :
			this(name, type, category, description, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
		public CustomPropertyItem(string name, Type type, string category, string description,string attributeName):
			this(name, type.AssemblyQualifiedName, category, description, null,attributeName) { }

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string attributeName):this(name,type,category,description,defaultValue,attributeName,false)
		{
		}

		/// <summary>
        /// 初始化自定义属性项
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
        /// <param name="isReadOnly">指定属性是否为只读属性</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string attributeName,bool isReadOnly):this(name,type,category,description,defaultValue,attributeName,isReadOnly,null,null)
		{
		}

		/// <summary>
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
        /// <param name="isReadOnly">指定属性是否为只读属性</param>
        /// <param name="getValue">获取属性值的方法</param>
        /// <param name="setValue">当属性值发生改变时，设置属性值的方法</param>
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
        /// 初始化自定义属性项.
		/// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
		public CustomPropertyItem(string name, Type type, string category, string description, object defaultValue,string attributeName) :
			this(name, type.AssemblyQualifiedName, category, description, defaultValue,attributeName) { }

        /// <summary>
        /// 初始化自定义属性项.
        /// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
        /// <param name="editor">显示在属性框中时，编辑属性值所对应的编辑器</param>
		public CustomPropertyItem(string name, string type, string category, string description, object defaultValue,string editor, string typeConverter,string attributeName) : this(name, type, category, description, defaultValue,attributeName)
		{
			this.editor = editor;
			this.typeConverter = typeConverter;
		}

        /// <summary>
        /// 初始化自定义属性项.
        /// </summary>
        /// <param name="name">显示在属性框中的名字.</param>
        /// <param name="type">对应的属性的类型.</param>
        /// <param name="category">当显示在属性框中时，设定该属性项所属的类别.</param>
        /// <param name="attributeName">对应的属性的名称</param>
        /// <param name="description">属性项的描述，该值将会出现在属性框的描述区</param>
        /// <param name="defaultValue">默认值，如果没有默认值，设置为null</param>
        /// <param name="editor">显示在属性框中时，编辑属性值所对应的编辑器</param>
		public CustomPropertyItem(string name, Type type, string category, string description, object defaultValue, string editor, string typeConverter, string attributeName) : this(name, type.AssemblyQualifiedName, category, description, defaultValue, attributeName)
        {
            this.editor = editor;
            this.typeConverter = typeConverter;
        }



        #endregion

        #region ..public properties
        /// <summary>
        /// 设置或者获取一个值，指示属性项如何获取属性值
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
		/// 当属性值发生改变时，指定如何设置属性项的相应值
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
		/// 属性项所属的类别.
		/// </summary>
		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		/// <summary>
		/// 属性项所对应的TypeConverter类型
		/// </summary>
		public string ConverterTypeName
		{
			get { return typeConverter; }
			set { typeConverter = value; }
		}

		/// <summary>
		/// 属性项的默认值，如果无默认值，设置为null.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		
		/// <summary>
		/// 属性项是否只读
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
		/// 显示在属性框描述框中的描述信息
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// 属性项对应的Editor类型
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
		/// 显示在属性框中的名称
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// 属性项对应值的类型
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
		/// 该属性值对应在svg图元中的属性名称
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
