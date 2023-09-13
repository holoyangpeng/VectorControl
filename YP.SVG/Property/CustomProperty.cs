using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace YP.SVG.Property
{
	/// <summary>
	/// define the custom property for the svg element
	/// </summary>
	internal class CustomProperty:CustomPropertyItem,IProperty
	{
		#region ..private fields
		SVG.SVGElement _ownerElement = null;
		#endregion		

		#region ..Constructor
		/// <summary>
		/// ��ʼ���Զ���������
		/// </summary>
		/// <param name="name">��ʾ�����Կ��е�����.</param>
		/// <param name="type">��Ӧ���Ե�����.</param>
        /// <param name="attributeName">��Ӧ����������</param>
        /// <param name="ownerElement">���������󶨵�SVG����</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type,string attributeName) : this(ownerElement,name, type, null, null, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, null, null, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category,string attributeName) : this(ownerElement,name, type, category, null, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category"></param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, category, null, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description,string attributeName) :
			this(ownerElement,name, type, category, description, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description,string attributeName):
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, null,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomPropertyItem class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,string attributeName):this(ownerElement,name,type,category,description,defaultValue,attributeName,false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CustomPropertyItem class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="isReadOnly">the property is readonly</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,string attributeName,bool isReadOnly):base(name,type,category, description,defaultValue,attributeName,isReadOnly)
		{
			this._ownerElement = ownerElement;
		}

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description, object defaultValue,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, defaultValue,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The fully qualified name of the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,string editor, string typeConverter,string attributeName) : base(name, type, category, description, defaultValue,editor,typeConverter,attributeName)
		{
			this._ownerElement = ownerElement;
		}

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The fully qualified name of the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description, object defaultValue,	string editor, string typeConverter,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The Type that represents the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,Type editor, string typeConverter,string attributeName) :
			this(ownerElement,name, type, category, description, defaultValue, editor.AssemblyQualifiedName,typeConverter,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The Type that represents the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description, object defaultValue,Type editor, string typeConverter,string attributeName) : 
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, defaultValue,editor.AssemblyQualifiedName, typeConverter,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The fully qualified name of the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,string editor, Type typeConverter,string attributeName) :
			this(ownerElement,name, type, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The fully qualified name of the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description, object defaultValue,string editor, Type typeConverter,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, defaultValue, editor,typeConverter.AssemblyQualifiedName,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The Type that represents the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, string type, string category, string description, object defaultValue,Type editor, Type typeConverter,string attributeName) :
			this(ownerElement,name, type, category, description, defaultValue, editor.AssemblyQualifiedName,typeConverter.AssemblyQualifiedName,attributeName) { }

		/// <summary>
		/// Initializes a new instance of the CustomProperty class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="editor">The Type that represents the type of the editor for this
		/// property.  This type must derive from UITypeEditor.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		public CustomProperty(SVG.SVGElement ownerElement,string name, Type type, string category, string description, object defaultValue,Type editor, Type typeConverter,string attributeName) :
			this(ownerElement,name, type.AssemblyQualifiedName, category, description, defaultValue,editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName,attributeName) { }

		#endregion

		#region ..GetPropertyValue
		/// <summary>
		/// �����԰󶨵�����ʱ����ȡ���Ե�ֵ
		/// </summary>
		/// <param name="document">��ǰ�󶨵��ĵ�</param>
		/// <returns></returns>
		public object GetPropertyValue()
		{
			object value1 = string.Empty;
			if(this.AttributeName.Trim().Length > 0)
				value1 = this._ownerElement.GetPropertyValue(this.AttributeName);
			if(this.GetValue != null)
			{
				PropertyValueEventArgs args = new PropertyValueEventArgs(this);
				args.PropertyValue = value1;
                args.ContinueDefaultAction = true;
				this.GetValue(this,args);
				if(args.ContinueDefaultAction)
					value1 = args.PropertyValue;
			}
			return value1;
		}
		#endregion

		#region ..SetPropertyValue
		/// <summary>
		/// �����Ըı�ʱ����������ֵ
		/// </summary>
		/// <param name="value">����ֵ</param>
		public void SetPropertyValue(object value)
		{
            object value1 = value;
			if(this.SetValue != null)
			{
				PropertyValueEventArgs args = new PropertyValueEventArgs(this);
				args.PropertyValue = value;
				args.ContinueDefaultAction = false;
				this.SetValue(this,args);
                value1 = args.PropertyValue;
				if(!args.ContinueDefaultAction)
					return;
			}

			if(this.AttributeName.Trim().Length > 0)
                this._ownerElement.SetPropertyValue(this.AttributeName, value1);
		}
		#endregion
	}
}
